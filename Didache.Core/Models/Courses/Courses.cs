using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Didache  {
	public class Courses {
		public static Course GetCourse(int courseID) {
			return new DidacheDb().Courses.SingleOrDefault(c => c.CourseID == courseID);
		}

		public static Course GetCourseBySlug(string slug) {
			string[] parts = slug.Split(new char[] { '-' });
			string sessionCode = parts[0].Trim().ToUpper();
			string courseCode = parts[1].Trim().ToUpper();

			var db = new DidacheDb();

			//Session session = db.Sessions.SingleOrDefault(s => s.SessionCode == sessionCode);
			//Course course = session.Courses.SingleOrDefault(c => c.CourseCode + c.Section == courseCode);

			Course course = db.Courses
								.Include("Session")
								.Include("Campus")
								.SingleOrDefault(c => c.CourseCode + c.Section == courseCode && c.Session.SessionCode == sessionCode);

			return course;
		}

		public static List<Course> GetCoursesBySession(int sessionID) {
			return new DidacheDb().Courses
					.Where(c => c.SessionID == sessionID)
					.OrderBy(c => c.CourseCode)
						.ThenBy(c => c.Section)
						.ThenBy(c => c.StartDate)
					.ToList();
		}

		public static List<Course> GetCurrentlyRunningCourses() {			
			return new DidacheDb().Courses.Where(c => c.StartDate < DateTime.Now && c.EndDate > DateTime.Now).ToList();			
		}

		public static List<CourseUser> GetUsersInCourse(int courseID, CourseUserRole roleID) {
			return new DidacheDb().CourseUsers
				.Include("Profile")
				.Where(cu => cu.CourseID == courseID && cu.RoleID == (int)roleID).ToList();
		}

		public static List<Course> GetUsersCourses(CourseUserRole roleID) {
			return GetUsersCourses(Users.GetLoggedInUser().UserID, roleID);
		}

		public static List<Course> GetUsersCourses(int userID, CourseUserRole roleID) {
			return new DidacheDb().Courses
				.Where(c => c.CourseUsers.Any(cu => cu.UserID == userID && cu.RoleID == (int)roleID))
				.OrderByDescending(c => c.Session.StartDate)
				.ToList();
		}

		public static List<Course> GetUsersRunningCourses(CourseUserRole roleID) {
			User profile = Users.GetLoggedInUser();

			if (profile != null)
				return GetUsersRunningCourses(profile.UserID, roleID);
			else
				return null;
		}

		public static List<Course> GetUsersRunningCourses(int userID, CourseUserRole roleID) {
			return new DidacheDb().Courses
				.Where(c => c.CourseUsers.Any(cu => cu.UserID == userID && cu.RoleID == (int)roleID) && c.StartDate <= DateTime.Now && c.EndDate >= DateTime.Now)
				.OrderByDescending(c => c.StartDate)
				.ToList();
		}

		public static List<CourseUserGroup> GetCourseUserGroups(int courseID) {
			return new DidacheDb().CourseUserGroups
				.Include("Students")
                //.Include("Facilitator")
				.Where(g => g.CourseID == courseID)
				.ToList();
		}



		public static List<Unit> GetCourseUnitsWithTasks(int courseID) {
			List<Unit> units = new DidacheDb().Units
				.Include("Tasks")
				.Where(u => u.CourseID == courseID)
				.OrderBy(u => u.SortOrder)
				.ToList();

			foreach (Unit unit in units) {
				unit.Tasks = unit.Tasks.ToList().OrderBy(t => t.SortOrder).ToList();
			}

			return units;
		}

		public static List<Unit> GetCourseUnits(int courseID) {
			return new DidacheDb().Units
				.Where(u => u.CourseID == courseID)
				.OrderBy(u => u.SortOrder)
				.ToList();
		}


		public static List<UserTaskData> GetImportantTasksForUser() {

			User user = Users.GetLoggedInUser();

			return new DidacheDb().UserTasks
				.Where(utd =>
						utd.UserID == user.UserID &&
						utd.Task.Priority > 1 &&
						utd.Task.DueDate != null &&
						utd.TaskStatus == 0)
				.OrderBy(utd =>
						utd.Task.DueDate)
				.ToList();
		}
	}
}
