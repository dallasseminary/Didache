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


	public class InteractionThread {
		[Key]
		public int ThreadID { get; set; }
		
		[Required]
		public int TaskID { get; set; }

		// legacy?
		//[Required]
		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string Subject { get; set; }

		[Required]
		public int UserID  { get; set; }

		[Required]
		public DateTime ThreadDate { get; set; }

		public int TotalReplies { get; set; }

		public virtual User User { get; set; }
		public virtual Task Task { get; set; }
		public virtual ICollection<InteractionPost> Posts { get; set; }
	}



}
