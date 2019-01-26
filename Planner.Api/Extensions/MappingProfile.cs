using AutoMapper;
using Planner.Domain.Entities;
using Planner.Dto;

namespace Planner.Api.Extensions
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ScheduledTask, ScheduledTaskDTO>();
        }
    }
}
