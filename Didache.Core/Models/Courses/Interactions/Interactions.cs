using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Didache {
	public class Interactions {
		
		public static List<InteractionThread> GetInteractionThreads(int taskID) {
            return new List<InteractionThread>();
            
            return new DidacheDb()
				.InteractionThreads
				.Include("Posts")
				.Include("Posts.User")
				.Where(t => t.TaskID == taskID)
				.OrderByDescending(t => t.ThreadDate)
				.ToList();
		}

		public static List<InteractionPost> GetInteractionPosts(int threadID) {

            return new List<InteractionPost>();
            
            return new DidacheDb()
				.InteractionPosts
				.Where(p => p.ThreadID == threadID)
				.OrderByDescending(p => p.PostDate)
				.ToList();

			// reorder with nesting?
		}
	}
}
