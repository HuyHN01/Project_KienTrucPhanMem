// Giả sử file này nằm trong LikeMovie.Business/Dtos/MenuDtos/CreateMenuDto.cs
using System.ComponentModel.DataAnnotations;

namespace LikeMovie.Business.Dtos.MenuDtos
{
    public class CreateMenuDto
    {
        [Required(ErrorMessage = "Tên menu không được để trống.")]
        [StringLength(100, ErrorMessage = "Tên menu không được vượt quá 100 ký tự.")]
        public string MenuName { get; set; } = string.Empty;

        // MenuLink có thể được tạo tự động nếu menu được tạo từ Genre,
        // hoặc người dùng nhập nếu là link tùy chỉnh.
        [StringLength(255, ErrorMessage = "Đường dẫn menu không được vượt quá 255 ký tự.")]
        public string? MenuLink { get; set; }

        public int? ParentId { get; set; } // ID của menu cha (nếu đây là menu con)

        [Required(ErrorMessage = "Thứ tự hiển thị không được để trống.")]
        [Range(1, int.MaxValue, ErrorMessage = "Thứ tự hiển thị phải là số dương.")]
        public int OrderNumber { get; set; }

        // Tùy chọn: Nếu menu được tạo từ một Genre (Thể loại)
        public int? GenreId { get; set; }
    }
}