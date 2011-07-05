using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Didache.TaskTypes {

	//[Display(Name = "Survey", Description = "Student reports on course progress")]
	public class Survey : ITaskType {

		public TaskTypeResult ProcessFormCollection(int taskID, int userID, FormCollection collection, HttpRequestBase request) {
			DidacheDb db = new DidacheDb();

			UserTaskData data = db.UserTasks.SingleOrDefault(d=> d.TaskID == taskID && d.UserID == userID);

			// save completion status!
			data.TaskCompletionStatus = TaskCompletionStatus.Completed;
			db.SaveChanges();

			return new TaskTypeResult () { Success = true };
		}
	}
}
