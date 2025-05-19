using LikeMovies.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LikeMovies.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        MovieEntities db= new MovieEntities();
        // GET: Admin/Home
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult DangNhap(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl; // Truyền returnUrl vào View để giữ lại URL gốc
            return View();
        }

        [HttpPost]
        public ActionResult DangNhap(FormCollection f, string returnUrl)
        {
            var sTenDN = f["TenDN"];
            var sMatKhau = f["MatKhau"];
            if (string.IsNullOrEmpty(sTenDN))
            {
                ViewData["Err1"] = "Bạn chưa nhập tên đăng nhập";
            }
            else if (string.IsNullOrEmpty(sMatKhau))
            {
                ViewData["Err2"] = "Phải nhập mật khẩu";
            }
            else
            {
                AdminMovie kh = db.AdminMovie.SingleOrDefault(n => n.UsernameAd == sTenDN && n.PasswordAd == sMatKhau);
                if (kh != null)
                {
                    ViewBag.ThongBao = "Chúc mừng đăng nhập thành công";
                    Session["Admin"] = kh;

                    if (f["remember"] != null && f["remember"].Contains("true"))
                    {
                        Response.Cookies["TenDN"].Value = sTenDN;
                        Response.Cookies["MatKhau"].Value = sMatKhau;
                        Response.Cookies["TenDN"].Expires = DateTime.Now.AddDays(1);
                        Response.Cookies["MatKhau"].Expires = DateTime.Now.AddDays(1);
                    }
                    else
                    {
                        Response.Cookies["TenDN"].Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies["MatKhau"].Expires = DateTime.Now.AddDays(-1);
                    }

                    // Điều hướng đến returnUrl nếu có, nếu không trở về trang chủ
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không đúng";
                }
            }
            // Nếu đăng nhập thất bại, trả lại view đăng nhập
            ViewBag.ReturnUrl = returnUrl; // Truyền lại returnUrl để thử lại
            return View();
        }


        public ActionResult DangXuat()
        {
            Session["Admin"] = null;
            return RedirectToAction("Index", "Home");

        }
    }
}