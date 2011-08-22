using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Didache.Web.Areas.Community.Controllers
{
	[Authorize]
    public class CommunityController : Controller
    {
        //
        // GET: /Profiles/Profiles/

		DidacheDb db = new DidacheDb();



        public ActionResult Index()
        {
			List<Course> courses = Courses.GetUsersCourses(CourseUserRole.Student);

			return View(courses);
        }

		public ActionResult Display(string name) {

			User user = null;
			int userID = 0;

			if (Int32.TryParse(name, out userID)) {
				user = Users.GetUser(userID);
			} else {
				user = Users.GetUser(name);
			}

			ViewBag.Student = db.Students.Find(user.UserID);
			ViewBag.Degrees = db.Degrees.Where(d => d.UserID == user.UserID).ToList();
			ViewBag.Employees = db.Employees.Where(d => d.UserID == user.UserID).ToList();

			return View(user);
		}

		

    }
}
