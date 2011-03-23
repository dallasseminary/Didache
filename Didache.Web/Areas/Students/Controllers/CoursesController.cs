using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Didache.Web.Areas.Students.Controllers
{
    public class CoursesController : Controller
    {
		DidacheDb db = new DidacheDb();

		public ActionResult Index() {

			List<Course> courses = null;
			User profile = Users.GetLoggedInUser();

			if (profile != null)
				courses = Didache.Courses.GetUsersCourses(profile.UserID, CourseUserRole.Student);
			else
				courses = new List<Course>();

			return View(courses);
		}


		public ActionResult Dashboard(string slug) {

			Course course = Didache.Courses.GetCourseBySlug(slug);

			return View(course);
		}

		public ActionResult Schedule(string slug, int? id) {

			Course course = Didache.Courses.GetCourseBySlug(slug);
			List<Unit> units = Didache.Units.GetCourseUnits(course.CourseID);
			Unit currentUnit = null;
			List<UserTaskData> userTasks = null;

			if (id.HasValue) {
				// pick unit specified in URL
				currentUnit = units.AsQueryable().SingleOrDefault(u => u.UnitID == id.Value);
			} else {
				// pick current URL by date
				currentUnit = units.AsQueryable().SingleOrDefault(u => u.StartDate <= DateTime.Now && u.EndDate >= DateTime.Now);
			}

			// fallback to first unit
			if (currentUnit == null && units.Count > 0) {
				currentUnit = units[0];
			}


			// get tasks
			if (currentUnit != null) {
				userTasks = Tasks.GetUserTaskDataInUnit(currentUnit.UnitID, Users.GetLoggedInUser().UserID);
			}
			
			ViewBag.Units = units;
			ViewBag.CurrentUnit = currentUnit;
			ViewBag.UserTasks = userTasks;

			return View(course);
		}

		public ActionResult Files(string slug) {

			Course course = Didache.Courses.GetCourseBySlug(slug);

			ViewBag.CourseFileGroups = CourseFiles.GetCourseFileGroups(course.CourseID);

			return View(course);
		}

		public ActionResult Roster(string slug) {

			//List<CourseUser> users = Didache.Courses.GetUsersInCourse(Didache.Courses.GetCourseBySlug(slug).CourseID, CourseUserRole.Student);

			//List<CourseUserGroup> users = Didache.Courses.GetUsersInCourse(Didache.Courses.GetCourseBySlug(slug).CourseID, CourseUserRole.Student);

			Course course = Didache.Courses.GetCourseBySlug(slug);

			ViewBag.UserGroups = Didache.Courses.GetCourseUserGroups(course.CourseID);

			return View(course);
		}

		public ActionResult Assignments(string slug) {

			Course course = Didache.Courses.GetCourseBySlug(slug);

			return View(course);
		}


		public ActionResult CourseFile(int id, string filename) {
			
			CourseFile file = new DidacheDb().CourseFiles.Find(id);

			string basePath = System.Configuration.ConfigurationManager.AppSettings["CourseFilesLocation"];
			string path = System.IO.Path.Combine(basePath, file.Filename);

			if (System.IO.File.Exists(path)) {

				return File(path, file.ContentType);
			} else {
				return HttpNotFound("Cannot find: " + file.Filename);
			}
		}

		public ActionResult StudentFile(int id, string filename) {

			CourseFile file = new DidacheDb().CourseFiles.Find(id);

			string basePath = System.Configuration.ConfigurationManager.AppSettings["StudentFilesLocation"];
			string path = System.IO.Path.Combine(basePath, file.Filename);

			if (System.IO.File.Exists(path)) {

				return File(path, file.ContentType);
			} else {
				return HttpNotFound("Cannot find: " + file.Filename);
			}
		}

		public ActionResult GradedFile(int id, string filename) {

			CourseFile file = new DidacheDb().CourseFiles.Find(id);

			string basePath = System.Configuration.ConfigurationManager.AppSettings["GradedFilesLocation"];
			string path = System.IO.Path.Combine(basePath, file.Filename);

			if (System.IO.File.Exists(path)) {

				return File(path, file.ContentType);
			} else {
				return HttpNotFound("Cannot find: " + file.Filename);
			}
		}


		public ActionResult DownloadAll(string slug) {

			Course course = Didache.Courses.GetCourseBySlug(slug);

			string basePath = System.Configuration.ConfigurationManager.AppSettings["StudentFilesLocation"];

			List<CourseFileGroup> groups = CourseFiles.GetCourseFileGroups(course.CourseID);

			// TODO: create zip file	
			
			return HttpNotFound("This doesn't work yet");
			
		}

    }
}
