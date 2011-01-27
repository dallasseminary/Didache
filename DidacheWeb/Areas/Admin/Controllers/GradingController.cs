using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Didache.Web.Areas.Admin.Controllers
{
    public class GradingController : Controller
    {
        //
        // GET: /Admin/Grading/

        public ActionResult Index(int id)
        {
            return View(Courses.GetCourse(id));
        }


		public ActionResult Grid(int id) {

			List<UserTaskData> allUserTasks = Didache.Grading.GetUserTaskData(id);
			List<Task> allTasks = Didache.Tasks.GetCourseTasks(id);

			ViewBag.AllUserTaskData = allUserTasks;
			ViewBag.AllTasks = allTasks;

			return View();
		}

		public ActionResult PostGrade(int id, FormCollection collection) {

			Course courseID = Courses.GetCourse(id);

			int userID = Int32.Parse(collection["UserID"]);

			return Json(new {success= true});
		}
    }
}
