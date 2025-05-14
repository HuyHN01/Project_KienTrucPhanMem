// LikeMovie.Business/Interfaces/IAuthService.cs
using LikeMovie.Business.Dtos.UserDtos;
using Microsoft.AspNetCore.Http; // Cho IFormFile
using System.Threading.Tasks;

namespace LikeMovie.Business.Interfaces
{
    public interface IAuthService
    {
        /// <summary>
        /// Xử lý đăng nhập cho người dùng thường.
        /// </summary>
        Task<(bool success, string? errorMessage, UserDto? user)> LoginAsync(UserLoginDto loginDto);

        /// <summary>
        /// Xử lý đăng ký tài khoản mới cho người dùng.
        /// </summary>
        Task<(bool success, string? errorMessage, UserDto? user)> RegisterAsync(UserRegisterDto registerDto, IFormFile? avatarFile);

        /// <summary>
        /// Kiểm tra xem username đã tồn tại chưa.
        /// </summary>
        Task<bool> UsernameExistsAsync(string username);

        /// <summary>
        /// Kiểm tra xem email đã tồn tại chưa.
        /// </summary>
        Task<bool> EmailExistsAsync(string email);

        /// <summary>
        /// Xử lý callback từ Google sau khi người dùng xác thực.
        /// </summary>
        Task<(bool success, string? errorMessage, UserDto? user)> HandleGoogleLoginCallbackAsync(string code);

        /// <summary>
        /// Xử lý callback từ Facebook sau khi người dùng xác thực.
        /// </summary>
        Task<(bool success, string? errorMessage, UserDto? user)> HandleFacebookLoginCallbackAsync(string code);

        /// <summary>
        /// Cho phép người dùng đang đăng nhập thay đổi mật khẩu.
        /// </summary>
        /// <param name="userId">ID của người dùng đang thực hiện.</param>
        Task<(bool success, string? errorMessage)> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);

        /// <summary>
        /// Xử lý yêu cầu quên mật khẩu (gửi OTP/link reset).
        /// </summary>
        Task<(bool success, string? errorMessage)> RequestPasswordResetAsync(ForgotPasswordDto forgotPasswordDto);

        /// <summary>
        /// Xác thực OTP và đặt lại mật khẩu mới.
        /// </summary>
        Task<(bool success, string? errorMessage)> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);

        // Task LogoutAsync(); // Logout thường xử lý ở Presentation Layer
    }
}