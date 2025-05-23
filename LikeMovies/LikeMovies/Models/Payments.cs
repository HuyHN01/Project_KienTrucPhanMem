//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LikeMovies.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Payments
    {
        public int PaymentID { get; set; }
        public Nullable<int> UserID { get; set; }
        public Nullable<int> PlanID { get; set; }
        public int SubscriptionDuration { get; set; }
        public decimal Amount { get; set; }
        public int PaymentMethod { get; set; }
        public Nullable<System.DateTime> TransactionDate { get; set; }
        public string TransactionStatus { get; set; }
        public Nullable<System.DateTime> CreatedAt { get; set; }
    
        public virtual SubscriptionPlans SubscriptionPlans { get; set; }
        public virtual Users Users { get; set; }
    }
}
