using Microsoft.EntityFrameworkCore;
using ParkeoApp.Domain.Entities;

namespace ParkeoApp.Infrastructure.Data;

public partial class ParkeoAppContext : DbContext
{
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<parking_lot>(entity =>
        {
            entity.HasKey(e => e.parking_lot_id).HasName("parking_lots_pkey");

            entity.HasIndex(e => e.tenant_id, "idx_parking_lots_tenant");

            entity.Property(e => e.parking_lot_id).HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.address).HasMaxLength(300);
            entity.Property(e => e.created_at).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.latitude).HasPrecision(9, 6);
            entity.Property(e => e.longitude).HasPrecision(9, 6);
            entity.Property(e => e.name).HasMaxLength(200);
            entity.Property(e => e.updated_at).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.tenant).WithMany(p => p.parking_lots)
                .HasForeignKey(d => d.tenant_id)
                .HasConstraintName("parking_lots_tenant_id_fkey");
        });

        modelBuilder.Entity<parking_spot>(entity =>
        {
            entity.HasKey(e => e.spot_id).HasName("parking_spots_pkey");

            entity.HasIndex(e => new { e.parking_lot_id, e.code }, "ux_spots_lot_code").IsUnique();

            entity.Property(e => e.spot_id).HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.code).HasMaxLength(50);
            entity.Property(e => e.created_at).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.floor).HasDefaultValue(0);
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.spottype)
                .HasMaxLength(50)
                .HasDefaultValueSql("'STANDARD'::character varying");
            entity.Property(e => e.status).HasMaxLength(50);
            entity.Property(e => e.updated_at).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.parking_lot).WithMany(p => p.parking_spots)
                .HasForeignKey(d => d.parking_lot_id)
                .HasConstraintName("parking_spots_parking_lot_id_fkey");

            entity.HasOne(d => d.tenant).WithMany(p => p.parking_spots)
                .HasForeignKey(d => d.tenant_id)
                .HasConstraintName("parking_spots_tenant_id_fkey");
        });

        modelBuilder.Entity<payment>(entity =>
        {
            entity.HasKey(e => e.payment_id).HasName("payments_pkey");

            entity.HasIndex(e => new { e.tenant_id, e.status }, "idx_payments_tenant_status");

            entity.Property(e => e.payment_id).HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.amount).HasPrecision(12, 2);
            entity.Property(e => e.created_at).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.currency).HasMaxLength(10);
            entity.Property(e => e.payment_method).HasMaxLength(50);
            entity.Property(e => e.status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'PENDING'::character varying");
            entity.Property(e => e.transaction_id).HasMaxLength(100);
            entity.Property(e => e.updated_at).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.reservation).WithMany(p => p.payments)
                .HasForeignKey(d => d.reservation_id)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("payments_reservation_id_fkey");

            entity.HasOne(d => d.tenant).WithMany(p => p.payments)
                .HasForeignKey(d => d.tenant_id)
                .HasConstraintName("payments_tenant_id_fkey");
        });

        modelBuilder.Entity<reservation>(entity =>
        {
            entity.HasKey(e => e.reservation_id).HasName("reservations_pkey");

            entity.HasIndex(e => new { e.tenant_id, e.code }, "ux_reservations_tenant_code").IsUnique();

            entity.Property(e => e.reservation_id).HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.code).HasMaxLength(100);
            entity.Property(e => e.created_at).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'RESERVED'::character varying");
            entity.Property(e => e.updated_at).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.spot).WithMany(p => p.reservations)
                .HasForeignKey(d => d.spot_id)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("reservations_spot_id_fkey");

            entity.HasOne(d => d.tenant).WithMany(p => p.reservations)
                .HasForeignKey(d => d.tenant_id)
                .HasConstraintName("reservations_tenant_id_fkey");

            entity.HasOne(d => d.user).WithMany(p => p.reservations)
                .HasForeignKey(d => d.user_id)
                .HasConstraintName("reservations_user_id_fkey");
        });

        modelBuilder.Entity<role>(entity =>
        {
            entity.HasKey(e => e.role_id).HasName("roles_pkey");

            entity.HasIndex(e => new { e.tenant_id, e.name }, "ux_roles_tenant_name").IsUnique();

            entity.Property(e => e.role_id).HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.created_at).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.name).HasMaxLength(100);
            entity.Property(e => e.updated_at).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.tenant).WithMany(p => p.roles)
                .HasForeignKey(d => d.tenant_id)
                .HasConstraintName("roles_tenant_id_fkey");
        });

        modelBuilder.Entity<tenant>(entity =>
        {
            entity.HasKey(e => e.tenant_id).HasName("tenants_pkey");

            entity.HasIndex(e => e.name, "idx_tenants_name");

            entity.Property(e => e.tenant_id).HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.created_at).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.domain).HasMaxLength(100);
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.name).HasMaxLength(200);
            entity.Property(e => e.updated_at).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<user>(entity =>
        {
            entity.HasKey(e => e.user_id).HasName("users_pkey");

            entity.HasIndex(e => new { e.tenant_id, e.email }, "ux_users_tenant_email").IsUnique();

            entity.Property(e => e.user_id).HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.created_at).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.email).HasMaxLength(256);
            entity.Property(e => e.firts_name).HasMaxLength(200);
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.last_name).HasMaxLength(200);
            entity.Property(e => e.password_hash).HasMaxLength(500);
            entity.Property(e => e.updated_at).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.tenant).WithMany(p => p.users)
                .HasForeignKey(d => d.tenant_id)
                .HasConstraintName("users_tenant_id_fkey");
        });

        modelBuilder.Entity<user_role>(entity =>
        {
            entity.HasKey(e => e.user_role_id).HasName("user_roles_pkey");

            entity.Property(e => e.user_role_id).HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.assigned_at).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.role).WithMany(p => p.user_roles)
                .HasForeignKey(d => d.role_id)
                .HasConstraintName("user_roles_role_id_fkey");

            entity.HasOne(d => d.user).WithMany(p => p.user_roles)
                .HasForeignKey(d => d.user_id)
                .HasConstraintName("user_roles_user_id_fkey");
        });

        modelBuilder.Entity<users_token>(entity =>
        {
            entity.HasKey(e => e.token_id).HasName("users_tokens_pkey");

            entity.HasIndex(e => e.tenant_id, "idx_users_tokens_tenant");

            entity.HasIndex(e => e.user_id, "idx_users_tokens_user");

            entity.HasIndex(e => new { e.user_id, e.is_active }, "idx_users_tokens_user_active");

            entity.Property(e => e.token_id).HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.access_token).HasMaxLength(500);
            entity.Property(e => e.created_at).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.refresh_token).HasMaxLength(500);

            entity.HasOne(d => d.tenant).WithMany(p => p.users_tokens)
                .HasForeignKey(d => d.tenant_id)
                .HasConstraintName("users_tokens_tenant_id_fkey");

            entity.HasOne(d => d.user).WithMany(p => p.users_tokens)
                .HasForeignKey(d => d.user_id)
                .HasConstraintName("users_tokens_user_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
