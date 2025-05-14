using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LikeMovie.Model.Entities;

public partial class SubscriptionPlan
{
    [Key]
    [Column("PlanID")]
    public int PlanId { get; set; }

    [StringLength(100)]
    public string PlanName { get; set; } = null!;

    [StringLength(500)]
    public string? Description { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Price { get; set; }

    public int? DevicesLimit { get; set; }

    public bool? AdFree { get; set; }

    public bool? VipContentAccess { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [InverseProperty("Plan")]
    public virtual ICollection<DiscountCode> DiscountCodes { get; set; } = new List<DiscountCode>();

    [InverseProperty("Plan")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
