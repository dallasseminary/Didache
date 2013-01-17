using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using Didache.Models;

namespace Didache.Web.Areas.Admin.Controllers
{

	[AdminAndBuilder]
    public class UsersController : Controller
    {
        //
        // GET: /Admin/Users/
		DidacheDb db = new DidacheDb();

		public ActionResult MakeUserPrivate(int userID) {
			Users.MakeUserPrivate(userID);

			return Json(new { success = true });
		}

		
        public ActionResult Index(string query)
        {
			List<User> users = null;

			if (query != null) {
				
				int number = 0;
				if (Int32.TryParse(query, out number)) {
					users = db.Users.Where(u => u.UserID == number).ToList();
				} else {
					users = db.Users.Where(u => 
												u.Username == query || 
												u.LastName == query ||
												(u.FirstName + " " + u.LastName == query) || 
												u.AliasFirstName == query || 
												u.AliasLastName == query).ToList();
				}

			}

			return View(users);
        }
		
		public ActionResult EditUser(int? id) {
			User user = (id.HasValue) ? db.Users.Find(id.Value) : new User();

			ViewBag.UsersRoles = System.Web.Security.Roles.GetRolesForUser(user.Username).ToList();
			ViewBag.AllRoles = UserRoles.SiteRoles.ToList();
			ViewBag.UsersCourses = Courses.GetUsersCourses(user.UserID, CourseUserRole.Student);

			/*
			foreach (string role in UserRoles.SiteRoles) {
				System.Web.Security.Roles.CreateRole(role);
			}
			 */


			return View(user);
		}



		
		[HttpPost]
		public ActionResult EditUser(User model, string[] roles) {
			if (model.UserID > 0) {
				User user = db.Users.Find(model.UserID);

				// Update model didn't work, so I'm just doing a simple update of the one propery we're editing
				user.AliasFirstName = model.AliasFirstName;
				user.AliasLastName = model.AliasLastName;

				user.AllowClassmateRequests = model.AllowClassmateRequests;
				user.AddressSecuritySetting = model.AddressSecuritySetting;
				user.BiographySecuritySetting = model.BiographySecuritySetting;
				user.BirthdateSecuritySetting = model.BirthdateSecuritySetting;
				user.ChildrenSecuritySetting = model.ChildrenSecuritySetting;
				user.EmailSecuritySetting = model.EmailSecuritySetting;
				user.PhoneSecuritySetting = model.PhoneSecuritySetting;
				user.ScheduleSecuritySetting = model.ScheduleSecuritySetting;
				user.SpouseSecuritySetting = model.SpouseSecuritySetting;
				user.PictureSecuritySetting = model.PictureSecuritySetting;
				//UpdateModel(user);


				db.SaveChanges();

				Users.ClearUserCache(user);


				// remove current roles
				//string[] currentRoles = System.Web.Security.Roles.GetRolesForUser(user.Username);
				//if (currentRoles.Length > 0) {
				//	System.Web.Security.Roles.RemoveUserFromRoles(user.Username, System.Web.Security.Roles.GetRolesForUser(user.Username));
				//}

				foreach (string role in UserRoles.SiteRoles) {
					if (System.Web.Security.Roles.IsUserInRole(user.Username, role))
						System.Web.Security.Roles.RemoveUserFromRole(user.Username, role);
				}



				// add new roles	
				if (roles != null && roles.Length > 0) {
					System.Web.Security.Roles.AddUserToRoles(user.Username, roles);
				}

				

			}

			return RedirectToAction("Index");
			/*
			if (user.UserID > 0) {
				user = db.Users.Find(user.UserID);

				if (ModelState.IsValid) {

					// user info
					UpdateModel(user);
					db.SaveChanges();

					// remove current roles
					string[] currentRoles = System.Web.Security.Roles.GetRolesForUser(user.Username);
					if (currentRoles.Length > 0) {
						System.Web.Security.Roles.RemoveUserFromRoles(user.Username, System.Web.Security.Roles.GetRolesForUser(user.Username));
					}

					// add new roles	
					if (roles != null && roles.Length > 0) {
						System.Web.Security.Roles.AddUserToRoles(user.Username, roles);
					}

					return RedirectToAction("Index");
				}
				else {
					ViewBag.UsersRoles = System.Web.Security.Roles.GetRolesForUser(user.Username).ToList();
					ViewBag.AllRoles = UserRoles.SiteRoles.ToList();

					return View(user);
				}
			}
			else {
				db.Users.Add(user);
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			*/
			

			
		}

    }
}
