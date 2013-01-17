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

	public class Degree {

		[Key]
		public int EdID { get; set; }
		public int UserID { get; set; }
		public string Program { get; set; }
		public string DegreeEarned { get; set; }
		public int GradYear { get; set; }

		public DateTime? GradDate { get; set; }
		public string Major1 { get; set; }
		public string Major2 { get; set; }

		//public virtual User User { get; set; }
	}
}
