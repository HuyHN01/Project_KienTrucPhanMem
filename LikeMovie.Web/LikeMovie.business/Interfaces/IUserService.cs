// LikeMovie.Business/Interfaces/IUserService.cs
using LikeMovie.Business.Dtos.UserDtos;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
// using X.PagedList;

namespace LikeMovie.Business.Interfaces
{
    public interface IUserService
    {
        Task<UserDto?> GetUserByIdAsync(int userId);
        Task<UserDto?> GetUserByUsernameAsync(string username); // Có thể cần cho một số trường hợp
        Task<UserDto?> GetUserByEmailAsync(string email);     // Có thể cần

        /// <summary>
        /// Cập nhật thông tin cá nhân của người dùng.
        /// </summary>
        /// <param name="userId">ID của người dùng cần cập nhật.</param>
        Task<(bool success, string? errorMessage, UserDto? updatedUser)> UpdateUserProfileAsync(int userId, UpdateUserDto updateDto, IFormFile? avatarFile);

        /// <summary>
        /// Lấy danh sách tất cả người dùng (cho Admin, có thể có phân trang).
        /// </summary>
        Task<IEnumerable<UserDto>> GetAllUsersForAdminAsync(/*int pageNumber, int pageSize, string? searchTerm*/);

        // Các nghiệp vụ khác liên quan đến User nếu có, ví dụ:
        // Task<bool> DeactivateUserAsync(int userId);
        // Task<bool> ActivateUserAsync(int userId);
    }
}