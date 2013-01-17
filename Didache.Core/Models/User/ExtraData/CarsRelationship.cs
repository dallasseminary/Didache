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

	public class CarsRelationship {
		[Key]
		public int RelationshipID { get; set; }
		public int PrimaryID { get; set; }
		public int SecondaryID { get; set; }
		public string Relationship { get; set; }

		public DateTime? BeginDate { get; set; }
		public DateTime? EndDate { get; set; }


		[ForeignKey("PrimaryID")]
		public virtual User PrimaryUser { get; set; }
		[ForeignKey("SecondaryID")]
		public virtual User SecondaryUser { get; set; }

	}

}
