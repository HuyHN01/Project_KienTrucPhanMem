// Giả sử file này nằm trong LikeMovie.Business/Dtos/PosterMovieDtos/UpdatePosterMovieDto.cs
using System.ComponentModel.DataAnnotations;

namespace LikeMovie.Business.Dtos.PosterMovieDtos // Hoặc SliderDtos
{
    public class UpdatePosterMovieDto
    {
        [Required(ErrorMessage = "ID của Poster/Slider là bắt buộc để cập nhật.")]
        public int PosterId { get; set; }

        public int? MovieId { get; set; } // Cho phép thay đổi phim liên kết

        // PosterUrl sẽ được xử lý riêng nếu cho phép thay đổi ảnh
        // public string? NewPosterUrl { get; set; } // Nếu muốn chỉ định URL mới

        [Range(0, int.MaxValue, ErrorMessage = "Thứ tự slider phải là số không âm.")]
        public int? SliderOrder { get; set; }
    }
}