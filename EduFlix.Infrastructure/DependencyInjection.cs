using EduFlix.Application;
using Microsoft.Extensions.DependencyInjection;

namespace EduFlix.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IBlobStorage, BlobStorage>();
        services.AddScoped<IVideoService, VideoService>();
        return services;
    }
}
