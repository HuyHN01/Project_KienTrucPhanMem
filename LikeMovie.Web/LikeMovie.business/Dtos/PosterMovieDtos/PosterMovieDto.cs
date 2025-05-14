// Giả sử file này nằm trong LikeMovie.Business/Dtos/PosterMovieDtos/PosterMovieDto.cs
namespace LikeMovie.Business.Dtos.PosterMovieDtos // Hoặc SliderDtos
{
    public class PosterMovieDto
    {
        public int PosterId { get; set; }
        public int? MovieId { get; set; } // ID của phim liên kết (nếu có)
        public string? MovieTitle { get; set; } // Tên phim liên kết để hiển thị
        public string? PosterUrl { get; set; } // Đường dẫn đến ảnh slider
        public int? SliderOrder { get; set; } // Thứ tự hiển thị của slider
        public bool IsSlider { get; set; } // Xác nhận đây là slider (có thể không cần nếu DTO này chỉ dành cho slider)
        // Bạn có thể thêm các trường khác từ Entity PosterMovie nếu cần hiển thị
        // public string? LinkUrl { get; set; } // Nếu slider có thể trỏ đến một link tùy chỉnh thay vì phim
    }
}