// LikeMovie.Business/Services/CommentService.cs
using AutoMapper;
using LikeMovie.Business.Dtos.CommentDtos;
using LikeMovie.Business.Interfaces;
using LikeMovie.DataAccess.Interfaces; // Namespace của Repository Interfaces
using LikeMovie.Model.Entities;       // Namespace của Entities
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LikeMovie.Business.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IMovieRepository _movieRepository; // Để kiểm tra phim tồn tại, lấy tên phim
        private readonly IUserRepository _userRepository;   // Để kiểm tra user tồn tại, lấy tên/avatar
        private readonly IUnitOfWork _unitOfWork;        // Hoặc LikeMovieDbContext
        private readonly IMapper _mapper;
        private readonly ILogger<CommentService> _logger;

        public CommentService(
            ICommentRepository commentRepository,
            IMovieRepository movieRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CommentService> logger)
        {
            _commentRepository = commentRepository;
            _movieRepository = movieRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<CommentDto>> GetCommentsByMovieAsync(int movieId, string? sortOrder = null /*, int pageNumber, int pageSize*/)
        {
            try
            {
                // Tùy chọn: Kiểm tra phim có tồn tại không
                var movieExists = await _movieRepository.GetByIdAsync(movieId) != null;
                if (!movieExists)
                {
                    _logger.LogWarning("Attempted to get comments for a non-existent movie with ID {MovieId}.", movieId);
                    return new List<CommentDto>(); // Hoặc ném lỗi tùy theo yêu cầu
                }

                // Giả định rằng ICommentRepository.GetByMovieIdAsync đã được thiết kế để
                // Include(c => c.User) và có thể Include(c => c.Movie) nếu cần tên phim từ đó.
                // Nếu không, AutoMapper sẽ không thể map UserName, UserAvatarUrl, MovieTitle.
                var commentEntities = await _commentRepository.GetByMovieIdAsync(movieId, sortOrder);

                // AutoMapper sẽ tự động map các trường có tên giống nhau.
                // Đối với UserName, UserAvatarUrl, MovieTitle, bạn cần cấu hình trong MappingProfile
                // để nó lấy từ Comment.User.UserName, Comment.User.AvatarUrl, Comment.Movie.Title
                return _mapper.Map<IEnumerable<CommentDto>>(commentEntities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting comments for MovieID {MovieId}.", movieId);
                return new List<CommentDto>(); // Trả về danh sách rỗng khi có lỗi
            }
        }

        public async Task<CommentDto?> GetCommentByIdAsync(int commentId)
        {
            try
            {
                // Giả định GetByIdAsync trong ICommentRepository cũng Include User và Movie nếu cần
                var commentEntity = await _commentRepository.GetByIdAsync(commentId);
                if (commentEntity == null)
                {
                    _logger.LogWarning("Comment with ID {CommentId} not found.", commentId);
                    return null;
                }
                return _mapper.Map<CommentDto>(commentEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting comment by ID {CommentId}.", commentId);
                return null;
            }
        }

        public async Task<(bool success, string? errorMessage, CommentDto? createdComment)> CreateCommentAsync(int userId, CreateCommentDto createDto)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("Attempted to create comment by non-existent UserID {UserId}.", userId);
                    return (false, "Người dùng không tồn tại.", null);
                }

                var movie = await _movieRepository.GetByIdAsync(createDto.MovieId);
                if (movie == null)
                {
                    _logger.LogWarning("Attempted to create comment for non-existent MovieID {MovieId}.", createDto.MovieId);
                    return (false, "Phim không tồn tại.", null);
                }

                var commentEntity = _mapper.Map<Comment>(createDto); // Map từ CreateCommentDto sang Comment Entity
                commentEntity.UserId = userId; // Gán UserId từ người dùng đang đăng nhập
                commentEntity.DateCreated = DateTime.UtcNow; // Hoặc DateTime.Now, nên dùng UtcNow cho CSDL
                commentEntity.Likes = 0; // Giá trị mặc định khi tạo mới

                await _commentRepository.AddAsync(commentEntity);
                await _unitOfWork.SaveChangesAsync(); // Lưu thay đổi vào CSDL

                // Map lại từ Entity vừa được tạo (đã có CommentId) sang CommentDto để trả về
                // Lúc này, commentEntity đã có ID từ CSDL.
                // Để CommentDto có đủ UserName, UserAvatarUrl, MovieTitle, chúng ta cần đảm bảo
                // commentEntity (sau khi Add và SaveChanges) có thể nạp các thông tin này.
                // Cách tốt nhất là sau khi SaveChanges, bạn có thể load lại commentEntity với Include nếu cần,
                // hoặc cấu hình AutoMapper để làm điều này.
                // Cách đơn giản: Gán thủ công các thông tin còn thiếu vào DTO sau khi map.

                var createdCommentDto = _mapper.Map<CommentDto>(commentEntity);
                // Gán thủ công nếu AutoMapper không tự làm được hoặc Repository không Include sẵn
                createdCommentDto.UserName = user.UserName;
                createdCommentDto.UserAvatarUrl = user.AvatarUrl;
                createdCommentDto.MovieTitle = movie.Title;


                _logger.LogInformation("Comment created successfully with ID {CommentId} by UserID {UserId} for MovieID {MovieId}.", createdCommentDto.CommentId, userId, createDto.MovieId);
                return (true, "Bình luận đã được thêm thành công.", createdCommentDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating comment by UserID {UserId} for MovieID {MovieId}.", userId, createDto.MovieId);
                return (false, "Đã có lỗi xảy ra khi tạo bình luận.", null);
            }
        }

        public async Task<(bool success, string? errorMessage)> DeleteCommentAsync(int commentId, int requestingUserId, string userRole)
        {
            try
            {
                var commentEntity = await _commentRepository.GetByIdAsync(commentId);
                if (commentEntity == null)
                {
                    _logger.LogWarning("Attempted to delete non-existent comment with ID {CommentId}.", commentId);
                    return (false, "Bình luận không tồn tại.");
                }

                // Logic kiểm tra quyền xóa (ví dụ: Admin hoặc chủ sở hữu bình luận)
                bool isAdmin = userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase); // Giả định role "Admin"
                if (!isAdmin && commentEntity.UserId != requestingUserId)
                {
                    _logger.LogWarning("User {RequestingUserId} (Role: {UserRole}) attempted to delete comment {CommentId} owned by UserID {OwnerUserId} without permission.",
                        requestingUserId, userRole, commentId, commentEntity.UserId);
                    return (false, "Bạn không có quyền xóa bình luận này.");
                }

                _commentRepository.Delete(commentEntity);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Comment {CommentId} deleted successfully by UserID {RequestingUserId} (Role: {UserRole}).", commentId, requestingUserId, userRole);
                return (true, "Xóa bình luận thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting comment {CommentId} by UserID {RequestingUserId}.", commentId, requestingUserId);
                return (false, "Đã có lỗi xảy ra khi xóa bình luận.");
            }
        }
    }
}