using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Didache.TaskTypes {

	//[Display(Name = "Turn In File (simple)", Description = "Student uploads a file for a grader to process.")]
	public class TurnInFileSimple : ITaskType {

		public TaskTypeResult ProcessFormCollection(int taskID, int userID, FormCollection collection, HttpRequestBase request) {
			DidacheDb db = new DidacheDb();

			UserTaskData data = db.UserTasks.SingleOrDefault(d=> d.TaskID == taskID && d.UserID == userID);

			StudentFile studentFile = null;

			// save file
			if (request.Files.Count > 0) {
				HttpPostedFileBase file = request.Files[0];

				studentFile = CourseFiles.SaveStudentFile(userID, data, file);				
			}

			if (studentFile == null) {
				return new TaskTypeResult() { Success = false, Message = "No file" };
			} else {

				data.StudentSubmitDate = DateTime.Now;
				data.TempStudentFileID = studentFile.FileID;
				data.TaskCompletionStatus = TaskCompletionStatus.Completed;

				db.SaveChanges();

				return new TaskTypeResult() { Success = true };
			}

		}
	}
}
