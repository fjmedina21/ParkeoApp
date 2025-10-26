using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ParkeoApp.Api.Models.Entities;

[Index("tenant_id", Name = "idx_parking_lots_tenant")]
public partial class parking_lot
{
    [Key]
    public Guid parking_lot_id { get; set; }

    public Guid tenant_id { get; set; }

    [StringLength(200)]
    public string name { get; set; } = null!;

    public string? description { get; set; }

    [StringLength(300)]
    public string? address { get; set; }

    [Precision(9, 6)]
    public decimal? latitude { get; set; }

    [Precision(9, 6)]
    public decimal? longitude { get; set; }

    public bool is_active { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public DateTime? deleted_at { get; set; }

    [InverseProperty("parking_lot")]
    public virtual ICollection<parking_spot> parking_spots { get; set; } = new List<parking_spot>();

    [ForeignKey("tenant_id")]
    [InverseProperty("parking_lots")]
    public virtual tenant tenant { get; set; } = null!;
}
