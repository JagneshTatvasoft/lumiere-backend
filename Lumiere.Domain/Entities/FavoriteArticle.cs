namespace Lumiere.Domain.Entities;

public partial class FavoriteArticle
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int ArticleId { get; set; }

    public ReactionType ReactionType { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Article Article { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}