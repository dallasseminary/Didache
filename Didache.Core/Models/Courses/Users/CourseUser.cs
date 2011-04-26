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
	public class CourseUser {

		public int UserID { get; set; }
		public int CourseID { get; set; }
		public int RoleID { get; set; }
		public int GroupID { get; set; }

		public virtual Course Course { get; set; }
		public virtual User User { get; set; }
	}
}
