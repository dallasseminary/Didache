using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Didache {
	public class UserRelationships {

		/// <summary>
		/// Gets the Users two different users are both connected to 
		/// </summary>
		/// <param name="userID1"></param>
		/// <param name="userID2"></param>
		/// <returns></returns>
		public static List<User> GetCommonRelationshipUsers(int userID1, int userID2) {

			List<User> friends1 = GetApprovedRelationshipUsers(userID1);
			List<User> friends2 = GetApprovedRelationshipUsers(userID2);

			return friends1
					.Where(user => friends2.Select(u2 => u2.UserID).Contains(user.UserID))
					.OrderBy(user => user.LastName)
					.ThenBy(user => user.FirstName)
					.ToList()
					;

		}


		public static List<User> GetApprovedRelationshipUsers() {
			User user = Users.GetLoggedInUser();
			if (user != null) {
				return GetRelationshipUsers(Users.GetLoggedInUser().UserID, RelationshipStatus.Approved);
			} else {
				return new List<User>();
			}
		}

		/// <summary>
		/// Gets a users approved users
		/// </summary>
		/// <param name="userID"></param>
		/// <returns></returns>
		public static List<User> GetApprovedRelationshipUsers(int userID) {
			return GetRelationshipUsers(userID, RelationshipStatus.Approved);
		}

		public static List<User> GetRelationshipUsers(int userID, RelationshipStatus status) {

			DidacheDb db = new DidacheDb();
			
			return db.UserRelationships
				.Include("TargetUser")
				.Where(ur => ur.RequesterUserID == userID && ur.Status == (int)status)
				.Select(ur => ur.TargetUser)
				.ToList();
		}

		public static List<UserRelationship> GetUserRelationships() {

			return GetUserRelationships(Users.GetLoggedInUser().UserID, false);
		}

		public static List<UserRelationship> GetUserRelationships(int userID, bool includeUserData) {

			DidacheDb db = new DidacheDb();

			if (includeUserData) {
				return db.UserRelationships
					.Include("TargetUser")
					.Where(ur => ur.RequesterUserID == userID)
					.ToList();
			} else {
				return db.UserRelationships
					.Where(ur => ur.RequesterUserID == userID)
					.ToList();
			}
		}

		public static List<CarsCourse> GetCommonCourses(int userID1, int userID2) {

			List<CarsCourse> courses1 = GetUsersCourses(userID1);
			List<CarsCourse> courses2 = GetUsersCourses(userID2);

			List<CarsCourse> together = courses1.FindAll(delegate(CarsCourse c1)
            {
                return (null != courses2.Find( delegate(CarsCourse c2) {
					return c1.Session == c2.Session 
							&& c1.Year == c2.Year
							&& c1.Section == c2.Section
							&& c1.CourseCode == c2.CourseCode;
				}));
            });

			return together;

		}

		public static List<CarsCourse> GetUsersCourses(int userID) {

			DidacheDb db = new DidacheDb();

			return db.CarsCourses
				.Where(cu => cu.UserID == userID)
				.ToList()
				.OrderBy(cu => cu.Year)
					.ThenBy(cu => cu.SessionOrder)
					.ThenBy(cu => cu.CourseCode)
				.ToList();
		}
		
		public static List<UserRelationship> GetRelationshipStatuses(List<User> foundUsers) {
			return GetRelationshipStatuses(Users.GetLoggedInUser().UserID, foundUsers);
		}

		public static List<UserRelationship> GetRelationshipStatuses(int userID, List<User> foundUsers) {
			
			// convert the search data into user relationshsips with status == none
			List<UserRelationship> searchRelationships = foundUsers
						.Select(user =>
							new UserRelationship() { 
								RelationshipStatus = RelationshipStatus.None,
								RequesterUserID = userID, 
								TargetUserID = user.UserID, 
								TargetUser = user })
						.ToList();

			// get real status data
			DidacheDb db = new DidacheDb();
			List<int> userIDs = foundUsers.Select(fu => fu.UserID).ToList();
			List<UserRelationship> statuses =
								db.UserRelationships
									.Where(ur => ur.RequesterUserID == userID && userIDs.Contains(ur.TargetUserID))
									.ToList();

			// merge data
			foreach (UserRelationship realRelationship in statuses) {
				searchRelationships.Find(ur => ur.TargetUserID == realRelationship.TargetUserID).RelationshipStatus = realRelationship.RelationshipStatus;
			}

			return searchRelationships;
		}

		public static UserRelationship GetRelationshipStatus(int requesterUserID, int targetUserID) {
			DidacheDb db = new DidacheDb();

			UserRelationship rel = db.UserRelationships
				.SingleOrDefault(ur => ur.RequesterUserID == requesterUserID && ur.TargetUserID == targetUserID);

			if (rel == null) {
				return new UserRelationship() {
					RequesterUserID =requesterUserID,
					TargetUserID = targetUserID,
					RelationshipStatus = RelationshipStatus.None
				};
			} else {
				return rel;
			}
		}

		public static bool IsRelationshipApproved(int requesterUserID, int targetUserID) {
			return GetRelationshipStatus(requesterUserID, targetUserID).RelationshipStatus == RelationshipStatus.Approved;
		}

		
		public static void AddClassmateRequest(int requesterUserID, int targetUserID) {

			DidacheDb db = new DidacheDb();

			UserRelationship rel = db.UserRelationships
				.SingleOrDefault(ur => ur.RequesterUserID == requesterUserID && ur.TargetUserID == targetUserID);

			if (rel == null) {
				
				// status from requester
				rel = new UserRelationship() {
					RequesterUserID = requesterUserID,
					TargetUserID = targetUserID,
					RelationshipStatus = RelationshipStatus.PendingTargetApproval
				};

				db.UserRelationships.Add(rel);

				// status from target
				rel = new UserRelationship() {
					RequesterUserID = targetUserID,
					TargetUserID = requesterUserID,
					RelationshipStatus = RelationshipStatus.PendingRequesterApproval
				};

				db.UserRelationships.Add(rel);

				db.SaveChanges();

				User requesterUser = Users.GetUser(requesterUserID);
				User targetUser = Users.GetUser(targetUserID);

				string classmateRequestBody = Emails.FormatEmail(Didache.Resources.emails.classmates_approvalrequest, null,null,null, requesterUser, targetUser, null, null, null);

				Emails.EnqueueEmail("automated@dts.edu", targetUser.Email, "Classmate Request", classmateRequestBody, false);
			}

		}

		public static void ApproveClassmateRequest(int requesterUserID, int targetUserID) {
			DidacheDb db = new DidacheDb();

			UserRelationship rel1 = db.UserRelationships
				.SingleOrDefault(ur => ur.RequesterUserID == requesterUserID && ur.TargetUserID == targetUserID);
			
			UserRelationship rel2 = db.UserRelationships
				.SingleOrDefault(ur => ur.RequesterUserID == targetUserID && ur.TargetUserID == requesterUserID);

			if (rel1 != null) {
				rel1.Status = (int) RelationshipStatus.Approved;
			}
			if (rel2 != null) {
				rel2.Status = (int) RelationshipStatus.Approved;
			}

			//db.SaveChanges();

			// add user actions
			UserAction ua = new UserAction() {
				SourceUserID = requesterUserID,
				TargetUserID = targetUserID,
				UserActionType = UserActionType.BecomeClassmates,
				ActionDate = DateTime.Now,
				GroupID =0,
				MessageID = 0,
				PostCommentID = 0,
				PostID = 0,
				Text = ""
			};
			db.UserActions.Add(ua);
			UserAction ua2 = new UserAction() {
				SourceUserID = targetUserID,
				TargetUserID = requesterUserID,
				UserActionType = UserActionType.BecomeClassmates,
				ActionDate = DateTime.Now,
				GroupID = 0,
				MessageID = 0,
				PostCommentID = 0,
				PostID = 0,
				Text = ""
			};
			db.UserActions.Add(ua2);
			db.SaveChanges();

		}
	}
}
