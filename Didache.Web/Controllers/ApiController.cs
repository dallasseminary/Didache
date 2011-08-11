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

		[Authorize]
		public ActionResult GetCourseUsers(int id) {

			//return Json(serializer.Serialize(Didache.Courses.GetUsersInCourse(id)), JsonRequestBehavior.AllowGet);

			return Json(Didache.Courses.GetUsersInCourse(id).Select(u => new {
								UserID = u.UserID, 
								CourseID = u.CourseID,
								GroupID = u.GroupID,
								RoleID = u.RoleID,
								User = new {
									UserID = u.User.UserID,
									FirstName = u.User.FirstName,
									MiddleName = u.User.MiddleName,
									LastName = u.User.LastName,
									FullName = u.User.FullName,
									NickName = u.User.NickName,
									NameFormat = u.User.NameFormat,
									FormattedName = u.User.FormattedName,
									FormattedNameLastFirst = u.User.FormattedNameLastFirst,
									Email = u.User.Email
								}
					}), JsonRequestBehavior.AllowGet);
		}


		[Authorize]
		public ActionResult FindUsers(string query) {
			
			var dbQuery = db.Users.AsQueryable();

			foreach (String part in query.Split(new char[] {' '})) {
				if (String.IsNullOrEmpty(part))
					continue;

				int id = 0;
				if (Int32.TryParse(part, out id)) {
					dbQuery = dbQuery.Where(u => u.UserID == id);
				} else {
					dbQuery = dbQuery.Where(u => u.LastName == part || u.FirstName == part);
				}
			}

			List<User> users = dbQuery.ToList();

			return Json(serializer.Serialize(users.Select(u => new { UserID = u.UserID, FormattedName = u.FormattedName, FormattedNameLastFirst = u.FormattedNameLastFirst, FirstName = u.FirstName, LastName = u.LastName })), JsonRequestBehavior.AllowGet);
		}


    }
}
