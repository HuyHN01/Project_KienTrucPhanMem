// LikeMovie.Business/Services/GenreService.cs
using AutoMapper;
using LikeMovie.Business.Dtos.GenreDtos;
using LikeMovie.Business.Interfaces;
using LikeMovie.DataAccess.Interfaces;
using LikeMovie.Model.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LikeMovie.Business.Services
{
    public class GenreService : IGenreService
    {
        private readonly IGenreRepository _genreRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GenreService> _logger;

        public GenreService(
            IGenreRepository genreRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<GenreService> logger)
        {
            _genreRepository = genreRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<GenreDto>> GetAllGenresAsync()
        {
            var genreEntities = await _genreRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<GenreDto>>(genreEntities);
        }

        public async Task<GenreDto?> GetGenreByIdAsync(int id)
        {
            var genreEntity = await _genreRepository.GetByIdAsync(id);
            return _mapper.Map<GenreDto>(genreEntity);
        }

        public async Task<(bool success, string? errorMessage, GenreDto? createdGenre)> CreateGenreAsync(CreateGenreDto createDto)
        {
            try
            {
                // Kiểm tra tên thể loại đã tồn tại chưa (không phân biệt hoa thường)
                var existingGenre = await _genreRepository.GetByNameAsync(createDto.Name);
                if (existingGenre != null)
                {
                    return (false, "Tên thể loại đã tồn tại.", null);
                }

                var genreEntity = _mapper.Map<Genre>(createDto);
                await _genreRepository.AddAsync(genreEntity);
                await _unitOfWork.SaveChangesAsync();

                var createdGenreDto = _mapper.Map<GenreDto>(genreEntity);
                _logger.LogInformation("Genre '{GenreName}' created successfully with ID {GenreId}.", createdGenreDto.Name, createdGenreDto.GenreId);
                return (true, null, createdGenreDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating genre with name {GenreName}.", createDto.Name);
                return (false, "Đã có lỗi xảy ra khi tạo thể loại.", null);
            }
        }

        public async Task<(bool success, string? errorMessage, GenreDto? updatedGenre)> UpdateGenreAsync(UpdateGenreDto updateDto)
        {
            try
            {
                var genreEntity = await _genreRepository.GetByIdAsync(updateDto.GenreId);
                if (genreEntity == null)
                {
                    return (false, "Không tìm thấy thể loại để cập nhật.", null);
                }

                // Kiểm tra xem tên mới có trùng với tên của một thể loại khác không (ngoại trừ chính nó)
                var existingGenreWithNewName = await _genreRepository.GetByNameAsync(updateDto.Name);
                if (existingGenreWithNewName != null && existingGenreWithNewName.GenreId != updateDto.GenreId)
                {
                    return (false, "Tên thể loại mới đã được sử dụng bởi một thể loại khác.", null);
                }

                _mapper.Map(updateDto, genreEntity); // Cập nhật các trường từ DTO sang Entity
                _genreRepository.Update(genreEntity); // EF Core sẽ theo dõi thay đổi
                await _unitOfWork.SaveChangesAsync();

                var updatedGenreDto = _mapper.Map<GenreDto>(genreEntity);
                _logger.LogInformation("Genre ID {GenreId} updated successfully to '{GenreName}'.", updatedGenreDto.GenreId, updatedGenreDto.Name);
                return (true, null, updatedGenreDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating genre with ID {GenreId}.", updateDto.GenreId);
                return (false, "Đã có lỗi xảy ra khi cập nhật thể loại.", null);
            }
        }

        public async Task<(bool success, string? errorMessage)> DeleteGenreAsync(int id)
        {
            try
            {
                var genreEntity = await _genreRepository.GetByIdAsync(id);
                if (genreEntity == null)
                {
                    return (false, "Không tìm thấy thể loại để xóa.");
                }

                // TODO: Kiểm tra xem thể loại này có đang được sử dụng bởi bất kỳ phim nào không.
                // Nếu có, không cho xóa hoặc có cảnh báo.
                // Ví dụ: var moviesWithThisGenre = await _movieRepository.GetMoviesByGenreAsync(id, 1);
                // if (moviesWithThisGenre.Any())
                // {
                //    return (false, "Thể loại này đang được sử dụng bởi một hoặc nhiều phim và không thể xóa.");
                // }

                _genreRepository.Delete(genreEntity);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Genre ID {GenreId} deleted successfully.", id);
                return (true, null);
            }
            catch (Exception ex)
            {
                // Xử lý trường hợp lỗi khóa ngoại nếu CSDL có ràng buộc
                _logger.LogError(ex, "Error deleting genre with ID {GenreId}.", id);
                if (ex.InnerException != null && ex.InnerException.Message.Contains("conflicted with the REFERENCE constraint"))
                {
                    return (false, "Không thể xóa thể loại này vì nó đang được sử dụng bởi các phim. Hãy xóa hoặc thay đổi thể loại của các phim đó trước.");
                }
                return (false, "Đã có lỗi xảy ra khi xóa thể loại.");
            }
        }
    }
}