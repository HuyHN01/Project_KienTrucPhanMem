using LikeMovie.Model.Entities; // Đảm bảo using đúng namespace của Entity SubscriptionPlan
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LikeMovie.DataAccess.Interfaces
{
    public interface ISubscriptionPlanRepository
    {
        Task<SubscriptionPlan?> GetByIdAsync(int planId);
        Task<IEnumerable<SubscriptionPlan>> GetAllAsync(); // Có thể bạn sẽ cần lấy tất cả các gói
        Task<IEnumerable<SubscriptionPlan>> GetPlansByLevelAsync(int? levelVip); // Dựa trên logic GiaHanVip
        Task AddAsync(SubscriptionPlan plan);
        void Update(SubscriptionPlan plan);
        void Delete(SubscriptionPlan plan);
    }
}