using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Didache.Models {
	public class HomeViewModel {
		public List<Announcement> Announcements { get; set; }
		public List<UserPost> UserPosts { get; set; }
	}
}
