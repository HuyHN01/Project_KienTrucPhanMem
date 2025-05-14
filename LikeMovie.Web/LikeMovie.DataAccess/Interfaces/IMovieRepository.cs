// LikeMovie.DataAccess/Interfaces/IMovieRepository.cs
using LikeMovie.Model.Entities;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LikeMovie.DataAccess.Interfaces
{
    public interface IMovieRepository
    {
        // CRUD cơ bản
        Task AddAsync(Movie movie);
        void Update(Movie movie);
        void Delete(Movie movie);

        // Get cơ bản
        Task<Movie?> GetByIdAsync(int id);
        Task<Movie?> GetByIdWithDetailsAsync(int id); // Bao gồm Genres, PosterMovie, etc.
        Task<IEnumerable<Movie>> GetAllAsync(string? sortBy = null);
        Task<IEnumerable<Movie>> GetAllOrderedByIdAsync(); // Cho Admin Index

        // Cho trang chủ và danh sách đặc biệt
        Task<IEnumerable<Movie>> GetRecentlyReleasedAsync(int count);
        Task<IEnumerable<Movie>> GetTopViewedAsync(int count);
        Task<IEnumerable<Movie>> GetTopRatedAsync(int count);
        Task<IEnumerable<Movie>> GetComingSoonAsync(int? count = null, Expression<Func<Movie, bool>>? dateFilter = null);

        // Lọc và sắp xếp
        Task<IEnumerable<Movie>> GetMoviesByTypeAsync(int type, int? count = null, string? sortBy = null);
        Task<IEnumerable<Movie>> GetMoviesByGenreAsync(int genreId, int? count = null, string? sortBy = null); // genreId cụ thể
        Task<IEnumerable<Movie>> GetMoviesByOptionalGenreAsync(int? genreId, int? count = null, string? sortBy = null); // genreId có thể null
        Task<IEnumerable<Movie>> GetMoviesByYearAsync(int? year, int? count = null, string? sortBy = null);

        // Chức năng cụ thể
        Task<IEnumerable<Movie>> GetRelatedMoviesAsync(int currentMovieId, IEnumerable<int> genreIds, int count);
        Task<IEnumerable<Movie>> SearchMoviesAsync(string searchTerm);

        // Tiện ích
        Task<IEnumerable<int>> GetDistinctReleaseYearsAsync();
    }
}