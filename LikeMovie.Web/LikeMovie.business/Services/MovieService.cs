// LikeMovie.Business/Services/MovieService.cs
using AutoMapper;
using LikeMovie.Business.Dtos.MovieDtos;
using LikeMovie.Business.Interfaces;
using LikeMovie.DataAccess.Interfaces;
using LikeMovie.Model.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions; // Cần cho việc tạo dateFilter nếu bạn làm ở đây
using System.Threading.Tasks;

namespace LikeMovie.Business.Services
{
    public class MovieService : IMovieService // Đảm bảo implement IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly IUnitOfWork _unitOfWork; // Hoặc LikeMovieDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<MovieService> _logger;
        // private readonly IFileStorageService _fileStorageService; // Inject nếu bạn đã tạo service này

        public MovieService(
            IMovieRepository movieRepository,
            IGenreRepository genreRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<MovieService> logger
            /*, IFileStorageService fileStorageService */)
        {
            _movieRepository = movieRepository;
            _genreRepository = genreRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            // _fileStorageService = fileStorageService;
        }

        // --- Implement các phương thức từ IMovieService ---

        public async Task<MovieDto?> GetMovieByIdAsync(int id)
        {
            try
            {
                var movieEntity = await _movieRepository.GetByIdWithDetailsAsync(id); // Giả định repository có phương thức này
                if (movieEntity == null)
                {
                    _logger.LogWarning("Movie with ID {MovieId} not found for GetMovieByIdAsync.", id);
                    return null;
                }
                return _mapper.Map<MovieDto>(movieEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetMovieByIdAsync for MovieID {MovieId}", id);
                return null;
            }
        }

        public async Task<IEnumerable<MovieSummaryDto>> GetAllMoviesForUserAsync(string? sortBy = null)
        {
            var movieEntities = await _movieRepository.GetAllAsync(sortBy);
            return _mapper.Map<IEnumerable<MovieSummaryDto>>(movieEntities);
        }

        public async Task<IEnumerable<MovieSummaryDto>> GetAllMoviesForAdminAsync(string? sortBy = null)
        {
            // Bạn có thể dùng GetAllAsync hoặc GetAllOrderedByIdAsync tùy theo nhu cầu sắp xếp cho Admin
            var movieEntities = await _movieRepository.GetAllOrderedByIdAsync();
            // Nếu muốn dùng sortBy chung:
            // var movieEntities = await _movieRepository.GetAllAsync(sortBy);
            return _mapper.Map<IEnumerable<MovieSummaryDto>>(movieEntities);
        }

        public async Task<IEnumerable<MovieSummaryDto>> GetRecentlyReleasedMoviesAsync(int count)
        {
            var movieEntities = await _movieRepository.GetRecentlyReleasedAsync(count);
            return _mapper.Map<IEnumerable<MovieSummaryDto>>(movieEntities);
        }

        public async Task<IEnumerable<MovieSummaryDto>> GetTopViewedMoviesAsync(int count)
        {
            var movieEntities = await _movieRepository.GetTopViewedAsync(count);
            return _mapper.Map<IEnumerable<MovieSummaryDto>>(movieEntities);
        }

        public async Task<IEnumerable<MovieSummaryDto>> GetTopRatedMoviesAsync(int count)
        {
            var movieEntities = await _movieRepository.GetTopRatedAsync(count);
            return _mapper.Map<IEnumerable<MovieSummaryDto>>(movieEntities);
        }

        // Trong MovieService.cs - phương thức GetComingSoonMoviesAsync
        public async Task<IEnumerable<MovieSummaryDto>> GetComingSoonMoviesAsync(int? count = null)
        {
            // Service sẽ quyết định dateFilter
            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow); // Lấy ngày hiện tại (UTC) dưới dạng DateOnly

            // Biểu thức lọc dựa trên DateOnly
            Expression<Func<Movie, bool>> dateFilter = m => m.ReleaseDate.HasValue && m.ReleaseDate.Value > today;

            var movieEntities = await _movieRepository.GetComingSoonAsync(count, dateFilter);
            return _mapper.Map<IEnumerable<MovieSummaryDto>>(movieEntities);
        }

        public async Task<IEnumerable<MovieSummaryDto>> GetMoviesByTypeAsync(int type, int? count = null, string? sortBy = null)
        {
            var movieEntities = await _movieRepository.GetMoviesByTypeAsync(type, count, sortBy);
            return _mapper.Map<IEnumerable<MovieSummaryDto>>(movieEntities);
        }

        public async Task<IEnumerable<MovieSummaryDto>> GetMoviesByGenreAsync(int genreId, int? count = null, string? sortBy = null)
        {
            var movieEntities = await _movieRepository.GetMoviesByGenreAsync(genreId, count, sortBy);
            return _mapper.Map<IEnumerable<MovieSummaryDto>>(movieEntities);
        }

        public async Task<IEnumerable<MovieSummaryDto>> GetMoviesByOptionalGenreAsync(int? genreId, int? count = null, string? sortBy = null)
        {
            var movieEntities = await _movieRepository.GetMoviesByOptionalGenreAsync(genreId, count, sortBy);
            return _mapper.Map<IEnumerable<MovieSummaryDto>>(movieEntities);
        }

        public async Task<IEnumerable<MovieSummaryDto>> GetMoviesByYearAsync(int? year, int? count = null, string? sortBy = null)
        {
            var movieEntities = await _movieRepository.GetMoviesByYearAsync(year, count, sortBy);
            return _mapper.Map<IEnumerable<MovieSummaryDto>>(movieEntities);
        }

        public async Task<IEnumerable<MovieSummaryDto>> GetRelatedMoviesAsync(int currentMovieId, int count)
        {
            var currentMovie = await _movieRepository.GetByIdWithDetailsAsync(currentMovieId); // Cần Include Genres
            if (currentMovie == null || currentMovie.Genres == null || !currentMovie.Genres.Any())
            {
                return new List<MovieSummaryDto>();
            }
            var genreIds = currentMovie.Genres.Select(g => g.GenreId).ToList();
            var relatedEntities = await _movieRepository.GetRelatedMoviesAsync(currentMovieId, genreIds, count);
            return _mapper.Map<IEnumerable<MovieSummaryDto>>(relatedEntities);
        }

        public async Task<IEnumerable<MovieSummaryDto>> SearchMoviesAsync(string searchTerm)
        {
            var movieEntities = await _movieRepository.SearchMoviesAsync(searchTerm);
            return _mapper.Map<IEnumerable<MovieSummaryDto>>(movieEntities);
        }

        public async Task<IEnumerable<int>> GetDistinctReleaseYearsAsync()
        {
            return await _movieRepository.GetDistinctReleaseYearsAsync();
        }

        public async Task<(MovieDto? movie, string? errorMessage)> CreateMovieAsync(CreateMovieDto movieDto, IFormFile? thumbnailFile)
        {
            try
            {
                var movieEntity = _mapper.Map<Movie>(movieDto);

                if (thumbnailFile != null)
                {
                    // TODO: Gọi _fileStorageService.SaveFileAsync(...) để lưu file và lấy đường dẫn
                    // Ví dụ: movieEntity.Thumbnail = await _fileStorageService.SaveMovieThumbnailAsync(thumbnailFile);
                    // Tạm thời gán placeholder:
                    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(thumbnailFile.FileName)}";
                    // var filePath = Path.Combine("wwwroot/uploads/movies", fileName); // Cần cấu hình đường dẫn đúng
                    // using (var stream = new FileStream(filePath, FileMode.Create)) { await thumbnailFile.CopyToAsync(stream); }
                    movieEntity.Thumbnail = $"/uploads/movies/{fileName}"; // Đường dẫn tương đối để lưu vào DB
                    _logger.LogInformation("Placeholder thumbnail path set for {MovieTitle}: {ThumbnailPath}", movieDto.Title, movieEntity.Thumbnail);
                }

                if (movieDto.GenreIds != null && movieDto.GenreIds.Any())
                {
                    movieEntity.Genres = new List<Genre>();
                    foreach (var genreId in movieDto.GenreIds)
                    {
                        var genre = await _genreRepository.GetByIdAsync(genreId);
                        if (genre != null) movieEntity.Genres.Add(genre);
                        else _logger.LogWarning("GenreId {GenreId} not found for movie {Title}", genreId, movieDto.Title);
                    }
                }

                await _movieRepository.AddAsync(movieEntity);
                await _unitOfWork.SaveChangesAsync();

                var createdMovieDto = _mapper.Map<MovieDto>(movieEntity);
                return (createdMovieDto, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating movie with title {MovieTitle}", movieDto.Title);
                return (null, "Lỗi khi tạo phim.");
            }
        }

        public async Task<(bool success, string? errorMessage, MovieDto? updatedMovie)> UpdateMovieAsync(int id, UpdateMovieDto movieDto, IFormFile? thumbnailFile)
        {
            try
            {
                var movieEntity = await _movieRepository.GetByIdWithDetailsAsync(id); // Lấy cả Genres
                if (movieEntity == null) return (false, "Không tìm thấy phim.", null);

                _mapper.Map(movieDto, movieEntity); // Cập nhật các trường từ DTO

                if (thumbnailFile != null)
                {
                    // TODO: Xử lý lưu file mới, có thể xóa file cũ (cần IFileStorageService)
                    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(thumbnailFile.FileName)}";
                    movieEntity.Thumbnail = $"/uploads/movies/{fileName}";
                }

                if (movieDto.GenreIds != null)
                {
                    // Xóa các genre cũ không còn trong danh sách mới
                    var genresToRemove = movieEntity.Genres.Where(g => !movieDto.GenreIds.Contains(g.GenreId)).ToList();
                    foreach (var genre in genresToRemove) movieEntity.Genres.Remove(genre);

                    // Thêm các genre mới chưa có
                    foreach (var genreId in movieDto.GenreIds)
                    {
                        if (!movieEntity.Genres.Any(g => g.GenreId == genreId))
                        {
                            var genre = await _genreRepository.GetByIdAsync(genreId);
                            if (genre != null) movieEntity.Genres.Add(genre);
                        }
                    }
                }

                _movieRepository.Update(movieEntity); // EF Core theo dõi thay đổi
                await _unitOfWork.SaveChangesAsync();

                return (true, null, _mapper.Map<MovieDto>(movieEntity));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating movie with ID {MovieId}", id);
                return (false, "Lỗi khi cập nhật phim.", null);
            }
        }

        public async Task<(bool success, string? errorMessage)> DeleteMovieAsync(int id)
        {
            try
            {
                var movieEntity = await _movieRepository.GetByIdAsync(id);
                if (movieEntity == null) return (false, "Không tìm thấy phim.");

                _movieRepository.Delete(movieEntity);
                // TODO: Xóa file thumbnail liên quan (cần IFileStorageService)
                await _unitOfWork.SaveChangesAsync();
                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting movie with ID {MovieId}", id);
                return (false, "Lỗi khi xóa phim.");
            }
        }

        public async Task<bool> CanUserViewMovieAsync(int? userId, int movieId)
        {
            var movie = await _movieRepository.GetByIdAsync(movieId);
            if (movie == null)
            {
                _logger.LogWarning("CanUserViewMovieAsync: Movie with ID {MovieId} not found.", movieId);
                return false;
            }

            // Kiểm tra VipType của Movie Entity
            if (movie.Viptype == 0 || !movie.Viptype.HasValue) return true; // Phim free hoặc không yêu cầu VIP

            if (!userId.HasValue) return false; // Chưa đăng nhập

            var user = await _unitOfWork.Users.GetByIdAsync(userId.Value); // Giả sử IUnitOfWork có Users Repository
            if (user == null)
            {
                _logger.LogWarning("CanUserViewMovieAsync: User with ID {UserId} not found.", userId.Value);
                return false;
            }

            if (user.LevelVip.HasValue && movie.Viptype.HasValue && user.LevelVip.Value >= movie.Viptype.Value)
            {
                if (user.TimeVip.HasValue && user.TimeVip.Value >= DateTime.UtcNow)
                {
                    return true;
                }
            }
            _logger.LogInformation("User {UserId} does not have permission to view movie {MovieId}. User VIP Level: {UserVipLevel}, Movie VIP Type: {MovieVipType}, VIP Expiry: {UserVipExpiry}",
                userId, movieId, user.LevelVip, movie.Viptype, user.TimeVip);
            return false;
        }
    }
}