// LikeMovie.Business/Services/AdminAuthService.cs
using AutoMapper;
using LikeMovie.Business.Dtos.AdminMovieDtos; // Hoặc AdminUserDtos
using LikeMovie.Business.Interfaces;
using LikeMovie.DataAccess.Interfaces; // Cho IAdminMovieRepository
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
// using System.Security.Cryptography; // Nếu tự hash password
// using Microsoft.AspNetCore.Identity; // Nếu dùng ASP.NET Core Identity cho Admin

namespace LikeMovie.Business.Services
{
    public class AdminAuthService : IAdminAuthService
    {
        private readonly IAdminMovieRepository _adminMovieRepository; // Hoặc IAdminUserRepository
        private readonly IMapper _mapper;
        private readonly ILogger<AdminAuthService> _logger;
        // private readonly IPasswordHashingService _passwordHashingService; // Nếu tách riêng

        public AdminAuthService(
            IAdminMovieRepository adminMovieRepository,
            IMapper mapper,
            ILogger<AdminAuthService> logger
            /*, IPasswordHashingService passwordHashingService */)
        {
            _adminMovieRepository = adminMovieRepository;
            _mapper = mapper;
            _logger = logger;
            // _passwordHashingService = passwordHashingService;
        }

        public async Task<(bool success, string? errorMessage, AdminUserDto? adminUser)> LoginAsync(AdminUserLoginDto loginDto)
        {
            try
            {
                var adminEntity = await _adminMovieRepository.GetByUsernameAsync(loginDto.UsernameAd);
                if (adminEntity == null)
                {
                    return (false, "Tên đăng nhập hoặc mật khẩu không đúng.", null);
                }

                // TODO: Implement password verification
                // Đây là ví dụ đơn giản, bạn PHẢI dùng cơ chế hashing an toàn
                // Ví dụ: if (!_passwordHashingService.VerifyPassword(loginDto.PasswordAd, adminEntity.PasswordHash))
                if (adminEntity.PasswordAd != loginDto.PasswordAd) // CHỈ DÙNG CHO DEMO - RẤT KHÔNG AN TOÀN
                {
                    _logger.LogWarning("Admin login failed for user {Username}: Invalid password.", loginDto.UsernameAd);
                    return (false, "Tên đăng nhập hoặc mật khẩu không đúng.", null);
                }

                // Nếu thành công, map sang DTO để trả về (không bao gồm password hash)
                var adminUserDto = _mapper.Map<AdminUserDto>(adminEntity);
                _logger.LogInformation("Admin {Username} logged in successfully.", loginDto.UsernameAd);
                return (true, null, adminUserDto);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Exception during admin login for {Username}.", loginDto.UsernameAd);
                return (false, "Đã có lỗi xảy ra trong quá trình đăng nhập.", null);
            }
        }
    }
}