using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Didache.Web.Areas.Admin.Controllers
{
    public class AnnouncementsController : Controller
    {
		private DidacheDb db = new DidacheDb();

        public ActionResult Index()
        {
            return View(db.Announcements.OrderByDescending(a => a.StartDate).ToList());
        }

		public ActionResult Edit(int? id) {
			Announcement announcement = db.Announcements.SingleOrDefault(a => a.AnnouncementID == id);
			if (announcement == null)
				announcement = new Announcement() {
									AnnouncementID = 0, 
									StartDate = DateTime.Now, 
									EndDate = DateTime.Now.AddDays(14)
									 };

			return View(announcement);
		}

		[HttpPost]
		public ActionResult Edit(Announcement model) {


			if (model.AnnouncementID > 0) {
				// EDIT MODE
				try {
					model = db.Announcements.Find(model.AnnouncementID);

					UpdateModel(model);

					model.StartDate = new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, 0, 0, 0);
					if (model.EndDate.HasValue)
						model.EndDate = new DateTime(model.EndDate.Value.Year, model.EndDate.Value.Month, model.EndDate.Value.Day, 23, 59, 59);

					db.SaveChanges();

					return RedirectToAction("Index");
				} catch (Exception) {
					ModelState.AddModelError("", "Edit Failure, see inner exception");
				}

				return View(model);
			} else {
				
				// silly stuff
				model.Title = "";
				model.StartDate = new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, 0, 0, 0);
				if (model.EndDate.HasValue)
					model.EndDate = new DateTime(model.EndDate.Value.Year, model.EndDate.Value.Month, model.EndDate.Value.Day, 23, 59, 59);


				// ADD MODE
				if (ModelState.IsValid) {
					db.Announcements.Add(model);
					db.SaveChanges();
					return RedirectToAction("Index");
				} else {
					return View(model);
				}
			}

		}

		[HttpPost]
		public RedirectToRouteResult Delete(int id, FormCollection collection) {
			Session session = db.Sessions.Find(id);
			db.Sessions.Remove(session);
			db.SaveChanges();

			return RedirectToAction("Sessions");
		}
    }
}
