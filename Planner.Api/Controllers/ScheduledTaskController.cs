using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Planner.Api.Extensions;
using Planner.Domain.Repositories.Interfaces;
using Planner.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planner.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduledTaskController : ControllerBase
    {
        private readonly IScheduledTaskRepository _scheduledTaskRepo;
        private readonly IMapper _mapper;

        public ScheduledTaskController(IScheduledTaskRepository scheduledTaskRepo
            , IMapper mapper)
        {
            _scheduledTaskRepo = scheduledTaskRepo;
            _mapper = mapper;
        }

        // GET: api/ScheduledTask
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var tasks = await _scheduledTaskRepo.GetScheduledTasksForUser(User.GetUserId());
                var dtos = _mapper.Map<IEnumerable<ScheduledTaskDTO>>(tasks);

                return Ok(dtos);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
