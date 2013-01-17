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
	public class DiscussionGroupMember {
		public int GroupID { get; set; }
		public int UserID { get; set; }
		public int GroupMembershipStatusID { get; set; }
		public bool IsSubscribed { get; set; }

		public GroupMembershipStatus GroupMembershipStatus {
			get {
				return (GroupMembershipStatus)GroupMembershipStatusID;
			}
			set {
				GroupMembershipStatusID = (int)value;
			}
		}

		public virtual User User  { get; set; }
		public virtual DiscussionGroup DiscussionGroup  { get; set; }
	}
}
