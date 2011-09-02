using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Didache {
	public class ForumReplyActivity : ActivityStreamItemBase {

		public Post Post { get; set; }

		public override string FormatActivity() {
			return ((User != null) ? "<a href=\"" + User.ProfileDisplayUrl + "\">" + User.SecureShortName + "</a>" : "(unknown)") + " added a new post to the thread <a href=\"" + Post.PostUrl + "\">" + Post.Thread.Subject + "</a>";
		}
	}
}
