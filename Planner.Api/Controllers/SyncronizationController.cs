using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Planner.Api.Services;
using Planner.Domain.Entities;
using Planner.Domain.Repositories.Interfaces;
using Planner.Domain.UnitOfWork;
using Planner.Dto;
using System;
using System.Collections.Generic;
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
        private readonly IMapper _mapper;
        private readonly ILogger<SyncronizationController> _logger;

        public SyncronizationController(IScheduledTaskRepository scheduledTaskRepo
            , IUnitOfWork unitOfWork
            , IMapper mapper
            , ILogger<SyncronizationController> logger)
        {
            _scheduledTaskRepo = scheduledTaskRepo;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DateTime lastSyncedOn)
        {
            try
            {
                var newTasks = await _scheduledTaskRepo.GetNewScheduledTasksForUserAsync(User.GetUserId(), lastSyncedOn);

                return Ok(_mapper.Map<IEnumerable<GetScheduledTaskDTO>>(newTasks));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Threw exception while getting new ScheduledTasks: { ex }");
                return new StatusCodeResult(500);
            }
        }

        [HttpPut("{lockId}")]
        public async Task<IActionResult> Put(Guid lockId, IEnumerable<PutScheduledTaskDTO> taskDtos)
        {
            try
            {
                if (taskDtos == null)
                    return BadRequest();

                var tasks = _mapper.Map<IEnumerable<ScheduledTask>>(taskDtos);

                await _scheduledTaskRepo.AddOrUpdateScheduledTasksAsync(tasks, User.GetUserId());

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Threw exception while creating new ScheduledTasks: { ex }");
                return new StatusCodeResult(500);
            }
        }
    }
}