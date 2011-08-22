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


	public class Forum {
		[Key]
		public int ForumID { get; set; }
		public int CourseID { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public string Description { get; set; }

		public int SortOrder { get; set;}
		public int TotalThreads { get; set; }
		public int TotalPosts	 { get; set;}

		public virtual Course Course { get; set; }
		public virtual ICollection<Thread> Threads { get; set; }
	}



}
