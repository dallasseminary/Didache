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


	public class UserTaskData {
		[Key]
		public int TempID { get; set; }
		
		public int TaskID { get; set; }
		public int UserID { get; set; }

		public int TaskStatus { get; set; }
		public TaskCompletionStatus TaskCompletionStatus { get { return (TaskCompletionStatus)TaskStatus; } set { TaskStatus = (int)value; } }

		public DateTime? StudentSubmitDate { get; set; }
		public string StudentComments { get; set; }

		public string LetterGrade { get; set; }
		public int? NumericGrade { get; set; }

		public int GradeStatus { get; set; }
		public DateTime? GraderSubmitDate { get; set; }

		public string GraderComments { get; set; }
		public int GraderUserID { get; set; }
		
		public string TaskData { get; set; }

		public int TempGraderFileID { get; set; }
		public int TempStudentFileID { get; set; }
		public int TempPostID { get; set; }


		public virtual User Profile { get; set; }
		public virtual Task Task { get; set; }
	
	}

}

