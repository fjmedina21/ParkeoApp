using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ParkeoApp.Domain.Entities;
using ParkeoApp.Infrastructure;

namespace ParkeoApp.Infrastructure.Data;

public partial class ParkeoAppContext : DbContext
{
    public ParkeoAppContext(DbContextOptions<ParkeoAppContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ParkingLot> ParkingLots { get; set; }

    public virtual DbSet<ParkingSpot> ParkingSpots { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolesPermission> RolesPermissions { get; set; }

    public virtual DbSet<Tenant> Tenants { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<UsersToken> UsersTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<ParkingLot>(entity =>
        {
            entity.HasKey(e => e.ParkingLotId).HasName("parking_lots_pkey");

            entity.ToTable("parking_lots");

            entity.HasIndex(e => e.TenantId, "idx_parking_lots_tenant");

            entity.Property(e => e.ParkingLotId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("parking_lot_id");
            entity.Property(e => e.Address)
                .HasMaxLength(300)
                .HasColumnName("address");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.FloorLevels)
                .HasDefaultValue(1)
                .HasColumnName("floor_levels");
            entity.Property(e => e.HourlyRate)
                .HasPrecision(18, 2)
                .HasColumnName("hourly_rate");
            entity.Property(e => e.Latitude).HasColumnName("latitude");
            entity.Property(e => e.Longitude).HasColumnName("longitude");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
            entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Tenant).WithMany(p => p.ParkingLots)
                .HasForeignKey(d => d.TenantId)
                .HasConstraintName("parking_lots_tenant_id_fkey");
        });

        modelBuilder.Entity<ParkingSpot>(entity =>
        {
            entity.HasKey(e => e.SpotId).HasName("parking_spots_pkey");

            entity.ToTable("parking_spots");

            entity.HasIndex(e => new { e.ParkingLotId, e.Code }, "ux_spots_lot_code").IsUnique();

            entity.Property(e => e.SpotId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("spot_id");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("code");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.Floor)
                .HasDefaultValue(0)
                .HasColumnName("floor");
            entity.Property(e => e.ParkingLotId).HasColumnName("parking_lot_id");
            entity.Property(e => e.SpotType)
                .HasMaxLength(50)
                .HasDefaultValueSql("'STANDARD'::character varying")
                .HasColumnName("spot_type");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'Available'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.ParkingLot).WithMany(p => p.ParkingSpots)
                .HasForeignKey(d => d.ParkingLotId)
                .HasConstraintName("parking_spots_parking_lot_id_fkey");

            entity.HasOne(d => d.Tenant).WithMany(p => p.ParkingSpots)
                .HasForeignKey(d => d.TenantId)
                .HasConstraintName("parking_spots_tenant_id_fkey");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("payments_pkey");

            entity.ToTable("payments");

            entity.HasIndex(e => new { e.TenantId, e.Status }, "idx_payments_tenant_status");

            entity.Property(e => e.PaymentId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("payment_id");
            entity.Property(e => e.Amount)
                .HasPrecision(12, 2)
                .HasColumnName("amount");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.Currency)
                .HasMaxLength(10)
                .HasColumnName("currency");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .HasColumnName("payment_method");
            entity.Property(e => e.ReservationId).HasColumnName("reservation_id");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'PENDING'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            entity.Property(e => e.TransactionId)
                .HasMaxLength(100)
                .HasColumnName("transaction_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Reservation).WithMany(p => p.Payments)
                .HasForeignKey(d => d.ReservationId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("payments_reservation_id_fkey");

            entity.HasOne(d => d.Tenant).WithMany(p => p.Payments)
                .HasForeignKey(d => d.TenantId)
                .HasConstraintName("payments_tenant_id_fkey");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.PermissionId).HasName("permissions_pkey");

            entity.ToTable("permissions");

            entity.HasIndex(e => new { e.TenantId, e.Name }, "ux_permissions_tenant_name").IsUnique();

            entity.Property(e => e.PermissionId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("permission_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Tenant).WithMany(p => p.Permissions)
                .HasForeignKey(d => d.TenantId)
                .HasConstraintName("permissions_tenant_id_fkey");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.ReservationId).HasName("reservations_pkey");

            entity.ToTable("reservations");

            entity.HasIndex(e => new { e.TenantId, e.Code }, "ux_reservations_tenant_code").IsUnique();

            entity.Property(e => e.ReservationId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("reservation_id");
            entity.Property(e => e.Code)
                .HasMaxLength(100)
                .HasColumnName("code");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.EndAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("end_at");
            entity.Property(e => e.SpotId).HasColumnName("spot_id");
            entity.Property(e => e.StartAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("start_at");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'Reserved'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            entity.Property(e => e.TotalCost)
                .HasPrecision(18, 2)
                .HasColumnName("total_cost");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Spot).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.SpotId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("reservations_spot_id_fkey");

            entity.HasOne(d => d.Tenant).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.TenantId)
                .HasConstraintName("reservations_tenant_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("reservations_user_id_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("roles_pkey");

            entity.ToTable("roles");

            entity.HasIndex(e => new { e.TenantId, e.Name }, "ux_roles_tenant_name").IsUnique();

            entity.Property(e => e.RoleId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("role_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Tenant).WithMany(p => p.Roles)
                .HasForeignKey(d => d.TenantId)
                .HasConstraintName("roles_tenant_id_fkey");
        });

        modelBuilder.Entity<RolesPermission>(entity =>
        {
            entity.HasKey(e => e.RolePermissionId).HasName("roles_permissions_pkey");

            entity.ToTable("roles_permissions");

            entity.Property(e => e.RolePermissionId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("role_permission_id");
            entity.Property(e => e.AssignedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("assigned_at");
            entity.Property(e => e.PermissionId).HasColumnName("permission_id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Permission).WithMany(p => p.RolesPermissions)
                .HasForeignKey(d => d.PermissionId)
                .HasConstraintName("roles_permissions_permission_id_fkey");

            entity.HasOne(d => d.Role).WithMany(p => p.RolesPermissions)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("roles_permissions_role_id_fkey");
        });

        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.HasKey(e => e.TenantId).HasName("tenants_pkey");

            entity.ToTable("tenants");

            entity.HasIndex(e => e.Name, "idx_tenants_name");

            entity.Property(e => e.TenantId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("tenant_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.Domain)
                .HasMaxLength(100)
                .HasColumnName("domain");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => new { e.TenantId, e.Email }, "ux_users_tenant_email").IsUnique();

            entity.Property(e => e.UserId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.Email)
                .HasMaxLength(256)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(200)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(200)
                .HasColumnName("last_name");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(500)
                .HasColumnName("password_hash");
            entity.Property(e => e.ProfilePictureUrl).HasColumnName("profile_picture_url");
            entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Tenant).WithMany(p => p.Users)
                .HasForeignKey(d => d.TenantId)
                .HasConstraintName("users_tenant_id_fkey");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.UserRoleId).HasName("user_roles_pkey");

            entity.ToTable("user_roles");

            entity.Property(e => e.UserRoleId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("user_role_id");
            entity.Property(e => e.AssignedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("assigned_at");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("user_roles_role_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("user_roles_user_id_fkey");
        });

        modelBuilder.Entity<UsersToken>(entity =>
        {
            entity.HasKey(e => e.TokenId).HasName("users_tokens_pkey");

            entity.ToTable("users_tokens");

            entity.HasIndex(e => e.TenantId, "idx_users_tokens_tenant");

            entity.HasIndex(e => e.UserId, "idx_users_tokens_user");

            entity.Property(e => e.TokenId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("token_id");
            entity.Property(e => e.AccessExpiresAt).HasColumnName("access_expires_at");
            entity.Property(e => e.AccessToken)
                .HasMaxLength(500)
                .HasColumnName("access_token");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.RefreshExpiresAt).HasColumnName("refresh_expires_at");
            entity.Property(e => e.RefreshToken)
                .HasMaxLength(500)
                .HasColumnName("refresh_token");
            entity.Property(e => e.RevokedAt).HasColumnName("revoked_at");
            entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Tenant).WithMany(p => p.UsersTokens)
                .HasForeignKey(d => d.TenantId)
                .HasConstraintName("users_tokens_tenant_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.UsersTokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("users_tokens_user_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
