using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Didache.Web.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

		public ActionResult Help() {
			return View();
		}

		public ActionResult Tour() {
			return View();
		}

		public ActionResult About() {
			return View();
		}

    }
}
