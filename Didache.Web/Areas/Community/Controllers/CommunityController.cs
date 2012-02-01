using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Didache.Models;

namespace Didache.Web.Areas.Community.Controllers
{
	[Authorize]
    public class CommunityController : Controller
    {
        //
        // GET: /Profiles/Profiles/

		DidacheDb db = new DidacheDb();

		public ActionResult Index() {

			User user = Users.GetLoggedInUser();
			SearchResultsModel searchResults = new SearchResultsModel();
			searchResults.UserRelationships = null;
			searchResults.Courses = db.CarsCourses
										.Where(cc => cc.UserID == user.UserID)
										.OrderBy(cc => cc.Year)
											.ThenBy(cc => cc.Session)
										.ToList();
			return View();
			
			//return Redirect("/community/search/");
		}


		public ActionResult Search(string s, string c) {

			User user = Users.GetLoggedInUser();
			SearchResultsModel searchResults = new SearchResultsModel();
			searchResults.UserRelationships = null;
			searchResults.Courses = db.CarsCourses
										.Where(cc => cc.UserID == user.UserID)
										.OrderBy(cc => cc.Year)
				//.ThenBy(cc => cc.SessionOrder)
										.ToList()
										.OrderBy(cc => cc.Year)
											.ThenBy(cc => cc.SessionOrder)
										.ToList();




			if (!String.IsNullOrEmpty(s)) {
				//results.FriendUserIDs = UserRelationships.GetApprovedFriends().Select(user => user.UserID).ToList();

				string query = s;

				List<User> foundUsers = db.Users
											.Include("Degrees")
											.Include("Students")
											.Include("Employees")
											.Where(u =>
												u.FirstName == query ||
												u.LastName == query ||
												//u.MiddleName == query ||
												(u.FirstName + " " + u.LastName == query) ||
												//(u.MiddleName + " " + u.LastName == query) ||
												u.AliasFirstName == query ||
												u.AliasLastName == query)
											.ToList()
												.OrderByDescending(u => (u.Degrees.Count > 0 || u.Students.Count > 0 || u.Employees.Count > 0))
											.ToList();

				searchResults.UserRelationships = UserRelationships.GetRelationshipStatuses(foundUsers);

				UserAction ua = new UserAction() {
					SourceUserID = user.UserID,
					TargetUserID = 0,
					UserActionType = UserActionType.SimpleSearch,
					ActionDate = DateTime.Now,
					GroupID = 0,
					MessageID = 0,
					PostCommentID = 0,
					PostID = 0,
					Text = s
				};
				db.UserActions.Add(ua);
				db.SaveChanges();
			
			} else if (!String.IsNullOrEmpty(c)) {
				string[] parts = c.Split(new char[] { ',' });
				int year = Int32.Parse(parts[0]);
				string session = parts[1];
				string courseCode = parts[2];
				string section = parts[3];

				List<int> classmateIds = db.CarsCourses
												.Where(cc => cc.Year == year && cc.Session == session && cc.CourseCode == courseCode && cc.Section == section)
												.Select(cc => cc.UserID)
												.ToList();

				List<User> usersInCourse = db.Users
												.Where(u => classmateIds.Contains(u.UserID)
													/* && u.ScheduleSecurity == (int) UserSecuritySetting.Public */ )
												.OrderBy(u => u.LastName)
													.ThenBy(u => u.FirstName)
												.ToList();

				searchResults.UserRelationships = UserRelationships.GetRelationshipStatuses(usersInCourse);

				UserAction ua = new UserAction() {
					SourceUserID = user.UserID,
					TargetUserID = 0,
					UserActionType = UserActionType.ScheduleSearch,
					ActionDate = DateTime.Now,
					GroupID = 0,
					MessageID = 0,
					PostCommentID = 0,
					PostID = 0,
					Text = c
				};
				db.UserActions.Add(ua);
				db.SaveChanges();
			}

			/*
			if (searchResults.UserRelationships != null) {
				searchResults.UserRelationships.RemoveAll(rel => rel.TargetUserID == user.UserID);
			}
			*/

			return View(searchResults);
		}

		public ActionResult Classmates() {

			List<UserRelationship> allRelationships = UserRelationships.GetUserRelationships(Users.GetLoggedInUser().UserID, true);

			ClassmatesViewModel viewModel = new ClassmatesViewModel();
			viewModel.ApprovedUsers = allRelationships
										.Where(ur => ur.RelationshipStatus == RelationshipStatus.Approved)
										.Select(ur => ur.TargetUser)
										.OrderBy(u => u.LastName)
											.ThenBy(u => u.FirstName)
										.ToList();

			viewModel.PendingUsers = allRelationships
										.Where(ur => ur.RelationshipStatus == RelationshipStatus.PendingRequesterApproval)
										.Select(ur => ur.TargetUser)
										.OrderBy(u => u.LastName)
											.ThenBy(u => u.FirstName)
										.ToList();

			return View(viewModel);
		}

		public ActionResult Display(string name) {

			ProfileViewModel profileViewModel = new ProfileViewModel();
			User displayUser = null;
			int userID = 0;
			User thisUser = Users.GetLoggedInUser();

			if (Int32.TryParse(name, out userID)) {
				displayUser = Users.GetUser(userID);
			} else {
				displayUser = Users.GetUser(name);
			}

		
			profileViewModel.User = displayUser;

			// relationships
			if (displayUser.UserID != thisUser.UserID) {
				profileViewModel.CommonUserRelationships = UserRelationships.GetCommonRelationshipUsers(displayUser.UserID, thisUser.UserID);
				profileViewModel.CommonCarsCourses = UserRelationships.GetCommonCourses(displayUser.UserID, thisUser.UserID);

				profileViewModel.ViewerRelationshipToUser = UserRelationships.GetRelationshipStatus(thisUser.UserID, displayUser.UserID);
			} else {
				
				profileViewModel.CommonUserRelationships = null;
				profileViewModel.CommonCarsCourses = null;
			}

			/*
			profileViewModel.Children = db.FamilyMember
												.Where(fm => fm.UserID == displayUser.UserID && fm.Family == "C")
												.OrderBy(fm => fm.BirthDate)
												.ToList();

			profileViewModel.Spouses = db.CarsRelationships
												.Where(fm => (fm.PrimaryID == displayUser.UserID && fm.Relationship == "HW") ||
														     (fm.SecondaryID == displayUser.UserID && fm.Relationship == "WH"))
												.ToList();
			*/

			UserAction ua = new UserAction() {
				SourceUserID = thisUser.UserID,
				TargetUserID = displayUser.UserID,
				UserActionType = UserActionType.ViewProfile,
				ActionDate = DateTime.Now,
				GroupID = 0,
				MessageID = 0,
				PostCommentID = 0,
				PostID = 0,
				Text = ""
			};
			db.UserActions.Add(ua);
			db.SaveChanges();


			return View(profileViewModel);
		}

		public ActionResult Feed() {

			List<int> includeThese = new List<int>() { 
										(int) UserActionType.BecomeClassmates, 
										(int) UserActionType.SimpleSearch, 
										(int) UserActionType.ScheduleSearch, 
										(int) UserActionType.UpdatePicture, 
										(int) UserActionType.UpdateSettings };

			List<UserAction> actions = db.UserActions
											.Include("SourceUser")
											.Where(ua => includeThese.Contains(ua.UserActionTypeID) )
											.OrderByDescending(ua => ua.ActionDate)
											.Take(100)
											.ToList();

			return View ( actions );

		}


    }
}
