// LikeMovie.Business/Interfaces/IMovieService.cs
using LikeMovie.Business.Dtos.MovieDtos;
using Microsoft.AspNetCore.Http; // Cho IFormFile
using System.Collections.Generic;
// using System.Linq.Expressions; // Bỏ đi nếu không có phương thức nào trực tiếp dùng nó làm tham số ở Service Interface
using System.Threading.Tasks;

namespace LikeMovie.Business.Interfaces
{
    public interface IMovieService
    {
        // --- Lấy dữ liệu phim ---
        Task<MovieDto?> GetMovieByIdAsync(int id);
        Task<IEnumerable<MovieSummaryDto>> GetAllMoviesForUserAsync(string? sortBy = null);
        Task<IEnumerable<MovieSummaryDto>> GetAllMoviesForAdminAsync(string? sortBy = null);

        Task<IEnumerable<MovieSummaryDto>> GetRecentlyReleasedMoviesAsync(int count);
        Task<IEnumerable<MovieSummaryDto>> GetTopViewedMoviesAsync(int count);
        Task<IEnumerable<MovieSummaryDto>> GetTopRatedMoviesAsync(int count);
        Task<IEnumerable<MovieSummaryDto>> GetComingSoonMoviesAsync(int? count = null); // Service sẽ tự xử lý logic ngày hiện tại

        Task<IEnumerable<MovieSummaryDto>> GetMoviesByTypeAsync(int type, int? count = null, string? sortBy = null);
        Task<IEnumerable<MovieSummaryDto>> GetMoviesByGenreAsync(int genreId, int? count = null, string? sortBy = null);
        Task<IEnumerable<MovieSummaryDto>> GetMoviesByOptionalGenreAsync(int? genreId, int? count = null, string? sortBy = null);
        Task<IEnumerable<MovieSummaryDto>> GetMoviesByYearAsync(int? year, int? count = null, string? sortBy = null);

        Task<IEnumerable<MovieSummaryDto>> GetRelatedMoviesAsync(int currentMovieId, int count);
        Task<IEnumerable<MovieSummaryDto>> SearchMoviesAsync(string searchTerm);

        Task<IEnumerable<int>> GetDistinctReleaseYearsAsync();

        // --- Quản lý phim (Admin) ---
        Task<(MovieDto? movie, string? errorMessage)> CreateMovieAsync(CreateMovieDto movieDto, IFormFile? thumbnailFile);
        Task<(bool success, string? errorMessage, MovieDto? updatedMovie)> UpdateMovieAsync(int id, UpdateMovieDto movieDto, IFormFile? thumbnailFile);
        Task<(bool success, string? errorMessage)> DeleteMovieAsync(int id);

        // --- Nghiệp vụ khác ---
        Task<bool> CanUserViewMovieAsync(int? userId, int movieId);
    }
}