using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LikeMovies.Areas.Admin.Controllers
{
    public class CNPMController : Controller
    {
        // GET: Admin/CNPM
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ThongKeVaBaoCao()
        {
            return View();
        }
        public ActionResult QLGoiDichVu()
        {
            return View();
        }
        public ActionResult QLThanhToanVaHoaDon()
        {
            return View();
        }
        public ActionResult QLTaiKhoanNV()
        {
            return View();
        }
        public ActionResult QLQuangCao()
        {
            return View();
        }
    }
}