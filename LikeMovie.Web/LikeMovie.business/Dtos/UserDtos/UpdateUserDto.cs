// Giả sử file này nằm trong LikeMovie.Business/Dtos/UserDtos/UpdateUserDto.cs
using System.ComponentModel.DataAnnotations;

namespace LikeMovie.Business.Dtos.UserDtos
{
    public class UpdateUserDto
    {
        // UserId thường được lấy từ người dùng đang xác thực, không cần client gửi lên
        // public int UserId { get; set; }

        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự.")]
        public string? FullName { get; set; }

        // Email có thể cho phép thay đổi hoặc không, tùy theo yêu cầu
        // [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
        // [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự.")]
        // public string? Email { get; set; }

        // AvatarURL sẽ được xử lý riêng nếu cho phép thay đổi avatar
    }
}