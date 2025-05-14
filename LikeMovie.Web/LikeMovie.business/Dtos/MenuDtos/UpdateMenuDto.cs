// Giả sử file này nằm trong LikeMovie.Business/Dtos/MenuDtos/UpdateMenuDto.cs
using System.ComponentModel.DataAnnotations;

namespace LikeMovie.Business.Dtos.MenuDtos
{
    public class UpdateMenuDto
    {
        [Required(ErrorMessage = "ID menu là bắt buộc để cập nhật.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên menu không được để trống.")]
        [StringLength(100, ErrorMessage = "Tên menu không được vượt quá 100 ký tự.")]
        public string MenuName { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "Đường dẫn menu không được vượt quá 255 ký tự.")]
        public string? MenuLink { get; set; }

        // ParentId thường không cho phép thay đổi trực tiếp khi update để tránh làm hỏng cấu trúc cây.
        // Nếu muốn thay đổi cha, có thể cần một quy trình phức tạp hơn.
        // public int? ParentId { get; set; }

        [Required(ErrorMessage = "Thứ tự hiển thị không được để trống.")]
        [Range(1, int.MaxValue, ErrorMessage = "Thứ tự hiển thị phải là số dương.")]
        public int OrderNumber { get; set; }
    }
}