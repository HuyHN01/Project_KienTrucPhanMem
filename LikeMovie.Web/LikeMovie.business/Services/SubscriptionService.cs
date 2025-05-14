// LikeMovie.Business/Services/SubscriptionService.cs
using AutoMapper;
using LikeMovie.Business.Dtos.SubscriptionPlanDtos;
using LikeMovie.Business.Dtos.UserDtos;
using LikeMovie.Business.Interfaces;
using LikeMovie.DataAccess.Interfaces;
using LikeMovie.Model.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LikeMovie.Business.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<SubscriptionService> _logger;

        public SubscriptionService(
            ISubscriptionPlanRepository subscriptionPlanRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<SubscriptionService> logger)
        {
            _subscriptionPlanRepository = subscriptionPlanRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<SubscriptionPlanDto>> GetAllSubscriptionPlansAsync()
        {
            var plans = await _subscriptionPlanRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<SubscriptionPlanDto>>(plans);
        }

        public async Task<SubscriptionPlanDto?> GetSubscriptionPlanByIdAsync(int planId)
        {
            var plan = await _subscriptionPlanRepository.GetByIdAsync(planId);
            return _mapper.Map<SubscriptionPlanDto>(plan);
        }

        public async Task<IEnumerable<SubscriptionPlanDto>> GetApplicablePlansForUserAsync(int? currentUserLevelVip)
        {
            // Logic lấy các gói phù hợp, ví dụ: chỉ lấy các gói bằng hoặc cao hơn level hiện tại
            // Hoặc có thể lấy tất cả và để UI xử lý việc hiển thị.
            // Ví dụ đơn giản: lấy tất cả
            // var plans = await _subscriptionPlanRepository.GetAllAsync();
            // Nếu có logic lọc theo levelVip trong repository thì dùng:
            var plans = await _subscriptionPlanRepository.GetPlansByLevelAsync(currentUserLevelVip);
            return _mapper.Map<IEnumerable<SubscriptionPlanDto>>(plans);
        }

        public async Task<(bool success, string? errorMessage, UserDto? updatedUser)> ProcessSuccessfulSubscriptionAsync(int userId, int planId, int subscriptionDurationMonths)
        {
            try
            {
                var userEntity = await _userRepository.GetByIdAsync(userId);
                if (userEntity == null)
                {
                    _logger.LogError("User not found with ID {UserId} during VIP processing.", userId);
                    return (false, "Người dùng không tồn tại.", null);
                }

                var planEntity = await _subscriptionPlanRepository.GetByIdAsync(planId);
                if (planEntity == null)
                {
                    _logger.LogError("Subscription plan not found with ID {PlanId} for UserID {UserId}.", planId, userId);
                    return (false, "Gói VIP không hợp lệ.", null);
                }

                // Logic cập nhật LevelVip và TimeVip
                // Giả sử PlanID trực tiếp tương ứng với LevelVIP (1->1, 2->2, 3->3)
                // Nếu không, bạn cần một trường Level trong SubscriptionPlan Entity
                if (planEntity.PlanId >= 1 && planEntity.PlanId <= 3) // Giả sử có 3 level VIP
                {
                    userEntity.LevelVip = planEntity.PlanId;
                }
                else
                {
                    // Xử lý trường hợp PlanID không hợp lệ để gán level
                    // Hoặc lấy Level từ một thuộc tính khác của planEntity
                    _logger.LogWarning("Invalid PlanID {PlanId} to determine VIP level for UserID {UserId}.", planId, userId);
                }


                if (userEntity.TimeVip == null || userEntity.TimeVip < DateTime.UtcNow)
                {
                    userEntity.TimeVip = DateTime.UtcNow.AddMonths(subscriptionDurationMonths);
                }
                else
                {
                    userEntity.TimeVip = userEntity.TimeVip.Value.AddMonths(subscriptionDurationMonths);
                }

                _userRepository.Update(userEntity);
                // SaveChangesAsync sẽ được gọi bởi PaymentService sau khi cả Payment và User được cập nhật trạng thái
                // Nếu SubscriptionService được gọi độc lập, nó sẽ tự gọi SaveChanges.
                // Trong trường hợp này, PaymentService điều phối, nên SaveChanges ở PaymentService.

                _logger.LogInformation("VIP status updated for UserID {UserId}. New Level: {LevelVip}, New Expiry: {TimeVip}", userId, userEntity.LevelVip, userEntity.TimeVip);
                return (true, "Cập nhật trạng thái VIP thành công.", _mapper.Map<UserDto>(userEntity));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing successful subscription for UserID {UserId}, PlanID {PlanId}.", userId, planId);
                return (false, "Lỗi khi cập nhật trạng thái VIP.", null);
            }
        }

        // --- Nghiệp vụ Admin cho SubscriptionPlan ---
        public async Task<(bool success, string? errorMessage, SubscriptionPlanDto? createdPlan)> CreateSubscriptionPlanAsync(CreateSubscriptionPlanDto createDto)
        {
            try
            {
                var planEntity = _mapper.Map<SubscriptionPlan>(createDto);
                await _subscriptionPlanRepository.AddAsync(planEntity);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Subscription Plan '{PlanName}' created successfully.", planEntity.PlanName);
                return (true, null, _mapper.Map<SubscriptionPlanDto>(planEntity));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subscription plan '{PlanName}'.", createDto.PlanName);
                return (false, "Lỗi khi tạo gói VIP.", null);
            }
        }

        public async Task<(bool success, string? errorMessage, SubscriptionPlanDto? updatedPlan)> UpdateSubscriptionPlanAsync(UpdateSubscriptionPlanDto updateDto)
        {
            try
            {
                var planEntity = await _subscriptionPlanRepository.GetByIdAsync(updateDto.PlanId);
                if (planEntity == null) return (false, "Không tìm thấy gói VIP.", null);

                _mapper.Map(updateDto, planEntity);
                _subscriptionPlanRepository.Update(planEntity);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Subscription Plan ID {PlanId} updated successfully.", planEntity.PlanId);
                return (true, null, _mapper.Map<SubscriptionPlanDto>(planEntity));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating subscription plan ID {PlanId}.", updateDto.PlanId);
                return (false, "Lỗi khi cập nhật gói VIP.", null);
            }
        }

        public async Task<(bool success, string? errorMessage)> DeleteSubscriptionPlanAsync(int planId)
        {
            try
            {
                var planEntity = await _subscriptionPlanRepository.GetByIdAsync(planId);
                if (planEntity == null) return (false, "Không tìm thấy gói VIP.");

                // TODO: Kiểm tra xem gói này có đang được user nào sử dụng không, hoặc có ràng buộc gì không
                _subscriptionPlanRepository.Delete(planEntity);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Subscription Plan ID {PlanId} deleted successfully.", planId);
                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting subscription plan ID {PlanId}.", planId);
                return (false, "Lỗi khi xóa gói VIP.");
            }
        }
    }
}