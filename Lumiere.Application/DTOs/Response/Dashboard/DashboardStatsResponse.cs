namespace Lumiere.Application.DTOs.Response.Dashboard;

public class DashboardStatsResponse
{
     public int TotalArticles { get; set; }
    public int TotalUsers { get; set; }
    public int TotalLikes { get; set; }
    public int TotalCategories { get; set; }
    public List<CategoryStatResponse> CategoryStats { get; set; } = new();
}
