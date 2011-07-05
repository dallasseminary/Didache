using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Didache;

namespace Didache.Web.Areas.Students.Controllers
{
    public class DiscussionController : Controller
    {
        //
        // GET: /Courses/Discussion/
		[Authorize]
        public ActionResult Index(string slug)
        {
			// show forums
			Course course = Courses.GetCourseBySlug(slug);
			List<Forum> forums = Forums.GetCourseForums(course.CourseID);

			if (forums.Count == 1)
				return RedirectToAction("Forum", new { slug = slug, id = forums[0].ForumID });

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
		public ActionResult CreatePost(string slug, int id, FormCollection collection) {
			// add post to post id

			return View();
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

			DidacheDb db = new DidacheDb();
			db.Posts.Add(post);
			db.SaveChanges();

			return RedirectToAction("Thread", new { slug = slug, id = id });
		}

    }
}
