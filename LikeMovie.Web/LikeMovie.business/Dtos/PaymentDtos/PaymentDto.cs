// Giả sử file này nằm trong LikeMovie.Business/Dtos/PaymentDtos/PaymentDto.cs
using System;

namespace LikeMovie.Business.Dtos.PaymentDtos
{
    public class PaymentDto
    {
        public int PaymentId { get; set; } // Hoặc kiểu long nếu ID lớn
        public int UserId { get; set; }
        public string? UserName { get; set; } // Tên người dùng thực hiện giao dịch (nếu cần hiển thị)
        public int PlanId { get; set; }
        public string? PlanName { get; set; } // Tên gói VIP (nếu cần hiển thị)
        public int SubscriptionDuration { get; set; } // Thời hạn đăng ký (ví dụ: số tháng)
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? TransactionStatus { get; set; } // Ví dụ: "Success", "Failed", "Pending"
        public int PaymentMethod { get; set; } // Ví dụ: 1 cho VNPAY, 2 cho MoMo
        public string? PaymentMethodDescription // Thuộc tính tính toán để hiển thị tên phương thức
        {
            get
            {
                return PaymentMethod switch
                {
                    1 => "VNPAY",
                    2 => "MoMo",
                    _ => "Không xác định"
                };
            }
        }
        public DateTime CreatedAt { get; set; }
        public string? TransactionReference { get; set; } // Mã giao dịch từ cổng thanh toán (nếu có)
    }
}