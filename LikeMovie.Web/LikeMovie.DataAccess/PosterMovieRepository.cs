using LikeMovie.DataAccess.Interfaces;
using LikeMovie.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// using X.PagedList;

namespace LikeMovie.DataAccess.Repositories
{
    public class PosterMovieRepository : IPosterMovieRepository
    {
        private readonly LikeMovieDbContext _context;

        public PosterMovieRepository(LikeMovieDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PosterMovie>> GetSlidersAsync()
        {
            return await _context.PosterMovies // Giả định DbSet tên là PosterMovies
                                 .Where(p => p.IsSlider == true) // Hoặc p.IsSlider.HasValue && p.IsSlider.Value nếu IsSlider là nullable bool
                                 .OrderBy(p => p.SliderOrder)
                                 .ToListAsync();
        }

        public async Task<PosterMovie?> GetByIdAsync(int posterId)
        {
            return await _context.PosterMovies.FindAsync(posterId);
        }

        public async Task<PosterMovie?> GetByIdWithMovieAsync(int posterId)
        {
            // Giả sử PosterMovie có navigation property tên là Movie (hoặc Movies nếu là collection)
            // Nếu PosterMovie.Movies là ICollection<Movie>, bạn có thể cần FirstOrDefault() nếu chỉ có 1 movie liên kết.
            // Hoặc nếu PosterMovie có MovieId và navigation property Movie:
            return await _context.PosterMovies
                                 .Include(p => p.Movie) // Hoặc p.Movies nếu tên khác
                                 .FirstOrDefaultAsync(p => p.PosterId == posterId);
        }

        public async Task AddAsync(PosterMovie posterMovie)
        {
            // Đảm bảo IsSlider được set là true trước khi gọi AddAsync từ BLL nếu cần
            await _context.PosterMovies.AddAsync(posterMovie);
            // SaveChangesAsync ở BLL/UnitOfWork
        }

        public void Update(PosterMovie posterMovie)
        {
            // Đảm bảo IsSlider được set là true trước khi gọi Update từ BLL nếu cần
            _context.PosterMovies.Update(posterMovie);
            // SaveChangesAsync ở BLL/UnitOfWork
        }

        public void Delete(PosterMovie posterMovie)
        {
            _context.PosterMovies.Remove(posterMovie);
            // SaveChangesAsync ở BLL/UnitOfWork
        }

        public async Task<IEnumerable<PosterMovie>> GetActiveSlidersWithMoviesAsync()
        {
            // Logic này có thể giống hệt GetSlidersAsync nếu không có thêm điều kiện "active" nào khác
            // Hoặc nếu "active" có nghĩa là IsSlider == true và một số điều kiện khác (ví dụ: ngày hiệu lực)
            // Hiện tại, chúng ta giả định "active sliders" là những poster được đánh dấu IsSlider = true
            return await _context.PosterMovies
                                 .Where(p => p.IsSlider == true) // Đảm bảo chỉ lấy slider
                                 .Include(p => p.Movie)       // Include thông tin Movie
                                 .OrderBy(p => p.SliderOrder) // Sắp xếp (nếu có SliderOrder)
                                 .ToListAsync();
        }
        // Nếu implement GetPagedSlidersAsync:
        // public async Task<IPagedList<PosterMovie>> GetPagedSlidersAsync(int pageNumber, int pageSize)
        // {
        //     return await _context.PosterMovies
        //                        .Where(p => p.IsSlider == true)
        //                        .OrderBy(p => p.SliderOrder)
        //                        .ToPagedListAsync(pageNumber, pageSize);
        // }
    }
}