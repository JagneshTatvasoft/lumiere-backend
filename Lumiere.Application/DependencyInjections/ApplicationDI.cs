using AutoMapper;
using FluentValidation;
using Lumiere.Application.Interfaces.Services;
using Lumiere.Application.Mappings;
using Lumiere.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Lumiere.Application.DependencyInjections;

public static class ApplicationDI
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register AutoMapper
        // This scans the Application assembly for your Mapping Profiles (like CustomerProfile)
        services.AddAutoMapper(typeof(IAutoMapper).Assembly);
        // services.AddAutoMapper(typeof(ApplicationAssemblyMarker).Assembly);
    //     var mapperConfig = new MapperConfiguration(cfg =>
    //    {
    //        cfg.AddMaps(typeof(ApplicationAssemblyMarker).Assembly);
    //    },null);

    //     mapperConfig.AssertConfigurationIsValid(); // catches profile errors at startup

    //     services.AddSingleton(mapperConfig);
    //     services.AddSingleton<IMapper>(sp =>
    //         sp.GetRequiredService<MapperConfiguration>().CreateMapper());
        services.AddValidatorsFromAssembly(typeof(ApplicationDI).Assembly);


        // Register all Services (Business Logic)

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IArticleService, ArticleService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IFavoriteService, FavoriteService>();
        services.AddScoped<IDashboardService, DashboardService>();

        return services;
    }
}
