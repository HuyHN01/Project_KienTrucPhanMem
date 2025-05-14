using LikeMovie.Model.Entities;
using System.Collections.Generic; // Thêm nếu có GetAllAsync
using System.Threading.Tasks;

namespace LikeMovie.DataAccess.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int userId);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByUsernameAsync(string username); // Thêm phương thức này

        Task AddAsync(User user);
        void Update(User user); // Giữ lại phương thức này

        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);

        // (Tùy chọn) Nếu Admin có trang quản lý tất cả người dùng
        // Task<IEnumerable<User>> GetAllAsync(string? searchTerm = null, string? sortBy = null /*, int pageNumber, int pageSize*/);

        // (Tùy chọn) Nếu cần xóa người dùng (cân nhắc soft delete)
        // void Delete(User user);
        // Task<bool> DeleteByIdAsync(int userId);
    }
}