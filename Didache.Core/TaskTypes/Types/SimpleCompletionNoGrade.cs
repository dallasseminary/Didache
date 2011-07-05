using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Didache.TaskTypes {

	//[Display(Name = "Simple Completion", Description = "Student presses a complete button, then the grade is marked as 100.")]
	public class SimpleCompletion : ITaskType {

		public TaskTypeResult ProcessFormCollection(int taskID, int userID, FormCollection collection, HttpRequestBase request)
		{
			DidacheDb db = new DidacheDb();

			UserTaskData data = db.UserTasks.SingleOrDefault(d=> d.TaskID == taskID && d.UserID == userID);

			data.TaskCompletionStatus = (TaskCompletionStatus)Int32.Parse(collection["TaskStatus"]);

			if (data.TaskCompletionStatus == TaskCompletionStatus.Completed) {
				data.NumericGrade = 100;
			} else {
				data.NumericGrade = null;
			}

			db.SaveChanges();

			return new TaskTypeResult () { Success = true };
		}
	}
}
