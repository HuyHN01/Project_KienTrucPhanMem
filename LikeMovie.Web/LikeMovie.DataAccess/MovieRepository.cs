// LikeMovie.DataAccess/Repositories/MovieRepository.cs
using LikeMovie.DataAccess.Interfaces;
using LikeMovie.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System; // Cần cho DateTime và DateOnly (nếu dùng)
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LikeMovie.DataAccess.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly LikeMovieDbContext _context;

        public MovieRepository(LikeMovieDbContext context)
        {
            _context = context;
        }

        // Helper private method cho ApplySorting
        private IQueryable<Movie> ApplySorting(IQueryable<Movie> movies, string? sortBy)
        {
            switch (sortBy?.ToLower()) // an toàn với null và không phân biệt hoa thường
            {
                case "rating":
                    return movies.OrderByDescending(m => m.Rating); // Giả sử có Rating
                case "view":
                    return movies.OrderByDescending(m => m.ViewCount); // Giả sử có ViewCount
                case "favorite":
                    // Giả sử Movie có navigation property Favorites (ICollection<Favorite>)
                    // Cần đảm bảo Favorites được Include nếu không sẽ lỗi NullReference hoặc query không hiệu quả
                    // Tốt nhất là tính toán số lượng yêu thích ở một nơi khác hoặc lưu trữ trực tiếp trong Movie Entity
                    return movies.OrderByDescending(m => m.Favorites.Count());
                case "releasedate_desc":
                    return movies.OrderByDescending(m => m.ReleaseDate);
                case "releasedate_asc":
                    return movies.OrderBy(m => m.ReleaseDate);
                // Thêm các trường hợp sắp xếp khác nếu cần
                default: // Mặc định hoặc nếu sortBy là null/empty
                    return movies.OrderBy(m => m.Title); // Hoặc OrderByDescending(m => m.MovieId)
            }
        }

        // --- Implement các phương thức từ IMovieRepository ---

        public async Task AddAsync(Movie movie)
        {
            await _context.Movies.AddAsync(movie);
            // SaveChangesAsync() sẽ ở BLL/UnitOfWork
        }

        public void Update(Movie movie)
        {
            _context.Movies.Update(movie);
            // SaveChangesAsync() sẽ ở BLL/UnitOfWork
        }

        public void Delete(Movie movie)
        {
            _context.Movies.Remove(movie);
            // SaveChangesAsync() sẽ ở BLL/UnitOfWork
        }

        public async Task<Movie?> GetByIdAsync(int id)
        {
            return await _context.Movies.FindAsync(id);
        }

        public async Task<Movie?> GetByIdWithDetailsAsync(int id)
        {
            // Hãy chắc chắn bạn có các navigation properties này trong Movie Entity
            // và chúng được cấu hình đúng trong DbContext (OnModelCreating nếu cần)
            return await _context.Movies
                                 .Include(m => m.Genres)
                                 .Include(m => m.PosterMovies) // Giả sử tên là PosterMovie
                                                              // .Include(m => m.Episodes) // Nếu có
                                                              // .Include(m => m.Seasons)  // Nếu có
                                 .FirstOrDefaultAsync(m => m.MovieId == id);
        }

        public async Task<IEnumerable<Movie>> GetAllAsync(string? sortBy = null)
        {
            var query = _context.Movies.AsQueryable();
            query = ApplySorting(query, sortBy);
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Movie>> GetAllOrderedByIdAsync()
        {
            return await _context.Movies.OrderBy(m => m.MovieId).ToListAsync();
        }

        public async Task<IEnumerable<Movie>> GetRecentlyReleasedAsync(int count)
        {
            // Giả định ReleaseDate là DateTime?
            // Nếu là DateOnly?, bạn cần xử lý so sánh và sắp xếp cho DateOnly
            return await _context.Movies
                                 .Where(m => m.ReleaseDate.HasValue)
                                 .OrderByDescending(m => m.ReleaseDate.Value)
                                 .Take(count)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Movie>> GetTopViewedAsync(int count)
        {
            return await _context.Movies
                                 .OrderByDescending(m => m.ViewCount) // Giả sử có ViewCount
                                 .Take(count)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Movie>> GetTopRatedAsync(int count)
        {
            return await _context.Movies
                                 .Where(m => m.Rating.HasValue) // Giả sử có Rating
                                 .OrderByDescending(m => m.Rating.Value)
                                 .Take(count)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Movie>> GetComingSoonAsync(int? count = null, Expression<Func<Movie, bool>>? dateFilter = null)
        {
            var query = _context.Movies.AsQueryable();
            if (dateFilter != null)
            {
                query = query.Where(dateFilter);
            }
            // Nếu không có dateFilter, bạn có thể muốn một điều kiện mặc định ở đây
            // ví dụ: .Where(m => m.ReleaseDate.HasValue && m.ReleaseDate.Value > DateTime.UtcNow)
            // nhưng tốt nhất là để BLL truyền dateFilter.

            query = query.OrderBy(m => m.ReleaseDate); // Sắp xếp phim sắp ra mắt theo ngày gần nhất

            if (count.HasValue)
            {
                query = query.Take(count.Value);
            }
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Movie>> GetMoviesByTypeAsync(int type, int? count = null, string? sortBy = null)
        {
            var query = _context.Movies.Where(m => m.Type == type); // Giả sử Movie Entity có thuộc tính 'Type'
            query = ApplySorting(query, sortBy);
            if (count.HasValue)
            {
                query = query.Take(count.Value);
            }
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Movie>> GetMoviesByGenreAsync(int genreId, int? count = null, string? sortBy = null)
        {
            var query = _context.Movies.Where(m => m.Genres.Any(g => g.GenreId == genreId));
            query = ApplySorting(query, sortBy);
            if (count.HasValue)
            {
                query = query.Take(count.Value);
            }
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Movie>> GetMoviesByOptionalGenreAsync(int? genreId, int? count = null, string? sortBy = null)
        {
            var query = _context.Movies.AsQueryable();
            if (genreId.HasValue)
            {
                query = query.Where(m => m.Genres.Any(g => g.GenreId == genreId.Value));
            }
            query = ApplySorting(query, sortBy);
            if (count.HasValue)
            {
                query = query.Take(count.Value);
            }
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Movie>> GetMoviesByYearAsync(int? year, int? count = null, string? sortBy = null)
        {
            var query = _context.Movies.AsQueryable();
            if (year.HasValue)
            {
                query = query.Where(m => m.ReleaseDate.HasValue && m.ReleaseDate.Value.Year == year.Value);
            }
            query = ApplySorting(query, sortBy);
            if (count.HasValue)
            {
                query = query.Take(count.Value);
            }
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Movie>> GetRelatedMoviesAsync(int currentMovieId, IEnumerable<int> genreIds, int count)
        {
            if (genreIds == null || !genreIds.Any())
            {
                return new List<Movie>();
            }
            return await _context.Movies
                                 .Include(m => m.Genres)
                                 .Where(m => m.MovieId != currentMovieId && m.Genres.Any(g => genreIds.Contains(g.GenreId)))
                                 .OrderByDescending(m => m.ReleaseDate) // Ví dụ sắp xếp theo ngày mới nhất
                                 .Take(count)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Movie>> SearchMoviesAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new List<Movie>();
            }

            var searchTermLower = searchTerm.ToLower();

            return await _context.Movies
                                 .Include(m => m.Genres)
                                 .Where(m =>
                                     (m.Title != null && m.Title.ToLower().Contains(searchTermLower)) ||
                                     (m.Director != null && m.Director.ToLower().Contains(searchTermLower)) ||
                                     (m.Genres != null && m.Genres.Any(g => g.Name != null && g.Name.ToLower().Contains(searchTermLower)))
                                 )
                                 .ToListAsync();
        }

        public async Task<IEnumerable<int>> GetDistinctReleaseYearsAsync()
        {
            return await _context.Movies
                                 .Where(m => m.ReleaseDate.HasValue)
                                 .Select(m => m.ReleaseDate!.Value.Year) // Dùng !.Value vì đã Where HasValue
                                 .Distinct()
                                 .OrderByDescending(y => y)
                                 .ToListAsync();
        }
    }
}