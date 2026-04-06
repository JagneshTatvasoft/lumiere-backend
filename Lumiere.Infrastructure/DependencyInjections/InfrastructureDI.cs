using Lumiere.Application.Interfaces.Repositoies;
using Lumiere.Application.Interfaces.Services;
using Lumiere.Infrastructure.Authentications;
using Lumiere.Infrastructure.Data;
using Lumiere.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lumiere.Infrastructure.DependencyInjections;

public static class InfrastructureDI
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<LumiereJewelryDBContext>(options =>
                   options.UseSqlServer(configuration.GetConnectionString("LumiereJewelryDB")));


        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Individual repositories (optional — UoW is preferred)
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IArticleRepository, ArticleRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IFavoriteRepository, FavoriteRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();

        return services;
    }
}
