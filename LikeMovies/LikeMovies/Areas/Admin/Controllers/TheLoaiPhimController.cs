using LikeMovies.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LikeMovies.Areas.Admin.Controllers
{
    public class TheLoaiPhimController : BaseAdminController
    {
        MovieEntities db= new MovieEntities();
        // GET: Admin/TheLoaiPhim
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public JsonResult DsTheLoai()
        {
            try
            {
                var dsTL = (from tl in db.Genres
                            select new
                            {
                                MaTL = tl.GenreID,
                                TenTL = tl.Name
                            }).ToList();

                return Json(new { code = 200, dsTL = dsTL, msg = "Lấy danh sách thể loại thành công" },
                            JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lấy danh sách thể loại thất bại" + ex.Message },
                            JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public ActionResult Detail(int genresID)
        {
            try
            {
                var tl = (from s in db.Genres
                          where (s.GenreID == genresID)
                          select new
                          {
                              genresID = s.GenreID,
                              TenCD = s.Name
                          }).SingleOrDefault();

                return Json(new { code = 200, tl = tl, msg = "Lấy thông tin thể loại thành công" },
                            JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lấy thông tin thể loại thất bại" + ex.Message },
                            JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult AddTheLoai(string strTenTL)
        {
            try
            {
                var tl = new Genres();
                tl.Name = strTenTL;
                db.Genres.Add(tl);
                db.SaveChanges();
                return Json(new { code = 200, msg = "Thêm thể loại thành công." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    code = 500,
                    msg = "Thêm thể loại thất bại. Lỗi " +
                    ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Update(int maTL, string strTenTL)
        {
            try
            {
                var cd = db.Genres.SingleOrDefault(c => c.GenreID == maTL);
                cd.Name = strTenTL;
                db.SaveChanges();
                return Json(new { code = 200, msg = "Sửa thể loại thành công." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    code = 500,
                    msg = "Sửa thể loại thất bại. Lỗi " + ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Delete(int maTL)
        {
            try
            {
                var cd = db.Genres.SingleOrDefault(c => c.GenreID == maTL);
                db.Genres.Remove(cd);
                db.SaveChanges();
                return Json(new { code = 200, msg = "Xóa thể loại thành công." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    code = 500,
                    msg = "Xóa thể loại thất bại. Lỗi " + ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}