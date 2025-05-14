using LikeMovie.DataAccess.Interfaces;
using LikeMovie.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LikeMovie.DataAccess.Repositories
{
    public class CommentRepository : ICommentRepository // Đảm bảo implement ICommentRepository
    {
        private readonly LikeMovieDbContext _context;

        public CommentRepository(LikeMovieDbContext context)
        {
            _context = context;
        }

        // --- Implement các phương thức từ ICommentRepository ---

        public async Task AddAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
            // SaveChangesAsync() sẽ ở BLL Service hoặc UnitOfWork
        }

        public void Update(Comment comment) // Chỉ implement nếu ICommentRepository có khai báo
        {
            _context.Comments.Update(comment);
            // SaveChangesAsync() sẽ ở BLL Service hoặc UnitOfWork
        }

        public void Delete(Comment comment)
        {
            _context.Comments.Remove(comment);
            // SaveChangesAsync() sẽ ở BLL Service hoặc UnitOfWork
        }

        public async Task<Comment?> GetByIdAsync(int id)
        {
            // Để CommentService có đủ thông tin cho CommentDto, hãy Include ở đây
            return await _context.Comments
                                 .Include(c => c.User)  // Giả định Comment Entity có navigation property User
                                 .Include(c => c.Movie) // Giả định Comment Entity có navigation property Movie
                                 .FirstOrDefaultAsync(c => c.CommentId == id);
        }

        public async Task<IEnumerable<Comment>> GetAllAsync() // Implement nếu ICommentRepository có
        {
            return await _context.Comments
                                 .Include(c => c.User)
                                 .OrderByDescending(c => c.DateCreated) // Ví dụ sắp xếp
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Comment>> GetAllOrderedByIdAsync()
        {
            return await _context.Comments
                                 .Include(c => c.User) // Có thể cần User info
                                 .OrderBy(c => c.CommentId)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Comment>> GetByMovieIdAsync(int movieId, string? sortOrder = null)
        {
            var query = _context.Comments
                                .Include(c => c.User)  // QUAN TRỌNG: Nạp thông tin User
                                .Include(c => c.Movie) // Tùy chọn: Nạp thông tin Movie nếu CommentDto cần MovieTitle trực tiếp từ đây
                                .Where(c => c.MovieId == movieId)
                                .AsQueryable();

            switch (sortOrder?.ToLower())
            {
                case "oldest":
                    query = query.OrderBy(c => c.DateCreated);
                    break;
                case "mostliked":
                    query = query.OrderByDescending(c => c.Likes); // Giả sử Comment Entity có Likes
                    break;
                case "newest":
                default:
                    query = query.OrderByDescending(c => c.DateCreated);
                    break;
            }
            return await query.ToListAsync();
        }

        // (Tùy chọn) Implement GetByUserIdAsync nếu có trong interface
        // public async Task<IEnumerable<Comment>> GetByUserIdAsync(int userId)
        // {
        //     return await _context.Comments
        //                          .Include(c => c.Movie)
        //                          .Where(c => c.UserId == userId)
        //                          .OrderByDescending(c => c.DateCreated)
        //                          .ToListAsync();
        // }
    }
}