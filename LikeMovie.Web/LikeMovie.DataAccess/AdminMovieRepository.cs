using LikeMovie.DataAccess.Interfaces;
using LikeMovie.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LikeMovie.DataAccess.Repositories
{
    public class AdminMovieRepository : IAdminMovieRepository
    {
        private readonly LikeMovieDbContext _context;

        public AdminMovieRepository(LikeMovieDbContext context)
        {
            _context = context;
        }

        public async Task<AdminMovie?> GetByUsernameAsync(string username)
        {
            // Giả sử Entity AdminMovie có thuộc tính UsernameAd
            return await _context.AdminMovies
                                 .SingleOrDefaultAsync(a => a.UsernameAd == username);
        }

        // Implement các phương thức khác nếu có trong interface
        // public async Task<AdminMovie?> GetByIdAsync(int adminId) { ... }
        // public async Task AddAsync(AdminMovie admin) { ... }
        // public void Update(AdminMovie admin) { ... }
    }
}