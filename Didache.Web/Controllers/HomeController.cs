using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Didache.Models;

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

			HelpModel model = new HelpModel();
			User user = Users.GetLoggedInUser();

			if (user != null) {
				model.Email = user.Email;
				model.Name = user.FormattedName;
			}

			return View(model);
		}

		[HttpPost]
		public ActionResult Help(HelpModel model) {

			if (ModelState.IsValid) {

				User user = Users.GetLoggedInUser();
				List<string> emails = new List<string>() {"babegg@dts.edu","mmckee@dts.edu","coursemanagement@dts.edu", "jdyer@dts.edu"};
				string lang = Users.GetUserLanguage();
				
				if (lang == "zh-TW" || lang == "zh-CN") {
					emails.Add("eshyu@dts.edu");
				}

				string message = @"
Name: " + model.Name + @"
Email: " + model.Email + @"
Message: 
" + model.Message + 
  
"\n\nUser Agent: " + Request.UserAgent;


				Emails.SendEmail(model.Email, emails, "Help Request", message);

				model.IsSubmitted = true;
			}

			return View(model);
		}

		public ActionResult Tour() {
			return View();
		}

		public ActionResult About() {
			return View();
		}

    }
}
