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
			return Redirect("/community/search/");
		}

		public ActionResult Search(string s) {

			List<UserRelationship> searchRelationships = null;

			if (!String.IsNullOrEmpty(s)) {
				//results.FriendUserIDs = UserRelationships.GetApprovedFriends().Select(user => user.UserID).ToList();

				string query = s;

				List<User> foundUsers = db.Users
											.Include("Degrees")
											.Where(u =>
												u.FirstName == query ||
												u.LastName == query ||
												(u.FirstName + " " + u.LastName == query) ||
												u.AliasFirstName == query ||
												u.AliasLastName == query).ToList();

				searchRelationships = UserRelationships.GetRelationshipStatuses(foundUsers);
			}
			
			return View(searchRelationships);
		}

		public ActionResult Classmates() {

			List<UserRelationship> allRelationships = UserRelationships.GetUserRelationships(Users.GetLoggedInUser().UserID, true);

			ClassmatesViewModel viewModel = new ClassmatesViewModel();
			viewModel.ApprovedUsers = allRelationships
										.Where(ur => ur.RelationshipStatus == RelationshipStatus.Approved)
										.Select(ur => ur.TargetUser)
										.ToList();

			viewModel.PendingUsers = allRelationships
										.Where(ur => ur.RelationshipStatus == RelationshipStatus.PendingRequesterApproval)
										.Select(ur => ur.TargetUser)
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




			return View(profileViewModel);
		}

		

    }
}
