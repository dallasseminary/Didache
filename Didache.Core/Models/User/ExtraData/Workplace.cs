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

	public class Workplace {
		[Key]
		public int WorkplaceID { get; set; }
		public string FullName { get; set; }
		public string Address1 { get; set; }
		public string Address2 { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string Zip { get; set; }
		public string Country { get; set; }
		public string Phone { get; set; }
		public DateTime LastUpdatedDate { get; set; }
		public string URL { get; set; }
		public string Type { get; set; }
		public Double? Latitude { get; set; }
		public Double? Longitude { get; set; }
		public bool IsGeocoded { get; set; }
		public bool AttemptedGeocode { get; set; }
	}
}
