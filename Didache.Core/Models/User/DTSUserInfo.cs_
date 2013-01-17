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
		public int StudentID { get; set; }

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

	public class FamilyMember {
		[Key]
		public int FamilyID { get; set; }
		public int UserID { get; set; }
		public string Family { get; set; }
		public string FirstName { get; set; }
		public string NickName { get; set; }
		public string Gender { get; set; }
		public DateTime? BirthDate { get; set; }
	}

	public class CarsRelationship {
		[Key]
		public int RelationshipID { get; set; }
		public int PrimaryID { get; set; }
		public int SecondaryID { get; set; }
		public string Relationship { get; set; }

		public DateTime? BeginDate { get; set; }
		public DateTime? EndDate { get; set; }


		[ForeignKey("PrimaryID")]
		public virtual User PrimaryUser { get; set; }
		[ForeignKey("SecondaryID")]
		public virtual User SecondaryUser { get; set; }

	}


	public class WorkplaceWorker {
		[Key]
		public int WorkerID{ get; set; }
		public int UserID{ get; set; }
		public int WorkplaceID{ get; set; }
		public string Position{ get; set; }
		public string OccupationCode { get; set; }
		public string OccupationCode2 { get; set; }
		public string OccupationCode3 { get; set; }
		public string Phone { get; set; }
		//public DateTime BeginDate{ get; set; }
		public string BusinessPhone { get; set; }
		public string BusinessName { get; set; }

		public virtual User User { get; set; }
		public virtual Workplace Workplace { get; set; }
	}

	public class Workplace {
		[Key]
		public int WorkplaceID{ get; set; }
		public string FullName{ get; set; }
		public string Address1{ get; set; }
		public string Address2{ get; set; }
		public string City{ get; set; }
		public string State{ get; set; }
		public string Zip{ get; set; }
		public string Country{ get; set; }
		public string Phone{ get; set; }
		public DateTime LastUpdatedDate{ get; set; }
		public string URL{ get; set; }
		public string Type{ get; set; }
		public Double? Latitude { get; set; }
		public Double? Longitude { get; set; }
		public bool IsGeocoded{ get; set; }
		public bool AttemptedGeocode { get; set; }
	}
}
