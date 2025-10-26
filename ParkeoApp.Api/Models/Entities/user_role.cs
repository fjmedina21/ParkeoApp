using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ParkeoApp.Api.Models.Entities;

public partial class user_role
{
    [Key]
    public Guid user_role_id { get; set; }

    public Guid user_id { get; set; }

    public Guid role_id { get; set; }

    public DateTime assigned_at { get; set; }

    [ForeignKey("role_id")]
    [InverseProperty("user_roles")]
    public virtual role role { get; set; } = null!;

    [ForeignKey("user_id")]
    [InverseProperty("user_roles")]
    public virtual user user { get; set; } = null!;
}
