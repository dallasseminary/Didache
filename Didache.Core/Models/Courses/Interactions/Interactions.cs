using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Didache {
	public class Interactions {
		
		public static List<InteractionThread> GetInteractionThreads(int taskID) {
            DidacheDb db = new DidacheDb();
            User user = Users.GetLoggedInUser();
            Task task = db.Tasks.Find(taskID);
            Course course = Courses.GetCourse(task.CourseID);
            CourseUser courseUser = course.CourseUsers.SingleOrDefault(u => u.UserID == user.UserID);
            List<int> courseUserIDs = new List<int>();
            foreach (CourseUser cu in course.CourseUsers) {
                if (cu.GroupID == courseUser.GroupID) { courseUserIDs.Add(cu.UserID); }
            }

			List<InteractionThread> threads = db
				.InteractionThreads
				.Include("Posts")
				.Include("Posts.User")
				.Where(t => t.TaskID == taskID && courseUserIDs.Contains(t.UserID))
				.OrderByDescending(t => t.ThreadDate)
				.ToList();

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
	}
}
