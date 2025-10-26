using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ParkeoApp.Api.Models.Entities;

[Index("tenant_id", "status", Name = "idx_payments_tenant_status")]
public partial class payment
{
    [Key]
    public Guid payment_id { get; set; }

    public Guid tenant_id { get; set; }

    public Guid? reservation_id { get; set; }

    [Precision(12, 2)]
    public decimal amount { get; set; }

    [StringLength(10)]
    public string currency { get; set; } = null!;

    [StringLength(50)]
    public string? payment_method { get; set; }

    [StringLength(50)]
    public string status { get; set; } = null!;

    [StringLength(100)]
    public string? transaction_id { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    [ForeignKey("reservation_id")]
    [InverseProperty("payments")]
    public virtual reservation? reservation { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("payments")]
    public virtual tenant tenant { get; set; } = null!;
}
