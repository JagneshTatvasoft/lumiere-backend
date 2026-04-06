namespace Lumiere.Application.DTOs.Request.Category;

public class UpdateCategoryRequest
{
     public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string? Description { get; set; }
}
