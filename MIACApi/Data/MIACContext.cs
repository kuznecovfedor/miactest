using System;
using System.Collections.Generic;
using MIACApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MIACApi.Data;

public partial class MIACContext : DbContext
{
    public MIACContext()
    {
    }

    public MIACContext(DbContextOptions<MIACContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Material> Materials { get; set; }

    public virtual DbSet<Seller> Sellers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Material>(entity =>
        {
            entity.HasKey(e => e.IdMaterial).HasName("pk_material");

            entity.ToTable("material");

            entity.HasIndex(e => new { e.IdSeller, e.Name }, "uq_id_seller_name").IsUnique();

            entity.Property(e => e.IdMaterial)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_material");
            entity.Property(e => e.IdSeller).HasColumnName("id_seller");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasPrecision(12, 2)
                .HasColumnName("price");

            entity.HasOne(d => d.IdSellerNavigation).WithMany(p => p.Materials)
                .HasForeignKey(d => d.IdSeller)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_seller_material");
        });

        modelBuilder.Entity<Seller>(entity =>
        {
            entity.HasKey(e => e.IdSeller).HasName("pk_seller");

            entity.ToTable("seller");

            entity.HasIndex(e => e.Login, "uq_login").IsUnique();

            entity.Property(e => e.IdSeller)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_seller");
            entity.Property(e => e.Login)
                .HasMaxLength(30)
                .HasColumnName("login");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(60)
                .HasColumnName("password_hash");
            entity.Property(e => e.Patronymic)
                .HasMaxLength(100)
                .HasColumnName("patronymic");
            entity.Property(e => e.RegistrationDate)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("registration_date");
            entity.Property(e => e.Surname)
                .HasMaxLength(100)
                .HasColumnName("surname");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
