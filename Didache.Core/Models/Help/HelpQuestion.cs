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
	public class HelpQuestion {
		[Key]
		public int QuestionID { get; set; }
		public int CategoryID { get; set; }
		public string Question { get; set; }
		public string Answer { get; set; }
		public int SortOrder { get; set; }

		public virtual HelpCategory HelpCategory { get; set; }
	}
}
