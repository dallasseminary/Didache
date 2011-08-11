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

        public ActionResult Index2()
        {
			// get this users courses
			List<Course> coursesAsFacilitator = Didache.Courses.GetUsersRunningCourses(CourseUserRole.Faciliator);
			
			// get all courses if administrator
			List<Course> allCourses = User.IsInRole(UserRoles.Administrator) ?
				Didache.Courses.GetCurrentlyRunningCourses() : coursesAsFacilitator;

			ViewBag.CoursesAsFacilitator = coursesAsFacilitator;
			ViewBag.Sessions = Sessions.GetSessions();

			return View(allCourses);
        }

		public ActionResult Index(int? sessionID) {
			
			// all courses ever
			List<Course> coursesAsFacilitator = null;
			List<Course> sessionCourses = null;

			if (sessionID.HasValue) {
				// get all courses
				coursesAsFacilitator = Didache.Courses.GetUsersCourses(CourseUserRole.Faciliator);


				sessionCourses = (User.IsInRole(UserRoles.Administrator)) ?  
							Didache.Courses.GetCoursesBySession(sessionID.Value) :
							coursesAsFacilitator.Where(c => c.SessionID == sessionID.Value).ToList();
			} else {

				// just active courses 
				coursesAsFacilitator = Didache.Courses.GetUsersRunningCourses(CourseUserRole.Faciliator);

				sessionCourses = User.IsInRole(UserRoles.Administrator) ?
							Didache.Courses.GetCurrentlyRunningCourses() : 
							coursesAsFacilitator;

			}

													
			ViewBag.Sessions = Sessions.GetSessions();
			ViewBag.CoursesAsFacilitator = coursesAsFacilitator;

			return View(sessionCourses);
		}

    }
}
