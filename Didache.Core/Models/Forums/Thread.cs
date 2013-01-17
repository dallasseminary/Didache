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


	public class Thread {
		[Key]
		public int ThreadID { get; set; }
		public int ForumID { get; set; }

		[Required]
		public string Subject { get; set; }

		public int UserID  { get; set; }
		public string UserName { get; set; }
		public DateTime ThreadDate { get; set; }

		public int LastPostID { get; set; }
		public int LastPostUserID { get; set; }
		public string LastPostUserName { get; set; }
		public string LastPostSubject { get; set; }
		public DateTime LastPostDate { get; set; }

		public int TotalViews { get; set; }
		public int TotalReplies { get; set; }

		
		//[ForeignKey("LastPostID")]
		//public virtual Post LastPost { get; set; }
		[ForeignKey("LastPostUserID")]
		public virtual User LastPostUser { get; set; }
		[ForeignKey("UserID")]
		public virtual User User { get; set; }
	

		public virtual Forum Forum { get; set; }
		public virtual ICollection<ForumPost> Posts { get; set; }
	}



}
