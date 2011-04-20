using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Didache {
	public abstract class ActivityStreamItemBase {

		
		public DateTime ActivityDate { get; set; }
		public User User { get; set; }

		public abstract string FormatActivity();
	}
}
