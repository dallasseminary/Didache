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

	public class WorkplaceWorker {
		[Key]
		public int WorkerID { get; set; }
		public int UserID { get; set; }
		public int WorkplaceID { get; set; }
		public string Position { get; set; }
		public string OccupationCode { get; set; }
		public string OccupationCode2 { get; set; }
		public string OccupationCode3 { get; set; }
		public string Phone { get; set; }
		//public DateTime BeginDate{ get; set; }
		public string BusinessPhone { get; set; }
		public string BusinessName { get; set; }

		public virtual User User { get; set; }
		public virtual Workplace Workplace { get; set; }
	}

}
