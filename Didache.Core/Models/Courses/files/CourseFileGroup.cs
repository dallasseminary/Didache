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

	public class CourseFileGroup {
		[Key]
		public int GroupID { get; set; }
		public int CourseID { get; set; }
		public string Name { get; set; }
		public int SortOrder { get; set; }

		[ScriptIgnore]
		public virtual Course Course { get; set; }

		public virtual ICollection<CourseFileAssociation> CourseFileAssociations { get; set; }
	}

}
