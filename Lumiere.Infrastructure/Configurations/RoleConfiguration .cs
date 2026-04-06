using Lumiere.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        // Table
        builder.ToTable("Roles");

        // Primary Key
        builder.HasKey(r => r.Id);

        // Properties
        builder.Property(r => r.Name)
            .HasMaxLength(50)
            .IsRequired();

        // Index (Unique)
        builder.HasIndex(r => r.Name)
            .IsUnique()
            .HasDatabaseName("UQ_Roles_Name");

        // Relationships

        // Role -> Users (One-to-Many)
        builder.HasMany(r => r.Users)
            .WithOne(u => u.Role)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}