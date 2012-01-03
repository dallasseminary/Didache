using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Didache;

namespace Didache.Web.Areas.Students.Controllers
{
	[Authorize]
    public class DiscussionController : Controller
    {
		DidacheDb db = new DidacheDb();

        //
        // GET: /Courses/Discussion/
		[Authorize]
        public ActionResult Index(string slug)
        {
			// show forums
			Course course = Courses.GetCourseBySlug(slug);
			List<Forum> forums = Forums.GetCourseForums(course.CourseID);

			// auto create!
			if (forums.Count == 0) {
				Forum forum = new Forum() {
					Name = course.CourseCode + course.Section + " Open Discussion",
					CourseID = course.CourseID,
					Description = "Open discussion for course content. This is NOT for interactions",
					SortOrder = 9999,
					TotalPosts = 0,
					TotalThreads = 0
				};
				db.Forums.Add(forum);
				db.SaveChanges();

				forums.Add(forum);
			}
				
			if (forums.Count == 1) {
				return RedirectToAction("Forum", new { slug = slug, id = forums[0].ForumID });
			}

			ViewBag.Course = course;

			return View(forums);
        }

		[Authorize]
		public ActionResult Forum(string slug,int id) {
			
			// show threads in form			
			//List<Thread> threads = Forums.GetThreads(id);
			Forum forum = Forums.GetForum(id, true);

			return View(forum);
		}

		[Authorize]
		public ActionResult Thread(string slug, int id) {
			
			// show posts in thread
			//List<Post> posts = Forums.GetPosts(id);
			Thread thread = Forums.GetThread(id, true);

			return View(thread);
		}

		[Authorize]
		public ActionResult CreatePost(string slug, int id) {
			// add thread/post to forum id

			return View();
		}

		[Authorize]
		[HttpPost]
		public ActionResult CreatePost(string slug, Thread thread, FormCollection collection) {
			// add post to post id

			User profile = Users.GetLoggedInUser();
			DidacheDb db = new DidacheDb();

			// make thread
			thread.UserID = profile.UserID;
			thread.TotalReplies = 0;
			thread.TotalViews = 0;
			thread.ThreadDate = DateTime.Now;
			thread.UserName = profile.Username;
			thread.LastPostUserName = profile.Username;
			thread.LastPostUserID = profile.UserID;
			thread.LastPostID = 0;
			thread.LastPostDate = DateTime.Now;
			thread.LastPostSubject = collection["Subject"];
			

			db.Threads.Add(thread);
			db.SaveChanges();

			// add post to thread
			Post post = new Post();
			post.ThreadID = thread.ThreadID;
			post.ForumID = Int32.Parse(collection["ForumID"]);
			post.PostDate = DateTime.Now;
			post.UserID = profile.UserID;
			post.UserName = profile.FullName;
			post.ReplyToPostID = 0;
			post.Subject = collection["Subject"];
			post.PostContent = collection["PostContent"];
			post.PostContentFormatted = Forums.FormatPost(post.PostContent);

			
			db.Posts.Add(post);
			db.SaveChanges();



			return RedirectToAction("Thread", new { slug = slug, id = thread.ThreadID });
		}

		[Authorize]
		[HttpPost]
		public ActionResult Reply(string slug, int id, FormCollection collection) {
			// add post to thread id

			User profile = Users.GetLoggedInUser();

			Post post = new Post();
			post.ThreadID = id;
			post.ForumID = Int32.Parse(collection["ForumID"]);
			post.PostDate = DateTime.Now;
			post.UserID = profile.UserID;
			post.UserName = profile.FullName;
			post.ReplyToPostID = Int32.Parse(collection["ReplyToPostID"]);
			post.Subject = collection["Subject"];
			post.PostContent = collection["PostContent"];
			post.PostContentFormatted = Forums.FormatPost(post.PostContent);

			db.Posts.Add(post);
			db.SaveChanges();

			Thread thread = db.Threads.Include("Posts").FirstOrDefault(t => t.ThreadID == id);

			thread.LastPostDate = DateTime.Now;
			thread.LastPostID = post.PostID;
			thread.LastPostUserID = profile.UserID;
			thread.TotalReplies = thread.Posts.Count - 1;
			thread.LastPostSubject = post.Subject;

			db.SaveChanges();

			return RedirectToAction("Thread", new { slug = slug, id = id });
		}

    }
}
