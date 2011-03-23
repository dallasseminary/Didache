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

	public class CourseFile : BaseFile {

		public bool IsActive { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }

		public string FileUrl {
			get {
				return "/courses/coursefile/" + FileID + "/" + Filename;
			}
		}
	}



}
