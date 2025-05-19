using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LikeMovies.Models
{
    public class SubscriptionPlanViewModel
    {
        public int planID { get; set; }
        public string PlanName { get; set; }
        public decimal Price { get; set; }
        public int DevicesLimit { get; set; }
        public bool AdFree { get; set; }
        public bool VipContentAccess { get; set; }
    }
}