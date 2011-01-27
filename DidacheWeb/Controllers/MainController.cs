using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Didache.Controllers {
	public class MainController : Controller {
		public ActionResult Index() {
			ViewBag.Message = "Welcome to ASP.NET MVC!";

			return View();
		}

		public ActionResult About() {
			return View();
		}

		public ActionResult Help() {
			return View();
		}


		public ActionResult DataTest() {
			return View(new DidacheDb().Sessions);
		}


	}
}
