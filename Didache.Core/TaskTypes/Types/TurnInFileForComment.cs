using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Didache.TaskTypes {

	//[Display(Name = "Turn In File (for comment)", Description = "Student uploads a file for a grader to process and for other students to comment upon.")]
	public class TurnInFileForComment : ITaskType {

		public object ProcessFormCollection(int taskID, int userID, FormCollection collection, HttpRequestBase request) {
			DidacheDb db = new DidacheDb();

			UserTaskData data = db.UserTasks.SingleOrDefault(d=> d.TaskID == taskID && d.UserID == userID);

			// handle file data

			// create post

			return new { Success = true };
		}
	}
}
