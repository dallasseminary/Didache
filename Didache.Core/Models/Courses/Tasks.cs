using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Didache {
	public class Tasks {

		public static List<UserTaskData> GetUserTaskDataInUnit(int unitID, int userID, bool createFakeData) {

			DidacheDb db = new DidacheDb();
			List<UserTaskData> userTasks = null;

			if (createFakeData) {
				// for graders
				List<Task> tasks = db.Tasks.Where(t => t.UnitID == unitID).OrderBy(t => t.SortOrder).ToList();

				userTasks = new List<UserTaskData>();
				
				foreach (Task task in tasks) {
					UserTaskData utd = new UserTaskData() {
						TaskID = task.TaskID,
						CourseID = task.CourseID,
						UnitID = task.UnitID,
						TaskCompletionStatus = TaskCompletionStatus.NotStarted,
						Task = task
					};

					//utd.Task = task;

					userTasks.Add(utd);
				}

			}
			else {
				userTasks = db.UserTasks
					.Include("Task")
					.Where(d => d.UserID == userID && d.Task.UnitID == unitID)
					.OrderBy(d => d.Task.SortOrder)
					.ToList();

			}

			return userTasks;
		}

		public static UserTaskData GetUserTaskData(int taskID, int userID) {
			return new DidacheDb().UserTasks.SingleOrDefault(d => d.UserID == userID && d.TaskID == taskID);
		}

	


		public static List<Task> GetUnitTasks(int unitID) {
			return new DidacheDb().Tasks
				.Where(t => t.UnitID == unitID)
				.OrderBy(t=>t.SortOrder)
				.ToList();
		}

		public static List<Task> GetCourseTasks(int courseID) {
			return new DidacheDb().Tasks
				.Where(t => t.Unit.CourseID == courseID)
				.OrderBy(t => t.Unit.SortOrder)
					.ThenBy(t => t.SortOrder)
				.ToList();
		}
	}
}
