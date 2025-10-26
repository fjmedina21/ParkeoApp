using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ParkeoApp.Api.Models.Entities;

[Index("tenant_id", "email", Name = "ux_users_tenant_email", IsUnique = true)]
public partial class user
{
    [Key]
    public Guid user_id { get; set; }

    public Guid tenant_id { get; set; }

    [StringLength(200)]
    public string firts_name { get; set; } = null!;

    [StringLength(200)]
    public string last_name { get; set; } = null!;

    [StringLength(256)]
    public string email { get; set; } = null!;

    [StringLength(500)]
    public string password_hash { get; set; } = null!;

    public string? profile_picture_url { get; set; }

    public bool is_active { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public DateTime? deleted_at { get; set; }

    [InverseProperty("user")]
    public virtual ICollection<reservation> reservations { get; set; } = new List<reservation>();

    [ForeignKey("tenant_id")]
    [InverseProperty("users")]
    public virtual tenant tenant { get; set; } = null!;

    [InverseProperty("user")]
    public virtual ICollection<user_role> user_roles { get; set; } = new List<user_role>();

    [InverseProperty("user")]
    public virtual ICollection<users_token> users_tokens { get; set; } = new List<users_token>();
}
