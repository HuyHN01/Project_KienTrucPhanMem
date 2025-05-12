using System;

namespace LikeMovie.Model.Entities
{
    public class DiscountCodes
    {
        public int DiscountID { get; set; }
        public int? PlanID { get; set; }
        public string DiscountCode { get; set; }
        public decimal? DiscountAmount { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool? IsActive { get; set; }

        public virtual SubscriptionPlans SubscriptionPlans { get; set; }
    }
}