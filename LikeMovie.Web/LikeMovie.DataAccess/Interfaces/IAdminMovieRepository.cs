using LikeMovie.Model.Entities; // Giả sử AdminMovie là một Entity trong LikeMovie.Model.Entities
using System.Threading.Tasks;

namespace LikeMovie.DataAccess.Interfaces
{
    public interface IAdminMovieRepository
    {
        // Lấy thông tin Admin theo Username
        Task<AdminMovie?> GetByUsernameAsync(string username);

        // Có thể bạn sẽ cần các phương thức khác sau này, ví dụ:
        // Task<AdminMovie?> GetByIdAsync(int adminId);
        // Task AddAsync(AdminMovie admin);
        // void Update(AdminMovie admin);
    }
}