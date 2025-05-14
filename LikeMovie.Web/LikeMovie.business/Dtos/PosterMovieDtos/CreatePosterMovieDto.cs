// Giả sử file này nằm trong LikeMovie.Business/Dtos/PosterMovieDtos/CreatePosterMovieDto.cs
using System.ComponentModel.DataAnnotations;

namespace LikeMovie.Business.Dtos.PosterMovieDtos // Hoặc SliderDtos
{
    public class CreatePosterMovieDto
    {
        public int? MovieId { get; set; } // ID của phim được chọn để làm slider (có thể nullable nếu slider là ảnh/link tự do)

        // PosterUrl sẽ được xử lý riêng (IFormFile ở Controller và BLL Service)
        // BLL Service sẽ nhận file, lưu và gán đường dẫn vào Entity.
        // public string PosterUrl { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Thứ tự slider phải là số không âm.")]
        public int? SliderOrder { get; set; }

        // IsSlider sẽ được BLL Service tự động gán là true khi tạo qua nghiệp vụ "tạo slider"
        // public bool IsSlider { get; set; } = true;
    }
}