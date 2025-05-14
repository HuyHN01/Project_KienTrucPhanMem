using LikeMovie.DataAccess.Interfaces;
using LikeMovie.Model.Entities; // Giả sử Entity là Menu
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LikeMovie.DataAccess.Repositories
{
    public class MenuRepository : IMenuRepository
    {
        private readonly LikeMovieDbContext _context;

        public MenuRepository(LikeMovieDbContext context)
        {
            _context = context;
        }

        public async Task<Menu?> GetByIdAsync(int id)
        {
            return await _context.Menus.FindAsync(id);
        }

        public async Task<IEnumerable<Menu>> GetRootMenusAsync()
        {
            return await _context.Menus
                                 .Where(m => m.ParentId == null)
                                 .OrderBy(m => m.OrderNumber)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Menu>> GetChildMenusAsync(int parentId)
        {
            return await _context.Menus
                                 .Where(m => m.ParentId == parentId)
                                 .OrderBy(m => m.OrderNumber)
                                 .ToListAsync();
        }

        public async Task<int> CountChildrenAsync(int parentId)
        {
            return await _context.Menus.CountAsync(m => m.ParentId == parentId);
        }

        public async Task AddAsync(Menu menu)
        {
            await _context.Menus.AddAsync(menu);
            // Việc cập nhật OrderNumber của các menu khác và SaveChangesAsync sẽ ở BLL/UnitOfWork
        }

        public void Update(Menu menu)
        {
            _context.Menus.Update(menu);
            // Việc cập nhật OrderNumber của các menu khác (nếu STT thay đổi) và SaveChangesAsync sẽ ở BLL/UnitOfWork
        }

        public void Delete(Menu menu)
        {
            _context.Menus.Remove(menu);
            // Việc cập nhật OrderNumber của các menu khác và SaveChangesAsync sẽ ở BLL/UnitOfWork
        }

        // Implement các phương thức phức tạp để lấy danh sách menu cần cập nhật OrderNumber
        public async Task<List<Menu>> GetMenusToIncrementOrderAsync(int? parentId, int orderNumber)
        {
            return await _context.Menus
                .Where(k => k.ParentId == parentId && k.OrderNumber >= orderNumber)
                .ToListAsync();
        }

        public async Task<List<Menu>> GetMenusToDecrementOrderAsync(int? parentId, int orderNumber)
        {
            return await _context.Menus
                .Where(k => k.ParentId == parentId && k.OrderNumber > orderNumber)
                .ToListAsync();
        }

        public async Task<List<Menu>> GetMenusToReorderOnUpdateAsync(int menuIdToUpdate, int? parentId, int oldOrderNumber, int newOrderNumber)
        {
            var query = _context.Menus.Where(m => m.Id != menuIdToUpdate && m.ParentId == parentId);

            if (newOrderNumber < oldOrderNumber) // Kéo lên (OrderNumber giảm)
            {
                query = query.Where(m => m.OrderNumber >= newOrderNumber && m.OrderNumber < oldOrderNumber);
            }
            else // Kéo xuống (OrderNumber tăng)
            {
                query = query.Where(m => m.OrderNumber > oldOrderNumber && m.OrderNumber <= newOrderNumber);
            }
            return await query.ToListAsync();
        }
    }
}