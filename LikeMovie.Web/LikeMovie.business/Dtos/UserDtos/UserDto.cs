// Giả sử file này nằm trong LikeMovie.Business/Dtos/UserDtos/UserDto.cs
using System;

namespace LikeMovie.Business.Dtos.UserDtos
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; } // Nếu Entity User có trường này
        public string? AvatarUrl { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? LevelVip { get; set; } // Hoặc tên bạn đặt, ví dụ: levelVIP
        public DateTime? TimeVip { get; set; } // Thời hạn VIP
        public bool IsActive { get; set; }
        // Thêm các trường khác nếu cần hiển thị, ví dụ: Role
        // public string RoleName { get; set; }
    }
}