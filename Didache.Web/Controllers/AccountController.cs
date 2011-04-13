using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Didache.Models;

namespace Didache.Web.Controllers {
	public class AccountController : Controller {

		
		public ActionResult EditProfile() {

			User user = Users.GetLoggedInUser();

			return View(user);
		}

		[HttpPost]
		public ActionResult EditProfile(User model) {
		
			DidacheDb db = new DidacheDb();
			
			try {
				model = db.Users.Find(model.UserID);

				UpdateModel(model);

				db.SaveChanges();

			} catch (Exception ex) {
				ModelState.AddModelError("", "Edit Failure, see inner exception: " + ex.ToString());
			}
			return RedirectToAction("EditProfile");
		}
	}
}
