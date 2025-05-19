using LikeMovies.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Linq.Dynamic;
using System.Linq.Expressions;
namespace LikeMovies.Controllers
{
    public class SearchMovieController : Controller
    {
        private MovieEntities db = new MovieEntities();
        // GET: SearchMovie
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Search(string strSearch = "", int? page = 1)
        {
            ViewBag.Search = strSearch;
            int pageSize = 10;  // Kích thước mỗi trang
            int pageNumber = (page ?? 1);

            ViewBag.TrendingMovies = db.Movies .OrderByDescending(m => m.ViewCount).Take(6).ToList();
            ViewBag.TopRatedMovies = db.Movies.OrderByDescending(m => m.Rating).Take(5).ToList();

            if (!string.IsNullOrEmpty(strSearch))
            {
                // Tìm kiếm phim theo tên, thể loại, hoặc đạo diễn
                var kq = from p in db.Movies
                         where p.Title.Contains(strSearch) || p.Genres.Any(g => g.Name.Contains(strSearch)) || p.Director.Contains(strSearch)
                         select p;

                // Đảm bảo bạn chuyển đổi kq thành List trước khi gọi .Any()
                var results = kq.ToList();
                if (results.Any())
                {
                    return View(results.ToPagedList(pageNumber, pageSize));
                }
                else
                {
                    ViewBag.Message = "Không có kết quả tìm kiếm phù hợp.";
                    return View();
                }
            }
            else
            {
                return View();
            }
        }



    }
}