using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Didache.Models;

/*
- Index
 x center: all groups
 x right: your groups
  
- Group
 x center: posts 
 x right: people on right
 * join/edit
 
- Create/Edit
 * center: details
 * right: approvals?
  
*/
namespace Didache.Web.Areas.Community.Controllers
{
	[Authorize]
    public class GroupController : Controller
    {
        //
        // GET: /Community/Group/
		DidacheDb db = new DidacheDb();

		public ActionResult Index()
        {
			DiscussionGroupListViewModel viewModel = new DiscussionGroupListViewModel();


			viewModel.AllGroups = db.DiscussionGroups
										.Include("GroupMembers")
										.Where(g => g.IsApproved && (g.GroupTypeID == (int)DiscussionGroupType.Open || g.GroupTypeID == (int)DiscussionGroupType.InviteOnly))
										.OrderBy(g => g.Name)
										.ToList();

			User user = Users.GetLoggedInUser();
			viewModel.UsersGroups = db.DiscussionGroups
										.Where(g => g.GroupMembers.Any(gm => gm.UserID == user.UserID))
										.OrderBy(g => g.Name)
										.ToList();

			return View(viewModel);
        }

		public ActionResult Edit(int id = 0) {

			DiscussionGroup group = null;
			User user = Users.GetLoggedInUser();

			if (id > 0) {
				group = db.DiscussionGroups.Find(id);

				DiscussionGroupMember member = db.DiscussionGroupMembers.SingleOrDefault(dgm => dgm.UserID == user.UserID && dgm.GroupID == id);
				if (member == null || (member.GroupMembershipStatus != GroupMembershipStatus.Administrator && !User.IsInRole(UserRoles.Administrator)) ) {
					return Redirect(group.GroupUrl);
				}
			} else {
				group = new DiscussionGroup();
			}

			return View(group);
		}

		[HttpPost]
		public ActionResult Edit(DiscussionGroup model, HttpPostedFileBase groupimage) {


			DidacheDb db = new DidacheDb();

			try {
				// store the old email so we can know if it's changed
				DiscussionGroup group = db.DiscussionGroups.Find(model.GroupID);
				
				UpdateModel(group);

				db.SaveChanges();


				// do image
				
				// double check for image type
				if (groupimage != null && groupimage.ContentLength > 0 && System.Text.RegularExpressions.Regex.IsMatch(groupimage.FileName, "(jpeg|jpg|jpe)$", System.Text.RegularExpressions.RegexOptions.IgnoreCase)) {


					// save it to new path
					string groupOriginalPath = Server.MapPath("~/images/groups/" + model.GroupID + "-original.jpg");
					string groupProfilePath = Server.MapPath("~/images/groups/" + model.GroupID + ".jpg");
					string groupThumbPath = Server.MapPath("~/images/groups/" + model.GroupID + "-thumb.jpg");
					

					if (System.IO.File.Exists(groupOriginalPath))
						System.IO.File.Delete(groupOriginalPath);

					if (System.IO.File.Exists(groupProfilePath))
						System.IO.File.Delete(groupProfilePath);

					if (System.IO.File.Exists(groupThumbPath))
						System.IO.File.Delete(groupThumbPath);

					// save original for later
					groupimage.SaveAs(groupOriginalPath);

					// resize for profile
					ImageTools.ScaleImage(groupOriginalPath, groupProfilePath, 180, 1000, 90);

					// resize for thumb
					ImageTools.ScaleImage(groupOriginalPath, groupThumbPath, 50, 50, 90);
				}

				return Redirect(group.GroupUrl);

			} catch (Exception ex) {
				ModelState.AddModelError("", "Edit Failure, see inner exception: " + ex.ToString());

				//throw ex;

				return View(model);
			}

		}

		public ActionResult View(int id) {

			DiscussionGroup group = db.DiscussionGroups
										.Include("GroupMembers")
										.SingleOrDefault(dg => dg.GroupID == id);

			// does group exist?
			if (group == null) {
				return Redirect("/groups/");
			}

			User user = Users.GetLoggedInUser();
			DiscussionGroupMember member = group.GroupMembers.SingleOrDefault(gm => gm.UserID == user.UserID);

			// if private, check security
			if (group.GroupType == DiscussionGroupType.Private) {
				if (member == null && !User.IsInRole(UserRoles.Administrator)) {
					return Redirect("/groups/");
				}
			}

			group.GroupMembers = group.GroupMembers
										.Where(gm => gm.GroupMembershipStatus == GroupMembershipStatus.Administrator || gm.GroupMembershipStatus == GroupMembershipStatus.Member)
										.OrderBy(gm => gm.GroupMembershipStatus)
										.ThenBy(gm => gm.User.LastName)
										.ToList();

			DiscussionGroupViewModel viewModel = new DiscussionGroupViewModel();

			viewModel.DiscussionGroup = group;
			viewModel.Membership = member;

			return View(viewModel);
		}

    }
}
