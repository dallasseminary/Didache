using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Didache {
	public class Grading {

		public static List<UserTaskData> GetUserTaskData(int courseID) {
			return new DidacheDb().UserTasks
				.Include("Task").Include("Task.Course").Include("PRofile")
				.Where(d => d.Task.CourseID == courseID)
				.OrderBy(d=>d.UserID)
					.ThenBy(d=>d.Task.Unit.SortOrder)
					.ThenBy(d => d.Task.SortOrder)
				.ToList();
		}

	}
}
