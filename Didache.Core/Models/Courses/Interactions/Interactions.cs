using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Didache {
	public class Interactions {
		
		public static List<InteractionThread> GetInteractionThreads(int taskID) {

			List<InteractionThread> threads = new DidacheDb()
				.InteractionThreads
				.Include("Posts")
				.Include("Posts.User")
				.Where(t => t.TaskID == taskID)
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
