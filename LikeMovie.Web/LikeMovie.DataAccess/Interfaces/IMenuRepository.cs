using LikeMovie.Model.Entities; // Giả sử Entity là Menu
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LikeMovie.DataAccess.Interfaces
{
    public interface IMenuRepository
    {
        Task<Menu?> GetByIdAsync(int id);
        Task<IEnumerable<Menu>> GetRootMenusAsync(); // Lấy menu cấp gốc, đã sắp xếp
        Task<IEnumerable<Menu>> GetChildMenusAsync(int parentId); // Lấy menu con, đã sắp xếp

        // Đếm số menu con trực tiếp (để tránh N+1 query từ Controller cũ)
        Task<int> CountChildrenAsync(int parentId);

        Task AddAsync(Menu menu);
        void Update(Menu menu); // EF Core theo dõi
        void Delete(Menu menu); // Chỉ đánh dấu xóa

        // Các phương thức chuyên biệt để xử lý OrderNumber khi Add, Update, Delete
        // Những phương thức này có thể sẽ load các menu liên quan, thay đổi OrderNumber của chúng
        // và sau đó SaveChanges sẽ được gọi ở tầng BLL Service.
        // Việc này hơi phức tạp và có thể một phần logic này nên ở BLL nếu nó liên quan đến nhiều bước.
        // Tuy nhiên, nếu Repository có thể xử lý các truy vấn để lấy các menu cần cập nhật OrderNumber,
        // thì BLL sẽ gọi các phương thức này và sau đó gọi SaveChanges.

        // Ví dụ, để lấy danh sách các menu cần tăng OrderNumber khi thêm mới
        Task<List<Menu>> GetMenusToIncrementOrderAsync(int? parentId, int orderNumber);
        // Ví dụ, để lấy danh sách các menu cần giảm OrderNumber khi xóa
        Task<List<Menu>> GetMenusToDecrementOrderAsync(int? parentId, int orderNumber);
        // Ví dụ, để lấy danh sách các menu cần điều chỉnh khi OrderNumber của một menu thay đổi
        Task<List<Menu>> GetMenusToReorderOnUpdateAsync(int menuIdToUpdate, int? parentId, int oldOrderNumber, int newOrderNumber);

        // Cần phương thức để lấy Genre để tạo menu (nếu logic này vẫn giữ)
        // => Điều này nên do IGenreRepository cung cấp, và BLL sẽ kết hợp.
    }
}