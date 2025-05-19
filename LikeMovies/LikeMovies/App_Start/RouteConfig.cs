using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace LikeMovies
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Trang Chu",
                url: "TrangChu",
                defaults: new { controller = "LikeMovie", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                 name: "phimtheotheloai",
                 url: "phim-theo-the-loai-{genreId}",
                 defaults: new { controller = "DanhMucPhim", action = "PhimTheoTheLoai", genreId = UrlParameter.Optional },
                 namespaces: new[] { "LikeMovies.Controllers" }
             );
            routes.MapRoute(
                 name: "phimtheonam",
                 url: "phim-theo-nam-{year}",
                 defaults: new { controller = "DanhMucPhim", action = "PhimTheoNam", year = UrlParameter.Optional },
                 namespaces: new[] { "LikeMovies.Controllers" }
             );
            routes.MapRoute(
               name: "PhimChieuRap",
               url: "phim-chieu-rap",
               defaults: new { controller = "DanhMucPhim", action = "PhimChieuRap", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "PhimBo",
                url: "phim-bo",
                defaults: new { controller = "DanhMucPhim", action = "PhimBo", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "PhimLe",
                url: "phim-le",
                defaults: new { controller = "DanhMucPhim", action = "PhimBo", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}