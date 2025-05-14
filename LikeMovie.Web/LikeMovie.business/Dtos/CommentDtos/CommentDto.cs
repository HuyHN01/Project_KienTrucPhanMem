// Giả sử file này nằm trong LikeMovie.Business/Dtos/CommentDtos/CommentDto.cs
using System;

namespace LikeMovie.Business.Dtos.CommentDtos
{
    public class CommentDto
    {
        public int CommentId { get; set; }
        public int MovieId { get; set; }
        public string? MovieTitle { get; set; } // Tên phim để hiển thị (nếu cần)
        public int UserId { get; set; }
        public string? UserName { get; set; } // Tên người dùng đã bình luận
        public string? UserAvatarUrl { get; set; } // URL avatar của người dùng
        public string? CommentText { get; set; }
        public DateTime DateCreated { get; set; }
        public int Likes { get; set; }
        public int? Rating { get; set; } // Đánh giá sao (nếu có)
        // Bạn có thể thêm các thuộc tính khác nếu cần, ví dụ: danh sách trả lời bình luận
        // public List<ReplyDto> Replies { get; set; } = new List<ReplyDto>();
    }
}