namespace Lumiere.Domain.Entities;

public partial class Article
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string ShortDescription { get; set; } = null!;

    public decimal Price { get; set; }

    public string? ImageUrl { get; set; }

    public string? Material { get; set; }

    public string? Gemstone { get; set; }

    public int Stock { get; set; }

    public int CategoryId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<FavoriteArticle> FavoriteArticles { get; set; } = new List<FavoriteArticle>();
}
