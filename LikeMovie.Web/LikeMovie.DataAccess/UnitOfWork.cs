using LikeMovie.Business.Interfaces; // Hoặc DataAccess.Interfaces tùy nơi bạn đặt IUnitOfWork
using LikeMovie.DataAccess.Interfaces; // Namespace của các Repository Interfaces
using LikeMovie.DataAccess.Repositories; // Namespace của các Repository Implementations
using LikeMovie.Model.Entities; // Cần cho LikeMovieDbContext nếu nó ở namespace khác
using System; // Cho IDisposable và GC
using System.Threading.Tasks;

namespace LikeMovie.DataAccess.Repositories // Hoặc Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly LikeMovieDbContext _context;
        private IMovieRepository? _movieRepository;
        private ICommentRepository? _commentRepository;
        private IUserRepository? _userRepository;
        private IPaymentRepository? _paymentRepository;
        private IPosterMovieRepository? _posterMovieRepository;
        private ISubscriptionPlanRepository? _subscriptionPlanRepository;
        private IGenreRepository? _genreRepository; // Ví dụ thêm
        private IAdminMovieRepository? _adminMovieRepository; // Ví dụ thêm
        private IMenuRepository? _menuRepository; // Ví dụ thêm
        // Thêm các private fields cho tất cả các repository interfaces khác trong IUnitOfWork

        public UnitOfWork(LikeMovieDbContext context)
        {
            _context = context;
        }

        public IMovieRepository Movies => _movieRepository ??= new MovieRepository(_context);
        public ICommentRepository Comments => _commentRepository ??= new CommentRepository(_context);
        public IUserRepository Users => _userRepository ??= new UserRepository(_context);
        public IPaymentRepository Payments => _paymentRepository ??= new PaymentRepository(_context);
        public IPosterMovieRepository PosterMovies => _posterMovieRepository ??= new PosterMovieRepository(_context);
        public ISubscriptionPlanRepository SubscriptionPlans => _subscriptionPlanRepository ??= new SubscriptionPlanRepository(_context);
        public IGenreRepository Genres => _genreRepository ??= new GenreRepository(_context); // Ví dụ thêm
        public IAdminMovieRepository AdminMovies => _adminMovieRepository ??= new AdminMovieRepository(_context); // Ví dụ thêm
        public IMenuRepository Menus => _menuRepository ??= new MenuRepository(_context); // Ví dụ thêm
        // Implement các public properties cho tất cả các repository interfaces khác trong IUnitOfWork

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}