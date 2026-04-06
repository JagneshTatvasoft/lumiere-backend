namespace Lumiere.Domain.Entities;

public partial class User
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? Country { get; set; }

    public int RoleId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }
    public virtual ICollection<FavoriteArticle> FavoriteArticles { get; set; } = new List<FavoriteArticle>();

    public virtual Role Role { get; set; } = null!;
}
