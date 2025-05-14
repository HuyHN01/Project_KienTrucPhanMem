// Giả sử file này nằm trong LikeMovie.Business/Dtos/UserDtos/ResetPasswordDto.cs
using System.ComponentModel.DataAnnotations;

namespace LikeMovie.Business.Dtos.UserDtos
{
    public class ResetPasswordDto
    {
        [Required] // Thông tin này thường được ẩn hoặc truyền qua token
        public string? UserIdOrEmailOrToken { get; set; } // Dùng để xác định người dùng

        [Required(ErrorMessage = "Mã OTP không được để trống.")] // Nếu dùng OTP
        public string Otp { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu mới không được để trống.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu mới phải có ít nhất 6 ký tự.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu mới và xác nhận mật khẩu mới không khớp.")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}