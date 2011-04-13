using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Didache.Web.Areas.Community.Controllers
{
    public class CommunityController : Controller
    {
        //
        // GET: /Profiles/Profiles/

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

			return View(user);
		}

		

    }
}
