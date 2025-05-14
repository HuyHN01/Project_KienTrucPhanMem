using LikeMovie.Model.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LikeMovie.DataAccess.Interfaces
{
    public interface IGenreRepository
    {
        Task<IEnumerable<Genre>> GetAllAsync(); // Lấy tất cả Genre (MaTL, TenTL)
        Task<Genre?> GetByIdAsync(int genreId); // Lấy Genre theo ID (genresID, TenCD)
        Task AddAsync(Genre genre);             // Thêm Genre mới
        void Update(Genre genre);             // Cập nhật Genre
        void Delete(Genre genre);             // Xóa Genre
        Task<Genre?> GetByNameAsync(string name); // Hữu ích để kiểm tra tên đã tồn tại chưa
        Task<bool> ExistsAsync(int genreId);   // Kiểm tra sự tồn tại bằng ID
    }
}