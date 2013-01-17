using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Didache.TaskTypes {

	//[Display(Name = "Simple Completion (no grade)", Description = "Student presses a complete button, but the grade is not changed.")]
	public class SimpleCompletionNoGrade : ITaskType {

		public TaskTypeResult ProcessFormCollection(int taskID, int userID, FormCollection collection, HttpRequestBase request) {
			DidacheDb db = new DidacheDb();

			UserTaskData data = db.UserTasks.SingleOrDefault(d=> d.TaskID == taskID && d.UserID == userID);

			data.TaskCompletionStatus = (TaskCompletionStatus)Int32.Parse(collection["TaskStatus"]);
			data.StudentSubmitDate = DateTime.Now;
			data.NumericGrade = null;

			db.SaveChanges();

			return new TaskTypeResult () { Success = true };
		}
	}
}
