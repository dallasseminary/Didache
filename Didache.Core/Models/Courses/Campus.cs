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

	public class Campus {
		[Key]
		public int CampusID { get; set; }

		[Display(Name = "Is Active")]
		public bool IsActive { get; set; }

		[Required]
		[Display(Name = "Campus Code")]
		public string CampusCode { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		[Display(Name = "Sort Order")]
		[DefaultValue(9999)]
		public int SortOrder { get; set; }
	}



}
