using LikeMovies.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
namespace LikeMovies.Areas.Admin.Controllers
{
    public class SliderController : Controller
    {
        MovieEntities db = new MovieEntities();
        // GET: Admin/Slider
        public ActionResult Index(int? page)
        {
            int pageSize = 10; 
            int pageNumber = (page ?? 1); 

            var sliders = db.PosterMovie
                            .Where(p => p.IsSlider.HasValue && p.IsSlider.Value)
                            .OrderBy(p => p.SliderOrder)
                            .ToPagedList(pageNumber, pageSize);

            return View(sliders);
        }

        // Tạo mới slider
        public ActionResult Create()
        {
            ViewBag.Movies = db.Movies.Select(m => new SelectListItem
            {
                Value = m.MovieID.ToString(),
                Text = m.Title
            }).ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PosterMovie slider)
        {
            if (ModelState.IsValid)
            {
                slider.IsSlider = true;
                db.PosterMovie.Add(slider);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(slider);
        }

        // Sửa slider
        public ActionResult Edit(int id)
        {
            var slider = db.PosterMovie.Find(id);
            if (slider == null || !slider.IsSlider.GetValueOrDefault(false))
            {
                return HttpNotFound();
            }
            ViewBag.Movies = db.Movies.Select(m => new SelectListItem
            {
                Value = m.MovieID.ToString(),
                Text = m.Title
            }).ToList();
            return View(slider);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PosterMovie slider)
        {
            if (ModelState.IsValid)
            {
                slider.IsSlider = true;
                db.Entry(slider).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(slider);
        }

        // Xóa slider
        public ActionResult Delete(int id)
        {
            var slider = db.PosterMovie.Include(p => p.Movies) // Bao gồm Movies khi tìm slider
                               .FirstOrDefault(p => p.PosterID == id);

            if (slider == null || !slider.IsSlider.GetValueOrDefault(false))
            {
                return HttpNotFound();
            }
            return View(slider);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var slider = db.PosterMovie.Find(id);
            if (slider == null)
            {
                return HttpNotFound();
            }

            // Xóa slider ngay lập tức
            db.PosterMovie.Remove(slider);
            db.SaveChanges();

            // Quay lại trang danh sách slider
            return RedirectToAction("Index");
        }

    }
}