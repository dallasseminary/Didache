using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace Didache  {
	public class Users {

		private static string _usernameKey = "user-name-{0}";
		private static string _userIdKey = "user-id-{0}";


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

				HttpContext.Current.Cache.Add(key, user, null, Cache.NoAbsoluteExpiration, new TimeSpan(1, 0, 0), CacheItemPriority.Default, null);
			}


			return user;
		}

		public static User GetUser(string username) {

			string key = string.Format(_usernameKey, username);
			User user = (HttpContext.Current != null) ? HttpContext.Current.Cache[key] as User : null;

			if (user == null) {
				user = new DidacheDb().Users.SingleOrDefault(u => u.Username == username);

				HttpContext.Current.Cache.Add(key, user, null, Cache.NoAbsoluteExpiration, new TimeSpan(1, 0, 0), CacheItemPriority.Default, null);
			}

			return user;			
			
			return new DidacheDb().Users.SingleOrDefault(u => u.Username == username);
		}



		public static User GetLoggedInUser() {
			if (HttpContext.Current != null && HttpContext.Current.User.Identity.IsAuthenticated) {
				return GetUser(HttpContext.Current.User.Identity.Name);
			} else {
				return null;
			}
		}


		public static void ClearUserCache(User user) {
			
			string key = string.Format(_usernameKey, user.Username);
			if (HttpContext.Current.Cache[key] != null)
				HttpContext.Current.Cache.Remove(key);

			key = string.Format(_userIdKey, user.UserID);
			if (HttpContext.Current.Cache[key] != null)
				HttpContext.Current.Cache.Remove(key);
		}
	}
}
