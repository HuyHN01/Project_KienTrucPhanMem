using LikeMovies.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.IO;

namespace LikeMovies.Controllers
{
    public class PhimController : Controller
    {
        MovieEntities db = new MovieEntities();
        // GET: Phim
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PartialMoiPhatHanh()
        {
            var movies = db.Movies.Where(m => m.ReleaseDate != null).OrderByDescending(m => m.ReleaseDate).Take(10).ToList();

            return PartialView("_PartialMoiPhatHanh", movies);
        }
        public ActionResult PartialHanhDong()
        {
            var movies = db.Movies.Where(m => m.Genres.Any(g => g.GenreID == 1)).OrderByDescending(m => m.ReleaseDate).Take(8).ToList();
            return PartialView("_PartialHanhDong", movies);
        }
        public ActionResult PartialKhoaHocVienTuong()
        {
            var movies = db.Movies.Where(m => m.Genres.Any(g => g.GenreID == 14)).OrderByDescending(m => m.ReleaseDate).Take(8).ToList();
            return PartialView("_PartialKhoaHocVienTuong", movies);
        }
        public ActionResult PartialTopSeries()
        {
            var movies = db.Movies.Where(m => m.Type == 1).OrderByDescending(m => m.ReleaseDate).Take(8).ToList();

            return PartialView("_PartialTopSeries", movies);
        }
        public ActionResult PartialHoatHinh()
        {
            var movies = db.Movies.Where(m => m.Genres.Any(g => g.GenreID == 3)).OrderByDescending(m => m.ReleaseDate).Take(8).ToList();
            return PartialView("_PartialHoatHinh", movies);
        }
        public ActionResult PartialAnime()
        {
            var movies = db.Movies.Where(m => m.Genres.Any(g => g.GenreID == 21)).OrderByDescending(m => m.ReleaseDate).Take(8).ToList();
            return PartialView("_PartialAnime", movies);
        }


        public ActionResult ChiTietPhim(int id)
        {
            var movie = db.Movies.Include("PosterMovie").Include("Genres")
                        .FirstOrDefault(m => m.MovieID == id);

            if (movie == null)
            {
                return HttpNotFound();
            }

            var khachHang = (Users)Session["TaiKhoan"];

            // Kiểm tra quyền truy cập phim dựa trên LevelVip của người dùng và VIPtype của phim
            if (khachHang != null && khachHang.levelVIP < movie.VIPType)
            {
                ViewBag.ErrorMessage = "Bạn không có quyền xem phim này. Vui lòng nâng cấp tài khoản VIP để xem phim này.";
            }

            ViewBag.MovieID = movie.MovieID;
            return View(movie);
        }

        //Hiển thị những phim có cùng thể loại với phim hiện tại trong chi tiết phim.
        public PartialViewResult RelatedMoviesPartial(int movieId)
        {
            // Lấy thể loại của phim hiện tại
            var currentMovie = db.Movies.Include(m => m.Genres).FirstOrDefault(m => m.MovieID == movieId);

            if (currentMovie == null)
            {
                return PartialView("_RelatedMoviesPartial", new List<Movies>());
            }

            // Lấy danh sách GenreID của phim hiện tại (đưa vào bộ nhớ)
            var currentGenreIds = currentMovie.Genres.Select(cg => cg.GenreID).ToList();

            // Lấy danh sách phim có cùng thể loại, ngoại trừ phim hiện tại
            var relatedMovies = db.Movies
                .Include(m => m.Genres) // Bao gồm Genres để sử dụng khi cần hiển thị
                .Where(m => m.MovieID != movieId && m.Genres.Any(g => currentGenreIds.Contains(g.GenreID)))
                .Take(10) // Lấy tối đa 10 phim liên quan
                .ToList();

            return PartialView("_RelatedMoviesPartial", relatedMovies);
        }

        public ActionResult LoadCommentSection(int movieId)
        {
            // Lấy danh sách bình luận từ bảng Comments
            var comments = db.Comments
                .Where(c => c.MovieID == movieId)
                .OrderByDescending(c => c.DateCreated) // Sắp xếp mới nhất
                .ToList();

            // Trả về Partial View "_CommentSection" với model là danh sách bình luận
            return PartialView("_CommentSection", comments);
        }

        public ActionResult GetComments(int movieId, string sortOrder)
        {
            var comments = db.Comments
                .Where(c => c.MovieID == movieId)
                .AsQueryable();

            // Sắp xếp bình luận
            switch (sortOrder)
            {
                case "newest":
                    comments = comments.OrderByDescending(c => c.DateCreated);
                    break;
                case "oldest":
                    comments = comments.OrderBy(c => c.DateCreated);
                    break;
                case "mostLiked":
                    comments = comments.OrderByDescending(c => c.Likes);
                    break;
                default:
                    comments = comments.OrderByDescending(c => c.DateCreated); // Mặc định: mới nhất
                    break;
            }

            // Trả về Partial View "_CommentList" với model là danh sách bình luận
            return PartialView("_CommentList", comments.ToList());
        }


        [HttpPost]
        public JsonResult AddCommentAndRating(int MovieID, string CommentText, int Rating)
        {
            if (Session["TaiKhoan"] == null)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để bình luận và đánh giá." });
            }

            var khachHang = (Users)Session["TaiKhoan"];

            // Lưu bình luận và đánh giá sao vào bảng Comments
            var newComment = new Comments
            {
                MovieID = MovieID,
                UserID = khachHang.UserID,
                CommentText = CommentText,
                DateCreated = DateTime.Now,
                Likes = 0,
                Rating = Rating // Lưu giá trị đánh giá sao trực tiếp vào Comments
            };

            db.Comments.Add(newComment);
            db.SaveChanges();

            return Json(new { success = true, message = "Bình luận và đánh giá thành công." });
        }


        public ActionResult XemPhim(int movieId)
        {
            // Lấy thông tin phim từ cơ sở dữ liệu
            var movie = db.Movies.Include(m => m.PosterMovie).Include(m => m.Genres)
                                 .FirstOrDefault(m => m.MovieID == movieId);

            // Kiểm tra nếu không tìm thấy phim
            if (movie == null)
            {
                return HttpNotFound();
            }

            // Kiểm tra nếu người dùng không đăng nhập
            if (Session["TaiKhoan"] == null)
            {
                return RedirectToAction("DangNhap", "LikeMovie");
            }

            var khachHang = (Users)Session["TaiKhoan"];

            // Kiểm tra quyền truy cập phim dựa trên LevelVip của người dùng và VIPtype của phim
            if (khachHang.levelVIP < movie.VIPType)
            {
                // Chuyển hướng đến trang thông báo nếu LevelVip của người dùng nhỏ hơn VIPtype của phim
                ViewBag.ErrorMessage = "Bạn không có quyền xem phim này. Vui lòng nâng cấp tài khoản VIP để xem phim này.";
                return RedirectToAction("ChiTietPhim", new { id = movieId });
            }

            // Trả về View với dữ liệu phim, bao gồm MovieURL
            return View(movie);
        }

    }
}