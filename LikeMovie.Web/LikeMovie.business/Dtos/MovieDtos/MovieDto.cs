// Giả sử file này nằm trong LikeMovie.Business/Dtos/MovieDtos/MovieDto.cs
using System;
using System.Collections.Generic;

namespace LikeMovie.Business.Dtos.MovieDtos
{
    public class MovieDto
    {
        public int MovieId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public int? Duration { get; set; }
        public string? Director { get; set; }
        public string? Thumbnail { get; set; } // Đường dẫn tương đối đến ảnh
        public string? MovieUrl { get; set; }
        public string? TrailerUrl { get; set; }
        public int? ViewCount { get; set; }
        public double? Rating { get; set; } // Hoặc decimal
        public int? VipType { get; set; }
        public int? Type { get; set; } // 0: Phim lẻ/chiếu rạp, 1: Phim bộ/series

        public List<string> GenreNames { get; set; } = new List<string>();
        // Có thể thêm các thông tin khác như danh sách PosterMovieDto, danh sách EpisodeDto nếu cần
        // public List<PosterMovieDto> Posters { get; set; } = new List<PosterMovieDto>();
        // public List<EpisodeDto> Episodes { get; set; } = new List<EpisodeDto>();
    }
}