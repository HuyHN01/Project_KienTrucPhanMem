// Giả sử file này nằm trong LikeMovie.Business/Dtos/GenreDtos/GenreDto.cs
namespace LikeMovie.Business.Dtos.GenreDtos
{
    public class GenreDto
    {
        public int GenreId { get; set; }
        public string? Name { get; set; }
        // Có thể thêm số lượng phim thuộc thể loại này nếu cần cho hiển thị
        // public int MovieCount { get; set; }
    }
}