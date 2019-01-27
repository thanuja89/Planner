using AutoMapper;
using Planner.Domain.Entities;
using Planner.Dto;
using System;

namespace Planner.Api.Extensions
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ScheduledTask, GetScheduledTaskDTO>();
            CreateMap<PostScheduledTaskDTO, ScheduledTask>()
                .ForMember(t => t.CreatedOnUtc, opt => opt.MapFrom(t => DateTime.UtcNow));
            CreateMap<PutScheduledTaskDTO, ScheduledTask>()
                .ForMember(t => t.UpdatedOnUtc, opt => opt.MapFrom(t => DateTime.UtcNow));
        }
    }
}
