using System;

namespace LikeMovie.Model.Entities
{
    public class Payments
    {
        public int PaymentID { get; set; }
        public int? UserID { get; set; }
        public int? PlanID { get; set; }
        public int SubscriptionDuration { get; set; }
        public decimal Amount { get; set; }
        public int PaymentMethod { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string TransactionStatus { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual SubscriptionPlans SubscriptionPlans { get; set; }
        public virtual Users Users { get; set; }
    }
}