using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Script.Serialization;
using System.Runtime.Serialization;

namespace Didache {

	public class CourseFileAssociation {
		public int FileID { get; set; }
		public int GroupID { get; set; }
		public int SortOrder { get; set; }
		public DateTime DateAdded { get; set; }
		public bool IsActive { get; set; }

		[ScriptIgnore]
		public virtual CourseFileGroup CourseFileGroup { get; set; }

		public virtual CourseFile CourseFile { get; set; }
	}



}
