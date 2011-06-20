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

		public ActionResult GetCourseFileGroups(int id) {
			return Json(serializer.Serialize(Didache.CourseFiles.GetCourseFileGroups(id)), JsonRequestBehavior.AllowGet);
		}

		public ActionResult GetCourseFile(int id) {
			return Json(serializer.Serialize(db.CourseFiles.Find(id)), JsonRequestBehavior.AllowGet);
		}

		public ActionResult GetCourseFileGroup(int id) {
			return Json(serializer.Serialize(db.CourseFileGroups.Find(id)), JsonRequestBehavior.AllowGet);
		}

		[Authorize]
		public ActionResult GetCourseUserGroups(int id) {
			return Json(serializer.Serialize(Didache.Courses.GetCourseUserGroups(id)), JsonRequestBehavior.AllowGet);
		}

		[Authorize]
		public ActionResult GetCourseUserGroup(int id) {
			CourseUserGroup g = db.CourseUserGroups.Find(id);
			g.Students = new List<CourseUser>();

			return Json(serializer.Serialize(g), JsonRequestBehavior.AllowGet);
		}


    }
}
