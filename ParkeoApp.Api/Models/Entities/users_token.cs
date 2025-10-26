using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ParkeoApp.Api.Models.Entities;

[Index("tenant_id", Name = "idx_users_tokens_tenant")]
[Index("user_id", Name = "idx_users_tokens_user")]
[Index("user_id", "is_active", Name = "idx_users_tokens_user_active")]
public partial class users_token
{
    [Key]
    public Guid token_id { get; set; }

    public Guid tenant_id { get; set; }

    public Guid user_id { get; set; }

    [StringLength(500)]
    public string access_token { get; set; } = null!;

    public DateTime access_expires_at { get; set; }

    [StringLength(500)]
    public string refresh_token { get; set; } = null!;

    public DateTime refresh_expires_at { get; set; }

    public DateTime created_at { get; set; }

    public DateTime? revoked_at { get; set; }

    public bool is_active { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("users_tokens")]
    public virtual tenant tenant { get; set; } = null!;

    [ForeignKey("user_id")]
    [InverseProperty("users_tokens")]
    public virtual user user { get; set; } = null!;
}
