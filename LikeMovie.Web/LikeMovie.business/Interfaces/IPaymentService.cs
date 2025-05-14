// LikeMovie.Business/Interfaces/IPaymentService.cs
using LikeMovie.Business.Dtos.PaymentDtos;
using LikeMovie.Business.Dtos.UserDtos; // Có thể cần UserDto
using System.Collections.Generic; // Cho các tham số của cổng thanh toán
using System.Threading.Tasks;

namespace LikeMovie.Business.Interfaces
{
    public interface IPaymentService
    {
        /// <summary>
        /// Tạo yêu cầu thanh toán đến VNPAY.
        /// </summary>
        /// <param name="userId">ID của người dùng thực hiện thanh toán.</param>
        /// <param name="planId">ID của gói VIP.</param>
        /// <param name="subscriptionDurationMonths">Thời hạn đăng ký (số tháng).</param>
        /// <param name="totalAmount">Tổng số tiền cần thanh toán.</param>
        /// <param name="ipAddress">Địa chỉ IP của người dùng.</param>
        /// <param name="httpContextBaseUrl">Base URL của ứng dụng (ví dụ: "https://yourdomain.com") để tạo returnUrl.</param>
        /// <returns>URL chuyển hướng đến cổng VNPAY hoặc null nếu có lỗi.</returns>
        Task<string?> CreateVnPayPaymentUrlAsync(int userId, int planId, int subscriptionDurationMonths, decimal totalAmount, string ipAddress, string httpContextBaseUrl);

        /// <summary>
        /// Xử lý kết quả trả về từ VNPAY (Return URL).
        /// </summary>
        /// <param name="queryString">Dữ liệu query string từ VNPAY.</param>
        /// <returns>Trạng thái thanh toán, thông báo, và thông tin người dùng/giao dịch (nếu có).</returns>
        Task<(bool success, string message, PaymentDto? payment, UserDto? updatedUser)> ProcessVnPayReturnAsync(Dictionary<string, string> queryString);


        /// <summary>
        /// Tạo yêu cầu thanh toán đến MoMo.
        /// </summary>
        Task<(string? payUrl, string? errorMessage)> CreateMomoPaymentRequestAsync(int userId, int planId, int subscriptionDurationMonths, decimal totalAmount, string httpContextBaseUrl);

        /// <summary>
        /// Xử lý kết quả trả về từ MoMo (Return URL).
        /// </summary>
        Task<(bool success, string message, PaymentDto? payment, UserDto? updatedUser)> ProcessMomoReturnAsync(Dictionary<string, string> responseData);

        /// <summary>
        /// Xử lý IPN (Instant Payment Notification) từ MoMo (hoặc VNPAY nếu có).
        /// </summary>
        Task<(bool success, string message)> ProcessMomoIpnAsync(Dictionary<string, string> ipnData);


        /// <summary>
        /// Lấy lịch sử giao dịch của một người dùng.
        /// </summary>
        Task<IEnumerable<PaymentDto>> GetPaymentHistoryByUserAsync(int userId /*, int pageNumber, int pageSize*/);
    }
}