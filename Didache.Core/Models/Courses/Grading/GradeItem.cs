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
	public class GradeItem {
		[Key]
		public int GradeItemID { get; set; }

		public int GradeGroupID { get; set; }
		public int TaskID { get; set; }
		public int SortOrder { get; set; }

		public virtual Task Task { get; set; }
	}
}
