using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Didache;

namespace Didache.Web.Areas.Admin.Controllers
{
    public class CoursesController : Controller
    {
		private DidacheDb db = new DidacheDb();

		
		//
        // GET: /Admin/Courses/

        public ActionResult Index()
        {
			List<Course> courses = Didache.Courses.GetCurrentlyRunningCourses();

			return View("List", courses);
        }

		public ActionResult BySession(int id) {

			List<Course> courses = Didache.Courses.GetCoursesBySession(id);

			return View("List", courses);
		}

		public ActionResult Groups(int id) {

			List<CourseUserGroup> groups = Didache.Courses.GetCourseUserGroups(id);

			return View(groups);
		}


		public ActionResult Units(int id) {

			List<Unit> units = Didache.Courses.GetCourseUnits(id);

			return View(units);
		}

		// UNIT EDIT

		public ActionResult Tasks(int id) {

			List<Task> tasks = Didache.Tasks.GetUnitTasks(id);

			return View(tasks);
		}



		public ActionResult Task(int? id, int? unitID) {
			Task task = db.Tasks.SingleOrDefault(t => t.TaskID == id);
			if (task == null)
				task = new Task();

			return View(task);
		}

		[HttpPost]
		//[ValidateInput(false)]
		public ActionResult Task(Task model) {

			if (model.TaskID > 0) {
				// EDIT MODE
				try {
					model = db.Tasks.Find(model.TaskID);

					UpdateModel(model);

					db.SaveChanges();

					return RedirectToAction("tasks", new { id = model.UnitID });					
				} catch (Exception ex) {
					ModelState.AddModelError("", "Edit Failure, see inner exception: " + ex.ToString());
				}

				return View(model);
			} else {

				// ADD MODE
				if (ModelState.IsValid) {
					db.Tasks.Add(model);
					db.SaveChanges();
					return RedirectToAction("tasks", new { id = model.UnitID });
				} else {
					return View(model);
				}
			}

		}


		public ActionResult Edit(int? id)
		{
			Course course = db.Courses.SingleOrDefault(s => s.CourseID == id);
			if (course == null)
				course = new Course() { CourseID = 0 };
			return View(course);
		}

		[HttpPost]
		public ActionResult Edit(Course model)
		{
			if (model.CourseID > 0)
			{
				// EDIT MODE
				try
				{
					model = db.Courses.Find(model.CourseID);

					UpdateModel(model);

					db.SaveChanges();

					//return RedirectToAction("Courses", new { id = model.ID });
					return RedirectToAction("Index");
				}
				catch (Exception)
				{
					ModelState.AddModelError("", "Edit Failure, see inner exception");
				}
				return View(model);
			}
			else
			{
				// ADD MODE
				if (ModelState.IsValid)
				{
					db.Courses.Add(model);
					db.SaveChanges();
					return RedirectToAction("Index");
				}
				else
				{
					return View(model);
				}
			}
		}

    }
}
