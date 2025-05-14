// Giả sử file này nằm trong LikeMovie.Business/Dtos/GenreDtos/CreateGenreDto.cs
using System.ComponentModel.DataAnnotations;

namespace LikeMovie.Business.Dtos.GenreDtos
{
    public class CreateGenreDto
    {
        [Required(ErrorMessage = "Tên thể loại không được để trống.")]
        [StringLength(100, ErrorMessage = "Tên thể loại không được vượt quá 100 ký tự.")]
        public string Name { get; set; } = string.Empty;
    }
}