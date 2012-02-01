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
	public class UserAction {
		[Key]
		public int UserActionID { get; set; }
		public DateTime ActionDate { get; set; }
		public int UserActionTypeID { get; set; }
		public UserActionType UserActionType {
			get {
				return (UserActionType) UserActionTypeID;
			}
			set {
				UserActionTypeID = (int)value;
			}
		}
		public int SourceUserID { get; set; }
		public int TargetUserID { get; set; }
		
		public int GroupID { get; set; }
		public int PostID { get; set; }
		public int PostCommentID { get; set; }
		public int MessageID { get; set; }
		public string Text { get; set; }

		[ForeignKey("SourceUserID")]
		public virtual User SourceUser { get; set; }

		[ForeignKey("TargetUserID")]
		public virtual User TargetUser { get; set; }


		public string FormattedDescription {
			get {
				switch (UserActionType) {
					default:
						return "other: " + UserActionType.ToString();
					case UserActionType.BecomeClassmates:
						return "became classmates with <a href=\"" + TargetUser.ProfileDisplayUrl + "\">" + TargetUser.SecureFormattedName + "</a>";
					case UserActionType.UpdatePicture:
						return "updated " + SourceUser.PossessivePronoun + " profile picture.";
					case UserActionType.UpdateSettings:
					case UserActionType.UpdateContactInformation:
						return "updated " + SourceUser.PossessivePronoun + " settings.";
					case UserActionType.SimpleSearch:
						return "searched for '" + Text + "'.";
					case UserActionType.ScheduleSearch:
						return "schedule search on '" + Text + "'.";
				}
			}
		}
	}
}
