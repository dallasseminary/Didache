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

		
        public ActionResult Index(string query)
        {
			List<User> users = null;

			if (query != null) {
				
				int number = 0;
				if (Int32.TryParse(query, out number)) {
					users = db.Users.Where(u => u.UserID == number).ToList();
				} else {
					users = db.Users.Where(u => u.Username == query || (u.FirstName + " " + u.LastName == query) || u.LastName == query).ToList();
				}

			}

			return View(users);
        }
		
		public ActionResult EditUser(int? id) {
			User user = (id.HasValue) ? db.Users.Find(id.Value) : new User();

			ViewBag.UsersRoles = System.Web.Security.Roles.GetRolesForUser(user.Username).ToList();
			ViewBag.AllRoles = UserRoles.SiteRoles.ToList();

			/*
			foreach (string role in UserRoles.SiteRoles) {
				System.Web.Security.Roles.CreateRole(role);
			}
			 */


			return View(user);
		}

		
		[HttpPost]
		public ActionResult EditUser(User user, string[] roles) {
			if (user.UserID > 0) {
				user = db.Users.Find(user.UserID);

				// remove current roles
				string[] currentRoles = System.Web.Security.Roles.GetRolesForUser(user.Username);
				if (currentRoles.Length > 0) {
					System.Web.Security.Roles.RemoveUserFromRoles(user.Username, System.Web.Security.Roles.GetRolesForUser(user.Username));
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
