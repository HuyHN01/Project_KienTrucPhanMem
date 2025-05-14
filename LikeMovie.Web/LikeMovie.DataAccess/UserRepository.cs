// LikeMovie.DataAccess/Repositories/UserRepository.cs
using LikeMovie.DataAccess.Interfaces;
using LikeMovie.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq; // << THÊM USING NÀY CHO SingleOrDefaultAsync VÀ AnyAsync
using System.Threading.Tasks;

namespace LikeMovie.DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly LikeMovieDbContext _context;

        public UserRepository(LikeMovieDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        }

        // --- THÊM IMPLEMENTATION CHO GetByUsernameAsync ---
        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.UserName == username);
        }
        // --- KẾT THÚC PHẦN THÊM ---

        public async Task AddAsync(User user) // Sửa lại: nên là async Task
        {
            await _context.Users.AddAsync(user);
            // SaveChangesAsync ở BLL/UnitOfWork
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
            // SaveChangesAsync() sẽ được gọi ở BLL Service hoặc UnitOfWork
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.UserName == username);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }
    }
}