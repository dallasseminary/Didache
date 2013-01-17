using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Didache {
	public class Interactions {

		public static int GetActiveReplyCount(int postID) {
			DidacheDb db = new DidacheDb();

			InteractionPost post = db.InteractionPosts.SingleOrDefault(p=>p.PostID == postID);

			if (post == null) {
				return 0;
			} else {
				return db.InteractionPosts.Count(p => p.ThreadID == post.ThreadID && p.IsDeleted == false);
			}

		}
		
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
            Task task = db.Tasks
							.Include("Course.CourseUsers")
							.Include("Course.CourseUserGroups")
							.SingleOrDefault(t => t.TaskID == taskID);
			Course course = task.Course;

			/// get all users associations
            CourseUser courseUserAsStudent = course.CourseUsers
											.SingleOrDefault(u => u.UserID == user.UserID && 
																  u.RoleID == (int) CourseUserRole.Student);
			CourseUser courseUserAsFaculty = course.CourseUsers
											.SingleOrDefault(u => u.UserID == user.UserID &&
																  u.RoleID == (int)CourseUserRole.Faculty);
			CourseUser courseUserAsFaciliator = course.CourseUsers
											.SingleOrDefault(u => u.UserID == user.UserID &&
																  u.RoleID == (int)CourseUserRole.Faciliator);

			// find out which group(s) this person should be associated with
			// facilitators can be in many groups
            List<int> courseUserIDs = new List<int>();
			List<int> usersGroupIDs = new List<int>();
			List<CourseUserGroup> usersGroups = new List<CourseUserGroup>();

			// faculty members should see all posts from all groups
			if (courseUserAsFaculty != null) {
				//usersGroupIDs = -1;
				//usersGroup = null;
				usersGroups = course.CourseUserGroups.ToList();
				usersGroupIDs = usersGroups.Select(cup => cup.GroupID).ToList();

			// facilitators should see only their group
			// BUT, for now we have to find that groupID in the groups
			} else if (courseUserAsFaciliator != null) {
				usersGroups = course.CourseUserGroups.Where(cup => cup.FacilitatorUserID == user.UserID).ToList();
				usersGroupIDs = usersGroups.Select(cup => cup.GroupID).ToList();

			// students should just see their group
			} else if (courseUserAsStudent != null) {
				usersGroupIDs.Add(courseUserAsStudent.GroupID);
				usersGroups.AddRange(course.CourseUserGroups.Where(cup => usersGroupIDs.Contains(cup.GroupID)).ToList());

			} else {
				// should be an adminstrator
			}


			// FIND group members
			if (usersGroupIDs.Count > 0) {
				// just members in the group
				courseUserIDs = course.CourseUsers.Where(cu =>
															usersGroupIDs.Contains(cu.GroupID) && 
															cu.RoleID == (int) CourseUserRole.Student).Select(cu => cu.UserID).ToList();
					
				// include all facilitators (nasty)
				courseUserIDs.AddRange(usersGroups.Select(cup => cup.FacilitatorUserID).ToList());

				// always include faculty?
				courseUserIDs.AddRange(course.CourseUsers.Where(cu => cu.RoleID == (int)CourseUserRole.Faculty).Select(cu => cu.UserID).ToList());

				
			} else {
				// faculty should get all members
				courseUserIDs = course.CourseUsers.Select(cu => cu.UserID).ToList();
			}


			// Finally get some data
			List<InteractionThread> threads = db
				.InteractionThreads
				.Include("Posts")
				.Include("Posts.User")
				.Where(t => t.TaskID == taskID && courseUserIDs.Contains(t.UserID))
				.OrderByDescending(t => t.ThreadDate)
				.ToList();

			// SORTING

			// pull facilitators to the top
			List<InteractionThread> facilitatorThreads = threads.FindAll(t => course.CourseUsers.Where(cu => cu.CourseUserRole == CourseUserRole.Faciliator).Select(cu => cu.UserID).Contains(t.UserID));
			foreach (InteractionThread thread in facilitatorThreads) {
				threads.Remove(thread);
			}
			threads.InsertRange(0, facilitatorThreads);

			// pull faculty to the very top
			List<InteractionThread> facultyThreads = threads.FindAll(t => course.CourseUsers.Where(cu => cu.CourseUserRole == CourseUserRole.Faculty).Select(cu => cu.UserID).Contains(t.UserID));
			foreach (InteractionThread thread in facultyThreads) {
				threads.Remove(thread);
			}
			threads.InsertRange(0, facultyThreads);

			// sorting the posts in memory!! Yeah, save DB time so we can cripple the web server
			foreach (InteractionThread thread in threads) {
				var posts = thread.Posts.ToList();

				posts.Sort(delegate(InteractionPost a, InteractionPost b) {
					return a.PostDate.CompareTo(b.PostDate);
				});

				thread.Posts = posts;
			}

			// sort the threads by recent posts
			threads.Sort(delegate(InteractionThread a, InteractionThread b) {
				return (b.Posts.Count > 0 && a.Posts.Count > 0) ? b.Posts.OrderByDescending(p => p.PostDate).FirstOrDefault().PostDate.CompareTo(a.Posts.OrderByDescending(p => p.PostDate).FirstOrDefault().PostDate) : 0;
			});

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
			return Markdown.Transform(input);


			return "<p>" + input.Replace("\r", "").Replace("\n\n", "</p><p>").Replace("\n", "<br>") + "</p>";
		}
	}
}
