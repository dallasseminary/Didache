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
	public class GradeGroup {

		[Key]
		public int GradeGroupID { get; set;}
		public int CourseID { get; set; }
		public string Name { get; set; }
		public int SortOrder { get; set; }
		public Double Percentage { get; set; }
		public int SkippableCount { get; set; }

		public virtual ICollection<GradeItem> GradeItems { get; set; }
	}
}
