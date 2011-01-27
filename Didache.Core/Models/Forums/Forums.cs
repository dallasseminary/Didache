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
			return new DidacheDb().Threads.Where(t => t.ForumID == forumID).ToList();
		}

		public static List<Post> GetPosts(int threadID) {
			return new DidacheDb().Posts.Where(t => t.ThreadID == threadID).ToList();
		}

		public static Forum GetForum(int forumID, bool includeThreads) {
			if (includeThreads) {
				return new DidacheDb().Forums.Include("Threads").SingleOrDefault(f => f.ForumID == forumID);
			} else {
				return new DidacheDb().Forums.SingleOrDefault(f => f.ForumID == forumID);
			}
		}

		public static Thread GetThread(int threadID, bool includePosts) {
			if (includePosts) {
				return new DidacheDb().Threads.Include("Posts").SingleOrDefault(t => t.ThreadID == threadID);
			} else {
				return new DidacheDb().Threads.SingleOrDefault(t => t.ThreadID == threadID);
			}
		}

		public static string FormatPost(string input) {
			return input.Replace("\n", "<br />");
		}
	}
}
