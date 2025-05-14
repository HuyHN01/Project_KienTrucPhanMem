// Giả sử file này nằm trong LikeMovie.Business/Dtos/PaymentDtos/CreatePaymentDto.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace LikeMovie.Business.Dtos.PaymentDtos
{
    public class CreatePaymentDto
    {
        [Required(ErrorMessage = "ID người dùng không được để trống.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "ID gói VIP không được để trống.")]
        public int PlanId { get; set; }

        [Required(ErrorMessage = "Thời hạn đăng ký không được để trống.")]
        [Range(1, int.MaxValue, ErrorMessage = "Thời hạn đăng ký phải lớn hơn 0.")]
        public int SubscriptionDuration { get; set; } // Số tháng

        [Required(ErrorMessage = "Số tiền không được để trống.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn 0.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Trạng thái giao dịch không được để trống.")]
        public string TransactionStatus { get; set; } = string.Empty; // Ví dụ: "Success", "Failed"

        [Required(ErrorMessage = "Phương thức thanh toán không được để trống.")]
        public int PaymentMethod { get; set; } // 1: VNPAY, 2: MoMo

        public string? TransactionReference { get; set; } // Mã giao dịch từ cổng thanh toán
        // TransactionDate và CreatedAt sẽ được gán trong BLL Service
    }
}