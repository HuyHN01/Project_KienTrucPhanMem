// Giả sử file này nằm trong LikeMovie.Business/Dtos/UserDtos/ForgotPasswordDto.cs
using System.ComponentModel.DataAnnotations;

namespace LikeMovie.Business.Dtos.UserDtos
{
    public class ForgotPasswordDto
    {
        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
        public string Email { get; set; } = string.Empty;
    }
}