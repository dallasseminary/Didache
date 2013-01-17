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

			AliasName = "";
			AliasFirstName = "";
			AliasLastName = "";

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

		[ScriptIgnore]
		public bool IsDeceased { get; set; }

		[ScriptIgnore]
		public string FirstName { get; set; }

		[ScriptIgnore]
		public string MiddleName { get; set; }

		[ScriptIgnore]
		public string LastName { get; set; }

		[ScriptIgnore]
		public string FullName { get; set; }

		[ScriptIgnore]

		public string NickName { get; set; }
		[ScriptIgnore]
		public string NameFormat { get; set; }

		[ScriptIgnore]
		public string SpouseName { get; set; }

		public string Location {
			get {
				if (Country == "USA" || Country == "UNITED STATES OF AMERICA") {
					return City + ", " + State;
				} else {
					return ToTitleCase(Country);
				}
			}
		}

		private string ToTitleCase(string mText) {
			string rText = "";
			try {
				System.Globalization.CultureInfo cultureInfo =
	System.Threading.Thread.CurrentThread.CurrentCulture;
				System.Globalization.TextInfo TextInfo = cultureInfo.TextInfo;
				rText = TextInfo.ToTitleCase(mText);
			} catch {
				rText = mText;
			}
			return rText;
		}  

		[ScriptIgnore]
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

		[ScriptIgnore]
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

		[ScriptIgnore]
		public string ShortName {
			get {
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

		public string SecureShortName {
			get {

				if (!String.IsNullOrWhiteSpace(AliasFirstName)) {
					return AliasFirstName + " " + (AliasLastName.Length > 1 ? AliasLastName.Substring(0,1) + "." : "");
				} else {
					return ShortName;
				}
			}
		}

		public string SecureFormattedName {
			get {
				if (!String.IsNullOrWhiteSpace(AliasFirstName)) {
					return AliasFirstName + " " + AliasLastName;
				} else {
					return FormattedName;
				}
			}
		}

		public string SecureFirstName {
			get {
				if (!String.IsNullOrWhiteSpace(AliasFirstName)) {
					return AliasFirstName;
				} else {
					return FirstName;
				}
			}
		}

		public string SecureLastName {
			get {
				if (!String.IsNullOrWhiteSpace(AliasLastName)) {
					return AliasLastName;
				} else {
					return LastName;
				}
			}
		}

		public string SecureFormattedNameLastFirst {
			get {
				if (!String.IsNullOrWhiteSpace(AliasFirstName)) {
					return AliasLastName + ", " + AliasFirstName;
				} else {
					return FormattedNameLastFirst;
				}
			}
		}


		public string ProfileImageUrl {
			get {
				int userID = (PictureSecuritySetting == UserSecuritySetting.Private) ? 0 : UserID;
				
				return "//www.dts.edu/images/carsphotos/photo.ashx?id=" + userID;
			}
		}

		public string GetProfileImageUrl(int width, int height) {

			int userID = (PictureSecuritySetting == UserSecuritySetting.Private) ? 0 : UserID;
				
			return "//www.dts.edu/images/carsphotos/photo.ashx?id=" + userID + "&width=" + width + "&height=" + height;		
		}


		public string ProfileDisplayUrl {
			get {
				//return "/community/" + ((Username != "") ? Username.ToLower() : UserID.ToString());
				return "/community/" + UserID.ToString();
			}
		}

		[ScriptIgnore]
		public string Title { get; set; }

		[ScriptIgnore]
		public string Suffix { get; set; }

		public string Address1 { get; set; }
		public string Address2 { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string Zip { get; set; }
		public string Country { get; set; }

		[ScriptIgnore]
		public string Phone { get; set; }

		[Required]
		[DataType(DataType.EmailAddress)]
		[RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage="Invalid Email address")]
		public string Email { get; set; }

		[ScriptIgnore]
		public string Gender { get; set; }
		[ScriptIgnore]
		public string Race { get; set; }
		[ScriptIgnore]
		public string Citizen { get; set; }

		[ScriptIgnore]
		public DateTime? BirthDate { get; set; }

		[ScriptIgnore]
		public DateTime? LastUpdatedDate { get; set; }

		[ScriptIgnore]
		public DateTime? DeceasedDate { get; set; }

		// NON CARS
		public Double Latitude { get; set; }
		public Double Longitude { get; set; }

		[ScriptIgnore]
		public bool IsGeocoded { get; set; }
		
		[ScriptIgnore]
        public bool AttemptedGeocode { get; set; }
		
		[ScriptIgnore]
        public string Hometown { get; set; }
		
		[ScriptIgnore]
        public string AlmaMater { get; set; }

		[MaxLength]
		[ScriptIgnore]
        public string Bio { get; set; }

		[ScriptIgnore]
        public string MinistryGoals { get; set; }
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string Website { get; set; }

		// LOGIN details
		[ScriptIgnore]
		public string Username { get; set; }

		[ScriptIgnore]
		public Guid AspnetUserID { get; set; }

		[ScriptIgnore]
		public Guid PersonID { get; set; }

		[ScriptIgnore]
		public bool IsRegistered { get; set; }

		// preferences
		public string Language { get; set; }
		public Double TimezoneOffset { get; set; }

		[ScriptIgnore]
		public bool AllowClassmateRequests {get; set;}


		// security
		[ScriptIgnore]
		public string AliasName { get; set; }

		[Display(Name="Alias First Name")]
		[ScriptIgnore]
		public string AliasFirstName { get; set; }

		[Display(Name = "Alias Last Name")]
		[ScriptIgnore]
		public string AliasLastName { get; set; }

		// preferences
		[ScriptIgnore]
		public bool NotifyInteractionThreadsReplies { get; set; }
		[ScriptIgnore]
		public bool NotifyInteractionPostReplies { get; set; }
		[ScriptIgnore]
		public bool NotifyUserPostReplies { get; set; }
		[ScriptIgnore]
		public bool NotifyUserPostCommentReplies { get; set; }


		[ScriptIgnore]
		public int PictureSecurity { get; set; }
		[ScriptIgnore]
		public int AddressSecurity { get; set; }
		[ScriptIgnore]
		public int EmailSecurity { get; set; }
		[ScriptIgnore]
		public int PhoneSecurity { get; set; }
		[ScriptIgnore]
		public int SpouseSecurity { get; set; }
		[ScriptIgnore]
		public int ChildrenSecurity { get; set; }
		[ScriptIgnore]
		public int BiographySecurity { get; set; }
		[ScriptIgnore]
		public int ScheduleSecurity { get; set; }
		[ScriptIgnore]
		public int BirthdateSecurity { get; set; }




		[ScriptIgnore]
		public UserSecuritySetting PictureSecuritySetting { get { return (UserSecuritySetting)PictureSecurity; } set { PictureSecurity = (int)value; } }
		[ScriptIgnore]
		public UserSecuritySetting AddressSecuritySetting { get { return (UserSecuritySetting)AddressSecurity; } set { AddressSecurity = (int)value; } }
		[ScriptIgnore]
		public UserSecuritySetting EmailSecuritySetting { get { return (UserSecuritySetting)EmailSecurity; } set { EmailSecurity = (int)value; } }
		[ScriptIgnore]
		public UserSecuritySetting PhoneSecuritySetting { get { return (UserSecuritySetting)PhoneSecurity; } set { PhoneSecurity = (int)value; } }
		[ScriptIgnore]
		public UserSecuritySetting SpouseSecuritySetting { get { return (UserSecuritySetting)SpouseSecurity; } set { SpouseSecurity = (int)value; } }
		[ScriptIgnore]
		public UserSecuritySetting ChildrenSecuritySetting { get { return (UserSecuritySetting)ChildrenSecurity; } set { ChildrenSecurity = (int)value; } }
		[ScriptIgnore]
		public UserSecuritySetting BiographySecuritySetting { get { return (UserSecuritySetting)BiographySecurity; } set { BiographySecurity = (int)value; } }
		[ScriptIgnore]
		public UserSecuritySetting ScheduleSecuritySetting { get { return (UserSecuritySetting)ScheduleSecurity; } set { ScheduleSecurity = (int)value; } }
		[ScriptIgnore]
		public UserSecuritySetting BirthdateSecuritySetting { get { return (UserSecuritySetting)BirthdateSecurity; } set { BirthdateSecurity = (int)value; } }

		


		[ScriptIgnore]
		public virtual ICollection<Degree> Degrees { get; set; }

		[ScriptIgnore]
		//public virtual Student Student { get; set; }
		public virtual ICollection<AlumniInfo> AlumniInfos { get; set; }

		[ScriptIgnore]
		//public virtual Student Student { get; set; }
		public virtual ICollection<Student> Students { get; set; }

		[ScriptIgnore]
		public virtual ICollection<Employee> Employees { get; set; }

		[ScriptIgnore]
		public virtual ICollection<FamilyMember> FamilyMembers { get; set; }

		[ScriptIgnore]
		[ForeignKey("PrimaryID")]
		public virtual ICollection<CarsRelationship> CarsRelationships { get; set; }

		[ScriptIgnore]
		public virtual ICollection<WorkplaceWorker> WorkplaceWorkers { get; set; }

		private User spouse = null;
		
		[ScriptIgnore]
		public User Spouse {
			get {
				// if it's been loaded once
				if (spouse != null) 
					return spouse;

				// try to get spouse
				var db = new DidacheDb();
				CarsRelationship carsRelationship = db.CarsRelationships.FirstOrDefault(cr => cr.PrimaryID == UserID || cr.SecondaryID == UserID);
				

				if (carsRelationship != null) {
					if (carsRelationship.PrimaryID == UserID)
						spouse = carsRelationship.SecondaryUser;
					else if (carsRelationship.SecondaryID == UserID)
						spouse = carsRelationship.PrimaryUser;
				}
			
				return spouse;
			
			}
		}

		[ScriptIgnore]
		public string ChildrenList {
			get {
				return String.Join(", ",FamilyMembers.Where(fm => fm.Family == "C").OrderBy(fm => fm.BirthDate).Select(fm=>fm.FirstName).ToArray());
			}
		}

		public string PossessivePronoun {
			get {
				if (Gender == "M")
					return "his";
				else if (Gender == "F")
					return "her";
				else
					return "their";
			}
		}
	}
}
