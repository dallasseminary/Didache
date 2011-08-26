using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Didache.Web.Areas.Facilitators.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Grading/Home/

		public ActionResult Index(int? sessionID) {
			
			// all courses ever
			List<Course> coursesAsFacilitator = null;
			List<Course> sessionCourses = null;
			Session session = null;

			if (sessionID.HasValue) {
				// get all courses
				coursesAsFacilitator = Didache.Courses.GetUsersCourses(CourseUserRole.Faciliator);
				sessionCourses = coursesAsFacilitator.Where(c => c.SessionID == sessionID.Value).ToList();


				session = new DidacheDb().Sessions.Find(sessionID.Value);

			} else {

				// just active courses 
				coursesAsFacilitator = Didache.Courses.GetUsersRunningCourses(CourseUserRole.Faciliator);

				sessionCourses = coursesAsFacilitator;

			}

			ViewBag.Session = session;					
			ViewBag.Sessions = Sessions.GetSessions();
			ViewBag.CoursesAsFacilitator = coursesAsFacilitator;

			return View(sessionCourses);
		}

    }
}
