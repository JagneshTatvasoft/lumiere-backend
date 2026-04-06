using Lumiere.Domain;

namespace Lumiere.Application.DTOs.Response.Favorite;

public class FavoriteResponse
{
     public int Id { get; set; }
    public int UserId { get; set; }
    public int ArticleId { get; set; }
    public string ArticleName { get; set; } = string.Empty;
    public string? ArticleImageUrl { get; set; }
    public decimal ArticlePrice { get; set; }
    public ReactionType ReactionType { get; set; }
    public DateTime CreatedAt { get; set; }
}
