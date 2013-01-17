using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Script.Serialization;
using System.Runtime.Serialization;

namespace Didache {


	public class UserPost {
		
		[Key]
        public int PostID { get; set; }

		public int UserID { get; set; }

		[ScriptIgnore]
		public bool IsDeleted { get; set; }

		public int DeletedByUserID { get; set; }
		public DateTime? DeletedDate { get; set; }

		public bool IsPinned { get; set; }
		public DateTime PostDate { get; set; }

		[ScriptIgnore]
		[AllowHtml]
		[MaxLength]
		public string Text { get; set; }

		[AllowHtml]
		[MaxLength]
		public string TextFormatted { get; set; }

		[ScriptIgnore]
		public int CourseID { get; set; }

		[ScriptIgnore]
		public int CourseGroupID { get; set; }

		[ScriptIgnore]
		public int UserGroupID { get; set; }

		[ScriptIgnore]
		public int FileID { get; set; }

		[ScriptIgnore]
		public int DiscussionGroupID { get; set; }

		[ScriptIgnore]
		public int UserPostTypeID { get; set; }

		public UserPostType UserPostType {
			get {
				return (UserPostType)UserPostTypeID;
			}
			set {
				UserPostTypeID = (int)value;
			}
		}

		public string UserPostTypeFormatted {
			get {
				switch (UserPostType) {
					case UserPostType.Public:
						return "Public";
					case UserPostType.Classmates:
						return "Classmates";
					case UserPostType.Course:
						return (Course != null ? Course.CourseCode + Course.Section : "unknown course");
					case UserPostType.CourseGroup:
						return (Course != null ? Course.CourseCode + Course.Section : "unknown course") + 
							" - " +
							(CourseUserGroup != null ? CourseUserGroup.Name : "unknown course group");
					case UserPostType.DiscussionGroup:
						return DiscussionGroup.Name;
					default:
						return "Unknown type";
				}
			}
		}

		public string UserPostTypeUrl {
			get {
				switch (UserPostType) {
					default:
					case UserPostType.Public:
					case UserPostType.Classmates:
						return "";
					case UserPostType.Course:
					case UserPostType.CourseGroup:
						return Course.CourseUrl + "dashboard";
					case UserPostType.DiscussionGroup:
						return DiscussionGroup.GroupUrl;
				}
			}
		}

		public string PostUrl {
			get {
				return "/post/" + PostID.ToString();
			}
		}

		public virtual User User { get; set; }
		public virtual Course Course { get; set; }

		[ForeignKey("CourseGroupID")]
		public virtual CourseUserGroup CourseUserGroup { get; set; }

		[ForeignKey("DiscussionGroupID")]
		public virtual DiscussionGroup DiscussionGroup { get; set; }

		public virtual ICollection<UserPostComment> PostComments { get; set; }
    }

}
