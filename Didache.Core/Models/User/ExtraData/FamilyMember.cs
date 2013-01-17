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

	public class FamilyMember {
		[Key]
		public int FamilyID { get; set; }
		public int UserID { get; set; }
		public string Family { get; set; }
		public string FirstName { get; set; }
		public string NickName { get; set; }
		public string Gender { get; set; }
		public DateTime? BirthDate { get; set; }
	}

}
