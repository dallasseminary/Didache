using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Script.Serialization;
using System.Runtime.Serialization;


namespace Didache {


	public class UserPostComment {

		[Key]
		public int PostCommentID { get; set; }
		
		public int PostID { get; set; }

		public int UserID { get; set; }
		
		[ScriptIgnore]
		public bool IsDeleted { get; set; }
		public int DeletedByUserID { get; set; }
		public DateTime? DeletedDate { get; set; }
		
		public DateTime CommentDate { get; set; }

		[ScriptIgnore]
		[AllowHtml]
		[MaxLength]
		public string Text { get; set; }

		[AllowHtml]
		[MaxLength]
		public string TextFormatted { get; set; }

		public virtual User User { get; set; }
	}

}
