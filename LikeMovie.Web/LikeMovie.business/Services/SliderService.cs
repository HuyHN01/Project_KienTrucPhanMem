// LikeMovie.Business/Services/SliderService.cs
using AutoMapper;
using LikeMovie.Business.Dtos.PosterMovieDtos; // Hoặc SliderDtos
using LikeMovie.Business.Interfaces;
using LikeMovie.DataAccess.Interfaces; // Cho IPosterMovieRepository, IMovieRepository
using LikeMovie.Model.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LikeMovie.Business.Services
{
    public class SliderService : ISliderService
    {
        private readonly IPosterMovieRepository _posterMovieRepository; // Hoặc ISliderRepository
        private readonly IMovieRepository _movieRepository; // Để kiểm tra MovieId tồn tại
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<SliderService> _logger;
        // private readonly IFileStorageService _fileStorageService;

        public SliderService(
            IPosterMovieRepository posterMovieRepository,
            IMovieRepository movieRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<SliderService> logger
            /*, IFileStorageService fileStorageService */)
        {
            _posterMovieRepository = posterMovieRepository;
            _movieRepository = movieRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            // _fileStorageService = fileStorageService;
        }

        public async Task<IEnumerable<PosterMovieDto>> GetActiveSlidersAsync()
        {
            // Trong Repository, GetSlidersAsync() hoặc GetActiveSlidersWithMoviesAsync() đã lấy các slider hợp lệ
            var sliderEntities = await _posterMovieRepository.GetActiveSlidersWithMoviesAsync();
            return _mapper.Map<IEnumerable<PosterMovieDto>>(sliderEntities);
        }

        public async Task<IEnumerable<PosterMovieDto>> GetAllSlidersForAdminAsync()
        {
            // Giả sử repository có phương thức lấy tất cả (bao gồm cả không phải slider để admin quản lý)
            // hoặc bạn lọc ở đây. Để đơn giản, lấy tất cả poster và map.
            var allPosterMovies = await _posterMovieRepository.GetSlidersAsync(); // Hoặc một phương thức GetAll chung hơn
            return _mapper.Map<IEnumerable<PosterMovieDto>>(allPosterMovies.Where(p => p.IsSlider == true)); // Lọc lại nếu GetSlidersAsync không lọc
        }


        public async Task<PosterMovieDto?> GetSliderByIdAsync(int id)
        {
            var sliderEntity = await _posterMovieRepository.GetByIdWithMovieAsync(id); // Lấy cả movie nếu cần
            if (sliderEntity == null || sliderEntity.IsSlider != true) return null;
            return _mapper.Map<PosterMovieDto>(sliderEntity);
        }

        public async Task<(bool success, string? errorMessage, PosterMovieDto? createdSlider)> CreateSliderAsync(CreatePosterMovieDto createDto, IFormFile? imageFile)
        {
            try
            {
                if (createDto.MovieId.HasValue)
                {
                    var movieExists = await _movieRepository.GetByIdAsync(createDto.MovieId.Value) != null;
                    if (!movieExists)
                    {
                        return (false, "Phim được chọn không tồn tại.", null);
                    }
                }

                var sliderEntity = _mapper.Map<PosterMovie>(createDto);
                sliderEntity.IsSlider = true; // Luôn là slider khi tạo qua service này

                if (imageFile != null)
                {
                    // TODO: Gọi _fileStorageService.SaveFileAsync(...)
                    // sliderEntity.PosterUrl = await _fileStorageService.SaveSliderImageAsync(imageFile);
                    sliderEntity.PosterUrl = $"/uploads/sliders/{Guid.NewGuid()}_{imageFile.FileName}"; // Placeholder
                }
                else
                {
                    return (false, "Vui lòng chọn ảnh cho slider.", null); // Ảnh là bắt buộc cho slider
                }

                await _posterMovieRepository.AddAsync(sliderEntity);
                await _unitOfWork.SaveChangesAsync();

                var createdSliderDto = _mapper.Map<PosterMovieDto>(sliderEntity);
                _logger.LogInformation("Slider created successfully for MovieID {MovieId} with PosterID {PosterId}.", createDto.MovieId, createdSliderDto.PosterId);
                return (true, null, createdSliderDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating slider for MovieID {MovieId}.", createDto.MovieId);
                return (false, "Đã có lỗi xảy ra khi tạo slider.", null);
            }
        }

        public async Task<(bool success, string? errorMessage, PosterMovieDto? updatedSlider)> UpdateSliderAsync(UpdatePosterMovieDto updateDto, IFormFile? imageFile)
        {
            try
            {
                var sliderEntity = await _posterMovieRepository.GetByIdAsync(updateDto.PosterId);
                if (sliderEntity == null || sliderEntity.IsSlider != true)
                {
                    return (false, "Không tìm thấy slider để cập nhật.", null);
                }

                if (updateDto.MovieId.HasValue)
                {
                    var movieExists = await _movieRepository.GetByIdAsync(updateDto.MovieId.Value) != null;
                    if (!movieExists)
                    {
                        return (false, "Phim được chọn không tồn tại.", null);
                    }
                }


                _mapper.Map(updateDto, sliderEntity); // AutoMapper sẽ cập nhật các trường từ DTO
                sliderEntity.IsSlider = true; // Đảm bảo vẫn là slider

                if (imageFile != null)
                {
                    // TODO: Xử lý lưu file mới, có thể xóa file cũ
                    // sliderEntity.PosterUrl = await _fileStorageService.SaveSliderImageAsync(imageFile);
                    sliderEntity.PosterUrl = $"/uploads/sliders/{Guid.NewGuid()}_{imageFile.FileName}"; // Placeholder
                }

                _posterMovieRepository.Update(sliderEntity);
                await _unitOfWork.SaveChangesAsync();

                var updatedSliderDto = _mapper.Map<PosterMovieDto>(sliderEntity);
                _logger.LogInformation("Slider PosterID {PosterId} updated successfully.", updatedSliderDto.PosterId);
                return (true, null, updatedSliderDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating slider with PosterID {PosterId}.", updateDto.PosterId);
                return (false, "Đã có lỗi xảy ra khi cập nhật slider.", null);
            }
        }

        public async Task<(bool success, string? errorMessage)> DeleteSliderAsync(int id)
        {
            try
            {
                var sliderEntity = await _posterMovieRepository.GetByIdAsync(id);
                if (sliderEntity == null || sliderEntity.IsSlider != true)
                {
                    return (false, "Không tìm thấy slider để xóa.");
                }

                // TODO: Xóa file ảnh liên quan nếu cần
                // if (!string.IsNullOrEmpty(sliderEntity.PosterUrl))
                // {
                //     await _fileStorageService.DeleteFileAsync(sliderEntity.PosterUrl);
                // }

                _posterMovieRepository.Delete(sliderEntity);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Slider PosterID {PosterId} deleted successfully.", id);
                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting slider with PosterID {PosterId}.", id);
                return (false, "Đã có lỗi xảy ra khi xóa slider.");
            }
        }
    }
}