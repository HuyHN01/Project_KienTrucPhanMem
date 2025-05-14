// LikeMovie.Business/Interfaces/IAdminAuthService.cs
using LikeMovie.Business.Dtos.AdminMovieDtos; // Hoặc AdminUserDtos
using System.Threading.Tasks;

namespace LikeMovie.Business.Interfaces
{
    public interface IAdminAuthService
    {
        /// <summary>
        /// Xử lý đăng nhập cho quản trị viên.
        /// </summary>
        /// <param name="loginDto">Thông tin đăng nhập của admin.</param>
        /// <returns>Thông tin admin nếu đăng nhập thành công, cùng với trạng thái và thông báo lỗi (nếu có).</returns>
        Task<(bool success, string? errorMessage, AdminUserDto? adminUser)> LoginAsync(AdminUserLoginDto loginDto);

        // Logout cho Admin thường được xử lý bằng cách xóa cookie xác thực ở tầng Presentation (Controller).
        // Task LogoutAsync(); (Có thể không cần thiết ở đây)
    }
}