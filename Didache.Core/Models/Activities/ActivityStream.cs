using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Didache {
	public class ActivityStream {

		public static List<ActivityStreamItemBase> GetUsersItems() {
			var didache = new DidacheDb();

			// get the users courses first
			List<int> courseIDs = new List<int>();
			//var courses = Courses.GetUsersRunningCourses(CourseUserRole.Student);
			var courses = Courses.GetUsersCourses(CourseUserRole.Student);
			courses.RemoveAll(c => c.SessionID != 32);

			foreach (Course course in courses)
				courseIDs.Add(course.CourseID);

			return GetUsersItems(courseIDs);
		}

		public static List<ActivityStreamItemBase> GetUsersItems(int courseID) {
			return GetUsersItems(new List<int> { courseID });
		}

		private static List<ActivityStreamItemBase> GetUsersItems(List<int> courseIDs) {

			List<ActivityStreamItemBase> items = new List<ActivityStreamItemBase>();

			var didache = new DidacheDb();

			// discussion replies
			var posts = didache
							.ForumPosts
								.Include("User")
								.Include("Thread.Forum.Course")
							.Where(p => courseIDs.Contains(p.Thread.Forum.CourseID))
							.OrderByDescending(p => p.PostDate)
							.Take(20);
			foreach (ForumPost post in posts) {
				items.Add(new ForumReplyActivity() { User = post.User, ActivityDate = post.PostDate, Post = post });
			}

			// discussion replies
			var interactions = didache
								.InteractionPosts
									.Include("User")
									.Include("Thread.Task.Course")
								.Where(p => courseIDs.Contains(p.Thread.Task.CourseID))
								.OrderByDescending(p => p.PostDate)
								.Take(20);
			foreach (InteractionPost post in interactions) {
				items.Add(new InteractionActivity() { User = post.User, ActivityDate = post.PostDate, Post = post });
			}


			items.Sort(delegate(ActivityStreamItemBase a, ActivityStreamItemBase b) {
				return -a.ActivityDate.CompareTo(b.ActivityDate);
			});

			return items;
		}

	}
}
