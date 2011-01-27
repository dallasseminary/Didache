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
	public class CourseUserGroup {

		[Key]
		public int GroupID { get; set; }
		public int CourseID { get; set; }
		public int FacilitatorUserID { get; set; }
		public string Name{ get; set; }

		public virtual ICollection<CourseUser> Students { get; set;}
		//public virtual Profile Facilitator;

	}
}
