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


	public class InteractionPost {
		[Key]
		public int PostID { get; set; }
		public int ThreadID { get; set; }
		public int TaskID { get; set; }
		public bool IsApproved { get; set; }

		public DateTime PostDate { get; set; }
		public int UserID { get; set; }	
		public int ReplyToPostID { get; set; }

		public string Subject{ get; set; }
		public string PostContent { get; set; }
		public string PostContentFormatted { get; set; }

		public virtual InteractionThread Thread { get; set; }
		public virtual User User { get; set; }

		public string PostUrl {
			get {
				return "/courses/" + Thread.Task.Course.Slug + "/schedule/" + Thread.Task.UnitID + "#interaction-" + PostID;
			}
		}
	}



}
