// Giả sử file này nằm trong LikeMovie.Business/Dtos/AdminMovieDtos/AdminUserDto.cs
namespace LikeMovie.Business.Dtos.AdminMovieDtos // Hoặc AdminUserDtos
{
    public class AdminUserDto
    {
        public int AdminId { get; set; }
        public string? UsernameAd { get; set; }
        public string? NameAd { get; set; } // Tên của Admin
        public string? Email { get; set; }
        // Bạn có thể thêm các thuộc tính khác nếu Entity AdminMovie có, ví dụ: Role, LastLoginDate,...
        // public string? Role { get; set; }
    }
}