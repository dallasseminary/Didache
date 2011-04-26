using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Didache {
	public class InteractionActivity : ActivityStreamItemBase {

		public InteractionPost Post { get; set; }

		public override string FormatActivity() {
			return ((User != null) ? "<a href=\"" + User.ProfileDisplayUrl + "\">" + User.FormattedName + "</a>" : "(unknown)") + " responded to <a href=\"" + Post.PostUrl + "\">" + Post.Thread.User.FormattedName + "'s assignment</a>.";
		}
	}
}
