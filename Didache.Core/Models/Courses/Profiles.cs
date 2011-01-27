using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Didache  {
	public class Users {

		public static User GetUser(int id) {
			return new DidacheDb().Profiles.SingleOrDefault(u=>u.UserID == id);
		}

		public static User GetLoggedInProfile() {
			if (HttpContext.Current != null && HttpContext.Current.User.Identity.IsAuthenticated) {
				return new DidacheDb().Profiles.SingleOrDefault(u => u.Username == HttpContext.Current.User.Identity.Name);
			} else {
				return null;
			}
		}

	}
}
