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

	public class CourseFileAssociation {
		public int FileID { get; set; }
		public int CourseGroupID { get; set; }
		public int SortOrder { get; set; }

		public virtual CourseFileGroup CourseFileGroup { get; set; }
		public virtual CourseFile CourseFile { get; set; }
	}



}
