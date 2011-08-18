using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Didache {
	public class Interactions {
		
		public static List<InteractionThread> GetInteractionThreads(int taskID) {
			/*
			DidacheDb db = new DidacheDb();

			User user = Users.GetLoggedInUser();
			Task task = db.Tasks.Find(taskID);
			CourseUser courseUser = db.CourseUsers.SingleOrDefault(cu => cu.UserID == user.UserID && cu.CourseID == task.CourseID);
			List<CourseUser> groupMembers = db.CourseUsers.Where( cu => cu.CourseID == task.CourseID && cu.GroupID == courseUser.GroupID).ToList();
			List<int> groupMemberUserIds = new List<int>();
			foreach (CourseUser member in groupMembers) {
				groupMemberUserIds.Add(member.UserID);	
			}
			 

			List<InteractionThread> threads = db
				.InteractionThreads
				.Include("Posts")
				.Include("Posts.User")
				.Where(t => t.TaskID == taskID && groupMemberUserIds.Contains(t.UserID) )
				.OrderByDescending(t => t.ThreadDate)
				.ToList();
			 * 
			*/


			
			DidacheDb db = new DidacheDb();
            User user = Users.GetLoggedInUser();
            Task task = db.Tasks.Include("Course").SingleOrDefault(t => t.TaskID == taskID);
			Course course = task.Course;

			// find this user's group
            CourseUser courseUser = course.CourseUsers
											.SingleOrDefault(u => u.UserID == user.UserID && 
																  u.RoleID == (int) CourseUserRole.Student);

			// find all the people in the group
            List<int> courseUserIDs = new List<int>();
			if (courseUser != null) {
				foreach (CourseUser cu in course.CourseUsers) {
					if (cu.GroupID == courseUser.GroupID) {
						courseUserIDs.Add(cu.UserID);
					}
				}
			} else {
				courseUserIDs = course.CourseUsers.Select(cu => cu.UserID).ToList();
			}

			List<InteractionThread> threads = db
				.InteractionThreads
				.Include("Posts")
				.Include("Posts.User")
				.Where(t => t.TaskID == taskID && courseUserIDs.Contains(t.UserID))
				.OrderByDescending(t => t.ThreadDate)
				.ToList();

			// sorting the posts in memory!! Yeah, save DB time so we can cripple the web server
			foreach (InteractionThread thread in threads) {
				var posts = thread.Posts.ToList();

				posts.Sort(delegate(InteractionPost a, InteractionPost b) {
					return a.PostDate.CompareTo(b.PostDate);
				});

				thread.Posts = posts;
			}

			return threads;
		}

		public static List<InteractionPost> GetInteractionPosts(int threadID) {

            //return new List<InteractionPost>();
            
            return new DidacheDb()
				.InteractionPosts
				.Where(p => p.ThreadID == threadID)
				.OrderByDescending(p => p.PostDate)
				.ToList();

			// reorder with nesting?
		}

		public static string FormatPost(String input) {
			return "<p>" + input.Replace("\r", "").Replace("\n\n", "</p><p>").Replace("\n", "<br>") + "</p>";
		}
	}
}
