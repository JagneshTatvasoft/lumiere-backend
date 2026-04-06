using Lumiere.Domain;

namespace Lumiere.Application.DTOs.Request.Favorite;

public class ToggleFavoriteRequest
{
    public int UserId { get; set; }
    public int ArticleId { get; set; }
    public ReactionType ReactionType { get; set; }
}
