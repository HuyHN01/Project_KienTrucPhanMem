// Giả sử file này nằm trong LikeMovie.Business/Dtos/MovieDtos/CreateMovieDto.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Cần cho Validation Attributes

namespace LikeMovie.Business.Dtos.MovieDtos
{
    public class CreateMovieDto
    {
        [Required(ErrorMessage = "Tiêu đề phim không được để trống.")]
        [StringLength(250, ErrorMessage = "Tiêu đề phim không được vượt quá 250 ký tự.")]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Ngày phát hành không hợp lệ.")]
        public DateTime? ReleaseDate { get; set; }

        [Range(1, 1000, ErrorMessage = "Thời lượng phim phải từ 1 đến 1000 phút.")]
        public int? Duration { get; set; } // Đơn vị: phút

        [StringLength(150, ErrorMessage = "Tên đạo diễn không được vượt quá 150 ký tự.")]
        public string? Director { get; set; }

        // Thumbnail sẽ được xử lý riêng (IFormFile ở Controller và BLL Service)
        // public string? Thumbnail { get; set; } // Đường dẫn sẽ được gán sau khi upload

        [Url(ErrorMessage = "Đường dẫn URL phim không hợp lệ.")]
        public string? MovieUrl { get; set; }

        [Url(ErrorMessage = "Đường dẫn URL trailer không hợp lệ.")]
        public string? TrailerUrl { get; set; }

        [Range(0, 5, ErrorMessage = "Loại VIP không hợp lệ.")] // Giả sử VIPType từ 0 (free) đến 5
        public int? VipType { get; set; }

        [Range(0, 1, ErrorMessage = "Loại phim không hợp lệ (0: Chiếu rạp, 1: Phim bộ).")] // 0: Phim lẻ/chiếu rạp, 1: Phim bộ/series
        public int? Type { get; set; }

        public List<int>? GenreIds { get; set; } // Danh sách ID của các thể loại được chọn
    }
}