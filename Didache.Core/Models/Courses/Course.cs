﻿using System;
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

	
	public class Course {
		[Key]
		public int CourseID { get; set; }
		public int SessionID { get; set; }
		public int CampusID { get; set; }

		[Display(Name = "Is Active")]
		public bool IsActive { get; set; }

		[Required]
		[Display(Name = "Course Code")]
		public string CourseCode { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		[Display(Name = "Section")]
		public string Section { get; set; }

		[Required]
		[Display(Name = "Start Date")]
		public DateTime StartDate { get; set; }

		[Required]
		[Display(Name = "End Date")]
		public DateTime EndDate { get; set; }

		//[Required]
		[DataType(DataType.MultilineText)]
		[Display(Name = "Description")]		
		public string Description { get; set; }

		public string Slug {
			get {
				return (Session.SessionCode + "-" + CourseCode + Section).ToLower();
			}
		}


		// LINKAGES

		[ScriptIgnore]
		public virtual ICollection<Unit> Units { get; set; }

		[ScriptIgnore]
		public virtual Session Session { get; set; }
		
		[ScriptIgnore]
		public virtual Campus Campus { get; set; }

		[ScriptIgnore]
		public virtual ICollection<CourseUser> CourseUsers { get; set; }

		[ScriptIgnore]
		public virtual ICollection<CourseUserGroup> CourseUserGroups { get; set; }

		public override string ToString() {
			return CourseCode + " - " + Name;
		}
	}



}
