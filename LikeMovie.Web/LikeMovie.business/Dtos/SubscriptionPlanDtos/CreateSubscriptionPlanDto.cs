// Giả sử file này nằm trong LikeMovie.Business/Dtos/SubscriptionPlanDtos/CreateSubscriptionPlanDto.cs
using System.ComponentModel.DataAnnotations;

namespace LikeMovie.Business.Dtos.SubscriptionPlanDtos
{
    public class CreateSubscriptionPlanDto
    {
        [Required(ErrorMessage = "Tên gói VIP không được để trống.")]
        [StringLength(100, ErrorMessage = "Tên gói VIP không được vượt quá 100 ký tự.")]
        public string PlanName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Giá gói không được để trống.")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá gói phải là số không âm.")]
        public decimal Price { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Giới hạn thiết bị phải lớn hơn 0.")]
        public int DevicesLimit { get; set; }

        public bool AdFree { get; set; }
        public bool VipContentAccess { get; set; }

        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
        public string? Description { get; set; }

        public int? DurationMonths { get; set; } // Thời hạn của gói (ví dụ: số tháng)
    }
}