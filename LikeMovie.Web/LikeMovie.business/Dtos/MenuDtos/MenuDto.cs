// Giả sử file này nằm trong LikeMovie.Business/Dtos/MenuDtos/MenuDto.cs
using System.Collections.Generic;

namespace LikeMovie.Business.Dtos.MenuDtos
{
    public class MenuDto
    {
        public int Id { get; set; }
        public string? MenuName { get; set; }
        public string? MenuLink { get; set; }
        public int? ParentId { get; set; } // ID của menu cha (nếu có)
        public int OrderNumber { get; set; }
        public int ChildrenCount { get; set; } // Số lượng menu con trực tiếp
        public List<MenuDto> ChildMenus { get; set; } = new List<MenuDto>(); // Danh sách các menu con
    }
}