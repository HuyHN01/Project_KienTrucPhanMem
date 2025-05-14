// Giả sử file này nằm trong LikeMovie.Business/Dtos/MovieDtos/MovieSummaryDto.cs
using System;

namespace LikeMovie.Business.Dtos.MovieDtos
{
    public class MovieSummaryDto
    {
        public int MovieId { get; set; }
        public string? Title { get; set; }
        public string? Thumbnail { get; set; } // Đường dẫn tương đối
        public DateTime? ReleaseDate { get; set; }
        public double? Rating { get; set; } // Hoặc decimal
        public int? Duration { get; set; }
        public string? TypeDescription // Thuộc tính tính toán để hiển thị "Phim bộ" hoặc "Phim lẻ"
        {
            get
            {
                return Type switch
                {
                    0 => "Phim lẻ",
                    1 => "Phim bộ",
                    _ => "Không xác định"
                };
            }
        }
        public int? Type { get; set; } // Giữ lại Type gốc nếu cần cho logic khác
        // Có thể thêm một đoạn mô tả ngắn nếu cần
        // public string? ShortDescription { get; set; }
    }
}