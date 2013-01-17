using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Text.RegularExpressions;
using Didache;

namespace Didache.Web.Controllers
{
    public class ApiController : Controller
    {
		DidacheDb db = new DidacheDb();
		EntityObjectSerializer serializer = new EntityObjectSerializer();

		[Authorize]
		public ActionResult JoinDiscussionGroup(int discussionGroupID) {
			var group = db.DiscussionGroups.Find(discussionGroupID);
			User user = Users.GetLoggedInUser();

			if (group != null && user != null) {
				DiscussionGroupMember member = new DiscussionGroupMember();
				member.GroupID = discussionGroupID;
				member.UserID = user.UserID;
				member.GroupMembershipStatus = GroupMembershipStatus.Member;
				member.IsSubscribed = true;
				db.DiscussionGroupMembers.Add(member);

				UserAction ua = new UserAction();
				ua.SourceUserID = user.UserID;
				ua.ActionDate = DateTime.Now;
				ua.UserActionType = UserActionType.JoinDiscussionGroup;
				ua.DiscussionGroupID = discussionGroupID;
				ua.Text = "Joined " + group.Name;
				db.UserActions.Add(ua);
				
				db.SaveChanges();
			}

			return Json(new {success= true});
		}

		[Authorize]
		public ActionResult AcceptDiscussionGroupInvite(int discussionGroupID) {
			var group = db.DiscussionGroups.Find(discussionGroupID);
			User user = Users.GetLoggedInUser();
			DiscussionGroupMember member = db.DiscussionGroupMembers.SingleOrDefault(gm => gm.UserID == user.UserID && gm.GroupID == discussionGroupID);


			if (member != null) {
				
				member.GroupMembershipStatus = GroupMembershipStatus.Member;
				member.IsSubscribed = true;

				UserAction ua = new UserAction();
				ua.SourceUserID = user.UserID;
				ua.ActionDate = DateTime.Now;
				ua.UserActionType = UserActionType.JoinDiscussionGroup;
				ua.DiscussionGroupID = discussionGroupID;
				ua.Text = "Joined " + group.Name;
				db.UserActions.Add(ua);

				db.SaveChanges();
			}

			return Json(new { success = true });
		}

		[Authorize]
		public ActionResult ToggleDiscussionGroupSubscription(int discussionGroupID, bool isSubscribed) {
			var group = db.DiscussionGroups.Find(discussionGroupID);
			User user = Users.GetLoggedInUser();

			if (group != null && user != null) {
				DiscussionGroupMember member = db.DiscussionGroupMembers.SingleOrDefault(gm => gm.UserID == user.UserID && gm.GroupID == discussionGroupID);

				if (member != null) {
					member.IsSubscribed = isSubscribed;
					db.SaveChanges();
				}
			}

			return Json(new { success = true });
		}

		[Authorize]
		public ActionResult LeaveDiscussionGroup(int discussionGroupID) {
			var group = db.DiscussionGroups.Find(discussionGroupID);
			User user = Users.GetLoggedInUser();

			if (group != null && user != null) {
				DiscussionGroupMember member = db.DiscussionGroupMembers.SingleOrDefault(gm => gm.UserID == user.UserID && gm.GroupID == discussionGroupID);

				if (member != null) {
					db.DiscussionGroupMembers.Remove(member);


					UserAction ua = new UserAction();
					ua.SourceUserID = user.UserID;
					ua.ActionDate = DateTime.Now;
					ua.UserActionType = UserActionType.LeaveDiscussionGroup;
					ua.DiscussionGroupID = discussionGroupID;
					ua.Text = "Left " + group.Name;
					db.UserActions.Add(ua);

					db.SaveChanges();
				}
			}

			return Json(new { success = true });
		}


		[Authorize]
		public ActionResult DeleteUserPostComment(int commentID) {

			var comment = db.UserPostComments.Find(commentID);
			User user = Users.GetLoggedInUser();

			if (comment.UserID == user.UserID || Users.IsAdministratorFacultyOrFacilitator()) {
				comment.IsDeleted = true;
				comment.DeletedByUserID = user.UserID;
				comment.DeletedDate = DateTime.Now;
				db.SaveChanges();
			}

			return Json(new { success = true });
		}

		[Authorize]
		public ActionResult CreateUserPostComment(string text, int postID) {

			User user = Users.GetLoggedInUser();
			UserPostComment newUserPostComment = null;
			try {
				newUserPostComment = new UserPostComment() {
					UserID = user.UserID,
					PostID = postID,
					CommentDate = DateTime.Now,
					IsDeleted = false,
					Text = text,
					TextFormatted = Markdown.Transform(text)
					//TextFormatted = text.Replace("\n", "").Replace("\r", "<br />")
				};

				db.UserPostComments.Add(newUserPostComment);
				db.SaveChanges();
			} catch (Exception e) {


				return Json(new { success = false, message = e.ToString() });
			}


			// get the data back out
			newUserPostComment = db.UserPostComments
						.Include("User")
						.Single(uc => uc.PostCommentID == newUserPostComment.PostCommentID);


			// Send emails
			var post = db.UserPosts
						.Include("User")
						.Include("PostComments.User")
						.SingleOrDefault(p => p.PostID == newUserPostComment.PostID);

			if (post != null) {
				// find peeps who want to be notified
				List<User> usersToBeNotified = post.PostComments
														.Select(pc => pc.User)
														.Where(u => u.NotifyUserPostCommentReplies)
														.ToList();

				// check if starter wants a notification
				if (post.User.NotifyUserPostReplies) {
					usersToBeNotified.Add(post.User);
				}

				// remove dups
				usersToBeNotified = usersToBeNotified.Distinct().ToList();

				// remove commenter
				usersToBeNotified.RemoveAll(u => u.UserID == user.UserID);

				foreach (User userToBeNotified in usersToBeNotified) {
					Emails.EnqueueEmail(
						"automated@dts.edu",
						userToBeNotified.Email,
						"New Comment",
						Emails.FormatEmail(Didache.Resources.emails.userpost_comment, null, null, null, user, userToBeNotified, null, null, null, post, newUserPostComment),
						false);
				}
			}

			// user action
			db.UserActions.Add(new UserAction() {
				SourceUserID = user.UserID,
				ActionDate = DateTime.Now,
				UserActionType = UserActionType.MakeNewComment,
				PostID = newUserPostComment.PostID,
				Text = newUserPostComment.TextFormatted,
				GroupID = 0,
				MessageID = 0,
				PostCommentID = newUserPostComment.PostCommentID,
				TargetUserID = post.UserID
			});
			db.SaveChanges();


			return Json(new {
					PostCommentID = newUserPostComment.PostCommentID,
					CommentDate = newUserPostComment.CommentDate,
					CommentDateFormatted = newUserPostComment.CommentDate.ToString("MMM d, hh:mm tt"),
					//Text = Regex.Replace(c.TextFormatted, "</?(div|font|span|style|script|img).*?>", ""),
					Text = newUserPostComment.TextFormatted,
					User = new {
						SecureFormattedName = newUserPostComment.User.SecureFormattedName,
						ProfileDisplayUrl = newUserPostComment.User.ProfileDisplayUrl,
						ProfileImageUrl = newUserPostComment.User.ProfileImageUrl
					}
				
			});
		}


		[Authorize]
		public ActionResult DeleteUserPost(int postID) {

			var post = db.UserPosts.Find(postID);
			User user = Users.GetLoggedInUser();

			if (post.UserID == user.UserID || Users.IsAdministratorFacultyOrFacilitator()) {
				post.IsDeleted = true;
				post.DeletedByUserID = user.UserID;
				post.DeletedDate = DateTime.Now;
				db.SaveChanges();
			}

			return Json(new { success = true });
		}

		[Authorize]
		public ActionResult CreateUserPost(string text, string postType, bool notifyCourse, int? id) {

			User user = Users.GetLoggedInUser();
			UserPostType userPostType = (UserPostType)Enum.Parse(typeof(UserPostType), postType);

			int courseID = (userPostType == UserPostType.Course ? id.Value : 0);
			int groupID = (userPostType == UserPostType.CourseGroup ? id.Value : 0);
			int discussionGroupID = (userPostType == UserPostType.DiscussionGroup ? id.Value : 0);
			int userGroupID = 0;

			if (groupID > 0) {
				try {
					courseID = db.CourseUserGroups.Find(groupID).CourseID;
				} catch { }
			}

			UserPost p = new UserPost() {
				UserID = user.UserID,
				PostDate = DateTime.Now,
				IsDeleted = false,
				IsPinned = false,
				CourseGroupID = groupID,
				UserGroupID = userGroupID,

				CourseID = courseID,
				DiscussionGroupID = discussionGroupID,
				
				FileID = 0,
				UserPostType = userPostType,
				Text = text,
				TextFormatted = Markdown.Transform(text)
			};

			db.UserPosts.Add(p);
			db.SaveChanges();


			// get the data back out
			p = db.UserPosts
						.Include("User")
						.Include("PostComments.User")
						.Include("Course")
						.Include("DiscussionGroup")
						.Single(up => up.PostID == p.PostID);

			// user action
			db.UserActions.Add(new UserAction() {
				SourceUserID = user.UserID,
				ActionDate = DateTime.Now,
				UserActionType = UserActionType.MakeNewPost,
				PostID = p.PostID,
				Text = p.TextFormatted,
				GroupID = 0,
				MessageID = 0,
				PostCommentID = 0,
				TargetUserID = 0,
				DiscussionGroupID = discussionGroupID
			});
			db.SaveChanges();


			// send emails
			if (notifyCourse && (userPostType == UserPostType.CourseGroup || userPostType == UserPostType.Course)) {

				List<User> usersToBeNotified = null;
				string title = "";

				if (userPostType == UserPostType.Course) {
					usersToBeNotified = Courses.GetUsersInCourse(courseID)
											.Select(cu => cu.User)
											.Distinct()
											.ToList();

					Course course = db.Courses.Find(courseID);

					title = course.CourseCode + course.Section + " Announcement";

				} else if (userPostType == UserPostType.CourseGroup) {
					usersToBeNotified = Courses.GetUsersInCourse(courseID)
											.Where(cu => cu.GroupID == groupID)
											.Select(cu => cu.User)
											.Distinct()
											.ToList();

					Course course = db.Courses.Find(courseID);
					CourseUserGroup group = db.CourseUserGroups.Find(groupID);

					title = course.CourseCode + course.Section + " " + group.Name + " Announcement";
				}
				//Course course


				if (usersToBeNotified != null) {
					foreach (User userToBeNotified in usersToBeNotified) {
						Emails.EnqueueEmail(
							"automated@dts.edu",
							userToBeNotified.Email,
							title,
							Emails.FormatEmail(Didache.Resources.emails.userpost_announcement, null, null, null, user, userToBeNotified, null, null, null, p, null),
							false);
					}
				}


			}

			// send to group subscribers
			if (userPostType == UserPostType.DiscussionGroup) {

				DiscussionGroup group = db.DiscussionGroups.Find(discussionGroupID);

				// get subscribers
				List<User> subscribers = db.DiscussionGroupMembers
												.Include("User")
												.Where(m => m.GroupID == discussionGroupID)
												.Select(m => m.User)
												.ToList();

				foreach (User subscriber in subscribers) {
					Emails.EnqueueEmail(
						"automated@dts.edu",
						subscriber.Email,
						group.Name  + " Message",
						Emails.FormatEmail(Didache.Resources.emails.discussiongroup_message, null, null, null, user, subscriber, null, null, null, p, null, group),
						false);
				}



			}


			return Json(new {
				PostID = p.PostID,
				PostDate = p.PostDate,
				PostDateFormatted = p.PostDate.ToString("MMM d"),
				//Text = Regex.Replace(p.TextFormatted, "</?(div|font|span|style|script|img).*?>", ""),
				Text = p.TextFormatted,
				UserPostTypeFormatted = p.UserPostTypeFormatted,
				UserPostTypeUrl = p.UserPostTypeUrl,
				User = new {
					SecureFormattedName = p.User.SecureFormattedName,
					ProfileDisplayUrl = p.User.ProfileDisplayUrl,
					ProfileImageUrl = p.User.ProfileImageUrl
				},
				PostComments = p.PostComments.Select(c => new {
					PostCommentID = c.PostCommentID,
					CommentDate = c.CommentDate,
					CommentDateFormatted = c.CommentDate.ToString("MMM d, hh:mm tt"),
					//Text = Regex.Replace(c.TextFormatted, "</?(div|font|span|style|script|img).*?>", ""),
					Text = p.TextFormatted,
					User = new {
						SecureFormattedName = c.User.SecureFormattedName,
						ProfileDisplayUrl = c.User.ProfileDisplayUrl,
						ProfileImageUrl = c.User.ProfileImageUrl
					}
				})
			});
		}

		[Authorize]
		public ActionResult GetUserPosts(string type, int id, int pageNumber=1, int pageSize=20) {


			DateTime now = DateTime.Now;
			User user = Users.GetLoggedInUser();


			
			PostListType postListType = (PostListType) Enum.Parse(typeof(PostListType), type);

			var userPosts = db.UserPosts
						.Include("User")
						.Include("PostComments.User")
						.AsQueryable();
							

			if (postListType == PostListType.Public) {

				// all possible posts for user
				List<int> classmateIds = UserRelationships.GetApprovedRelationshipUsers().Select(ur => ur.UserID).ToList();
				classmateIds.Add(user.UserID);
				List<int> groupIds = Courses.GetUsersGroups().Select(cug => cug.GroupID).ToList();
				List<int> courseIds = Courses.GetUsersCourses().Select(c => c.CourseID).ToList();

				userPosts = userPosts
						.Where(up => !up.IsDeleted && up.PostID > 100000 && up.PostDate < now &&
									(
									up.UserPostTypeID == (int)UserPostType.Public
									||
									up.UserPostTypeID == (int)UserPostType.Classmates && classmateIds.Contains(up.UserID)
									||
									up.UserPostTypeID == (int)UserPostType.CourseGroup && groupIds.Contains(up.CourseGroupID)
									||
									up.UserPostTypeID == (int)UserPostType.Course && courseIds.Contains(up.CourseID)
									)
								);


			} else if (postListType == PostListType.Course) {

				// subset for course
				//List<int> classmateIds = UserRelationships.GetApprovedRelationshipUsers().Select(ur => ur.UserID).ToList();
				//List<int> groupIds = Courses.GetUsersGroups().Select(cug => cug.GroupID).ToList();
				//List<int> courseIds = Courses.GetUsersCourses().Select(c => c.CourseID).ToList();
				Course course = Courses.GetCourse(id);

				List<int> groupIds = Courses.GetUsersGroups()
												.Where(cug => cug.CourseID == course.CourseID)
												.Select(cug => cug.GroupID)
												.ToList();

				userPosts = userPosts
						.Where(up => !up.IsDeleted && up.PostDate < now &&
									(
									up.UserPostTypeID == (int)UserPostType.Course && up.CourseID == course.CourseID
									||
									(up.UserPostTypeID == (int)UserPostType.CourseGroup && groupIds.Contains(up.CourseGroupID))
									)
								);
			} else if (postListType == PostListType.Single) {
				userPosts = userPosts
						.Where(up => up.PostID == id);

			} else if (postListType == PostListType.Group) {
				userPosts = userPosts
						.Where(up => up.DiscussionGroupID == id);
			}

			// pull data
			userPosts = userPosts
							.OrderByDescending(up => up.PostDate)
							.Skip((pageNumber - 1) * pageSize)
							.Take(pageSize);


			//return Json(serializer.Serialize(userPosts), JsonRequestBehavior.AllowGet);

			// convert to JSON friendly data
			var postJsonData = userPosts.ToList().Select(p => new {
				PostID = p.PostID,
				PostDate = p.PostDate,
				PostDateFormatted = (p.PostDate.Year == DateTime.Now.Year) ? p.PostDate.ToString("MMM d") : p.PostDate.ToString("MMM d, yyyy"),
				Text = Regex.Replace(p.TextFormatted, "</?(div|font|span|style|script|img).*?>", ""),
				UserPostTypeFormatted = p.UserPostTypeFormatted,
				UserPostTypeUrl = p.UserPostTypeUrl,
				User = new {
					SecureFormattedName = p.User.SecureFormattedName,
					ProfileDisplayUrl = p.User.ProfileDisplayUrl,
					ProfileImageUrl = p.User.ProfileImageUrl
				},
				PostComments = p.PostComments.Where(c => !c.IsDeleted).Select(c => new {
					PostCommentID = c.PostCommentID,
					CommentDate = c.CommentDate,
					CommentDateFormatted = c.CommentDate.ToString("MMM d, hh:mm tt"),
					Text = Regex.Replace(c.TextFormatted, "</?(div|font|span|style|script|img).*?>", ""),
					User = new {
						SecureFormattedName = c.User.SecureFormattedName,
						ProfileDisplayUrl = c.User.ProfileDisplayUrl,
						ProfileImageUrl =c.User.ProfileImageUrl
					}
				}).ToList()
			});

			return Json(postJsonData, JsonRequestBehavior.AllowGet);
		}


		public ActionResult AddClassmate(int requesterUserID, int targetUserID) {

			UserRelationships.AddClassmateRequest(requesterUserID, targetUserID);

			return Json(new { success = true }, JsonRequestBehavior.AllowGet);
		}

		public ActionResult ApproveClassmate(int requesterUserID, int targetUserID) {

			UserRelationships.ApproveClassmateRequest(requesterUserID, targetUserID);

			return Json(new { success = true }, JsonRequestBehavior.AllowGet);
		}
		
		public ActionResult GetCourse(int id) {
            return Json(serializer.Serialize(db.Courses.Find(id)), JsonRequestBehavior.AllowGet);
        }

		public ActionResult GetUnit(int id) {
			return Json(serializer.Serialize(db.Units.Find(id)), JsonRequestBehavior.AllowGet);
		}


		public ActionResult GetTask(int id) {
			return Json(serializer.Serialize(db.Tasks.Find(id)), JsonRequestBehavior.AllowGet);
		}

	
		public ActionResult GetCourseUnits(int id) {
			return Json(serializer.Serialize(Didache.Courses.GetCourseUnitsWithTasks(id)), JsonRequestBehavior.AllowGet);
		}

		public ActionResult GetCourseFileGroups(int id) {
			return Json(serializer.Serialize(Didache.CourseFiles.GetCourseFileGroups(id)), JsonRequestBehavior.AllowGet);
		}

		public ActionResult GetCourseFile(int id) {
			return Json(serializer.Serialize(db.CourseFiles.Find(id)), JsonRequestBehavior.AllowGet);
		}

		public ActionResult GetCourseFileGroup(int id) {
			return Json(serializer.Serialize(db.CourseFileGroups.Find(id)), JsonRequestBehavior.AllowGet);
		}

		[Authorize]
		public ActionResult GetCourseUserGroups(int id) {			

			return Json(serializer.Serialize(Didache.Courses.GetCourseUserGroups(id)), JsonRequestBehavior.AllowGet);
		}

		[Authorize]
		public ActionResult GetCourseUserGroup(int id) {
			CourseUserGroup g = db.CourseUserGroups.Find(id);
			g.Students = new List<CourseUser>();

			return Json(serializer.Serialize(g), JsonRequestBehavior.AllowGet);
		}

		[Authorize]
		public ActionResult GetCourseUsers(int id) {

			//return Json(serializer.Serialize(Didache.Courses.GetUsersInCourse(id)), JsonRequestBehavior.AllowGet);

			return Json(Didache.Courses.GetUsersInCourse(id).Select(u => new {
								UserID = u.UserID, 
								CourseID = u.CourseID,
								GroupID = u.GroupID,
								RoleID = u.RoleID,
								User = new {
									UserID = u.User.UserID,
									FirstName = u.User.SecureFirstName,
									LastName = u.User.SecureLastName,
									SecureFormattedName = u.User.FormattedName,
									SecureFormattedNameLastFirst = u.User.FormattedNameLastFirst,
									Email = u.User.Email
								}
					}), JsonRequestBehavior.AllowGet);
		}


		[Authorize]
		public ActionResult FindUsers(string query) {
			
			var dbQuery = db.Users.AsQueryable();

			foreach (String part in query.Split(new char[] {' '})) {
				if (String.IsNullOrEmpty(part))
					continue;

				int id = 0;
				if (Int32.TryParse(part, out id)) {
					dbQuery = dbQuery.Where(u => u.UserID == id);
				} else {
					dbQuery = dbQuery.Where(u => 
							u.LastName == part ||
							u.FirstName == part || 
							u.AliasFirstName == part ||
							u.AliasLastName == part
							);
				}
			}

			List<User> users = dbQuery.ToList();

			return Json(serializer.Serialize(users.Select(u => new { UserID = u.UserID, FormattedName = u.FormattedName, FormattedNameLastFirst = u.FormattedNameLastFirst, FirstName = u.FirstName, LastName = u.LastName })), JsonRequestBehavior.AllowGet);
		}


		[Authorize]
		public ActionResult FindInvitees(string query, int discussionGroupID) {

			List<User> users = null;
			


			// first query
			users = db.Users.Where(u =>
								(u.FirstName + " " + u.LastName == query) ||
								(u.AliasFirstName + " " + u.LastName == query) ||
								(u.AliasFirstName + " " + u.AliasLastName == query) ||
								(u.MiddleName + " " + u.LastName == query) ||
								(u.LastName + ", " + u.FirstName == query)
								).ToList();

			if (users.Count == 0) {
				var dbQuery = db.Users.AsQueryable();

				foreach (String part in query.Split(new char[] { ' ' })) {
					if (String.IsNullOrEmpty(part))
						continue;

					int id = 0;
					if (Int32.TryParse(part, out id)) {
						dbQuery = dbQuery.Where(u => u.UserID == id);
					} else {
						dbQuery = dbQuery.Where(u =>
								u.LastName == part ||
								u.FirstName == part ||
								u.AliasFirstName == part ||
								u.AliasLastName == part
								);
					}
				}

				users = dbQuery.ToList();
			}

			List<DiscussionGroupMember> membershipStatuses = db.DiscussionGroupMembers.Where(dgm => dgm.GroupID == discussionGroupID).ToList();

			return Json(users.Select(u => new { 
								userid = u.UserID, 
								secureformattedname = u.SecureFormattedName, 
								displayprofileurl = u.ProfileDisplayUrl, 
								profileimageurl = u.GetProfileImageUrl(30,30),
								isMember = membershipStatuses.Count(dgm => dgm.UserID == u.UserID && dgm.GroupMembershipStatus != GroupMembershipStatus.Invited) > 0,
								isInvited = membershipStatuses.Count(dgm => dgm.UserID == u.UserID && dgm.GroupMembershipStatus == GroupMembershipStatus.Invited) > 0
						}), JsonRequestBehavior.AllowGet);
		}

		[Authorize]
		public ActionResult InviteUserToDiscussionGroup(int userID, int discussionGroupID) {

			DiscussionGroupMember member = db.DiscussionGroupMembers.SingleOrDefault(dgm => dgm.GroupID == discussionGroupID && dgm.UserID == userID);

			if (member == null) {

				User inviter = Users.GetLoggedInUser();
				User invitee = Users.GetUser(userID);
				DiscussionGroup group = db.DiscussionGroups.Find(discussionGroupID);

				member = new DiscussionGroupMember();
				member.UserID = userID;
				member.GroupID = discussionGroupID;
				member.GroupMembershipStatus = GroupMembershipStatus.Invited;
				member.IsSubscribed = true;

				db.DiscussionGroupMembers.Add(member);
				db.SaveChanges();

				Emails.EnqueueEmail(
						"automated@dts.edu",
						invitee.Email,
						inviter.SecureFormattedName + " invited you to join " + group.Name,
						Emails.FormatEmail(Didache.Resources.emails.discussiongroup_invitation, null, null, null, inviter, invitee, null, null, null, null, null, group),
						false);

			}


			return Json(new { success = true} , JsonRequestBehavior.AllowGet);
		}

    }
}
