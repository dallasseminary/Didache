﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Routing;
using System.Globalization;

namespace Didache  {
	public class Users {

		public static bool IsAdministratorFacultyOrFacilitator() {
			return
				System.Web.HttpContext.Current != null
				&& System.Web.HttpContext.Current.User.Identity.IsAuthenticated
				&& (System.Web.HttpContext.Current.User.IsInRole(UserRoles.Administrator) ||
					System.Web.HttpContext.Current.User.IsInRole(UserRoles.Facilitator) ||
					System.Web.HttpContext.Current.User.IsInRole(UserRoles.Faculty));

		}

	

		private static string _usernameKey = "user-name-{0}";
		private static string _userIdKey = "user-id-{0}";


		public static string GetUserLanguage() {
			string language = "";

			User user = Users.GetLoggedInUser();

			// is user logged in?
			if (user != null) {				
				language = user.Language;
			} else {
				// check if there is a value in the cookiez
				// this is for visitor who set via the drop down list
				HttpCookie langCookie = HttpContext.Current.Request.Cookies["Language"];
				if (langCookie != null && langCookie.Value != null) {
					// use cookie
					language = langCookie.Value;
				} else {
					// use default value from browser
					language = HttpContext.Current.Request.UserLanguages[0];
				}
			}

			return language;
		}

		public static string GetUserLanguageSetMethod() {
			string setMethod = "";
			
			// is user logged in?
			if (HttpContext.Current.User.Identity.IsAuthenticated) {
				setMethod = "database";
			} else {
				HttpCookie langCookie = HttpContext.Current.Request.Cookies["Language"];
				if (langCookie != null && langCookie.Value != null) {
					setMethod = "cookie";
				} else {
					setMethod = "browser";
				}
			}

			return setMethod;
		}

		public static bool HasPermission(int profileUserID, UserSecuritySetting setting) {
			return HasPermission(profileUserID, Users.GetLoggedInUser().UserID, setting);
		}

		public static bool HasPermission(int profileUserID, int viewerUserID, UserSecuritySetting setting) {

			return true;

			switch (setting) {
				default:
				case UserSecuritySetting.Private:
					return false;
				case UserSecuritySetting.Public:
					return true;
				case UserSecuritySetting.Friends:
					return true;
			}
		}

		public static User GetUser(int id) {

			string key = string.Format(_userIdKey, id);
			User user = (HttpContext.Current != null) ? HttpContext.Current.Cache[key] as User : null;

			if (user == null) {
				user = new DidacheDb().Users.SingleOrDefault(u => u.UserID == id);

				if (user != null) {
					HttpContext.Current.Cache.Add(key, user, null, Cache.NoAbsoluteExpiration, new TimeSpan(1, 0, 0), CacheItemPriority.Default, null);
				}
			}

			return user;
		}

		public static User GetUser(string username) {

			string key = string.Format(_usernameKey, username);
			User user = (HttpContext.Current != null) ? HttpContext.Current.Cache[key] as User : null;

			if (user == null) {
				user = new DidacheDb().Users.SingleOrDefault(u => u.Username == username);

				if (user != null) {
					HttpContext.Current.Cache.Add(key, user, null, Cache.NoAbsoluteExpiration, new TimeSpan(1, 0, 0), CacheItemPriority.Default, null);
				}
			}

			return user;			
			
			//return new DidacheDb().Users.SingleOrDefault(u => u.Username == username);
		}



		public static User GetLoggedInUser() {
			if (HttpContext.Current != null && HttpContext.Current.User.Identity.IsAuthenticated) {
				return GetUser(HttpContext.Current.User.Identity.Name);
			} else {
				return null;
			}
		}

		public static void ClearUserCache(int userID) {
			ClearUserCache(Users.GetUser(userID));
		}

		public static void ClearUserCache(User user) {
			
			string key = string.Format(_usernameKey, user.Username);
			if (HttpContext.Current.Cache[key] != null)
				HttpContext.Current.Cache.Remove(key);

			key = string.Format(_userIdKey, user.UserID);
			if (HttpContext.Current.Cache[key] != null)
				HttpContext.Current.Cache.Remove(key);
		}

		public static Student GetStudent(int userID) {
			return new DidacheDb().Students.Find(userID);
		}
	}
}
