namespace Lumiere.Application.DTOs.Response.Category;

public class CategoryResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string? Description { get; set; }
    public int ArticleCount { get; set; }
}
