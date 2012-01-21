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
	public class Student {

		public Student() {
			UserID = 0;
			Campus = "";
			Program = "";
			Degree = "";
			AdmittedSession = "";
			AdmittedYear = 0;
			PlannedGradSession = "";
			PlannedGradYear = 0;
			CommencementYear = 0;
		}

		[Key]
		public int UserID { get; set; }
        public string Campus { get; set; }
        public string Program { get; set; }
        public string Degree { get; set; }

		public string AdmittedSession { get; set; }
		public int AdmittedYear { get; set; }
		public string PlannedGradSession { get; set; }
		public int PlannedGradYear { get; set; }
		public int CommencementYear { get; set; }

		public virtual User User { get; set; }
	}

	public class Degree {
		
		[Key]
        public int EdID { get; set; }
		public int UserID { get; set; }
        public string Program { get; set; }
        public string DegreeEarned { get; set; }
        public int GradYear { get; set; }

		public DateTime? GradDate { get; set; }
		public string Major1 { get; set; }
		public string Major2 { get; set; }

		//public virtual User User { get; set; }
    }

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
