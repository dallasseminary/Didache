using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Didache.Models;

namespace Didache.Web.Controllers {
	public class AccountController : Controller {

		DidacheDb db = new DidacheDb();

		public ActionResult Login() {
			return View();
		}

		//
		// POST: /Account/LogOn

		[HttpPost]
		public ActionResult Login(LogOnModel model, string returnUrl) {
			if (ModelState.IsValid) {

				string username = model.UserName;
				string password = model.Password;
				bool rememberMe = model.RememberMe;
				User user = null;

				// check on ID
				int id = 0;
				if (Int32.TryParse(username, out id)) {
					user = db.Users.Find(id);
					if (user != null) {
						username = user.Username;
					}
				}
				// check for email
				else if (username.IndexOf("@") > -1) {
					user = db.Users.SingleOrDefault(u=> u.Email == username);
					if (user != null) {
						username = user.Username;
					}
				}

				if (Membership.ValidateUser(username, password)) {


					FormsAuthenticationTicket fat = new FormsAuthenticationTicket(2, username, DateTime.Now.AddMinutes(-5), DateTime.Now.AddMinutes(FormsAuthentication.Timeout.Minutes), rememberMe, username, FormsAuthentication.FormsCookiePath);
					HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName);
					cookie.Value = FormsAuthentication.Encrypt(fat);
					cookie.Expires = fat.Expiration;
					cookie.Domain = FormsAuthentication.CookieDomain; // ".dts.edu";

					Response.Cookies.Add(cookie);

					//FormsAuthentication.RedirectFromLoginPage(username, rememberMe);

					
					FormsAuthentication.SetAuthCookie(username, rememberMe);
					
					if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/") && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\")) {
						return Redirect(returnUrl);
					} else {
						return Redirect("/"); 
					}
					 
				} else {
					ModelState.AddModelError("", "The user name or password provided is incorrect.");
				}
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		//
		// GET: /Account/LogOff

		public ActionResult Logout() {
			FormsAuthentication.SignOut();

			return Redirect("/");
		}

		public ActionResult EditProfile() {

			User user = Users.GetLoggedInUser();

			return View(user);
		}

		[HttpPost]
		public ActionResult EditProfile(User model) {
		
			DidacheDb db = new DidacheDb();
			
			try {
				model = db.Users.Find(model.UserID);

				UpdateModel(model);

				db.SaveChanges();

				Users.ClearUserCache(model);

				return Redirect(model.ProfileDisplayUrl); // RedirectToAction("EditProfile");

			} catch (Exception ex) {
				ModelState.AddModelError("", "Edit Failure, see inner exception: " + ex.ToString());

				return View(model);
			}
			
		}






	}
}
