using System;
using System.Collections.Generic;
using Lumiere.Domain;
using Lumiere.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lumiere.Infrastructure.Data;

public partial class LumiereJewelryDBContext : DbContext
{
    public LumiereJewelryDBContext(DbContextOptions<LumiereJewelryDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Article> Articles { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<FavoriteArticle> FavoriteArticles { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Articles__3214EC077A39C122");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.Category).WithMany(p => p.Articles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Articles_Categories");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC07AAA0FEEC");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
        });

        modelBuilder.Entity<FavoriteArticle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Favorite__3214EC07627F0ECA");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.Article).WithMany(p => p.FavoriteArticles).HasConstraintName("FK_Favorites_Articles");

            entity.HasOne(d => d.User).WithMany(p => p.FavoriteArticles).HasConstraintName("FK_Favorites_Users");
        });

        modelBuilder.Entity<FavoriteArticle>()
        .Property(x => x.ReactionType)
        .HasConversion(
            v => v == ReactionType.Like,                      // enum → bool (DB)
            v => v ? ReactionType.Like : ReactionType.Dislike // bool → enum (C#)
        );

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC072D07694B");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07E7AF5A92");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
