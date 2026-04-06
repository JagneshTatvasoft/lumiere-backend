using Lumiere.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lumiere.Infrastructure.Configurations;

public class FavoriteArticleConfiguration : IEntityTypeConfiguration<FavoriteArticle>
{
    public void Configure(EntityTypeBuilder<FavoriteArticle> builder)
    {
        // Table
        builder.ToTable("FavoriteArticles");

        // Primary Key
        builder.HasKey(f => f.Id);

        // Properties
        builder.Property(f => f.UserId)
            .IsRequired();

        builder.Property(f => f.ArticleId)
            .IsRequired();

        builder.Property(f => f.ReactionType)
            .IsRequired();

        builder.Property(f => f.CreatedAt)
            .IsRequired();

        builder.Property(f => f.UpdatedAt);

        // Indexes
        builder.HasIndex(f => f.ArticleId)
            .HasDatabaseName("IX_FavoriteArticles_ArticleId");

        builder.HasIndex(f => f.UserId)
            .HasDatabaseName("IX_FavoriteArticles_UserId");

        builder.HasIndex(f => new { f.UserId, f.ArticleId })
            .IsUnique()
            .HasDatabaseName("UQ_Favorites_UserArticle");

        // Relationships

        // FavoriteArticle -> Article (Many-to-One)
        builder.HasOne(f => f.Article)
            .WithMany(a => a.FavoriteArticles)
            .HasForeignKey(f => f.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        // FavoriteArticle -> User (Many-to-One)
        builder.HasOne(f => f.User)
            .WithMany(u => u.FavoriteArticles)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}