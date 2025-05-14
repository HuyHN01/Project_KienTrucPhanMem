// LikeMovie.Business/Interfaces/ICommentService.cs
using LikeMovie.Business.Dtos.CommentDtos;
using System.Collections.Generic;
using System.Threading.Tasks;
// using X.PagedList; // Nếu muốn service trả về IPagedList

namespace LikeMovie.Business.Interfaces
{
    public interface ICommentService
    {
        /// <summary>
        /// Lấy danh sách bình luận cho một phim, có sắp xếp.
        /// </summary>
        /// <param name="movieId">ID của phim.</param>
        /// <param name="sortOrder">Tiêu chí sắp xếp (ví dụ: "newest", "oldest", "mostliked").</param>
        /// <returns>Danh sách các CommentDto.</returns>
        Task<IEnumerable<CommentDto>> GetCommentsByMovieAsync(int movieId, string? sortOrder = null /*, int pageNumber, int pageSize*/); // Cân nhắc phân trang

        /// <summary>
        /// Lấy một bình luận theo ID (thường dùng cho Admin hoặc nếu cho phép sửa/xóa).
        /// </summary>
        Task<CommentDto?> GetCommentByIdAsync(int commentId);

        /// <summary>
        /// Tạo một bình luận mới.
        /// </summary>
        /// <param name="userId">ID của người dùng tạo bình luận.</param>
        /// <param name="createDto">Dữ liệu bình luận mới.</param>
        /// <returns>CommentDto của bình luận vừa tạo và thông báo lỗi (nếu có).</returns>
        Task<(bool success, string? errorMessage, CommentDto? createdComment)> CreateCommentAsync(int userId, CreateCommentDto createDto);

        /// <summary>
        /// Xóa một bình luận (thường do Admin hoặc chủ sở hữu bình luận thực hiện).
        /// </summary>
        /// <param name="commentId">ID của bình luận cần xóa.</param>
        /// <param name="requestingUserId">ID của người dùng yêu cầu xóa (để kiểm tra quyền).</param>
        /// <param name="userRole">Vai trò của người dùng yêu cầu xóa (ví dụ: "Admin").</param>
        Task<(bool success, string? errorMessage)> DeleteCommentAsync(int commentId, int requestingUserId, string userRole);

        // Nếu có chức năng sửa bình luận:
        // Task<(bool success, string? errorMessage, CommentDto? updatedComment)> UpdateCommentAsync(int commentId, int requestingUserId, UpdateCommentDto updateDto);

        // Nếu có chức năng like/unlike bình luận:
        // Task<(bool success, string? errorMessage, int newLikeCount)> LikeCommentAsync(int commentId, int userId);
        // Task<(bool success, string? errorMessage, int newLikeCount)> UnlikeCommentAsync(int commentId, int userId);
    }
}