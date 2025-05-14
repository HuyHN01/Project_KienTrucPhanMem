// LikeMovie.Business/Interfaces/IGenreService.cs
using LikeMovie.Business.Dtos.GenreDtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LikeMovie.Business.Interfaces
{
    public interface IGenreService
    {
        Task<IEnumerable<GenreDto>> GetAllGenresAsync();
        Task<GenreDto?> GetGenreByIdAsync(int id);

        /// <summary>
        /// Tạo một thể loại mới.
        /// </summary>
        /// <returns>Thông tin thể loại đã tạo và thông báo lỗi (nếu có).</returns>
        Task<(bool success, string? errorMessage, GenreDto? createdGenre)> CreateGenreAsync(CreateGenreDto createDto);

        /// <summary>
        /// Cập nhật một thể loại.
        /// </summary>
        Task<(bool success, string? errorMessage, GenreDto? updatedGenre)> UpdateGenreAsync(UpdateGenreDto updateDto);

        /// <summary>
        /// Xóa một thể loại.
        /// </summary>
        /// <returns>True nếu xóa thành công, False nếu thất bại (ví dụ: thể loại đang được sử dụng).</returns>
        Task<(bool success, string? errorMessage)> DeleteGenreAsync(int id);
    }
}