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

	

	public class HelpModel {
		
		[Required]
		[DataType(DataType.Text)]
		[Display(Name = "help_name", ResourceType = typeof(Resources.labels))]
		public string Name { get; set; }

		[Required]
		[DataType(DataType.EmailAddress)]
		[Display(Name = "help_email", ResourceType = typeof(Resources.labels))]
		public string Email { get; set; }

		[Required]
		[DataType(DataType.MultilineText)]
		[Display(Name = "help_message", ResourceType = typeof(Resources.labels))]
		public string Message { get; set; }


		public bool IsSubmitted { get; set; }
	}
}

