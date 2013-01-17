using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Didache {
	public class Forums {
		public static List<Forum> GetCourseForums(int courseID) {
			return new DidacheDb().Forums.Where(f => f.CourseID == courseID).ToList();
		}

		public static List<Thread> GetThreads(int forumID) {
			return new DidacheDb().Threads.Where(t => t.ForumID == forumID).OrderBy(t => t.LastPostDate).ToList();
		}

		public static List<ForumPost> GetPosts(int threadID) {
			return new DidacheDb().ForumPosts.Where(t => t.ThreadID == threadID).ToList();
		}

		public static Forum GetForum(int forumID, bool includeThreads) {
			Forum forum = null;
			
			if (includeThreads) {
				forum = new DidacheDb().Forums.Include("Threads").SingleOrDefault(f => f.ForumID == forumID);

				forum.Threads = forum.Threads.OrderByDescending(t => t.LastPostDate).ToList();

			} else {
				forum = new DidacheDb().Forums.SingleOrDefault(f => f.ForumID == forumID);
			}

			return forum;
		}

		public static Thread GetThread(int threadID, bool includePosts) {
			if (includePosts) {
				return new DidacheDb().Threads.Include("Posts").SingleOrDefault(t => t.ThreadID == threadID);
			} else {
				return new DidacheDb().Threads.SingleOrDefault(t => t.ThreadID == threadID);
			}
		}

        public static string FormatPost(string input) {
            return input.Insert(input.Length, "</p>").Insert(0, "<p>").Replace("\n", "<br />");
            //return input.Replace("\n\n", "</p><p>");
		}
	}
}
