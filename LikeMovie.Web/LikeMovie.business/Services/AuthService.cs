// LikeMovie.Business/Services/AuthService.cs
using AutoMapper;
using LikeMovie.Business.Dtos.UserDtos;
using LikeMovie.Business.Interfaces;
using LikeMovie.DataAccess.Interfaces;
using LikeMovie.Model.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq; // Cần cho Any()
using System.Threading.Tasks;
// using System.Security.Cryptography; // Nếu tự hash
// using Microsoft.AspNetCore.Identity; // Nếu dùng ASP.NET Core Identity

namespace LikeMovie.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork; // Hoặc LikeMovieDbContext
        private readonly IMapper _mapper;
        private readonly ILogger<AuthService> _logger;
        private readonly IEmailService _emailService; // Inject nếu cần gửi mail
        // private readonly IFileStorageService _fileStorageService; // Inject nếu xử lý avatar
        // private readonly IPasswordHashingService _passwordHashingService;
        // private readonly IGoogleAuthService _googleAuthService; // Service con cho Google
        // private readonly IFacebookAuthService _facebookAuthService; // Service con cho Facebook

        public AuthService(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<AuthService> logger,
            IEmailService emailService
            /*, IFileStorageService fileStorageService, IPasswordHashingService passwordHashingService, ... */)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _emailService = emailService;
            // _fileStorageService = fileStorageService;
            // _passwordHashingService = passwordHashingService;
        }

        public async Task<(bool success, string? errorMessage, UserDto? user)> LoginAsync(UserLoginDto loginDto)
        {
            try
            {
                var userEntity = await _userRepository.GetByUsernameAsync(loginDto.UserName);
                if (userEntity == null)
                {
                    return (false, "Tên đăng nhập hoặc mật khẩu không đúng.", null);
                }

                // TODO: Implement password verification using a secure hashing service
                // Ví dụ: if (!_passwordHashingService.VerifyPassword(loginDto.Password, userEntity.PasswordHash))
                if (userEntity.PasswordHash != loginDto.Password) // CHỈ DÙNG CHO DEMO - RẤT KHÔNG AN TOÀN
                {
                    _logger.LogWarning("User login failed for {Username}: Invalid password.", loginDto.UserName);
                    return (false, "Tên đăng nhập hoặc mật khẩu không đúng.", null);
                }

                /*if (!userEntity.IsActive) // Giả sử có trường IsActive
                {
                    return (false, "Tài khoản của bạn đã bị khóa.", null);
                }*/

                var userDto = _mapper.Map<UserDto>(userEntity);
                _logger.LogInformation("User {Username} logged in successfully.", loginDto.UserName);
                return (true, null, userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during user login for {Username}.", loginDto.UserName);
                return (false, "Đã có lỗi xảy ra trong quá trình đăng nhập.", null);
            }
        }

        public async Task<(bool success, string? errorMessage, UserDto? user)> RegisterAsync(UserRegisterDto registerDto, IFormFile? avatarFile)
        {
            try
            {
                if (await _userRepository.UsernameExistsAsync(registerDto.UserName))
                {
                    return (false, "Tên đăng nhập đã tồn tại.", null);
                }
                if (await _userRepository.EmailExistsAsync(registerDto.Email))
                {
                    return (false, "Email đã được sử dụng.", null);
                }

                var userEntity = _mapper.Map<User>(registerDto);

                // TODO: Hash password securely
                // userEntity.PasswordHash = _passwordHashingService.HashPassword(registerDto.Password);
                userEntity.PasswordHash = registerDto.Password; // CHỈ DÙNG CHO DEMO - RẤT KHÔNG AN TOÀN

                userEntity.DateCreated = DateTime.UtcNow; // Hoặc DateTime.Now
                userEntity.IsActive = true; // Mặc định kích hoạt tài khoản
                userEntity.LevelVip = 0; // Mặc định level VIP

                if (avatarFile != null)
                {
                    // TODO: userEntity.AvatarUrl = await _fileStorageService.SaveUserAvatarAsync(avatarFile);
                    userEntity.AvatarUrl = $"/uploads/avatars/{Guid.NewGuid()}_{avatarFile.FileName}"; // Placeholder
                }
                else
                {
                    userEntity.AvatarUrl = "/images/avatar/default_user.png"; // Đường dẫn avatar mặc định
                }

                await _userRepository.AddAsync(userEntity);
                await _unitOfWork.SaveChangesAsync();

                // Gửi email xác nhận (không chặn kết quả trả về của đăng ký)
                // _ = _emailService.SendRegistrationConfirmationEmailAsync(userEntity.Email, userEntity.UserName);

                var userDto = _mapper.Map<UserDto>(userEntity);
                _logger.LogInformation("User {Username} registered successfully with ID {UserId}.", userDto.UserName, userDto.UserId);
                return (true, null, userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during user registration for {Username}.", registerDto.UserName);
                return (false, "Đã có lỗi xảy ra trong quá trình đăng ký.", null);
            }
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _userRepository.UsernameExistsAsync(username);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _userRepository.EmailExistsAsync(email);
        }

        // Implement các phương thức khác của IAuthService (ChangePasswordAsync, OAuth handlers,...)
        // Ví dụ HandleGoogleLoginCallbackAsync:
        // 1. Gọi Google API để exchange code lấy access token (có thể tạo GoogleAuthService riêng)
        // 2. Gọi Google API để lấy user info từ access token
        // 3. Kiểm tra user trong DB bằng email
        // 4. Nếu có, đăng nhập. Nếu chưa, tạo user mới rồi đăng nhập.
        // 5. Map sang UserDto và trả về.

        public async Task<(bool success, string? errorMessage, UserDto? user)> HandleGoogleLoginCallbackAsync(string code)
        {
            _logger.LogInformation("Handling Google login callback with code {Code}", code);
            // TODO: Implement Google OAuth logic
            // 1. Exchange code for tokens
            // 2. Get user info from Google
            // 3. Find or create user in DB
            // 4. Map to UserDto
            return await Task.FromResult((false, "Chức năng đăng nhập Google chưa được triển khai.", (UserDto?)null));
        }

        public async Task<(bool success, string? errorMessage, UserDto? user)> HandleFacebookLoginCallbackAsync(string code)
        {
            _logger.LogInformation("Handling Facebook login callback with code {Code}", code);
            // TODO: Implement Facebook OAuth logic
            return await Task.FromResult((false, "Chức năng đăng nhập Facebook chưa được triển khai.", (UserDto?)null));
        }


        public async Task<(bool success, string? errorMessage)> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
        {
            try
            {
                var userEntity = await _userRepository.GetByIdAsync(userId);
                if (userEntity == null)
                {
                    return (false, "Người dùng không tồn tại.");
                }

                // TODO: Verify current password
                // if (!_passwordHashingService.VerifyPassword(changePasswordDto.CurrentPassword, userEntity.PasswordHash))
                if (userEntity.PasswordHash != changePasswordDto.CurrentPassword) // KHÔNG AN TOÀN
                {
                    return (false, "Mật khẩu hiện tại không chính xác.");
                }

                // TODO: Hash new password
                // userEntity.PasswordHash = _passwordHashingService.HashPassword(changePasswordDto.NewPassword);
                userEntity.PasswordHash = changePasswordDto.NewPassword; // KHÔNG AN TOÀN

                _userRepository.Update(userEntity);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Password changed successfully for UserID {UserId}.", userId);
                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for UserID {UserId}.", userId);
                return (false, "Đã có lỗi xảy ra khi thay đổi mật khẩu.");
            }
        }

        public async Task<(bool success, string? errorMessage)> RequestPasswordResetAsync(ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                var userEntity = await _userRepository.GetByEmailAsync(forgotPasswordDto.Email);
                if (userEntity == null)
                {
                    // Không thông báo email không tồn tại để tránh lộ thông tin
                    _logger.LogWarning("Password reset requested for non-existent email {Email}.", forgotPasswordDto.Email);
                    return (true, "Nếu email của bạn tồn tại trong hệ thống, bạn sẽ nhận được hướng dẫn đặt lại mật khẩu.");
                }

                // TODO: Generate OTP or reset token, save it (e.g., with expiration)
                string otp = new Random().Next(100000, 999999).ToString(); // Ví dụ OTP đơn giản
                // Lưu OTP và UserId (có thể kèm thời gian hết hạn) vào Cache hoặc một bảng tạm
                // Session["ResetOtp"] = otp; Session["ResetUserId"] = userEntity.UserId; (Đây là cách cũ)

                // Gửi OTP qua email
                bool emailSent = await _emailService.SendOtpEmailAsync(userEntity.Email, userEntity.UserName ?? "User", otp);
                if (!emailSent)
                {
                    return (false, "Không thể gửi email OTP. Vui lòng thử lại.");
                }
                _logger.LogInformation("Password reset OTP sent to {Email} for UserID {UserId}.", userEntity.Email, userEntity.UserId);
                return (true, "Mã OTP đã được gửi đến email của bạn.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error requesting password reset for email {Email}.", forgotPasswordDto.Email);
                return (false, "Đã có lỗi xảy ra khi yêu cầu đặt lại mật khẩu.");
            }
        }

        public async Task<(bool success, string? errorMessage)> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                // TODO: Verify OTP and UserId/Token (lấy từ Cache hoặc bảng tạm đã lưu ở bước RequestPasswordReset)
                // var storedOtp = Session["ResetOtp"]?.ToString();
                // var storedUserId = (int?)Session["ResetUserId"];
                // if (storedOtp != resetPasswordDto.Otp || !storedUserId.HasValue || storedUserId.Value.ToString() != resetPasswordDto.UserIdOrEmailOrToken)
                // {
                //     return (false, "Mã OTP không hợp lệ hoặc đã hết hạn.");
                // }

                var userEntity = await _userRepository.GetByIdAsync(Convert.ToInt32(resetPasswordDto.UserIdOrEmailOrToken)); // Giả định UserIdOrEmailOrToken là UserId
                if (userEntity == null)
                {
                    return (false, "Người dùng không tồn tại.");
                }

                // TODO: Hash new password
                // userEntity.PasswordHash = _passwordHashingService.HashPassword(resetPasswordDto.NewPassword);
                userEntity.PasswordHash = resetPasswordDto.NewPassword; // KHÔNG AN TOÀN

                _userRepository.Update(userEntity);
                await _unitOfWork.SaveChangesAsync();

                // Xóa OTP đã sử dụng
                // Session.Remove("ResetOtp"); Session.Remove("ResetUserId");
                _logger.LogInformation("Password reset successfully for UserID {UserId}.", userEntity.UserId);
                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for {UserIdOrToken}.", resetPasswordDto.UserIdOrEmailOrToken);
                return (false, "Đã có lỗi xảy ra khi đặt lại mật khẩu.");
            }
        }
    }
}