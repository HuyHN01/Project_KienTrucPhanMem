// Giả sử file này nằm trong LikeMovie.Business/Dtos/UserDtos/UserLoginDto.cs
using System.ComponentModel.DataAnnotations;

namespace LikeMovie.Business.Dtos.UserDtos
{
    public class UserLoginDto
    {
        [Required(ErrorMessage = "Tên đăng nhập không được để trống.")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}