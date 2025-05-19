using LikeMovies.Models;
using System;
using Facebook;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data.Entity.Validation;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace LikeMovies.Controllers
{
    public class LikeMovieController : Controller
    {
        MovieEntities db = new MovieEntities();
        // GET: LikeMovies
        public ActionResult Index()
        {
            var sliders = db.PosterMovie.Include("Movies").ToList();
            return View(sliders);
        }
        [ChildActionOnly]

        public ActionResult PartialNav()
        {
            List<MENUs> lst = db.MENUs.Where(m => m.ParentId == null).OrderBy(m => m.OrderNumber).ToList();

            int[] a = new int[lst.Count()];
            for (int i = 0; i < lst.Count; i++)
            {
                int id = lst[i].Id;
                List<MENUs> l = db.MENUs.Where(m => m.ParentId == id).ToList();
                int k = l.Count();
                a[i] = k;
            }
            ViewBag.lst = a;
            return PartialView("_PartialNav", lst);
        }
        [ChildActionOnly]
        public ActionResult LoadChildMenu(int parentId)
        {
            List<MENUs> lst = db.MENUs
                                  .Where(m => m.ParentId == parentId)
                                  .OrderBy(m => m.OrderNumber)
                                  .ToList();

            // Đếm số lượng menu con
            ViewBag.Count = lst.Count;
            int[] a = new int[lst.Count];
            for (int i = 0; i < lst.Count; i++)
            {
                int id = lst[i].Id;
                List<MENUs> l = db.MENUs.Where(m => m.ParentId == id).ToList();
                int k = l.Count();
                a[i] = k;
            }

            ViewBag.lst = a;
            return PartialView("LoadChildMenu", lst); // Trả về PartialView với danh sách menu con
        }
        public ActionResult PartialSlider()
        {
            return PartialView("_PartialSlider");
        }
        public ActionResult PartialFooter()
        {
            return PartialView("_PartialFooter");
        }
    }
}