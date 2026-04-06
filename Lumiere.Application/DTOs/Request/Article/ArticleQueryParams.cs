namespace Lumiere.Application.DTOs.Request.Article;

public class ArticleQueryParams
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Search { get; set; }
    public int? CategoryId { get; set; }
    public bool? InStock { get; set; }
    public string? SortBy { get; set; } = "createdAt";
    public string? SortDir { get; set; } = "desc";
}
