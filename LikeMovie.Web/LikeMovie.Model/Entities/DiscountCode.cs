using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LikeMovie.DataAccess;

public partial class DiscountCode
{
    [Key]
    [Column("DiscountID")]
    public int DiscountId { get; set; }

    [Column("PlanID")]
    public int? PlanId { get; set; }

    [Column("DiscountCode")]
    [StringLength(50)]
    public string DiscountCode1 { get; set; } = null!;

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? DiscountAmount { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ExpirationDate { get; set; }

    public bool? IsActive { get; set; }

    [ForeignKey("PlanId")]
    [InverseProperty("DiscountCodes")]
    public virtual SubscriptionPlan? Plan { get; set; }
}
