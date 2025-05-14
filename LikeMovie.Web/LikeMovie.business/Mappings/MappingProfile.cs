// LikeMovie.Business/Mappings/MappingProfile.cs
using AutoMapper;
using LikeMovie.Business.Dtos.AdminMovieDtos;
using LikeMovie.Business.Dtos.CommentDtos;
using LikeMovie.Business.Dtos.GenreDtos;
using LikeMovie.Business.Dtos.MenuDtos;
using LikeMovie.Business.Dtos.MovieDtos;
using LikeMovie.Business.Dtos.PaymentDtos;
using LikeMovie.Business.Dtos.PosterMovieDtos;
using LikeMovie.Business.Dtos.SubscriptionPlanDtos;
using LikeMovie.Business.Dtos.UserDtos;
using LikeMovie.Model.Entities; // Namespace Entities của bạn
using System.Linq;

namespace LikeMovie.Business.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // --- Movie Mappings ---
            CreateMap<Movie, MovieDto>()
                .ForMember(dest => dest.GenreNames, opt => opt.MapFrom(src =>
                    src.Genres != null ? src.Genres.Select(g => g.Name).ToList() : new List<string>()))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => // Giả sử MovieDto có TypeDescription
                    src.Type == 0 ? "Phim lẻ" : (src.Type == 1 ? "Phim bộ" : "Không xác định")));

            CreateMap<Movie, MovieSummaryDto>()
                .ForMember(dest => dest.TypeDescription, opt => opt.MapFrom(src => // Giả sử MovieSummaryDto có TypeDescription
                    src.Type == 0 ? "Phim lẻ" : (src.Type == 1 ? "Phim bộ" : "Không xác định")));

            CreateMap<CreateMovieDto, Movie>()
                .ForMember(dest => dest.MovieId, opt => opt.Ignore()) // ID tự tăng
                .ForMember(dest => dest.Genres, opt => opt.Ignore())   // Sẽ xử lý mapping Genres riêng trong Service
                .ForMember(dest => dest.Thumbnail, opt => opt.Ignore());// Thumbnail sẽ được xử lý riêng (upload file)
                                                                        // Bỏ qua ViewCount, Rating (sẽ được tính toán hoặc mặc định)

            CreateMap<UpdateMovieDto, Movie>()
                .ForMember(dest => dest.Genres, opt => opt.Ignore())   // Sẽ xử lý mapping Genres riêng trong Service
                .ForMember(dest => dest.Thumbnail, opt => opt.Ignore());// Thumbnail sẽ được xử lý riêng
                                                                        // Bỏ qua ViewCount, Rating nếu không cho phép cập nhật trực tiếp


            // --- Comment Mappings ---
            CreateMap<Comment, CommentDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.UserName : null))
                .ForMember(dest => dest.UserAvatarUrl, opt => opt.MapFrom(src => src.User != null ? src.User.AvatarUrl : null))
                .ForMember(dest => dest.MovieTitle, opt => opt.MapFrom(src => src.Movie != null ? src.Movie.Title : null));
            CreateMap<CreateCommentDto, Comment>()
                .ForMember(dest => dest.CommentId, opt => opt.Ignore()) // ID tự tăng
                .ForMember(dest => dest.UserId, opt => opt.Ignore())  // Sẽ được gán trong Service từ user đang đăng nhập
                .ForMember(dest => dest.DateCreated, opt => opt.Ignore()) // Sẽ được gán trong Service
                .ForMember(dest => dest.Likes, opt => opt.Ignore());     // Mặc định là 0 khi tạo


            // --- User Mappings ---
            CreateMap<User, UserDto>();
            CreateMap<UserRegisterDto, User>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore()) // ID tự tăng
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) // Sẽ được hash và gán trong Service
                .ForMember(dest => dest.AvatarUrl, opt => opt.Ignore())    // Sẽ được xử lý (upload/default) trong Service
                .ForMember(dest => dest.DateCreated, opt => opt.Ignore()) // Sẽ được gán trong Service
                .ForMember(dest => dest.LevelVip, opt => opt.Ignore())   // Mặc định khi tạo
                .ForMember(dest => dest.TimeVip, opt => opt.Ignore())    // Mặc định khi tạo
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true)); // Mặc định là active khi đăng ký

            CreateMap<UpdateUserDto, User>()
                .ForMember(dest => dest.Email, opt => opt.Ignore()) // Thường không cho đổi email dễ dàng
                .ForMember(dest => dest.UserName, opt => opt.Ignore()) // Không cho đổi username
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.DateCreated, opt => opt.Ignore())
                .ForMember(dest => dest.LevelVip, opt => opt.Ignore())
                .ForMember(dest => dest.TimeVip, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore());
            // ChangePasswordDto và ResetPasswordDto không map trực tiếp sang User Entity, logic xử lý ở Service


            // --- AdminMovie (AdminUser) Mappings ---
            // Giả sử Entity tên là AdminMovie, nếu bạn đổi thành AdminUser thì sửa lại
            CreateMap<AdminMovie, AdminUserDto>();
            // AdminUserLoginDto không map trực tiếp sang Entity


            // --- Genre Mappings ---
            CreateMap<Genre, GenreDto>();
            CreateMap<CreateGenreDto, Genre>()
                .ForMember(dest => dest.GenreId, opt => opt.Ignore()); // ID tự tăng
            CreateMap<UpdateGenreDto, Genre>();


            // --- Menu Mappings ---
            CreateMap<Menu, MenuDto>()
                .ForMember(dest => dest.ChildrenCount, opt => opt.Ignore()) // Sẽ được tính và gán trong Service
                .ForMember(dest => dest.ChildMenus, opt => opt.Ignore());  // Sẽ được load và gán trong Service (nếu cần)
            CreateMap<CreateMenuDto, Menu>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()); // ID tự tăng
            CreateMap<UpdateMenuDto, Menu>();


            // --- PosterMovie (Slider) Mappings ---
            CreateMap<PosterMovie, PosterMovieDto>()
                .ForMember(dest => dest.MovieTitle, opt => opt.MapFrom(src => src.Movie != null ? src.Movie.Title : null));
            CreateMap<CreatePosterMovieDto, PosterMovie>()
                .ForMember(dest => dest.PosterId, opt => opt.Ignore()) // ID tự tăng
                .ForMember(dest => dest.PosterUrl, opt => opt.Ignore()) // Sẽ được xử lý (upload) trong Service
                .ForMember(dest => dest.IsSlider, opt => opt.MapFrom(src => true)); // Mặc định là slider khi tạo
            CreateMap<UpdatePosterMovieDto, PosterMovie>()
                .ForMember(dest => dest.PosterUrl, opt => opt.Ignore()); // Sẽ được xử lý (upload) riêng nếu có thay đổi


            // --- SubscriptionPlan Mappings ---
            CreateMap<SubscriptionPlan, SubscriptionPlanDto>();
            CreateMap<CreateSubscriptionPlanDto, SubscriptionPlan>()
                .ForMember(dest => dest.PlanId, opt => opt.Ignore()); // ID tự tăng
            CreateMap<UpdateSubscriptionPlanDto, SubscriptionPlan>();


            // --- Payment Mappings ---
            CreateMap<Payment, PaymentDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.UserName : null))
                .ForMember(dest => dest.PaymentMethodDescription, opt => opt.MapFrom(src =>
                    src.PaymentMethod == 1 ? "VNPAY" : (src.PaymentMethod == 2 ? "MoMo" : "Không xác định")));
            CreateMap<CreatePaymentDto, Payment>()
                .ForMember(dest => dest.PaymentId, opt => opt.Ignore()) // ID tự tăng
                .ForMember(dest => dest.TransactionDate, opt => opt.Ignore()) // Sẽ gán trong Service
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());   // Sẽ gán trong Service

            // --- Payment Mappings ---
            CreateMap<Payment, PaymentDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.UserName : null)) // Giả sử Payment Entity có navigation User
                .ForMember(dest => dest.PlanName, opt => opt.MapFrom(src => src.SubscriptionPlan != null ? src.SubscriptionPlan.PlanName : null)) // Giả sử có navigation SubscriptionPlan
                .ForMember(dest => dest.PaymentMethodDescription, opt => opt.MapFrom(src =>
                    src.PaymentMethod == 1 ? "VNPAY" : (src.PaymentMethod == 2 ? "MoMo" : "Không xác định")));
            // AutoMapper sẽ tự động map các trường có tên giống nhau như PaymentId, UserId, PlanId, Amount, TransactionDate, TransactionStatus, CreatedAt, TransactionReference

            // Bạn cũng cần mapping từ CreatePaymentDto sang Payment Entity (đã có ở câu trả lời trước)
            CreateMap<CreatePaymentDto, Payment>()
                .ForMember(dest => dest.PaymentId, opt => opt.Ignore())
                .ForMember(dest => dest.TransactionDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        }
    }
}