using LikeMovies.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LikeMovies.Controllers
{
    public class DanhMucPhimController : Controller
    {
        MovieEntities db = new MovieEntities();
        // GET: DanhMucPhim
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PartialNoiBat()
        {
            var movies = db.Movies.OrderByDescending(m => m.ViewCount).Take(8).ToList();

            return PartialView("_PartialNoiBat", movies);
        }
        public ActionResult PartialBXH()
        {
            var movies = db.Movies.Where(m => m.ReleaseDate != null).OrderByDescending(m => m.Rating).Take(10).ToList();

            return PartialView("_PartialBXH", movies);
        }
        public ActionResult PartialSapRaMat()
        {
            var movies = db.Movies.Where(m => m.ReleaseDate > DateTime.Now).OrderByDescending(m => m.ReleaseDate).Take(10).ToList();
            var comingSoonMovies = db.Movies.Where(m => m.ReleaseDate > DateTime.Now).ToList();
            ViewBag.ComingSoonMovies = comingSoonMovies;
            return PartialView("_PartialSapRaMat", movies);
        }
        public ActionResult PhimBo(string sortBy)
        {
            var movies = db.Movies.Where(m => m.Type == 1).AsQueryable();
            movies = ApplySorting(movies, sortBy);
            return View(movies.ToList());
        }
        public ActionResult PhimChieuRap(string sortBy)
        {
            var movies = db.Movies.Where(m => m.Type == 0).AsQueryable();
            movies = ApplySorting(movies, sortBy);
            return View(movies.ToList());
        }
        public ActionResult PartialSortLink()
        {
            return PartialView("_PartialSortLink");
        }
        public ActionResult PhimTheoTheLoai(int? genreId, string sortBy)
        {
            var movies = db.Movies.Where(m => genreId == null || m.Genres.Any(g => g.GenreID == genreId)).AsQueryable();
            movies = ApplySorting(movies, sortBy);
            ViewBag.Genres = db.Genres.ToList();  // Lấy danh sách thể loại
            ViewBag.SelectedGenre = genreId;
            return View(movies.ToList());
        }

        public ActionResult PhimTheoNam(int? year, string sortBy)
        {
            var movies = db.Movies.Where(m => !year.HasValue || m.ReleaseDate.Value.Year == year).AsQueryable();
            movies = ApplySorting(movies, sortBy);
            ViewBag.Years = db.Movies.Select(m => m.ReleaseDate.Value.Year).Distinct().OrderByDescending(y => y).ToList();
            ViewBag.SelectedYear = year;
            return View(movies.ToList());
        }
        #region Helper Methods

        // Phương thức dùng để lấy danh sách phim đã sắp xếp theo một tiêu chí nào đó (giảm trùng lặp code)
        private IQueryable<Movies> ApplySorting(IQueryable<Movies> movies, string sortBy)
        {
            switch (sortBy)
            {
                case "rating":
                    return movies.OrderByDescending(m => m.Rating);
                case "view":
                    return movies.OrderByDescending(m => m.ViewCount);
                case "favorite":
                    return movies.OrderByDescending(m => m.Favorites.Count());
                default:
                    return movies.OrderBy(m => m.Title); // Sắp xếp theo tên mặc định
            }
        }

        #endregion
        
    }
}