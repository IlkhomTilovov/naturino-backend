using Microsoft.Extensions.DependencyInjection;

namespace Naturino.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Feature service implementations (AuthService, MediaService, PageService) are
        // registered in Naturino.Infrastructure.DependencyInjection, since they depend
        // directly on ApplicationDbContext.
        return services;
    }
}