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
		DidacheDb db = new DidacheDb();

        public ActionResult Index()
        {
			HomeViewModel homeModel = new HomeViewModel();

			DateTime now = DateTime.Now;
			homeModel.Announcements = db.Announcements
											.Where(an => an.IsActive && now > an.StartDate && (an.EndDate == null || now < an.EndDate))
											.OrderBy(an => an.StartDate)
											.ToList();

            return View(homeModel);
        }

		[Authorize]
		public ActionResult Posts() {
			HomeViewModel homeModel = new HomeViewModel();

			DateTime now = DateTime.Now;
			homeModel.Announcements = db.Announcements
											.Where(an => an.IsActive && now > an.StartDate && (an.EndDate == null || now < an.EndDate))
											.OrderBy(an => an.StartDate)
											.ToList();

			return View(homeModel);
		}

		[Authorize]
		public ActionResult Post(int id) {
			return View(id);
		}




		public ActionResult Resources() {
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

				string message = 
					((user != null)  ? "ID: " + user.UserID + " https://online.dts.edu/admin/users/edituser/" + user.UserID + "\n" : "") +
@"
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

		public ActionResult Faq() {

			List<HelpCategory> categories = db.HelpCategories
												.Include("HelpQuestions")
												.OrderBy(cat => cat.SortOrder)
												.ToList();

			foreach (HelpCategory cat in categories) {
				cat.HelpQuestions = cat.HelpQuestions
											.OrderBy(question => question.SortOrder)
											.ToList();
			}

			return View(categories);
		}

    }
}
