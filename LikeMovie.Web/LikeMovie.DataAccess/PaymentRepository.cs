using LikeMovie.DataAccess.Interfaces;
using LikeMovie.Model.Entities;
using System.Threading.Tasks;

namespace LikeMovie.DataAccess.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly LikeMovieDbContext _context;

        public PaymentRepository(LikeMovieDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment); // Giả định DbSet tên là Payments
            // SaveChangesAsync() sẽ ở BLL Service hoặc UnitOfWork
        }
    }
}