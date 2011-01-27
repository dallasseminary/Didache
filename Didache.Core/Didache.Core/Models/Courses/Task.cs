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
		public int UnitID { get; set; }
		public int CourseID { get; set; }

		public bool IsActive { get; set; }

		[Required]
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
		public DateTime? DueDate { get; set; }
		public bool IsSkippable { get; set; }

		public string TaskTypeName { get; set; }


		public virtual Unit Unit { get; set; }
		public virtual Course Course { get; set; }
	}

}
