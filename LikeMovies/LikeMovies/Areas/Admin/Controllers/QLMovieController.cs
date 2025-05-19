using LikeMovies.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using PagedList;
using System.IO;
using System.Data.Entity;
using System.Net;

namespace LikeMovies.Areas.Admin.Controllers
{
    public class QLMovieController : BaseAdminController
    {
        MovieEntities db = new MovieEntities();

        // GET: Admin/QLMovie
        public ActionResult Index(int? page)
        {
            int iPageNum = (page ?? 1);
            int iPageSize = 7;

            return View(db.Movies.ToList().OrderBy(n => n.MovieID).ToPagedList(iPageNum, iPageSize));
        }
        public ActionResult Details(int id)
        {
            var movie = db.Movies.SingleOrDefault(n => n.MovieID == id);

            if (movie == null)
            {
                Response.StatusCode = 404;
                return HttpNotFound();
            }
            // Lấy danh sách comment liên quan đến phim
            var comments = db.Comments.Where(c => c.MovieID == id).ToList();
            ViewBag.Comments = comments;
            return View(movie);
        }

        public ActionResult GetThumbnail(int id)
        {
            var movie = db.Movies.SingleOrDefault(n => n.MovieID == id);
            if (movie == null || string.IsNullOrEmpty(movie.Thumbnail))
            {
                return HttpNotFound();
            }

            var filePath = Server.MapPath(movie.Thumbnail);
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            var fileMimeType = MimeMapping.GetMimeMapping(filePath);

            return File(fileBytes, fileMimeType);
        }
        [HttpGet]
        public ActionResult Create()
        {
            var genres = new List<SelectListItem>
            {
                new SelectListItem { Text = "Phim chiếu rạp", Value = "0" },
                new SelectListItem { Text = "Phim bộ", Value = "1" }
            };

            ViewBag.Genres = genres;

            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Movies movies, FormCollection f, HttpPostedFileBase fFileUpload)
        {
            if (fFileUpload == null)
            {
                ViewBag.ThongBao = "Hãy chọn ảnh bìa.";

                ViewBag.TenPhim = f["sTenPhim"];
                ViewBag.MoTa = f["sMoTa"];
                ViewBag.ThoiLuong = int.Parse(f["iThoiLuong"]);
                ViewBag.DaoDien = f["sDaoDien"];
                ViewBag.TrailerUrl = f["sTrailerUrl"];
                ViewBag.MovieUrl = f["MovieUrl"];
                return View();
            }
            else if (ModelState.IsValid)
            {
                var fileName = Path.GetFileName(fFileUpload.FileName);
                int type = int.Parse(f["iLoaiPhim"]);
                string directoryPath = type == 0 ? Server.MapPath("~/Images/PhimChieuRap") : Server.MapPath("~/Images/Phimbo");

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                var path = Path.Combine(directoryPath, fileName);
                fFileUpload.SaveAs(path);

                movies.Title = f["sTenPhim"];
                movies.Description = f["sMoTa"];
                movies.Thumbnail = $"~/Images/{(type == 0 ? "PhimChieuRap" : "Phimbo")}/{fileName}";
                movies.ReleaseDate = Convert.ToDateTime(f["dNgayCongChieu"]);
                movies.Duration = int.Parse(f["iThoiLuong"]);
                movies.Director = f["sDaoDien"];
                movies.TrailerURL = f["TrailerUrl"];
                movies.MovieURL = f["MovieUrl"];
                movies.VIPType = int.Parse(f["TypeVIP"]);
                db.Movies.Add(movies);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
     
            return View();
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var genres = new List<SelectListItem>
    {
        new SelectListItem { Text = "Phim chiếu rạp", Value = "0" },
        new SelectListItem { Text = "Phim bộ", Value = "1" }
    };

            ViewBag.Genres = genres;
            var movie = db.Movies.SingleOrDefault(n => n.MovieID == id);
            if (movie == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            // Truyền thêm type vào ViewBag
            ViewBag.Type = movie.Type; // Giả sử cột "Type" lưu trong bảng Movies
            return View(movie);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Movies movies, HttpPostedFileBase FileUpload)
        {
            // Lấy phim hiện tại từ cơ sở dữ liệu
            var movie = db.Movies.SingleOrDefault(n => n.MovieID == movies.MovieID);

            // Dữ liệu cho ViewBag.Genres để render lại form
            ViewBag.Genres = new List<SelectListItem>
    {
        new SelectListItem { Text = "Phim chiếu rạp", Value = "0" },
        new SelectListItem { Text = "Phim bộ", Value = "1" }
    };

            // Kiểm tra nếu không tìm thấy phim
            if (movie == null)
            {
                ModelState.AddModelError("", "Không tìm thấy phim.");
                return View(); // Render lại form nếu không tìm thấy phim
            }

            if (ModelState.IsValid)
            {
                // Xử lý file upload nếu có
                if (FileUpload != null && FileUpload.ContentLength > 0) // Kiểm tra nếu có file upload
                {
                    // Lấy giá trị 'Type' để xác định thư mục
                    string type = movies.Type.ToString(); // Đảm bảo đã có Type trong movies
                    string folder = type == "0" ? "PhimChieuRap" : "PhimBo";

                    // Lấy tên file và đường dẫn đầy đủ
                    string fileName = Path.GetFileName(FileUpload.FileName);
                    string path = Path.Combine(Server.MapPath($"~/Images/{folder}/"), fileName);

                    // Kiểm tra nếu file không tồn tại thì lưu lại
                    if (!System.IO.File.Exists(path))
                    {
                        // Tạo thư mục nếu chưa tồn tại
                        Directory.CreateDirectory(Path.GetDirectoryName(path));
                        // Lưu file
                        FileUpload.SaveAs(path);
                    }

                    // Cập nhật thumbnail của phim
                    movie.Thumbnail = $"~/Images/{folder}/{fileName}";
                }

                // Cập nhật các trường khác của phim từ đối tượng movies
                movie.Title = movies.Title;
                movie.Description = movies.Description;
                movie.ReleaseDate = movies.ReleaseDate;
                movie.Director = movies.Director;
                movie.MovieURL = movies.MovieURL;
                movie.Duration = movies.Duration;
                movie.TrailerURL = movies.TrailerURL;
                movie.VIPType = movies.VIPType;
                // Lưu thay đổi vào cơ sở dữ liệu
                db.SaveChanges();
                return RedirectToAction("Index"); // Chuyển hướng về trang danh sách
            }

            return View(movie); // Nếu có lỗi, render lại form
        }
        // GET: Movies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movies movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Movies movie = db.Movies.Find(id);
            db.Movies.Remove(movie);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}