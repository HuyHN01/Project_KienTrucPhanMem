using LikeMovie.Model.Entities; // Giả sử Entity là PosterMovie
using System.Collections.Generic;
using System.Threading.Tasks;
// using X.PagedList; // Cân nhắc

namespace LikeMovie.DataAccess.Interfaces
{
    public interface IPosterMovieRepository
    {
        // Lấy tất cả các PosterMovie được đánh dấu là Slider, đã sắp xếp
        // (Việc phân trang sẽ do tầng trên xử lý hoặc thêm tham số vào đây)
        Task<IEnumerable<PosterMovie>> GetSlidersAsync();

        Task<PosterMovie?> GetByIdAsync(int posterId);

        // Lấy PosterMovie theo ID, bao gồm cả thông tin Movie liên quan
        Task<PosterMovie?> GetByIdWithMovieAsync(int posterId);

        Task AddAsync(PosterMovie posterMovie);
        void Update(PosterMovie posterMovie);
        void Delete(PosterMovie posterMovie);

        Task<IEnumerable<PosterMovie>> GetActiveSlidersWithMoviesAsync();

        // Nếu bạn quyết định Repository xử lý phân trang:
        // Task<IPagedList<PosterMovie>> GetPagedSlidersAsync(int pageNumber, int pageSize);
    }
}