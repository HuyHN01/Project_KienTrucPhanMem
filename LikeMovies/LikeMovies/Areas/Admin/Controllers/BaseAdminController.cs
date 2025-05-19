using LikeMovies.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LikeMovies.Areas.Admin.Controllers
{
    public class BaseAdminController : Controller
    {
        // GET: Admin/BaseAdmin
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Kiểm tra session "Admin" xem có tồn tại hay không
            if (Session["Admin"] == null)
            {
                // Nếu không có session, điều hướng đến trang đăng nhập
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(
                        new { controller = "Home", action = "DangNhap" }
                    )
                );
            }
            base.OnActionExecuting(filterContext);
        }
    }
}