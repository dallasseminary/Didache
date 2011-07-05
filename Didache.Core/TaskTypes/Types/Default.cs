using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Didache.TaskTypes {

	// [Display(Name = "Default", Description = "Does nothing (do not use)")]
	public class Default : ITaskType {

		public TaskTypeResult ProcessFormCollection(int taskID, int userID, FormCollection collection, HttpRequestBase request) {
			DidacheDb db = new DidacheDb();

			UserTaskData data = db.UserTasks.SingleOrDefault(d=> d.TaskID == taskID && d.UserID == userID);

			// handle file data

			return new TaskTypeResult () { Success = true };
		}
	}
}
