using LikeMovie.DataAccess.Interfaces;
using LikeMovie.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LikeMovie.DataAccess.Repositories
{
    public class GenreRepository : IGenreRepository
    {
        private readonly LikeMovieDbContext _context;

        public GenreRepository(LikeMovieDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Genre>> GetAllAsync()
        {
            return await _context.Genres.OrderBy(g => g.Name).ToListAsync();
        }

        public async Task<Genre?> GetByIdAsync(int genreId)
        {
            return await _context.Genres.FindAsync(genreId);
        }

        public async Task AddAsync(Genre genre)
        {
            await _context.Genres.AddAsync(genre);
            // SaveChangesAsync() sẽ ở BLL Service hoặc UnitOfWork
        }

        public void Update(Genre genre)
        {
            _context.Genres.Update(genre);
            // SaveChangesAsync() sẽ ở BLL Service hoặc UnitOfWork
        }

        public void Delete(Genre genre)
        {
            _context.Genres.Remove(genre);
            // SaveChangesAsync() sẽ ở BLL Service hoặc UnitOfWork
        }

        public async Task<Genre?> GetByNameAsync(string name)
        {
            return await _context.Genres.FirstOrDefaultAsync(g => g.Name.ToLower() == name.ToLower());
        }

        public async Task<bool> ExistsAsync(int genreId)
        {
            return await _context.Genres.AnyAsync(g => g.GenreId == genreId);
        }
    }
}