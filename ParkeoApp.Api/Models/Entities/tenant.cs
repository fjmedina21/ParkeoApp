using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ParkeoApp.Api.Models.Entities;

[Index("name", Name = "idx_tenants_name")]
public partial class tenant
{
    [Key]
    public Guid tenant_id { get; set; }

    [StringLength(200)]
    public string name { get; set; } = null!;

    public bool is_active { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public DateTime? deleted_at { get; set; }

    [StringLength(100)]
    public string domain { get; set; } = null!;

    [InverseProperty("tenant")]
    public virtual ICollection<parking_lot> parking_lots { get; set; } = new List<parking_lot>();

    [InverseProperty("tenant")]
    public virtual ICollection<parking_spot> parking_spots { get; set; } = new List<parking_spot>();

    [InverseProperty("tenant")]
    public virtual ICollection<payment> payments { get; set; } = new List<payment>();

    [InverseProperty("tenant")]
    public virtual ICollection<reservation> reservations { get; set; } = new List<reservation>();

    [InverseProperty("tenant")]
    public virtual ICollection<role> roles { get; set; } = new List<role>();

    [InverseProperty("tenant")]
    public virtual ICollection<user> users { get; set; } = new List<user>();

    [InverseProperty("tenant")]
    public virtual ICollection<users_token> users_tokens { get; set; } = new List<users_token>();
}
