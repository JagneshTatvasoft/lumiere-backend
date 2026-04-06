using Lumiere.Application.DTOs.Response;
using Lumiere.Application.DTOs.Response.Dashboard;
using Lumiere.Application.Interfaces.Repositoies;
using Lumiere.Application.Interfaces.Services;
using Lumiere.Domain;

namespace Lumiere.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _uow;
 
    public DashboardService(IUnitOfWork uow)
    {
        _uow = uow;
    }
 
    public async Task<ApiResponse<DashboardStatsResponse>> GetStatsAsync(CancellationToken ct = default)
    {
        var articles = await _uow.Articles.GetAllAsync(ct);
        var users = await _uow.Users.GetAllAsync(ct);
        var favorites = await _uow.Favorites.GetAllAsync(ct);
        var categories = await _uow.Categories.GetAllAsync(ct);
 
        var activeArticles = articles.Where(a => !a.IsDeleted).ToList();
        var activeUsers = users.Where(u => !u.IsDeleted).ToList();
        var activeCategories = categories.Where(c => !c.IsDeleted).ToList();
        var totalLikes = favorites.Count(f => f.ReactionType == ReactionType.Like);
 
        var categoryStats = activeCategories.Select(cat =>
        {
            var count = activeArticles.Count(a => a.CategoryId == cat.Id);
            return new CategoryStatResponse
            {
                Name = cat.Name,
                Count = count,
                Percentage = activeArticles.Count > 0
                    ? (int)Math.Round((double)count / activeArticles.Count * 100)
                    : 0
            };
        }).ToList();
 
        return ApiResponse<DashboardStatsResponse>.Ok(new DashboardStatsResponse
        {
            TotalArticles = activeArticles.Count,
            TotalUsers = activeUsers.Count,
            TotalLikes = totalLikes,
            TotalCategories = activeCategories.Count,
            CategoryStats = categoryStats
        });
    }
}