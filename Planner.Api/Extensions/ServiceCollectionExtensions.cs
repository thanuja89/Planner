using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Planner.Api.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAutoMapperWithProfile(this IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();

            var accessor = provider.GetService<IHttpContextAccessor>();

            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile(new MappingProfile(accessor));
            });

            config.CompileMappings();

            var mapper = config.CreateMapper();

            services.AddSingleton(mapper);

            return services;
        }
    }
}
