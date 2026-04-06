namespace Lumiere.Application.DTOs.Request.Article;

public class UpdateArticleRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public string? Material { get; set; }
    public string? Gemstone { get; set; }
    public int Stock { get; set; } 
    public int CategoryId { get; set; }
}
