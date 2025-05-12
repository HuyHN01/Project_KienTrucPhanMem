using LikeMovies.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LikeMovies.Areas.Admin.Controllers
{
    public class MenuController : BaseAdminController
    {
        MovieEntities db = new MovieEntities();
        // GET: Admin/Menu
        public ActionResult Index()
        {
            var listMenu = db.MENUs.Where(m => m.ParentId == null).OrderBy(m => m.OrderNumber).ToList();
            int[] a = new int[listMenu.Count()];
            for (int i = 0; i < listMenu.Count; i++)
            {
                int id = listMenu[i].Id;
                var l = db.MENUs.Where(m => m.ParentId == id);
                int k = l.Count();
                a[i] = k;
            }
            ViewBag.lst = a;
            List<Genres> cd = db.Genres.ToList();
            ViewBag.Genres = cd;
            return View(listMenu);
        }
        [ChildActionOnly]
        public ActionResult ChildMenu(int parentId)
        {
            List<MENUs> lst = new List<MENUs>();
            lst = db.MENUs.Where(m => m.ParentId == parentId).OrderBy(m => m.OrderNumber).ToList();
            ViewBag.Count = lst.Count();
            int[] a = new int[lst.Count()];
            for (int i = 0; i < lst.Count; i++)
            {
                int id = lst[i].Id;
                var l = db.MENUs.Where(m => m.ParentId == id);
                int k = l.Count();
                a[i] = k;
            }
            ViewBag.lst = a;
            return PartialView("ChildMenu", lst);
        }
        [ChildActionOnly]
        public ActionResult ChildMenu1(int parentId)
        {
            List<MENUs> lst = new List<MENUs>();
            lst = db.MENUs.Where(m => m.ParentId == parentId).OrderBy(m => m.OrderNumber).ToList();
            ViewBag.Count = lst.Count();
            int[] a = new int[lst.Count()];
            for (int i = 0; i < lst.Count; i++)
            {
                int id = lst[i].Id;
                var l = db.MENUs.Where(m => m.ParentId == id);
                int k = l.Count();
                a[i] = k;
            }
            ViewBag.lst = a;
            return PartialView("ChildMenu1", lst);
        }
        [HttpPost]
        public ActionResult AddMenu(FormCollection f)
        {
            if (!String.IsNullOrEmpty(f["ThemTheLoaiPhim"]))
            {
                MENUs m = new MENUs();
                int id = int.Parse(f["GenreID"]);
                var cd = db.Genres.Where(c => c.GenreID == id).SingleOrDefault();
                m.MenuName = cd.Name;
                m.MenuLink = "phim-theo-the-loai-" + cd.GenreID;
                if (!String.IsNullOrEmpty(f["ParentID"]))
                {
                    m.ParentId = int.Parse(f["ParentID"]);
                }
                else
                {
                    m.ParentId = null;
                }
                m.OrderNumber = int.Parse(f["Number"]);
                List<MENUs> l = null;
                if (m.ParentId == null)
                    l = db.MENUs.Where(k => k.ParentId == null && k.OrderNumber >= m.OrderNumber).ToList();
                else
                    l = db.MENUs.Where(k => k.ParentId == m.ParentId && k.OrderNumber >= m.OrderNumber).ToList();
                for (int i = 0; i < l.Count; i++)
                    l[i].OrderNumber++;
                db.MENUs.Add(m);
                db.SaveChanges();
            }
            
            else if (!String.IsNullOrEmpty(f["ThemLink"]))
            {
                MENUs m = new MENUs();
                m.MenuName = f["TenMenu"];
                m.MenuLink = f["Link"];
                if (!String.IsNullOrEmpty(f["ParentID"]))
                {
                    m.ParentId = int.Parse(f["ParentID"]);
                }
                else
                {
                    m.ParentId = null;
                }
                m.OrderNumber = int.Parse(f["Number2"]);
                List<MENUs> l = null;
                if (m.ParentId == null)
                    l = db.MENUs.Where(k => k.ParentId == null && k.OrderNumber >= m.OrderNumber).ToList();
                else
                    l = db.MENUs.Where(k => k.ParentId == m.ParentId && k.OrderNumber >= m.OrderNumber).ToList();
                for (int i = 0; i < l.Count; i++)
                    l[i].OrderNumber++;
                db.MENUs.Add(m);
                db.SaveChanges();
            }
            return Redirect("~/Admin/Menu/Index");
        }
        [HttpPost]
        public JsonResult Delete(int id)
        {
            List<MENUs> submn = db.MENUs.Where(m => m.ParentId == id).ToList();
            if (submn.Count() > 0)
            {
                return Json(new { code = 500, msg = "Còn menu con, không xóa được." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var mn = db.MENUs.SingleOrDefault(m => m.Id == id);
                List<MENUs> l = null;

                if (mn.ParentId == null)
                {
                    l = db.MENUs.Where(k => k.ParentId == null && k.OrderNumber > mn.OrderNumber).ToList();
                }
                else
                {
                    l = db.MENUs.Where(k => k.ParentId == mn.ParentId && k.OrderNumber > mn.OrderNumber).ToList();
                }

                for (int i = 0; i < l.Count; i++)
                {
                    l[i].OrderNumber--;
                }

                db.MENUs.Remove(mn);
                db.SaveChanges();

                return Json(new { code = 200, msg = "Xóa thành công." }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult Update(int id)
        {
            try
            {
                // Truy vấn lấy thông tin menu từ database
                var mn = (from m in db.MENUs
                          where m.Id == id
                          select new
                          {
                              Id = m.Id,
                              MenuName = m.MenuName,
                              MenuLink = m.MenuLink,
                              OrderNumber = m.OrderNumber
                          }).SingleOrDefault();

                // Trả về kết quả nếu tìm thấy
                return Json(new { code = 200, mn = mn, msg = "Lấy thông tin thành công." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về thông báo thất bại
                return Json(new { code = 500, msg = "Lấy thông tin thất bại: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Update(int id, string strTenMenu, string strLink, int STT)
        {
            try
            {
                // Lấy menu cần cập nhật từ database
                var mn = db.MENUs.SingleOrDefault(m => m.Id == id);
                List<MENUs> l = null;

                if (STT < mn.OrderNumber)
                {
                    // Trường hợp OrderNumber mới nhỏ hơn OrderNumber hiện tại
                    if (mn.ParentId == null)
                    {
                        l = db.MENUs.Where(m => m.ParentId == null && m.OrderNumber >= STT && m.OrderNumber < mn.OrderNumber).ToList();
                    }
                    else
                    {
                        l = db.MENUs.Where(m => m.ParentId == mn.ParentId && m.OrderNumber >= STT && m.OrderNumber < mn.OrderNumber).ToList();
                    }
                    // Tăng OrderNumber cho các menu giữa STT mới và OrderNumber hiện tại
                    for (int i = 0; i < l.Count; i++)
                    {
                        l[i].OrderNumber++;
                    }
                }
                else if (STT > mn.OrderNumber)
                {
                    // Trường hợp OrderNumber mới lớn hơn OrderNumber hiện tại
                    if (mn.ParentId == null)
                    {
                        l = db.MENUs.Where(m => m.ParentId == null && m.OrderNumber > mn.OrderNumber && m.OrderNumber <= STT).ToList();
                    }
                    else
                    {
                        l = db.MENUs.Where(m => m.ParentId == mn.ParentId && m.OrderNumber > mn.OrderNumber && m.OrderNumber <= STT).ToList();
                    }
                    // Giảm OrderNumber cho các menu giữa OrderNumber hiện tại và STT mới
                    for (int i = 0; i < l.Count; i++)
                    {
                        l[i].OrderNumber--;
                    }
                }

                // Cập nhật thông tin cho menu
                mn.MenuName = strTenMenu;
                mn.MenuLink = strLink;
                mn.OrderNumber = STT;
                db.SaveChanges();

                // Trả về kết quả thành công
                return Json(new { code = 200, msg = "Sửa menu thành công." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Trả về thông báo lỗi nếu có ngoại lệ xảy ra
                return Json(new { code = 500, msg = "Sửa menu thất bại. Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}