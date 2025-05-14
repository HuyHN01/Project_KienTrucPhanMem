// LikeMovie.Business/Interfaces/ISubscriptionService.cs
using LikeMovie.Business.Dtos.SubscriptionPlanDtos;
using LikeMovie.Business.Dtos.UserDtos; // Có thể cần UserDto
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LikeMovie.Business.Interfaces
{
    public interface ISubscriptionService
    {
        /// <summary>
        /// Lấy tất cả các gói VIP đang có.
        /// </summary>
        Task<IEnumerable<SubscriptionPlanDto>> GetAllSubscriptionPlansAsync();

        /// <summary>
        /// Lấy thông tin chi tiết một gói VIP.
        /// </summary>
        Task<SubscriptionPlanDto?> GetSubscriptionPlanByIdAsync(int planId);

        /// <summary>
        /// Lấy các gói VIP phù hợp với level hiện tại của người dùng (cho chức năng gia hạn).
        /// </summary>
        Task<IEnumerable<SubscriptionPlanDto>> GetApplicablePlansForUserAsync(int? currentUserLevelVip);

        /// <summary>
        /// Xử lý logic sau khi người dùng thanh toán thành công một gói VIP (được gọi bởi PaymentService).
        /// Bao gồm việc cập nhật levelVIP, TimeVIP cho người dùng.
        /// </summary>
        /// <param name="userId">ID người dùng.</param>
        /// <param name="planId">ID gói đã mua.</param>
        /// <param name="subscriptionDurationMonths">Thời hạn gói (số tháng).</param>
        /// <returns>Thông tin người dùng đã được cập nhật.</returns>
        Task<(bool success, string? errorMessage, UserDto? updatedUser)> ProcessSuccessfulSubscriptionAsync(int userId, int planId, int subscriptionDurationMonths);


        // --- Các nghiệp vụ Admin cho SubscriptionPlan ---
        Task<(bool success, string? errorMessage, SubscriptionPlanDto? createdPlan)> CreateSubscriptionPlanAsync(CreateSubscriptionPlanDto createDto);
        Task<(bool success, string? errorMessage, SubscriptionPlanDto? updatedPlan)> UpdateSubscriptionPlanAsync(UpdateSubscriptionPlanDto updateDto);
        Task<(bool success, string? errorMessage)> DeleteSubscriptionPlanAsync(int planId);
    }
}