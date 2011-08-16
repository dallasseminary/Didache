using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Net;

namespace Didache {
	public class CarsConnection {
		public static bool SyncCourse(int courseID, string sessionYear, string sessionCode, string courseCode, string section) {
			
			DidacheDb db = new DidacheDb();
			bool success = false;

			Course course = db.Courses.Find(courseID);

			// get code from CARS
			List<CarsStudentUser> users = GetCarsData(sessionYear, sessionCode, courseCode, section);


			List<CarsStudentUser> toRemove = users.Where(u => u.CarsUserStatus == CarsUserStatus.NotActive).ToList();
			List<CarsStudentUser> toAdd = users.Where(u => u.CarsUserStatus == CarsUserStatus.NotActive).ToList();

			// remove all droppies
			foreach (CarsStudentUser carsStudentUser in toRemove) {
				CourseUsers.RemoveUserFromCourse(courseID, carsStudentUser.UserID, CourseUserRole.Student);
			}

			// add all the 'R'
			foreach (CarsStudentUser carsStudentUser in toAdd) {

				User user = db.Users.Find(carsStudentUser.UserID);
				
				// check for user
				if (user == null) {
					// create login
					MembershipUser membershipUser = null;
						
					// try to find
					membershipUser = Membership.GetUser(carsStudentUser.UserID.ToString(), false);
					if (membershipUser == null) 
						membershipUser = Membership.CreateUser(carsStudentUser.UserID.ToString(), carsStudentUser.UserID.ToString() + carsStudentUser.LastName.Substring(0, 1), carsStudentUser.Email);


					// create user
					user = new User() {
						UserID = carsStudentUser.UserID,
						FirstName = carsStudentUser.FirstName,
						MiddleName = carsStudentUser.MiddleName,
						LastName = carsStudentUser.LastName,
						Email = carsStudentUser.Email,
						NameFormat = carsStudentUser.NameFormat,
						Username = membershipUser.UserName,
						AspnetUserID = new Guid(membershipUser.ProviderUserKey.ToString()),
						BirthDate = null,
						DeceasedDate = null,
						LastUpdatedDate = DateTime.Now,
						IsDeceased = false,
						Language = "en-US",

						PersonID = Guid.Empty,
						IsRegistered = true,

						TimezoneOffset = -6,

						Hometown  = "",
						AlmaMater  = "",
						Bio  = "",
						MinistryGoals  = "",
						Facebook  = "",
						Twitter  = "",
						Website  = "",

						AliasName = "",

						PictureSecurity = 0,
						AddressSecurity = 0,
						EmailSecurity = 0,
						PhoneSecurity = 0,
						SpouseSecurity = 1,
						ChildrenSecurity = 1,
						BiographySecurity = 0,
						ScheduleSecurity = 1,
						BirthdateSecurity = 1
					};

					db.Users.Add(user);
					//try {
						db.SaveChanges();
					//} catch (Exception ex) {

					//}
				}

				CourseUsers.AddUserToCourse(courseID, user.UserID, 0, CourseUserRole.Student);
			
			}
	

			//CourseUsers.RemoveUserFromCourse(courseID, userID, (CourseUserRole)roleID);
			//CourseUsers.AddUserToCourse(courseID, userID, groupID, (CourseUserRole)roleID);
			

			return success;
		}

		private static List<CarsStudentUser> GetCarsData(string sessionYear, string sessionCode, string courseCode, string section) {

			List<CarsStudentUser> users = new List<CarsStudentUser>();

			// get data
			WebClient webClient = new WebClient();
			string text = webClient.DownloadString("https://webservices.dts.edu/export/get.aspx?sessionYear=" + sessionYear + "&sessionCode=" + sessionCode + "&courseCode=" + courseCode + "&section=" + section);

			// parse it
			string[] lines = text.Split(new char[] { '\n' });

			foreach (string line in lines) {
				string[] parts = line.Split(new char[] { '\t' });
				if (parts.Length > 2) {

					CarsStudentUser user = new CarsStudentUser() {
						UserID = Int32.Parse(parts[0].Trim()),
						CarsUserStatus = (parts[1].Trim() == "R") ? CarsUserStatus.Active : CarsUserStatus.NotActive,
						FirstName = parts[2].Trim(),
						MiddleName = parts[3].Trim(),
						LastName = parts[4].Trim(),
						NameFormat = parts[5].Trim(),
						Email = parts[6].Trim()
					};

					users.Add(user);
				}
			}

			return users;
		}




	}

	internal enum CarsUserStatus {
		Active = 1,
		NotActive = 2
	}

	
	internal class CarsStudentUser {
		public CarsUserStatus CarsUserStatus { get; set; }
		public int UserID { get; set; }
		public string FirstName { get; set; }
		public string MiddleName { get; set; }
		public string LastName { get; set; }
		public string NameFormat { get; set; }
		public string Email { get; set; }		
	}
}
