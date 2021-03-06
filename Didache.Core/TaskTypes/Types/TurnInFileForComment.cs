﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.ComponentModel.DataAnnotations;

namespace Didache.TaskTypes {

	//[Display(Name = "Turn In File (for comment)", Description = "Student uploads a file for a grader to process and for other students to comment upon.")]
	public class TurnInFileForComment : ITaskType {

		public TaskTypeResult ProcessFormCollection(int taskID, int userID, FormCollection collection, HttpRequestBase request) {
			DidacheDb db = new DidacheDb();

			Task task = db.Tasks.Find(taskID);
			UserTaskData data = db.UserTasks.SingleOrDefault(d=> d.TaskID == taskID && d.UserID == userID);

			StudentFile studentFile = null;

			// save file
			if (request.Files.Count > 0) {
				HttpPostedFileBase file = request.Files[0];

				/* What if this is a GTA?
				 * - Create a fake task for looking up and saving information
				 */ 
				UserTaskData dataForSaving = data;
				if (dataForSaving == null) {
					dataForSaving = new UserTaskData() {
						UserID = userID,
						TaskID = taskID
					};
				}


				studentFile = CourseFiles.SaveStudentFile(userID, dataForSaving, file);
			}

			if (studentFile == null) {
				return new TaskTypeResult() { Success = false, Message = "No file" };
			}
			else {

				// save this file, even if somethign messes up with the forums
				if (data != null) {
					data.StudentFileID = studentFile.FileID;
					db.SaveChanges();
				}

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

				//if (interactionTaskID == 0) {
				//	return new TaskTypeResult() { Success = false, Message = "No related task" };
				//}

				InteractionThread thread = null;
				InteractionPost post = null;
				bool isNewPost = true;

				// check for existing thread/post
				if (data != null && data.PostID > 0) {
					post = db.InteractionPosts.SingleOrDefault(p => p.PostID == data.PostID);
					if (post != null) {
						thread = db.InteractionThreads.SingleOrDefault(t => t.ThreadID == post.ThreadID);

						if (thread != null) {
							isNewPost = false;
						}
					}
				}

				// CREATE POST
				if (isNewPost) {
					thread = new InteractionThread();
				}
				thread.UserID = userID;
				thread.TotalReplies = 0;
				thread.IsDeleted = false;
				thread.Subject = "Assignment: " + task.Name;
				thread.TaskID = interactionTaskID;
				thread.ThreadDate = DateTime.Now;
				if (isNewPost) {
					db.InteractionThreads.Add(thread);
				}
				db.SaveChanges();

				if (isNewPost) {
					post = new InteractionPost();
				}
				post.IsApproved = true;
				post.IsDeleted = false;
				post.PostContent = request["usercomment"];
				post.PostContentFormatted = Interactions.FormatPost(request["usercomment"]);
				post.PostDate = DateTime.Now;
				post.ReplyToPostID = 0;
				post.ThreadID = thread.ThreadID;
				post.UserID = userID;
				post.Subject = "RE: Assignment: " + task.Name;
				post.TaskID = interactionTaskID;
				post.FileID = studentFile.FileID;
				if (isNewPost) {
					db.InteractionPosts.Add(post);
				}
				db.SaveChanges();

				if (data != null) {
					data.PostID = post.PostID;
					data.StudentSubmitDate = DateTime.Now;
					data.TaskCompletionStatus = TaskCompletionStatus.Completed;

					db.SaveChanges();
				}

				

				return new TaskTypeResult() { Success = true, UrlHash = "thread-" + thread.ThreadID };
			}
		}
	}
}
