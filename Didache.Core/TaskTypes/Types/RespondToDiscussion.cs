using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Didache.TaskTypes {

	public class RespondToDiscussion : ITaskType {

		public TaskTypeResult ProcessFormCollection(int taskID, int userID, FormCollection collection, HttpRequestBase request) {
			DidacheDb db = new DidacheDb();

			Task task = db.Tasks.Find(taskID);
			UserTaskData data = db.UserTasks.SingleOrDefault(d => d.TaskID == taskID && d.UserID == userID);

			// CREATE POST
			InteractionThread thread = new InteractionThread();
			thread.UserID = userID;
			thread.TotalReplies = 0;
			thread.Subject = "Assignment: " + task.Name;
			thread.TaskID = taskID;
			thread.ThreadDate = DateTime.Now;
			db.InteractionThreads.Add(thread);
			db.SaveChanges();

			InteractionPost post = new InteractionPost();
			post.IsApproved = true;
			post.PostContent = request["usercomment"];
			post.PostContentFormatted = Interactions.FormatPost(request["usercomment"]);
			post.PostDate = DateTime.Now;
			post.ReplyToPostID = 0;
			post.ThreadID = thread.ThreadID;
			post.UserID = userID;
			post.Subject = "RE: Assignment: " + task.Name;
			post.TaskID = taskID;
			db.InteractionPosts.Add(post);
			db.SaveChanges();

			return new TaskTypeResult() { Success = true, UrlHash = "thread-" + thread.ThreadID };
		
		}
	}
}
