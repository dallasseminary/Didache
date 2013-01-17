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
		
	
		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string Title { get; set; }
		
		[DisplayFormat(ConvertEmptyStringToNull = false)]
		[AllowHtml]
		[MaxLength]
		public string Text { get; set; }

		[Display(Name="Start")]
		public DateTime StartDate { get; set; }

		[Display(Name = "End")]
		public DateTime? EndDate { get; set; }
		
		public int? CourseID { get; set; }

		[Display(Name = "Active")]
		public bool IsActive { get; set; }
	}
}
