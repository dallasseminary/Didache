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
	public class HelpCategory {
		[Key]
		public int CategoryID { get; set; }
		public string Name { get; set; }
		public int SortOrder { get; set; }

		public virtual ICollection<HelpQuestion> HelpQuestions { get; set; }
	}
}
