using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ParkeoApp.Api.Models.Entities;

[Index("tenant_id", "code", Name = "ux_reservations_tenant_code", IsUnique = true)]
public partial class reservation
{
    [Key]
    public Guid reservation_id { get; set; }

    public Guid tenant_id { get; set; }

    public Guid user_id { get; set; }

    public Guid? spot_id { get; set; }

    [StringLength(100)]
    public string code { get; set; } = null!;

    public DateTime start_at { get; set; }

    public DateTime end_at { get; set; }

    [StringLength(50)]
    public string status { get; set; } = null!;

    public string? note { get; set; }

    public bool is_active { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public DateTime? deleted_at { get; set; }

    [InverseProperty("reservation")]
    public virtual ICollection<payment> payments { get; set; } = new List<payment>();

    [ForeignKey("spot_id")]
    [InverseProperty("reservations")]
    public virtual parking_spot? spot { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("reservations")]
    public virtual tenant tenant { get; set; } = null!;

    [ForeignKey("user_id")]
    [InverseProperty("reservations")]
    public virtual user user { get; set; } = null!;
}
