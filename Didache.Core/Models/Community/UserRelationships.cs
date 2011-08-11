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
	public class UserRelationship {
		public int RequesterUserID { get; set; }
		public int TargetUserID { get; set; }
		public int Status { get; set; }

		public RelationshipStatus RelationshipStatus { 
			get { 
				return (RelationshipStatus) Status;
			}	
			set {
				Status = (int) value;
			}
		}


		[ForeignKey("RequesterUserID")]
		public virtual User RequesterUser { get; set;}

		[ForeignKey("TargetUserID")]
		public virtual User TargetUser { get; set;}
	}
}
