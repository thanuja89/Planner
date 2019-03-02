using AutoMapper;
using Microsoft.AspNetCore.Http;
using Planner.Domain.Entities;
using Planner.Dto;

namespace Planner.Api.Services
{
    public class IdentityResolver : IValueResolver<PostScheduledTaskDTO, ScheduledTask, string>
    {
        private IHttpContextAccessor _httpContextAccessor;

        public IdentityResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string Resolve(PostScheduledTaskDTO source, ScheduledTask destination, string destMember, ResolutionContext context)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.GetUserId();

            return userId;
        }
    }
}
