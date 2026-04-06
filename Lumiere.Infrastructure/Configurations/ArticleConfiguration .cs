using Lumiere.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lumiere.Infrastructure.Configurations;

public class ArticleConfiguration : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
        // Table
        builder.ToTable("Articles");

        // Primary Key
        builder.HasKey(a => a.Id);

        // Properties
        builder.Property(a => a.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(a => a.Description)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(a => a.ShortDescription)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(a => a.Price)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(a => a.ImageUrl)
            .HasMaxLength(1000);

        builder.Property(a => a.Material)
            .HasMaxLength(100);

        builder.Property(a => a.Gemstone)
            .HasMaxLength(100);

        builder.Property(a => a.Stock)
            .IsRequired();

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.Property(a => a.UpdatedAt);

        builder.Property(a => a.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(a => a.DeletedAt);

        // Indexes
        builder.HasIndex(a => a.CategoryId)
            .HasDatabaseName("IX_Articles_CategoryId");

        builder.HasIndex(a => a.IsDeleted)
            .HasDatabaseName("IX_Articles_IsDeleted");

        builder.HasIndex(a => a.Price)
            .HasDatabaseName("IX_Articles_Price");

        builder.HasIndex(a => a.Stock)
            .HasDatabaseName("IX_Articles_Stock");

        // Relationships
        builder.HasOne(a => a.Category)
            .WithMany(c => c.Articles)
            .HasForeignKey(a => a.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(a => a.FavoriteArticles)
            .WithOne(f => f.Article)
            .HasForeignKey(f => f.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
