using Lumiere.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Lumiere.Tests.Integration.Setup;

public class TestWebAppFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        string dbName = $"LumiereTestDb_{Guid.NewGuid()}";

        // builder.ConfigureAppConfiguration((context, config) =>
        // {
        //     // This injects a dummy secret specifically for the test run
        //     config.AddInMemoryCollection(new Dictionary<string, string?>
        //     {
        //         // THIS MUST MATCH YOUR SERVICE FILE EXACTLY
        //         ["JwtSettings:Secret"] = "SuperSecretTestingKey12345678901234567890",
        //         ["JwtSettings:Issuer"] = "LumiereJewelry",
        //         ["JwtSettings:Audience"] = "LumiereJewelry"
        //     });
        // });

        builder.ConfigureServices(services =>
        {
            // Step 1: Remove the real DbContext registration

            RemoveService<DbContextOptions<LumiereJewelryDBContext>>(services);
            RemoveService<LumiereJewelryDBContext>(services);

            // var descriptor = services.SingleOrDefault(
            //     d => d.ServiceType == typeof(DbContextOptions<LumiereJewelryDBContext>));
            // if (descriptor != null)
            //     services.Remove(descriptor);

            var efDescriptors = services
               .Where(d => d.ServiceType.FullName != null &&
                           d.ServiceType.FullName.Contains("EntityFrameworkCore"))
               .ToList();

            foreach (var descriptor in efDescriptors)
                services.Remove(descriptor);

            // Step 2: Replace with in-memory database
            // Each test run gets a fresh DB with a unique name
            services.AddDbContext<LumiereJewelryDBContext>(options =>
            {
                options.UseInMemoryDatabase(dbName);
            });

            // Step 3: Build the service provider and seed test data
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<LumiereJewelryDBContext>();

            db.Database.EnsureCreated();
            SeedTestData(db);
        });

        // Use test environment — loads appsettings.Testing.json if it exists
        builder.UseEnvironment("Testing");
    }

    private static void RemoveService<T>(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(
            d => d.ServiceType == typeof(T));
        if (descriptor != null)
            services.Remove(descriptor);
    }

    // Seed minimum data every integration test needs
    private static void SeedTestData(LumiereJewelryDBContext db)
    {
        // Roles
        db.Roles.AddRange(
            new Domain.Entities.Role { Id = 1, Name = "User" },
            new Domain.Entities.Role { Id = 2, Name = "Admin" }
        );

        // Admin user — password is "Admin@123"
        db.Users.Add(new Domain.Entities.User
        {
            Id = 1,
            Name = "Test Admin",
            Email = "admin@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            RoleId = 2,
            Country = "IN",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null,
            IsDeleted = false,
            DeletedAt = null,
        });

        // Regular user — password is "User@123"
        db.Users.Add(new Domain.Entities.User
        {
            Id = 2,
            Name = "Test User",
            Email = "user@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123"),
            RoleId = 1,
            Country = "IN",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null,
            IsDeleted = false,
            DeletedAt = null,
        });

        // Category
        db.Categories.Add(new Domain.Entities.Category
        {
            Id = 1,
            Name = "Rings",
            Slug = "rings",
            Description = "",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null,
            IsDeleted = false,
            DeletedAt = null,
        });

        // Article
        db.Articles.Add(new Domain.Entities.Article
        {
            Id = 1,
            Name = "Gold Ring",
            Description = "",
            ShortDescription = "A beautiful gold ring",
            Price = 499.99m,
            Stock = 10,
            ImageUrl = null,
            Material = null,
            Gemstone = null,
            CategoryId = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null,
            IsDeleted = false,
            DeletedAt = null,
        });

        db.SaveChanges();
    }
}
