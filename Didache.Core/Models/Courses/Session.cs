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


	public class Session {
		[Key]
		public int SessionID { get; set; }

		[Display(Name = "Is Active")]
		public bool IsActive { get; set; }

		[Display(Name = "Program")]
		public string Program { get; set; }

		[Required]
		[Display(Name = "Session Code")]
		public string SessionCode { get; set; }

		[Required]
		[Display(Name = "Year")]
		public int SessionYear { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		[Display(Name = "Start Date")]
		public DateTime StartDate { get; set; }

		[Required]
		[Display(Name = "End Date")]
		public DateTime EndDate { get; set; }

		public virtual ICollection<Course> Courses { get; set; }
	}

	

}
