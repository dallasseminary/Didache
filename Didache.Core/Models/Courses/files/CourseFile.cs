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

	public class CourseFile {
		[Key]
		public int FileID { get; set; }
		public Guid UniqueID { get; set; }
		
		public bool IsActive { get; set; }
		public string ContentType { get; set; }
		public string Filename { get; set; }
		public int Length { get; set; }
		public string Description { get; set; }
		public int UploadedByUserID { get; set; }
		public DateTime UploadedDate { get; set; }
		public string Title { get; set; }
	}



}
