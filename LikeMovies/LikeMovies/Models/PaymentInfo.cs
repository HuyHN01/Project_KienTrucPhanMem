using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LikeMovies.Models
{
    public class PaymentInfo
    {
        public int paymentId {  get; set; }
        public int userId {  get; set; }
        public int planId {  get; set; }
        public int SubscriptionDuration {  get; set; }
        public int paymentMethod {  get; set; }
        public decimal amount {  get; set; }
        public DateTime transactionDate {  get; set; }
        public string transactionstatus { get; set; }

        public DateTime createAt {  get; set; }
    }
}