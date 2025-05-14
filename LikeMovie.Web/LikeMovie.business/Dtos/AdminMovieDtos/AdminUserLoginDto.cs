// Giả sử file này nằm trong LikeMovie.Business/Dtos/AdminMovieDtos/AdminUserLoginDto.cs
using System.ComponentModel.DataAnnotations;

namespace LikeMovie.Business.Dtos.AdminMovieDtos // Hoặc AdminUserDtos
{
    public class AdminUserLoginDto
    {
        [Required(ErrorMessage = "Tên đăng nhập không được để trống.")]
        [StringLength(50, ErrorMessage = "Tên đăng nhập không được vượt quá 50 ký tự.")]
        public string UsernameAd { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [DataType(DataType.Password)]
        public string PasswordAd { get; set; } = string.Empty;

        public bool RememberMe { get; set; } // Cho chức năng "Ghi nhớ đăng nhập"
    }
}