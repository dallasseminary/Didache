using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Didache {


	public class Post {
		[Key]
		public int PostID { get; set; }
		public int ThreadID { get; set; }
		public int ForumID { get; set; }
		public bool IsApproved { get; set; }

		public DateTime PostDate { get; set; }
		public int UserID { get; set; }
		public string UserName { get; set; }		
		public int ReplyToPostID { get; set; }
		public int TotalViews { get; set; }

		public string Subject{ get; set; }
		public string PostContent { get; set; }
		public string PostContentFormatted { get; set; }

		public string PostUrl {
			get {
				return "/courses/" + Thread.Forum.Course.Slug + "/discussion/thread/" + Thread.ThreadID + "#post-" + PostID;
			}
		}

		public virtual Thread Thread { get; set; }
		public virtual User User { get; set; }
	}



}
