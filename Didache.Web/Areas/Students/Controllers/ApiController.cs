using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using System.IO;

namespace Didache.Web.Areas.Students.Controllers
{
    public class ApiController : Controller
    {
        //
        // GET: /Students/Tasks/

		[HttpPost]
        public ActionResult TaskStatus(int userID, int taskID, FormCollection collection)
        {
			UserTaskData data = Tasks.GetUserTaskData(taskID, userID);

			// load type
			object returnValue = Didache.TaskTypes.TaskTypeManager.ProcessFormCollection(data.Task.TaskTypeName, taskID, userID, collection, Request);

			// do processing
			return Json(returnValue);
        }

		[HttpPost]
		public ActionResult TaskFile(int userID, int taskID, FormCollection collection) {
			UserTaskData data = Tasks.GetUserTaskData(taskID, userID);


			// save file
			foreach (string inputTagName in Request.Files) {
				HttpPostedFileBase file = Request.Files[inputTagName];
				if (file.ContentLength > 0) {
					string filePath = Path.Combine(HttpContext.Server.MapPath("~/uploads"), Path.GetFileName(file.FileName));
					file.SaveAs(filePath);
				}
			}


			// do processing
			object returnValue = Didache.TaskTypes.TaskTypeManager.ProcessFormCollection(data.Task.TaskTypeName, taskID, userID, collection, Request);



			return Redirect("/courses/" + data.Task.Course.Slug + "/schedule/" + data.Task.UnitID);
		}


		[HttpPost]
		public ActionResult InteractionReply(FormCollection collection) {

			int threadID = 0;

			if (Int32.TryParse(collection["threadID"], out threadID)) {

				DidacheDb db = new DidacheDb();


				InteractionThread thread = db.InteractionThreads.Find(threadID);
				InteractionPost post = new InteractionPost();
				post.PostContent = collection["text"];
				post.PostContentFormatted = collection["text"];
				post.IsApproved = true;
				post.UserID = Users.GetLoggedInUser().UserID;
				post.PostDate = DateTime.Now;
				post.ThreadID = threadID;
				post.TaskID = thread.TaskID;
				post.Subject = "";
				post.ReplyToPostID = 0;

				db.InteractionPosts.Add(post);
				db.SaveChanges();

				return Json(new {success= true, postID= post.PostID});

			} else {
				return Json(new {success= false});
			}

		}
    }
}
