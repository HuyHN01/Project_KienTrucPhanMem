// LikeMovie.Business/Interfaces/ISliderService.cs
using LikeMovie.Business.Dtos.PosterMovieDtos; // Hoặc SliderDtos
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
// using X.PagedList;

namespace LikeMovie.Business.Interfaces
{
    public interface ISliderService
    {
        /// <summary>
        /// Lấy tất cả các slider đang hoạt động để hiển thị (ví dụ: trên trang chủ).
        /// </summary>
        Task<IEnumerable<PosterMovieDto>> GetActiveSlidersAsync();

        /// <summary>
        /// Lấy danh sách slider cho trang quản lý Admin (có thể có phân trang).
        /// </summary>
        Task<IEnumerable<PosterMovieDto>> GetAllSlidersForAdminAsync(/*int pageNumber, int pageSize*/);

        Task<PosterMovieDto?> GetSliderByIdAsync(int id);

        /// <summary>
        /// Tạo một slider mới.
        /// </summary>
        Task<(bool success, string? errorMessage, PosterMovieDto? createdSlider)> CreateSliderAsync(CreatePosterMovieDto createDto, IFormFile? imageFile);

        /// <summary>
        /// Cập nhật một slider.
        /// </summary>
        Task<(bool success, string? errorMessage, PosterMovieDto? updatedSlider)> UpdateSliderAsync(UpdatePosterMovieDto updateDto, IFormFile? imageFile);

        /// <summary>
        /// Xóa một slider.
        /// </summary>
        Task<(bool success, string? errorMessage)> DeleteSliderAsync(int id);
    }
}