using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using System.IO;

namespace Didache.Web.Areas.Students.Controllers
{
	[Authorize]
    public class ApiController : Controller
    {
		DidacheDb db = new DidacheDb();
		EntityObjectSerializer serializer = new EntityObjectSerializer();

        //
        // GET: /Students/Tasks/

		[Authorize]
		[HttpPost]
        public ActionResult TaskStatus(int taskID, FormCollection collection)
        {
			User user = Users.GetLoggedInUser();

			UserTaskData data = Tasks.GetUserTaskData(taskID, user.UserID);

			object returnValue = null;

			if (data == null) {
				// fake data for graders/profs
				returnValue = new TaskTypeResult() { Success = true };
			}
			else {
				// load type
				returnValue = Didache.TaskTypes.TaskTypeManager.ProcessFormCollection(data.Task.TaskTypeName, taskID, user.UserID, collection, Request);
			}

			// do processing
			return Json(returnValue);
        }

		[HttpPost]
		public ActionResult TaskFile(int taskID, FormCollection collection) {
			
			User user = Users.GetLoggedInUser();
			UserTaskData data = Tasks.GetUserTaskData(taskID, user.UserID);
	
			// do processing
			TaskTypeResult returnValue = Didache.TaskTypes.TaskTypeManager.ProcessFormCollection(data.Task.TaskTypeName, taskID, user.UserID, collection, Request);


			// TODO: for interactions, we might need to go to an #post-123321

			return Redirect(
						"/courses/" + data.Task.Course.Slug + "/schedule/" + data.Task.UnitID + 
						((!String.IsNullOrWhiteSpace(returnValue.UrlHash)) ? "#" + returnValue.UrlHash : "")
					);
		}


		[HttpPost]
		public ActionResult TaskFile2(int taskID, FormCollection collection) {
			User user = Users.GetLoggedInUser();
			
			UserTaskData data = Tasks.GetUserTaskData(taskID, user.UserID);

			Guid uniqueID = Guid.NewGuid();
			string originalFilename = "";
			string originalExtension = "";
			int fileID = 0;
			HttpPostedFileBase file = null;

			// save file
			if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0) {
				file = Request.Files[0];
									
				originalFilename = Path.GetFileName(file.FileName);
				originalExtension = Path.GetExtension(file.FileName);


				string filePath = Path.Combine(Settings.StudentFilesLocation, uniqueID.ToString() + originalExtension);
				file.SaveAs(filePath);

				StudentFile studentFile = new StudentFile();
				studentFile.UserID = user.UserID;
				studentFile.UniqueID = uniqueID;
				studentFile.ContentType = file.ContentType;
				studentFile.Length = file.ContentLength;
				studentFile.Filename = originalFilename;
				studentFile.UploadedDate = DateTime.Now;

				var db = new DidacheDb();
				db.StudentFiles.Add(studentFile);
				db.SaveChanges();

				fileID = studentFile.FileID;
			}

			// do processing
			object returnObject = new {
				success= true, 
				fileID = fileID, 
				fileLength=file.ContentLength, 
				filename=originalFilename
			}; // Didache.TaskTypes.TaskTypeManager.ProcessFormCollection(data.Task.TaskTypeName, taskID, userID, collection, Request);

			return Json(returnObject);

			//return Redirect("/courses/" + data.Task.Course.Slug + "/schedule/" + data.Task.UnitID);
		}


		[HttpPost]
		public ActionResult InteractionReply(FormCollection collection) {

			int threadID = 0;

			if (Int32.TryParse(collection["threadID"], out threadID)) {

				DidacheDb db = new DidacheDb();
				User user = Users.GetLoggedInUser();
				InteractionThread thread = db.InteractionThreads.Find(threadID);
				
				InteractionPost post = new InteractionPost();
				post.PostContent = collection["text"];
				post.PostContentFormatted = Interactions.FormatPost(collection["text"]);
				post.IsApproved = true;
				post.UserID = user.UserID;
				post.PostDate = DateTime.Now;
				post.ThreadID = threadID;
				post.TaskID = thread.TaskID;
				post.FileID = 0;
				post.Subject = "RE: " + thread.Subject;

				int replyToPostID = 0;
				if (!Int32.TryParse(collection["ReplyToPostID"], out replyToPostID)) {

				}
				post.ReplyToPostID = replyToPostID;

				db.InteractionPosts.Add(post);
				db.SaveChanges();


				// check for completion
				// threads for this unit
				List<InteractionThread> threads = db.InteractionThreads.Where(t => t.TaskID == thread.TaskID).ToList();
				List<int> threadIDs = threads.Select(t => t.ThreadID).ToList();
				List<InteractionPost> posts = db.InteractionPosts
													.Where(p => threadIDs.Contains(p.ThreadID) && p.UserID == user.UserID)
													.ToList();

				int minimumInteractions = 4; // note: the student's initial task is also counted in this!
				bool isCompleted = false;
				if (posts.Count >= minimumInteractions) {
					isCompleted = true;
					UserTaskData data = db.UserTasks.SingleOrDefault(ud => ud.UserID == user.UserID && ud.TaskID == thread.TaskID);
					if (data != null) {
						data.TaskCompletionStatus = TaskCompletionStatus.Completed;
						db.SaveChanges();
					}
				}
				// 

				return Json(new { 
									success = true, 
									postID = post.PostID, 
									user = user,
									post = serializer.Serialize(post),
									isCompleted = isCompleted
				});

			} else {
				return Json(new {success= false});
			}

		}

		[Authorize]
		[HttpPost]
		public ActionResult SubmitUnitSurvey(int taskID, UnitSurvey model) {

			User user = Users.GetLoggedInUser();
			Task task = db.Tasks.Include("Course").SingleOrDefault(t => t.TaskID == taskID);

			model.DateStamp = DateTime.Now;
			model.UserID = user.UserID;

			db.UnitSurveys.Add(model);
			db.SaveChanges();

			UserTaskData userTaskData = db.UserTasks.SingleOrDefault(utd => utd.UserID == user.UserID && utd.TaskID == taskID);
			userTaskData.TaskCompletionStatus = TaskCompletionStatus.Completed;
			userTaskData.StudentSubmitDate = DateTime.Now;
			userTaskData.NumericGrade = 100;
			db.SaveChanges();

			//return Json(new { success = false });
			return Redirect("/courses/" + task.Course.Slug + "/schedule/" + task.UnitID);
		}

    }
}
