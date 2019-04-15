using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Planner.Api.Extensions;
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
    public class ScheduledTaskController : ControllerBase
    {
        private readonly IScheduledTaskRepository _scheduledTaskRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ScheduledTaskController> _logger;

        public ScheduledTaskController(IScheduledTaskRepository scheduledTaskRepo
            , IUnitOfWork unitOfWork
            , IMapper mapper
            , ILogger<ScheduledTaskController> logger)
        {
            _scheduledTaskRepo = scheduledTaskRepo;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/ScheduledTask
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var tasks = await _scheduledTaskRepo.GetScheduledTasksForUser(User.GetUserId());

                return Ok(_mapper.Map<IEnumerable<GetScheduledTaskDTO>>(tasks));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Threw exception while retrieving ScheduledTasks: { ex }");
                return new StatusCodeResult(500);
            }
        }

        // GET: api/ScheduledTask/1
        [HttpGet("{id}", Name = "TaskGet")]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                var task = await _scheduledTaskRepo.GetByIdAsync(id);

                if (task == null)
                    return NotFound($"Sheduled Task {id} was not found");

                return Ok(_mapper.Map<GetScheduledTaskDTO>(task));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Threw exception while retrieving ScheduledTask: { ex }");
                return new StatusCodeResult(500);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]PostScheduledTaskDTO task)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var entity = _mapper.Map<ScheduledTask>(task);

                    await CreateScheduledTaskAsync(entity);

                    var uri = Url.Link("TaskGet", new { entity.Id });

                    return Created(uri, _mapper.Map<GetScheduledTaskDTO>(entity));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Threw exception while creating ScheduledTask: { ex }");
                }
            }

            return BadRequest();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody]PutScheduledTaskDTO taskDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var task = await _scheduledTaskRepo.FindAsync(id);

                    if (task == null)
                    {
                        var entity = _mapper.Map<ScheduledTask>(taskDto);
                        entity.ApplicationUserId = User.GetUserId();

                        await CreateScheduledTaskAsync(entity);

                        return Ok(_mapper.Map<GetScheduledTaskDTO>(entity));
                    }
                        
                    _mapper.Map(taskDto, task);

                    await _unitOfWork.CompleteAsync();

                    return Ok(_mapper.Map<GetScheduledTaskDTO>(task));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Threw exception while creating ScheduledTask: { ex }");
                }
            }

            return BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var task = await _scheduledTaskRepo.FindAsync(id);

                if (task == null)
                    return NotFound($"Sheduled Task {id} was not found");

                _scheduledTaskRepo.Delete(task);

                await _unitOfWork.CompleteAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Threw exception while creating ScheduledTask: { ex }");
            }

            return BadRequest();
        }

        private async Task CreateScheduledTaskAsync(ScheduledTask entity)
        {
            await _scheduledTaskRepo.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
        }
    }
}
