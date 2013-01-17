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
	public class Student {

		public Student() {
			UserID = 0;
			Campus = "";
			Program = "";
			Degree = "";
			AdmittedSession = "";
			AdmittedYear = 0;
			PlannedGradSession = "";
			PlannedGradYear = 0;
			CommencementYear = 0;
		}

		[Key]
		public int StudentID { get; set; }

		public int UserID { get; set; }
		public string Campus { get; set; }
		public string Program { get; set; }
		public string Degree { get; set; }

		public string AdmittedSession { get; set; }
		public int AdmittedYear { get; set; }
		public string PlannedGradSession { get; set; }
		public int PlannedGradYear { get; set; }
		public int CommencementYear { get; set; }

		public virtual User User { get; set; }
	}
}
