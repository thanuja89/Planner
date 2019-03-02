using AutoMapper;
using Microsoft.AspNetCore.Http;
using Planner.Domain.Entities;
using Planner.Dto;
using System;

namespace Planner.Api.Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile(IHttpContextAccessor accessor)
        {
            CreateMap<ScheduledTask, GetScheduledTaskDTO>();
            CreateMap<PostScheduledTaskDTO, ScheduledTask>()
                .ForMember(t => t.CreatedOnUtc, opt => opt.MapFrom(t => DateTime.UtcNow))
                .ForMember(t => t.UpdatedOnUtc, opt => opt.MapFrom(t => DateTime.UtcNow))
                .ForMember(t => t.ApplicationUserId, opt => opt.MapFrom(new IdentityResolver(accessor)));
            CreateMap<PutScheduledTaskDTO, ScheduledTask>()
                .ForMember(t => t.UpdatedOnUtc, opt => opt.MapFrom(t => DateTime.UtcNow));
        }
    }
}
