using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ParkeoApp.Api.Models.Entities;

namespace ParkeoApp.Api.Data;

public partial class ParkeoAppContext : DbContext
{
    public ParkeoAppContext()
    {
    }

    public ParkeoAppContext(DbContextOptions<ParkeoAppContext> options)
        : base(options)
    {
    }

    public virtual DbSet<parking_lot> parking_lots { get; set; }

    public virtual DbSet<parking_spot> parking_spots { get; set; }

    public virtual DbSet<payment> payments { get; set; }

    public virtual DbSet<reservation> reservations { get; set; }

    public virtual DbSet<role> roles { get; set; }

    public virtual DbSet<tenant> tenants { get; set; }

    public virtual DbSet<user> users { get; set; }

    public virtual DbSet<user_role> user_roles { get; set; }

    public virtual DbSet<users_token> users_tokens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=nozomi.proxy.rlwy.net;Port=44750;Database=parkeoapp;Username=postgres;Password=OnUwQKWybAtcXkWCOzlnLjvAZqebwhNs;Pooling=true;SSL Mode=Disable;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<parking_lot>(entity =>
        {
            entity.HasKey(e => e.parking_lot_id).HasName("parking_lots_pkey");

            entity.Property(e => e.parking_lot_id).HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.created_at).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.updated_at).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.tenant).WithMany(p => p.parking_lots).HasConstraintName("parking_lots_tenant_id_fkey");
        });

        modelBuilder.Entity<parking_spot>(entity =>
        {
            entity.HasKey(e => e.spot_id).HasName("parking_spots_pkey");

            entity.Property(e => e.spot_id).HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.created_at).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.floor).HasDefaultValue(0);
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.spottype).HasDefaultValueSql("'STANDARD'::character varying");
            entity.Property(e => e.updated_at).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.parking_lot).WithMany(p => p.parking_spots).HasConstraintName("parking_spots_parking_lot_id_fkey");

            entity.HasOne(d => d.tenant).WithMany(p => p.parking_spots).HasConstraintName("parking_spots_tenant_id_fkey");
        });

        modelBuilder.Entity<payment>(entity =>
        {
            entity.HasKey(e => e.payment_id).HasName("payments_pkey");

            entity.Property(e => e.payment_id).HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.created_at).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.status).HasDefaultValueSql("'PENDING'::character varying");
            entity.Property(e => e.updated_at).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.reservation).WithMany(p => p.payments)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("payments_reservation_id_fkey");

            entity.HasOne(d => d.tenant).WithMany(p => p.payments).HasConstraintName("payments_tenant_id_fkey");
        });

        modelBuilder.Entity<reservation>(entity =>
        {
            entity.HasKey(e => e.reservation_id).HasName("reservations_pkey");

            entity.Property(e => e.reservation_id).HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.created_at).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.status).HasDefaultValueSql("'RESERVED'::character varying");
            entity.Property(e => e.updated_at).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.spot).WithMany(p => p.reservations)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("reservations_spot_id_fkey");

            entity.HasOne(d => d.tenant).WithMany(p => p.reservations).HasConstraintName("reservations_tenant_id_fkey");

            entity.HasOne(d => d.user).WithMany(p => p.reservations).HasConstraintName("reservations_user_id_fkey");
        });

        modelBuilder.Entity<role>(entity =>
        {
            entity.HasKey(e => e.role_id).HasName("roles_pkey");

            entity.Property(e => e.role_id).HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.created_at).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.updated_at).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.tenant).WithMany(p => p.roles).HasConstraintName("roles_tenant_id_fkey");
        });

        modelBuilder.Entity<tenant>(entity =>
        {
            entity.HasKey(e => e.tenant_id).HasName("tenants_pkey");

            entity.Property(e => e.tenant_id).HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.created_at).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.updated_at).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<user>(entity =>
        {
            entity.HasKey(e => e.user_id).HasName("users_pkey");

            entity.Property(e => e.user_id).HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.created_at).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.updated_at).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.tenant).WithMany(p => p.users).HasConstraintName("users_tenant_id_fkey");
        });

        modelBuilder.Entity<user_role>(entity =>
        {
            entity.HasKey(e => e.user_role_id).HasName("user_roles_pkey");

            entity.Property(e => e.user_role_id).HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.assigned_at).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.role).WithMany(p => p.user_roles).HasConstraintName("user_roles_role_id_fkey");

            entity.HasOne(d => d.user).WithMany(p => p.user_roles).HasConstraintName("user_roles_user_id_fkey");
        });

        modelBuilder.Entity<users_token>(entity =>
        {
            entity.HasKey(e => e.token_id).HasName("users_tokens_pkey");

            entity.Property(e => e.token_id).HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.created_at).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.is_active).HasDefaultValue(true);

            entity.HasOne(d => d.tenant).WithMany(p => p.users_tokens).HasConstraintName("users_tokens_tenant_id_fkey");

            entity.HasOne(d => d.user).WithMany(p => p.users_tokens).HasConstraintName("users_tokens_user_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
