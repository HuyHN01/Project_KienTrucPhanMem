// LikeMovie.Business/Services/UserService.cs
using AutoMapper;
using LikeMovie.Business.Dtos.UserDtos;
using LikeMovie.Business.Interfaces;
using LikeMovie.DataAccess.Interfaces;
using LikeMovie.Model.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LikeMovie.Business.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        // private readonly IFileStorageService _fileStorageService; // Inject nếu xử lý avatar

        public UserService(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<UserService> logger
            /*, IFileStorageService fileStorageService */)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            // _fileStorageService = fileStorageService;
        }

        public async Task<UserDto?> GetUserByIdAsync(int userId)
        {
            var userEntity = await _userRepository.GetByIdAsync(userId);
            return _mapper.Map<UserDto>(userEntity);
        }

        public async Task<UserDto?> GetUserByUsernameAsync(string username)
        {
            var userEntity = await _userRepository.GetByUsernameAsync(username);
            return _mapper.Map<UserDto>(userEntity);
        }
        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            var userEntity = await _userRepository.GetByEmailAsync(email);
            return _mapper.Map<UserDto>(userEntity);
        }


        public async Task<(bool success, string? errorMessage, UserDto? updatedUser)> UpdateUserProfileAsync(int userId, UpdateUserDto updateDto, IFormFile? avatarFile)
        {
            try
            {
                var userEntity = await _userRepository.GetByIdAsync(userId);
                if (userEntity == null)
                {
                    return (false, "Người dùng không tồn tại.", null);
                }


                if (avatarFile != null)
                {
                    // TODO: Gọi _fileStorageService để lưu avatar mới, có thể xóa avatar cũ
                    // userEntity.AvatarUrl = await _fileStorageService.SaveUserAvatarAsync(avatarFile);
                    userEntity.AvatarUrl = $"/uploads/avatars/{Guid.NewGuid()}_{avatarFile.FileName}"; // Placeholder
                }

                _userRepository.Update(userEntity);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("User profile updated successfully for UserID {UserId}.", userId);
                return (true, null, _mapper.Map<UserDto>(userEntity));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile for UserID {UserId}.", userId);
                return (false, "Đã có lỗi xảy ra khi cập nhật thông tin người dùng.", null);
            }
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersForAdminAsync(/* tham số phân trang, tìm kiếm */)
        {
            // var userEntities = await _userRepository.GetAllAsync(); // Giả sử có phương thức này
            // return _mapper.Map<IEnumerable<UserDto>>(userEntities);
            _logger.LogInformation("Fetching all users for admin.");
            return await Task.FromResult(new List<UserDto>()); // Placeholder, cần implement đầy đủ
        }
    }
}