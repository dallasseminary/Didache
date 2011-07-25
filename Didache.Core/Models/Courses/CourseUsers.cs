using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Didache {
	public class CourseUsers {

		/// <summary>
		/// 
		/// </summary>
		/// <param name="courseID"></param>
		/// <param name="userID"></param>
		/// <param name="groupID"></param>
		/// <param name="userRole"></param>
		/// <returns>Whether or not the user was added (false = just updated)</returns>
		public static bool AddUserToCourse(int courseID, int userID, int groupID, CourseUserRole userRole) {

			DidacheDb db = new DidacheDb();

			CourseUser courseUser = db.CourseUsers.SingleOrDefault(cu => cu.UserID == userID && cu.CourseID == courseID && cu.RoleID == (int) userRole);
			
			if (courseUser != null) {

				// update the group
				courseUser.GroupID = groupID;
				db.SaveChanges();

				return false;
			} else {
				courseUser = new CourseUser() {
					UserID = userID,
					CourseID = courseID,
					RoleID = (int) userRole,
					GroupID = groupID
				};

				db.CourseUsers.Add(courseUser);

				if (userRole == CourseUserRole.Student) {

					// check for assignments
					bool hasTasks = db.UserTasks.Count(ut => ut.UserID == userID && ut.Task.CourseID == courseID) > 0;

					if (!hasTasks) {


						// create assignments
						List<Task> tasks = db.Tasks.Where(t => t.CourseID == courseID).ToList();

						foreach (Task task in tasks) {
							UserTaskData utd = new UserTaskData() {
								TaskID = task.TaskID,
								CourseID = task.CourseID,
								UnitID = task.UnitID,
								GraderComments = "",
								GraderSubmitDate = null,
								GraderUserID = 0,
								GradeStatus = 0,
								LetterGrade = "",
								NumericGrade = null,
								StudentComments = "",
								StudentSubmitDate = null,
								TaskCompletionStatus = TaskCompletionStatus.NotStarted,
								TaskData = "",
								GraderFileID = 0,
								PostID = 0,
								StudentFileID = 0,
								UserID = userID
							};

							db.UserTasks.Add(utd);
						}
					}
				}

				db.SaveChanges();
				return true;

			}

			


		}

		public static void RemoveUserFromCourse(int courseID, int userID, CourseUserRole userRole) {
			DidacheDb db = new DidacheDb();

			CourseUser courseUser = db.CourseUsers.SingleOrDefault(cu => cu.UserID == userID && cu.CourseID == courseID && cu.RoleID == (int) userRole);
			
			if (courseUser != null) {
				db.CourseUsers.Remove(courseUser);
			}

			if (userRole == CourseUserRole.Student) {
				// don't delete assignments
			}

			db.SaveChanges();
		}


	}
}
