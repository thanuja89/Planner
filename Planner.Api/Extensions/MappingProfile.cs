using AutoMapper;
using Microsoft.AspNetCore.Http;
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
                .ForMember(t => t.CreatedOnUtc, opt => opt.MapFrom(t => DateTime.UtcNow))
                .ForMember(t => t.ApplicationUserId, opt => opt.MapFrom<IdentityResolver>());
            CreateMap<PutScheduledTaskDTO, ScheduledTask>()
                .ForMember(t => t.UpdatedOnUtc, opt => opt.MapFrom(t => DateTime.UtcNow));           
        }
    }
}
