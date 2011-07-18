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

		[Authorize]
		public ActionResult Index() {

			List<Course> courses = null;
			User profile = Users.GetLoggedInUser();

			if (profile != null)
				courses = Didache.Courses.GetUsersCourses(profile.UserID, CourseUserRole.Student);
			else
				courses = new List<Course>();

			return View(courses);
		}

		[Authorize]
		public ActionResult Dashboard(string slug) {

			Course course = Didache.Courses.GetCourseBySlug(slug);

			return View(course);
		}

		[Authorize]
		public ActionResult Schedule(string slug, int? id) {

			
			Course course = Didache.Courses.GetCourseBySlug(slug);
			User user = Users.GetLoggedInUser();
			List<Unit> units = Didache.Units.GetCourseUnits(course.CourseID);
			Unit currentUnit = null;
			List<UserTaskData> userTasks = null;			

			// (1) figure out which Unit to show (either by ID or date)
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


			// (2) get tasks
			if (currentUnit != null) {
				userTasks = Tasks.GetUserTaskDataInUnit(currentUnit.UnitID, user.UserID);
			}


			// (3) all other data
			// TODO: more efficient
			
			ViewBag.AllUserTasks = db.UserTasks.Where(ut => ut.UserID == user.UserID && ut.CourseID == course.CourseID).ToList();
            ViewBag.UserGroups = Didache.Courses.GetCourseUserGroups(course.CourseID);
			ViewBag.Units = units;
			ViewBag.CurrentUnit = currentUnit;
			ViewBag.UserTasks = userTasks;

			return View(course);
		}

		[Authorize]
		public ActionResult Files(string slug) {

			Course course = Didache.Courses.GetCourseBySlug(slug);

			ViewBag.CourseFileGroups = CourseFiles.GetCourseFileGroups(course.CourseID);

			return View(course);
		}

		[Authorize]
		public ActionResult Roster(string slug) {

			//List<CourseUser> users = Didache.Courses.GetUsersInCourse(Didache.Courses.GetCourseBySlug(slug).CourseID, CourseUserRole.Student);

			//List<CourseUserGroup> users = Didache.Courses.GetUsersInCourse(Didache.Courses.GetCourseBySlug(slug).CourseID, CourseUserRole.Student);

			Course course = Didache.Courses.GetCourseBySlug(slug);

			ViewBag.UserGroups = Didache.Courses.GetCourseUserGroups(course.CourseID);

			return View(course);
		}

		[Authorize]
		public ActionResult Assignments(string slug) {

			User user = Users.GetLoggedInUser();
            Course course = Didache.Courses.GetCourseBySlug(slug);

            //ViewBag.CourseFileGroups = CourseFiles.GetCourseFileGroups(course.CourseID);
            //ViewBag.StudentFiles = db.StudentFiles;


			ViewBag.AllUserTasks = db.UserTasks
										.Include("Task.Unit")
										.Include("StudentFile")
										.Include("GradedFile")
										.Where(ut => ut.UserID == user.UserID && ut.CourseID == course.CourseID).ToList();


			return View(course);
		}

		[Authorize]
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

		[Authorize]
		public ActionResult StudentFile(int id, string filename) {

			StudentFile file = new DidacheDb().StudentFiles.Find(id);

			string path = file.PhysicalPath;

			if (System.IO.File.Exists(path)) {

				return File(path, file.ContentType);
			} else {
				return HttpNotFound("Cannot find: " + file.Filename);
			}
		}

		[Authorize]
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

		[Authorize]
		public ActionResult DownloadAll(string slug) {

			Course course = Didache.Courses.GetCourseBySlug(slug);

			string basePath = System.Configuration.ConfigurationManager.AppSettings["StudentFilesLocation"];

			List<CourseFileGroup> groups = CourseFiles.GetCourseFileGroups(course.CourseID);

			// TODO: create zip file	
			
			return HttpNotFound("This doesn't work yet");
			
		}

		public ActionResult iCal(string slug, string type, int userID) {

			DidacheDb db = new DidacheDb();

			Course course = Didache.Courses.GetCourseBySlug(slug);
			User user = Users.GetUser(userID);

			List<UserTaskData> userTask = db.UserTasks
											.Include("Task.Unit")
											.Where(ut => ut.UserID == user.UserID && ut.CourseID == course.CourseID)
											.OrderBy(t => t.Task.Unit.SortOrder)
												.ThenBy(t => t.Task.SortOrder)
											.ToList();

			List<ICalEvent> iEvents = new List<ICalEvent>();

			foreach (UserTaskData task in userTask) {
				DateTime dueDate;

				if (task.Task.DueDate.HasValue)
					dueDate = task.Task.DueDate.Value;
				else					
					dueDate = task.Task.Unit.EndDate;

				iEvents.Add(new ICalEvent() {
					Summary = task.Task.Name + ((task.TaskCompletionStatus == TaskCompletionStatus.Completed) ? " [completed]" : ""),
					Description = Utility.StripHtml(task.Task.Instructions),
					Location = "",
					StartUtc = dueDate.ToUniversalTime(),
					EndUtc = dueDate.ToUniversalTime()
				});
			}

			return new ICalEventResult(
						course.ToString() + " Tasks",
						"Dallas Theological Seminary",
						"", //"DTS-" + course.CourseCode + course.Section + ".ics",
						iEvents);

		}

		public ActionResult Rss(string slug, string type) {
			return Json(new { }, JsonRequestBehavior.AllowGet);
		}

		/*
		public ActionResult iCalUnits(string slug) {

			Course course = Didache.Courses.GetCourseBySlug(slug);

		}
		*/

    }
}
