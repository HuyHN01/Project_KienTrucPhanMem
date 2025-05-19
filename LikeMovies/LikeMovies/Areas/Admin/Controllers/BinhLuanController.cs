using LikeMovies.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LikeMovies.Areas.Admin.Controllers
{
    public class BinhLuanController : BaseAdminController
    {
        MovieEntities db = new MovieEntities();
        // GET: Admin/BinhLuan
        public ActionResult Index(int? page)
        {
            int iPageNum = (page ?? 1);
            int iPageSize = 7;

            return View(db.Comments.ToList().OrderBy(n => n.CommentID).ToPagedList(iPageNum, iPageSize));
        }
        // GET: Admin/BinhLuan/Delete/5
        public ActionResult Delete(int id)
        {
            var comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Admin/BinhLuan/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }

            db.Comments.Remove(comment);
            db.SaveChanges();
            return RedirectToAction("Index"); // Redirect to the Index page after deletion
        }

    }
}