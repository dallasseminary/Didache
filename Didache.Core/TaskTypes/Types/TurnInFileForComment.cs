using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Didache.TaskTypes {

	//[Display(Name = "Turn In File (for comment)", Description = "Student uploads a file for a grader to process and for other students to comment upon.")]
	public class AnswerQuestionForDiscussion : ITaskType {

		public TaskTypeResult ProcessFormCollection(int taskID, int userID, FormCollection collection, HttpRequestBase request) {
			DidacheDb db = new DidacheDb();

			Task task = db.Tasks.Find(taskID);
			UserTaskData data = db.UserTasks.SingleOrDefault(d=> d.TaskID == taskID && d.UserID == userID);

			
			// find the related task to send this post to
			int interactionTaskID = task.RelatedTaskID;

			// nasty lookup
			if (interactionTaskID == 0) {
				List<Task> possibleMatches = db.Tasks
													.Where(t => t.UnitID == task.UnitID && t.TaskTypeName == "CommentOnClassmatesFile")
													.OrderBy(t => t.SortOrder)
													.ToList();

				if (possibleMatches.Count > 0) {
					interactionTaskID = possibleMatches[0].TaskID;
				}

			}
		

			// CREATE POST
			InteractionThread thread = new InteractionThread();
			thread.UserID = userID;
			thread.TotalReplies = 0;
			thread.Subject = "Assignment: " + task.Name;
			thread.TaskID = interactionTaskID;
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
			post.TaskID = interactionTaskID;
			db.InteractionPosts.Add(post);
			db.SaveChanges();

			data.PostID = post.PostID;
			data.StudentSubmitDate = DateTime.Now;				
			data.TaskCompletionStatus = TaskCompletionStatus.Completed;
			
			db.SaveChanges();

			return new TaskTypeResult() { Success = true, UrlHash = "thread-" + thread.ThreadID };
		
		}
	}
}
