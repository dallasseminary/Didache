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

		public string SecureName {
			get {

				if (!String.IsNullOrWhiteSpace(AliasName)) {
					return AliasName;
				}


				switch (NameFormat) {
					// John Charles Dyer
					case "FF":					
					// John C. Dyer
					default:
					case "FI":
						return ((String.IsNullOrEmpty(NickName)) ? FirstName : NickName) + " " + LastName.Substring(0, 1);
					// J. Charles Dyer
					case "IF":
						return MiddleName + " " + LastName.Substring(0, 1);
					// J. C. Dyer
					case "II":
						return ((FirstName.Length > 0) ? FirstName.Substring(0, 1) + ". " : "") + ((MiddleName.Length > 0) ? MiddleName.Substring(0, 1) + ". " : "") + " " + LastName;
				}
			}
		}

		public string ProfileImageUrl {
			get {
				return "//www.dts.edu/images/carsphotos/photo.ashx?id=" + UserID;
			}
		}

		public string GetProfileImageUrl(int width, int height) {
			return "//www.dts.edu/images/carsphotos/photo.ashx?id=" + UserID + "&width=" + width + "&height=" + height;		
		}


		public string ProfileDisplayUrl {
			get {
				//return "/community/" + ((Username != "") ? Username.ToLower() : UserID.ToString());
				return "/community/" + UserID.ToString();
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

		[Required]
		[DataType(DataType.EmailAddress)]
		[RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage="Invalid Email address")]
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
        public string Hometown { get; set; }
        public string AlmaMater { get; set; }

		[MaxLength]
        public string Bio { get; set; }
        public string MinistryGoals { get; set; }
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string Website { get; set; }

		// LOGIN details
		public string Username { get; set; }
		public Guid AspnetUserID { get; set; }
		public Guid PersonID { get; set; }
		public bool IsRegistered { get; set; }

		// preferences
		public string Language { get; set; }
		public Double TimezoneOffset { get; set; }


		// security
		public string AliasName { get; set; }

		public int PictureSecurity { get; set; }
		public int AddressSecurity { get; set; }
		public int EmailSecurity { get; set; }
		public int PhoneSecurity { get; set; }
		public int SpouseSecurity { get; set; }
		public int ChildrenSecurity { get; set; }
		public int BiographySecurity { get; set; }
		public int ScheduleSecurity { get; set; }
		public int BirthdateSecurity { get; set; }

		public UserSecuritySetting PictureSecuritySetting { get { return (UserSecuritySetting)PictureSecurity; } set { PictureSecurity = (int)value; } }
		public UserSecuritySetting AddressSecuritySetting { get { return (UserSecuritySetting)AddressSecurity; } set { AddressSecurity = (int)value; } }
		public UserSecuritySetting EmailSecuritySetting { get { return (UserSecuritySetting)EmailSecurity; } set { EmailSecurity = (int)value; } }
		public UserSecuritySetting PhoneSecuritySetting { get { return (UserSecuritySetting)PhoneSecurity; } set { PhoneSecurity = (int)value; } }
		public UserSecuritySetting SpouseSecuritySetting { get { return (UserSecuritySetting)SpouseSecurity; } set { SpouseSecurity = (int)value; } }
		public UserSecuritySetting ChildrenSecuritySetting { get { return (UserSecuritySetting)ChildrenSecurity; } set { ChildrenSecurity = (int)value; } }
		public UserSecuritySetting BiographySecuritySetting { get { return (UserSecuritySetting)BiographySecurity; } set { BiographySecurity = (int)value; } }
		public UserSecuritySetting ScheduleSecuritySetting { get { return (UserSecuritySetting)ScheduleSecurity; } set { ScheduleSecurity = (int)value; } }
		public UserSecuritySetting BirthdateSecuritySetting { get { return (UserSecuritySetting)BirthdateSecurity; } set { BirthdateSecurity = (int)value; } }
	}
}
