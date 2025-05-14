// Giả sử file này nằm trong LikeMovie.Business/Dtos/MovieDtos/UpdateMovieDto.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LikeMovie.Business.Dtos.MovieDtos
{
    public class UpdateMovieDto
    {
        [Required(ErrorMessage = "ID phim là bắt buộc để cập nhật.")]
        public int MovieId { get; set; }

        [Required(ErrorMessage = "Tiêu đề phim không được để trống.")]
        [StringLength(250, ErrorMessage = "Tiêu đề phim không được vượt quá 250 ký tự.")]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Ngày phát hành không hợp lệ.")]
        public DateTime? ReleaseDate { get; set; }

        [Range(1, 1000, ErrorMessage = "Thời lượng phim phải từ 1 đến 1000 phút.")]
        public int? Duration { get; set; }

        [StringLength(150, ErrorMessage = "Tên đạo diễn không được vượt quá 150 ký tự.")]
        public string? Director { get; set; }

        // Thumbnail sẽ được xử lý riêng (IFormFile ở Controller và BLL Service)
        // Nếu người dùng không upload file mới, giữ lại thumbnail cũ.
        // BLL Service sẽ quyết định việc này.
        // public string? Thumbnail { get; set; }

        [Url(ErrorMessage = "Đường dẫn URL phim không hợp lệ.")]
        public string? MovieUrl { get; set; }

        [Url(ErrorMessage = "Đường dẫn URL trailer không hợp lệ.")]
        public string? TrailerUrl { get; set; }

        [Range(0, 5, ErrorMessage = "Loại VIP không hợp lệ.")]
        public int? VipType { get; set; }

        [Range(0, 1, ErrorMessage = "Loại phim không hợp lệ (0: Chiếu rạp, 1: Phim bộ).")]
        public int? Type { get; set; }

        public List<int>? GenreIds { get; set; } // Danh sách ID của các thể loại được chọn để cập nhật
    }
}