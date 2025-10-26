using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ParkeoApp.Api.Models.Entities;

[Index("tenant_id", "name", Name = "ux_roles_tenant_name", IsUnique = true)]
public partial class role
{
    [Key]
    public Guid role_id { get; set; }

    public Guid tenant_id { get; set; }

    [StringLength(100)]
    public string name { get; set; } = null!;

    public bool is_active { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public DateTime? deleted_at { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("roles")]
    public virtual tenant tenant { get; set; } = null!;

    [InverseProperty("role")]
    public virtual ICollection<user_role> user_roles { get; set; } = new List<user_role>();
}
