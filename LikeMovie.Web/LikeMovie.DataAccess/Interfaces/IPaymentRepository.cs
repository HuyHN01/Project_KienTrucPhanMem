using LikeMovie.Model.Entities; // Giả sử Entity là Payment hoặc Payments
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LikeMovie.DataAccess.Interfaces
{
    public interface IPaymentRepository
    {
        Task AddAsync(Payment payment);
        // Có thể cần các phương thức khác sau này:
        // Task<Payment?> GetByIdAsync(long paymentId);
        // Task<IEnumerable<Payment>> GetByUserIdAsync(int userId);
    }
}