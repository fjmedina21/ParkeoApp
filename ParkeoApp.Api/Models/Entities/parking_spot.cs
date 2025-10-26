using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ParkeoApp.Api.Models.Entities;

[Index("parking_lot_id", "code", Name = "ux_spots_lot_code", IsUnique = true)]
public partial class parking_spot
{
    [Key]
    public Guid spot_id { get; set; }

    public Guid tenant_id { get; set; }

    public Guid parking_lot_id { get; set; }

    [StringLength(50)]
    public string code { get; set; } = null!;

    [StringLength(50)]
    public string? spottype { get; set; }

    [StringLength(50)]
    public string status { get; set; } = null!;

    public int? floor { get; set; }

    public string? note { get; set; }

    public bool is_active { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public DateTime? deleted_at { get; set; }

    [ForeignKey("parking_lot_id")]
    [InverseProperty("parking_spots")]
    public virtual parking_lot parking_lot { get; set; } = null!;

    [InverseProperty("spot")]
    public virtual ICollection<reservation> reservations { get; set; } = new List<reservation>();

    [ForeignKey("tenant_id")]
    [InverseProperty("parking_spots")]
    public virtual tenant tenant { get; set; } = null!;
}
