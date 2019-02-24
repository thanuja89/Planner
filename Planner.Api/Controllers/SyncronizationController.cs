using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Planner.Api.Extensions;
using Planner.Domain.Repositories.Interfaces;
using Planner.Domain.UnitOfWork;
using Planner.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Planner.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class SyncronizationController : Controller
    {
        private readonly IScheduledTaskRepository _scheduledTaskRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly SyncronizationService _syncService;
        private readonly IMapper _mapper;
        private readonly ILogger<SyncronizationController> _logger;

        public SyncronizationController(IScheduledTaskRepository scheduledTaskRepo
            , IUnitOfWork unitOfWork
            , SyncronizationService syncService
            , IMapper mapper
            , ILogger<SyncronizationController> logger)
        {
            _scheduledTaskRepo = scheduledTaskRepo;
            _unitOfWork = unitOfWork;
            _syncService = syncService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DateTime lastSyncedOn)
        {
            try
            {
                var lk = await _syncService.TakeLockAsync(User.GetUserId());

                if (lk == null)
                    return Conflict();

                var newTasks = await _syncService.GetNewScheduledTasksAsync(User.GetUserId(), lastSyncedOn);

                return Ok(_mapper.Map<IEnumerable<GetScheduledTaskDTO>>(newTasks));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Threw exception while getting new ScheduledTasks: { ex }");
                return new StatusCodeResult(500);
            }
        }

        //[HttpPut]
        //public async Task<IActionResult> Put(IEnumerable<PutSyncScheduledTaskDTO> tasks)
        //{
        //    try
        //    {
        //        if (tasks == null)
        //            return BadRequest();

        //        var newTasks = new List<PutSyncScheduledTaskDTO>();
        //        var oldTasks = new List<PutSyncScheduledTaskDTO>();

        //        foreach (var task in tasks)
        //        {
        //            if (task.IsNew)
        //                newTasks.Add(task);
        //            else
        //                oldTasks.Add(task);
        //        }


        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}
    }
}