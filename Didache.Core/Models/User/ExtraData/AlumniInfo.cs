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
	public class AlumniInfo {
		[Key]
		public int AlumniID { get; set; }

		public int UserID { get; set; }
		public string ClassYear { get; set; }
		public string ChurchDenomination { get; set; }
		public string ChurchSize { get; set; }
		public string ShortBiography { get; set; }

		public virtual User User { get; set; }
	}
}
