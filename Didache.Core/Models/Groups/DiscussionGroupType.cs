using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Didache  {
	public enum DiscussionGroupType {


		// for invites
		InviteOnly = 1,

		// for special invites
		Private = 2,

		// for classes
		Closed = 4,

		// for anyone
		Open = 8
	}
}
