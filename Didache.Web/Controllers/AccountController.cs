﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Didache.Models;
using System.Net;

namespace Didache.Web.Controllers {
	public class AccountController : Controller {

		DidacheDb db = new DidacheDb();

		public ActionResult Login() {
			return View();
		}

		public ActionResult SetLanguage(string language) {			

			string redirectUrl = (Request.UrlReferrer != null) ? Request.UrlReferrer.ToString() : "/";

			if (User.Identity.IsAuthenticated) {

				User user = db.Users.SingleOrDefault(u => u.Username == User.Identity.Name);
				if (user != null) {
					
					user.Language = language;
					db.SaveChanges();
					Users.ClearUserCache(user);
					
				}

				return Redirect(redirectUrl);


			} else {
				Response.Cookies.Add(new HttpCookie("Language", language));

				return Redirect(redirectUrl);
			}


			
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

			User user = Users.GetLoggedInUser(true);

			return View(user);
		}

		[HttpPost]
		public ActionResult EditProfile(User model) {
		
			DidacheDb db = new DidacheDb();
			
			try {
				// store the old email so we can know if it's changed
				User user = db.Users.Find(model.UserID);
				string oldEmail = user.Email;

				UpdateModel(user);

				// fix stuff
				if (!String.IsNullOrEmpty(user.Twitter) && user.Twitter.Contains("/")) {
					user.Twitter = user.Twitter.Substring(user.Twitter.LastIndexOf("/") + 1);
				}
				if (!String.IsNullOrEmpty(user.Facebook) && user.Facebook.Contains("facebook.com")) {
					user.Facebook = user.Facebook
											.Replace("http://", "")
											.Replace("https://", "")
											.Replace("www.facebook.com/", "")
											.Replace("facebook.com/", "");
				}
				if (!String.IsNullOrEmpty(user.Website) && !user.Website.Contains("http")) {
					user.Website = "http://" + user.Website;
				}

				UserAction ua = new UserAction() {
					SourceUserID = user.UserID,
					TargetUserID = 0,
					UserActionType = UserActionType.UpdateSettings,
					ActionDate = DateTime.Now,
					GroupID = 0,
					MessageID = 0,
					PostCommentID = 0,
					PostID = 0,
					Text = ""
				};
				db.UserActions.Add(ua);


				db.SaveChanges();


				// change of email changes
				if (user.Email != oldEmail) {

					// change membership user
					MembershipUser membershipUser = Membership.GetUser(user.Username, true);
					membershipUser.Email = model.Email;
					Membership.UpdateUser(membershipUser);

					// send to CARS
					// set to CampusNet
					try {
						string transactionUrl = "https://campus.dts.edu/cgi-bin/public/DSchkhold.cgi?id=" + user.UserID.ToString() + "&email=" + user.Email;
						WebClient webClient = new WebClient();
						string returnValue = webClient.DownloadString(transactionUrl);
					}
					catch {
						// don't worry. be happy.
					}
				}


				// save language to cookie for logout
				Response.Cookies.Add(new HttpCookie("Language", model.Language));

				Users.ClearUserCache(user);

				return Redirect(user.ProfileDisplayUrl); // RedirectToAction("EditProfile");

			} catch (Exception ex) {
				ModelState.AddModelError("", "Edit Failure, see inner exception: " + ex.ToString());

				return View(model);
			}
			
		}

		[Authorize]
		public ActionResult UploadPhoto() {

			User user = Users.GetLoggedInUser();
			
			return View(user);
		}

		[Authorize]
		[HttpPost]
		public ActionResult UploadPhoto(HttpPostedFileBase userimage) {

			User user = Users.GetLoggedInUser();

			// double check for image type
			if (userimage != null && userimage.ContentLength > 0 && System.Text.RegularExpressions.Regex.IsMatch(userimage.FileName, "(jpeg|jpg|jpe)$", System.Text.RegularExpressions.RegexOptions.IgnoreCase)) {
				
				// delete thumbs?
				//string[] thumbPaths = System.IO.Directory.GetFiles(@"D:\websites\www.dts.edu\www07\images\carsphotos\customthumbs\", user.UserID + "-*");
				string[] thumbPaths = System.IO.Directory.GetFiles(Settings.UserImageThumbLocation, user.UserID + "-*");
				
				foreach (string thumbPath in thumbPaths) {
					System.IO.File.Delete(thumbPath);
				}

				// save it to new path
				string customPath = Settings.UserImageLocation + user.UserID + ".jpg";
				string customOriginalPath = Settings.UserImageLocation + user.UserID + "-original.jpg";
				
				if (System.IO.File.Exists(customOriginalPath))
					System.IO.File.Delete(customOriginalPath);

				if (System.IO.File.Exists(customPath))
					System.IO.File.Delete(customPath);
				
				// save original for later
				userimage.SaveAs(customOriginalPath);

				// resize to a decent file size
				ImageTools.ScaleImage(customOriginalPath, customPath, 180, 1000, 90);


				UserAction ua = new UserAction() {
					SourceUserID = user.UserID,
					TargetUserID = 0,
					UserActionType = UserActionType.UpdatePicture,
					ActionDate = DateTime.Now,
					GroupID = 0,
					MessageID = 0,
					PostCommentID = 0,
					PostID = 0,
					Text = ""
				};
				db.UserActions.Add(ua);
				db.SaveChanges();
			}

			return Redirect(user.ProfileDisplayUrl);
		}

		[Authorize]
		public ActionResult ChangePassword() {
			return View();
		}
		
		[Authorize]
		[HttpPost]
		public ActionResult ChangePassword(ChangePasswordModel model) {
			if (ModelState.IsValid) {

				// ChangePassword will throw an exception rather
				// than return false in certain failure scenarios.
				bool changePasswordSucceeded;
				try {
					MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
					changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
				} catch (Exception) {
					changePasswordSucceeded = false;
				}

				if (changePasswordSucceeded) {
					return RedirectToAction("ChangePasswordSuccess");
				} else {
					ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
				}
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		//
		// GET: /Account/ChangePasswordSuccess

		public ActionResult ChangePasswordSuccess() {
			return View();
		}


	}
}
