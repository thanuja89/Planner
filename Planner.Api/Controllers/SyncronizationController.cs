using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Planner.Api.Extensions;
using Planner.Domain.DataModels;
using Planner.Domain.Repositories.Interfaces;
using Planner.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planner.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class SyncronizationController : ControllerBase
    {
        private readonly IScheduledTaskRepository _scheduledTaskRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<SyncronizationController> _logger;

        public SyncronizationController(IScheduledTaskRepository scheduledTaskRepo
            , IMapper mapper
            , ILogger<SyncronizationController> logger)
        {
            _scheduledTaskRepo = scheduledTaskRepo;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get([ModelBinder(binderType: typeof(DateTimeModelBinder))] DateTime lastSynced)
        {
            try
            {
                var newTasks = await _scheduledTaskRepo.GetNewScheduledTasksForUserAsync(User.GetUserId(), lastSynced);

                return Ok(_mapper.Map<IEnumerable<GetScheduledTaskDTO>>(newTasks));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Threw exception while getting new ScheduledTasks: { ex }");
                return new StatusCodeResult(500);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Put(IEnumerable<PutScheduledTaskDTO> taskDtos)
        {
            try
            {
                if (taskDtos == null)
                    return BadRequest();

                var tasks = _mapper.Map<IEnumerable<ScheduledTaskDataModel>>(taskDtos);

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