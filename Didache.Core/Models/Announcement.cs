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
	public class Announcement {
		[Key]
		public int AnnouncementID { get; set; }
		public string Title { get; set; }
		public string Text { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public int? CourseID { get; set; }
		public bool IsActive { get; set; }
	}
}
