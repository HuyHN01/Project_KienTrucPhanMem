// Giả sử file này nằm trong LikeMovie.Business/Dtos/CommentDtos/CreateCommentDto.cs
using System.ComponentModel.DataAnnotations;

namespace LikeMovie.Business.Dtos.CommentDtos
{
    public class CreateCommentDto
    {
        [Required(ErrorMessage = "ID phim không được để trống.")]
        public int MovieId { get; set; }

        // UserID sẽ được lấy từ người dùng đang đăng nhập trong BLL Service, không cần client gửi lên
        // public int UserId { get; set; }

        [Required(ErrorMessage = "Nội dung bình luận không được để trống.")]
        [StringLength(1000, ErrorMessage = "Nội dung bình luận không được vượt quá 1000 ký tự.")]
        public string CommentText { get; set; } = string.Empty;

        [Range(1, 5, ErrorMessage = "Đánh giá phải từ 1 đến 5 sao.")] // Nếu cho phép đánh giá từ 0 (không đánh giá) thì Range(0,5)
        public int? Rating { get; set; } // Đánh giá sao (tùy chọn)
    }
}