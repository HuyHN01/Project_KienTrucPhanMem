// Giả sử file này nằm trong LikeMovie.Business/Dtos/SubscriptionPlanDtos/SubscriptionPlanDto.cs
namespace LikeMovie.Business.Dtos.SubscriptionPlanDtos
{
    public class SubscriptionPlanDto
    {
        public int PlanId { get; set; } // Hoặc tên bạn đặt cho ID, ví dụ planID
        public string? PlanName { get; set; }
        public decimal Price { get; set; }
        public int DevicesLimit { get; set; }
        public bool AdFree { get; set; }
        public bool VipContentAccess { get; set; }
        public string? Description { get; set; } // Thêm mô tả nếu có
        public int? DurationMonths { get; set; } // Thời hạn của gói (nếu gói có thời hạn cố định)
    }
}