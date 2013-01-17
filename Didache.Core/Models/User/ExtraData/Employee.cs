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

	public class Employee {
		[Key]
		public int EmpID { get; set; }
		public int UserID { get; set; }

		public string DepartmentCode { get; set; }
		public string DepartmentName { get; set; }
		public string Phone { get; set; }
		public string PhoneExtension { get; set; }
		public string Building { get; set; }
		public string Room { get; set; }

		public bool IsDepartmentHead { get; set; }
		public bool IsFrontDesk { get; set; }
		public bool IsStaff { get; set; }
		public bool IsFaculty { get; set; }

		public string FacultyCategory { get; set; }
		public string DTSEmail { get; set; }

		//public virtual User User { get; set; }
	}

}
