using LikeMovie.DataAccess.Interfaces; // Hoặc namespace của repository interfaces
using System;
using System.Threading.Tasks;

namespace LikeMovie.Business.Interfaces // Hoặc DataAccess.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IMovieRepository Movies { get; }
        ICommentRepository Comments { get; }
        IUserRepository Users { get; }
        IGenreRepository Genres { get; }
        IMenuRepository Menus { get; }
        IPosterMovieRepository PosterMovies { get; } // Hoặc ISliderRepository
        IAdminMovieRepository AdminMovies { get; } // Hoặc IAdminUserRepository
        ISubscriptionPlanRepository SubscriptionPlans { get; }
        IPaymentRepository Payments { get; }
        // Thêm các interface repository khác ở đây

        Task<int> SaveChangesAsync();
    }
}