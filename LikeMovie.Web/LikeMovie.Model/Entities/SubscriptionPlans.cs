using System;
using System.Collections.Generic;

namespace LikeMovie.Model.Entities
{
    public class SubscriptionPlans
    {
        public SubscriptionPlans()
        {
            this.DiscountCodes = new HashSet<DiscountCodes>();
            this.Payments = new HashSet<Payments>();
        }

        public int PlanID { get; set; }
        public string PlanName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int? DevicesLimit { get; set; }
        public bool? AdFree { get; set; }      
        public bool? VipContentAccess { get; set; } 
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; } 

        public virtual ICollection<DiscountCodes> DiscountCodes { get; set; }
        public virtual ICollection<Payments> Payments { get; set; }
    }
}