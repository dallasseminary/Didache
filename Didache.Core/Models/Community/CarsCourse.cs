using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Didache {
	public class CarsCourse {
		public int UserID { get; set; }

		public string CourseCode { get; set; }
		public string Section { get; set; }
		public int Year { get; set; }
		public string Session { get; set; }
		public string Title { get; set; }

		public string CourseKey {
			get {
				return Year + "," + Session + "," + CourseCode + "," + Section;
			}
		}
		public string SessionKey {
			get {
				return Year + " " + Session;
			}
		}
		public int SessionOrder {
			get {
				switch (Session) {
					default:
					case "SP":
						return 1;
					case "SU":
						return 2;
					case "SU1":
						return 3;
					case "SU2":
						return 4;
					case "SU3":
						return 5;
					case "FA":
						return 6;
					case "WI":
						return 7;
				}
			}
		}

	}
}
