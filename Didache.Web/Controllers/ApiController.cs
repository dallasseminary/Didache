using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Didache.Web.Controllers
{
    public class ApiController : Controller
    {
		DidacheDb db = new DidacheDb();
		EntityObjectSerializer serializer = new EntityObjectSerializer();

        public ActionResult GetCourse(int id) {
            return Json(serializer.Serialize(db.Courses.Find(id)), JsonRequestBehavior.AllowGet);
        }

		public ActionResult GetUnit(int id) {
			return Json(serializer.Serialize(db.Units.Find(id)), JsonRequestBehavior.AllowGet);
		}


		public ActionResult GetTask(int id) {
			return Json(serializer.Serialize(db.Tasks.Find(id)), JsonRequestBehavior.AllowGet);
		}

	
		public ActionResult GetCourseUnits(int id) {
			return Json(serializer.Serialize(Didache.Courses.GetCourseUnitsWithTasks(id)), JsonRequestBehavior.AllowGet);
		}

		public ActionResult GetCourseFiles(int id) {
			return Json(serializer.Serialize(Didache.CourseFiles.GetCourseFileGroups(id)), JsonRequestBehavior.AllowGet);
		}




    }
}
