using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Didache  {
	public class Users {

		public static User GetUser(int id) {
			return new DidacheDb().Users.SingleOrDefault(u=>u.UserID == id);
		}

		public static User GetUser(string username) {
			return new DidacheDb().Users.SingleOrDefault(u => u.Username == username);
		}

		public static User GetLoggedInUser() {
			if (HttpContext.Current != null && HttpContext.Current.User.Identity.IsAuthenticated) {
				return new DidacheDb().Users.SingleOrDefault(u => u.Username == HttpContext.Current.User.Identity.Name);
			} else {
				return null;
			}
		}

	}
}
