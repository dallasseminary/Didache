using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Didache.Models {
	public class ContactStudentsModel {

		public Course Course { get; set; }

		public SelectList GroupsSelectList { get; set; }

		public List<CourseUser> Students { get; set; }
		
		[Display(Name="Group")]
		public int SelectedGroupID { get; set; }
		
		[Required]
		public string Subject {get; set;}

		[MaxLength]
		[AllowHtml]
		[Required]
		[DataType(DataType.MultilineText)]
		public string EmailText { get; set; }

		[Display(Name = "Send Me a Copy")]
		public bool SendACopy { get; set; }

	}
}
