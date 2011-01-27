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
	public class User {
		public User() {
			UserID = 0;
			IsDeceased = false;

			FirstName = "";
			MiddleName = "";
			LastName = "";
			FullName = "";
			NickName = "";

			Title = "";
			Suffix = "";

			Address1 = "";
			Address2 = "";
			City = "";
			State = "";
			Zip = "";
			Country = "";
			Phone = "";
			Email = "";

			Gender = "";
			Race = "";
			Citizen = "";

			BirthDate = DateTime.MinValue;


			Username = "";
			AspnetUserID = Guid.Empty;
			PersonID = Guid.Empty;
			IsRegistered = false;

			Latitude = 0;
			Longitude = 0;
			IsGeocoded = false;
			AttemptedGeocode = false;
		}

		[Key]
		public int UserID { get; set; }

		public bool IsDeceased { get; set; }

		public string FirstName { get; set; }
		public string MiddleName { get; set; }
		public string LastName { get; set; }
		public string FullName { get; set; }
		public string NickName { get; set; }
		public string NameFormat { get; set; }

		public string FormattedName {
			get {
				switch (NameFormat) {
					// John Charles Dyer
					case "FF":
						return ((String.IsNullOrEmpty(NickName)) ? FirstName : NickName) + " " + MiddleName + " " + LastName;
					// John C. Dyer
					default:
					case "FI":
						return ((String.IsNullOrEmpty(NickName)) ? FirstName : NickName) + " " + ((MiddleName.Length > 0) ? MiddleName.Substring(0, 1) + ". " : "") + LastName;
					// J. Charles Dyer
					case "IF":
						return ((FirstName.Length > 0) ? FirstName.Substring(0, 1) + ". " : "") + MiddleName + " " + LastName;
					// J. C. Dyer
					case "II":
						return ((FirstName.Length > 0) ? FirstName.Substring(0, 1) + ". " : "") + ((MiddleName.Length > 0) ? MiddleName.Substring(0, 1) + ". " : "") + " " + LastName;
				}
			}
		}

		public string FormattedNameLastFirst {
			get {
				switch (NameFormat) {
					// John Charles Dyer
					case "FF":
						return LastName + ", " + ((String.IsNullOrEmpty(NickName)) ? FirstName : NickName) + " " + MiddleName;
					// John C. Dyer
					default:
					case "FI":
						return LastName + ", " + ((String.IsNullOrEmpty(NickName)) ? FirstName : NickName) + " " + ((MiddleName.Length > 0) ? MiddleName.Substring(0, 1) + ". " : "");
					// J. Charles Dyer
					case "IF":
						return LastName + ", " + ((FirstName.Length > 0) ? FirstName.Substring(0, 1) + ". " : "") + MiddleName;
					// J. C. Dyer
					case "II":
						return LastName + ", " + ((FirstName.Length > 0) ? FirstName.Substring(0, 1) + ". " : "") + ((MiddleName.Length > 0) ? MiddleName.Substring(0, 1) + ". " : "");
				}
			}
		}

		public string ProfileImageUrl {
			get {
				return "http://www.dts.edu/images/carsphotos/photo.ashx?id=" + UserID;
			}
		}

		public string Title { get; set; }
		public string Suffix { get; set; }

		public string Address1 { get; set; }
		public string Address2 { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string Zip { get; set; }
		public string Country { get; set; }
		public string Phone { get; set; }
		public string Email { get; set; }

		public string Gender { get; set; }
		public string Race { get; set; }
		public string Citizen { get; set; }

		public DateTime? BirthDate { get; set; }

		public DateTime? LastUpdatedDate { get; set; }
		public DateTime? DeceasedDate { get; set; }

		// NON CARS
		public Double Latitude { get; set; }
		public Double Longitude { get; set; }
		public bool IsGeocoded { get; set; }
		public bool AttemptedGeocode { get; set; }

		// LOGIN details
		public string Username { get; set; }
		public Guid AspnetUserID { get; set; }
		public Guid PersonID { get; set; }
		public bool IsRegistered { get; set; }
	}
}
