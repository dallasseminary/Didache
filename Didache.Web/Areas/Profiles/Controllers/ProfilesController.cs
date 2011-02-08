using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Didache.Web.Areas.Profiles.Controllers
{
    public class ProfilesController : Controller
    {
        //
        // GET: /Profiles/Profiles/

        public ActionResult Index()
        {
            return View();
        }

		public ActionResult Display(string name) {

			User user = null;
			int userID = 0;

			if (Int32.TryParse(name, out userID)) {
				user = Users.GetUser(userID);
			} else {
				user = Users.GetUser(name);
			}

			return View(user);
		}

		public ActionResult Edit() {
			User user = Users.GetLoggedInUser();
			return View(user);
		}

		[HttpPost]
		public ActionResult Edit(User model) {
			DidacheDb db = new DidacheDb();
			
			// EDIT MODE
			try {
				model = db.Users.Find(model.UserID);

				UpdateModel(model);

				db.SaveChanges();

				return RedirectToAction("Display", new { name = model.Username });
			} catch (Exception ex) {
				ModelState.AddModelError("", "Edit Failure, see inner exception: " + ex.ToString());
			}

			return View(model);
		}

    }
}
