using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Didache.Web.Areas.Admin.Controllers
{
    public class CampusesController : Controller
    {

		private DidacheDb db = new DidacheDb();

		public ActionResult Index() {
			return View(Campuses.GetCampuses());
		}

		public ActionResult Edit(int? id) {
			Campus campus = db.Campuses.SingleOrDefault(s => s.CampusID == id);
			if (campus == null)
				campus = new Campus() { CampusID = 0 };

			return View(campus);
		}

		[HttpPost]
		public ActionResult Edit(Campus model) {

			if (model.CampusID > 0) {
				// EDIT MODE
				try {
					model = db.Campuses.Find(model.CampusID);

					UpdateModel(model);

					db.SaveChanges();

					//return RedirectToAction("Campuses", new { id = model.ID });
					return RedirectToAction("Index");
				} catch (Exception) {
					ModelState.AddModelError("", "Edit Failure, see inner exception");
				}

				return View(model);
			} else {

				// ADD MODE
				if (ModelState.IsValid) {
					db.Campuses.Add(model);
					db.SaveChanges();
					return RedirectToAction("Index");
				} else {
					return View(model);
				}
			}

		}

		[HttpPost]
		public RedirectToRouteResult Delete(int id, FormCollection collection) {
			Campus campus = db.Campuses.Find(id);
			db.Campuses.Remove(campus);
			db.SaveChanges();

			return RedirectToAction("Campuses");
		}

    }
}
