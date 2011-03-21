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


	public class Task {
		[Key]
		public int TaskID { get; set; }

		[Display(Name = "Unit ID")]
		public int UnitID { get; set; }

		[Display(Name = "Course ID")]
		public int CourseID { get; set; }

		[Display(Name = "Is Active")]
		public bool IsActive { get; set; }

		[Required]
		[Display(Name = "Sort Order")]
		public int SortOrder { get; set; }

		[Required]
		public string Name { get; set; }

		[DataType(DataType.MultilineText)]
		[AllowHtml]
		public string Instructions { get; set; }

		[DataType(DataType.MultilineText)]
		[AllowHtml]
		public string Description { get; set; }

		[DataType(DataType.DateTime)]

		[Display(Name = "Due Date")]
		public DateTime? DueDate { get; set; }

		[Display(Name = "Is Skippable")]
		public bool IsSkippable { get; set; }

		[Display(Name = "Task Type Name")]
		public string TaskTypeName { get; set; }


		public virtual Unit Unit { get; set; }
		public virtual Course Course { get; set; }
	}

}
