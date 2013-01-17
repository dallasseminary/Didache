using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

/*

SELECT GroupID, AuthorUserID as CreatedByUserID, IsPublished as IsApproved, Name, Description, GroupAccess as GroupTypeID, CreatedDate, GroupAccess
INTO oe_DiscussionGroups
FROM koin_Groups

SELECT 
	GroupID,
	UserID,
	GroupMembershipStatus as GroupMembershipStatusID
INTO oe_DiscussionGroups_Users
FROM koin_Groups_Users
 * 
 * 
INSERT INTO 
oe_UserPosts
(UserID, PostDate,  Text, TextFormatted, DiscussionGroupID,IsDeleted, CourseID, CourseGroupID, UserGroupID, UserPostTypeID)
SELECT
AuthorUserID, MessageDate, Text, Text, TargetGroupID, IsDeleted, 0, 0, 0, 5
FROM 
koin_GroupMessages 

 */
namespace Didache {
	public class DiscussionGroup {
		
		[Key]
		public int GroupID { get; set; }
		public bool IsApproved { get; set; }
		public string Name { get; set; }
		public int CreatedByUserID { get; set; }

		[MaxLength]
		public string Description { get; set; }
		public DateTime CreatedDate { get; set; }

		public int GroupTypeID { get; set; }

		public DiscussionGroupType GroupType {
			get {
				return (DiscussionGroupType)GroupTypeID;
			}
			set {
				GroupTypeID = (int)value;
			}

		}

		public string GroupUrl {
			get {
				return "/groups/" + GroupID;
			}
		}

		public string ProfileImageUrl {
			get {
				string path = "/images/groups/" + GroupID + ".jpg"; ;

				if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath("~" + path))) {
					return path;
				} else {
					return "/images/groups/default.jpg";
				}

			
			}
		}

		public string ThumbImageUrl {
			get {
				string path = "/images/groups/" + GroupID + "-thumb.jpg";
				string pathBak = "/images/groups/" + GroupID + ".jpg";

				if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath("~" + path))) {
					return path;
				} else if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath("~" + pathBak))) {
					return pathBak;
				} else {
					return "/images/groups/default-thumb.jpg";
				}
			}
		}

		public virtual ICollection<DiscussionGroupMember> GroupMembers { get; set; }
	
	}
}
