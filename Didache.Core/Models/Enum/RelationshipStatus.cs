using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Didache {
	public enum RelationshipStatus {
		/*
		Unapproved = 0,
		Approved = 1,
		Rejected = -1
		
		 
		 */

		Approved = 1,
		RejectedByTarget = 0,

		PendingTargetApproval = 2,
		PendingRequesterApproval = 3,
		RejectedByRequester = 4,

		None = -1
	}
}
