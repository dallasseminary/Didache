using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Didache.TaskTypes {

	//[Display(Name = "Enter Text", Description = "Student just enters text (no file).")]
	public class EnterText : ITaskType {

		public TaskTypeResult ProcessFormCollection(int taskID, int userID, FormCollection collection, HttpRequestBase request) {
			DidacheDb db = new DidacheDb();

			UserTaskData data = db.UserTasks.SingleOrDefault(d=> d.TaskID == taskID && d.UserID == userID);

			// store comments
			data.StudentSubmitDate = DateTime.Now;
			data.TaskCompletionStatus = TaskCompletionStatus.Completed;
			data.StudentComments = collection["usercomment"];

			db.SaveChanges();

			return new TaskTypeResult() { Success = true };
		}
	}
}
