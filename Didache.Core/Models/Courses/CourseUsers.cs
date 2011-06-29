using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Didache {
	public class CourseUsers {

		public static void AddUserToCourse(int courseID, int userID, int groupID, CourseUserRole userRole) {

			DidacheDb db = new DidacheDb();

			CourseUser courseUser = db.CourseUsers.SingleOrDefault(cu => cu.UserID == userID && cu.CourseID == courseID && cu.RoleID == (int) userRole);
			
			if (courseUser != null) {

				// update the group
				courseUser.GroupID = groupID;
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
								TempGraderFileID = 0,
								TempPostID = 0,
								TempStudentFileID = 0,
								UserID = userID
							};

							db.UserTasks.Add(utd);
						}
					}
				}

				

			}

			db.SaveChanges();


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
