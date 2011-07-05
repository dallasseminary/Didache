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


	public class Task {
		[Key]
		public int TaskID { get; set; }

		public int UnitID { get; set; }
		
		public int CourseID { get; set; }

		[Display(Name = "Is Active")]
		public bool IsActive { get; set; }

		[Required]
		public int SortOrder { get; set; }

		[Required]
		public string Name { get; set; }

		[DataType(DataType.MultilineText)]
		[AllowHtml]
		[StringLength(100000)]
		public string Instructions { get; set; }

		[DataType(DataType.MultilineText)]
		[AllowHtml]
		[StringLength(100000)]
		public string Description { get; set; }

		//[DataType(DataType.DateTime)]
		[Display(Name = "Submission Available")]
		public DateTime? SubmissionAvailableDate { get; set; }

		//[DataType(DataType.DateTime)]
		[Display(Name = "Due")]
		public DateTime? DueDate { get; set; }

		//[DataType(DataType.DateTime)]
		[Display(Name = "Instructions Available")]
		public DateTime? InstructionsAvailableDate { get; set; }

		[Display(Name = "Skippable?")]
		public bool IsSkippable { get; set; }

		[Display(Name = "Task Type")]
		public string TaskTypeName { get; set; }

		[Display(Name = "Priority")]
		public int Priority { get; set; }

		[Display(Name = "Related Task")]
		public int RelatedTaskID { get; set; }

		public string FileTypesAllowed { get; set; }

		[ScriptIgnore]
		public virtual Unit Unit { get; set; }
		[ScriptIgnore]
		public virtual Course Course { get; set; }

	}

}
