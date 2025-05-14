using LikeMovie.Model.Entities;
using System.Collections.Generic;
using System.Linq.Expressions; // Có thể cần nếu bạn thêm phương thức FindAsync tổng quát
using System.Threading.Tasks;

namespace LikeMovie.DataAccess.Interfaces
{
    public interface ICommentRepository
    {
        // === Các phương thức CRUD cơ bản ===
        Task AddAsync(Comment comment);
        void Update(Comment comment); // Nếu có chức năng sửa bình luận
        void Delete(Comment comment);

        // === Các phương thức Get ===
        Task<Comment?> GetByIdAsync(int id); // Lấy theo ID, nên Include User và Movie nếu CommentDto cần thông tin đó

        Task<IEnumerable<Comment>> GetAllAsync(); // Lấy tất cả bình luận

        Task<IEnumerable<Comment>> GetAllOrderedByIdAsync(); // Cho Admin Index trong BinhLuanController

        /// <summary>
        /// Lấy danh sách bình luận cho một phim cụ thể, có tùy chọn sắp xếp.
        /// Repository nên Include User để Service có thể lấy UserName, UserAvatarUrl.
        /// </summary>
        Task<IEnumerable<Comment>> GetByMovieIdAsync(int movieId, string? sortOrder = null);

        // (Tùy chọn) Task<IEnumerable<Comment>> GetByUserIdAsync(int userId);
    }
}