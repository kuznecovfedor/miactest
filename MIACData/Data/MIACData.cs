﻿using System;
using System.Collections.Generic;
using MIACData.Models;
using Microsoft.EntityFrameworkCore;

namespace MIACData.Data;

public partial class MIACData : DbContext
{
    public MIACData()
    {
    }

    public MIACData(DbContextOptions<MIACData> options)
        : base(options)
    {
    }

    public virtual DbSet<Material> Materials { get; set; }

    public virtual DbSet<Seller> Sellers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=hrujz.ddns.net;Port=65432;Database=miac_test;Username=miac;Password=9zs2SBY4F");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Material>(entity =>
        {
            entity.HasKey(e => e.IdMaterial).HasName("pk_material");

            entity.ToTable("material");

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

            entity.Property(e => e.IdSeller)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_seller");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
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
