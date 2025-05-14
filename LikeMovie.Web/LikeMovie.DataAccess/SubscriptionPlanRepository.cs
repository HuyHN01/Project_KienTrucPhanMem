using LikeMovie.DataAccess.Interfaces;
using LikeMovie.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LikeMovie.DataAccess.Repositories
{
    public class SubscriptionPlanRepository : ISubscriptionPlanRepository
    {
        private readonly LikeMovieDbContext _context;

        public SubscriptionPlanRepository(LikeMovieDbContext context)
        {
            _context = context;
        }

        public async Task<SubscriptionPlan?> GetByIdAsync(int planId)
        {
            return await _context.SubscriptionPlans.FindAsync(planId);
        }

        public async Task<IEnumerable<SubscriptionPlan>> GetAllAsync()
        {
            return await _context.SubscriptionPlans.ToListAsync();
        }
        public async Task AddAsync(SubscriptionPlan plan)
        {
            await _context.SubscriptionPlans.AddAsync(plan);
        }

        public void Update(SubscriptionPlan plan)
        {
            _context.SubscriptionPlans.Update(plan);
        }

        public void Delete(SubscriptionPlan plan)
        {
            _context.SubscriptionPlans.Remove(plan);
        }
        public async Task<IEnumerable<SubscriptionPlan>> GetPlansByLevelAsync(int? levelVip)
        {
            if (levelVip.HasValue)
            {
                // Theo logic cũ của bạn là PlanID == levelVip,
                // tuy nhiên, tên 'levelVip' gợi ý có thể có nhiều Plan cho một Level,
                // hoặc một Plan có một thuộc tính Level.
                // Giả định Entity SubscriptionPlan có thuộc tính PlanId tương ứng với levelVip
                return await _context.SubscriptionPlans.Where(s => s.PlanId == levelVip.Value).ToListAsync();
            }
            // Hoặc trả về danh sách rỗng/tất cả nếu levelVip là null, tùy theo logic bạn muốn
            return new List<SubscriptionPlan>();
        }
    }
}