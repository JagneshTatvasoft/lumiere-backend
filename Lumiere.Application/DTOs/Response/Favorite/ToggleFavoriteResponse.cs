using Lumiere.Domain;

namespace Lumiere.Application.DTOs.Response.Favorite;

public class ToggleFavoriteResponse
{
    public bool IsLiked { get; set; }
    public bool IsDisliked { get; set; }
    public bool WasRemoved { get; set; }
    public ReactionType? CurrentReaction { get; set; }
}
