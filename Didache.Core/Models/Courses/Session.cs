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
		public bool IsActive { get; set; }

		[Required]
		public string SessionCode { get; set; }

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
