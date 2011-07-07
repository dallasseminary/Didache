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
using Didache.Models;

namespace Didache {

	
	public class Course {
		[Key]
		public int CourseID { get; set; }
		public int SessionID { get; set; }
		public int CampusID { get; set; }

		[Display(Name = "Is Active")]
		public bool IsActive { get; set; }

		[Required]
		[Display(Name = "Course Code")]
		public string CourseCode { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		[Display(Name = "Section")]
		public string Section { get; set; }

		[Required]
		[Display(Name = "Start Date")]
		public DateTime StartDate { get; set; }

		[Required]
		[Display(Name = "End Date")]
		public DateTime EndDate { get; set; }

		//[Required]
		[DataType(DataType.MultilineText)]
		[Display(Name = "Description")]		
		public string Description { get; set; }

		public string Slug {
			get {
				return (Session.SessionCode + "-" + CourseCode + Section).ToLower();
			}
		}


		// LINKAGES

		[ScriptIgnore]
		public virtual ICollection<Unit> Units { get; set; }

		[ScriptIgnore]
		public virtual Session Session { get; set; }
		
		[ScriptIgnore]
		public virtual Campus Campus { get; set; }

		[ScriptIgnore]
		public virtual ICollection<CourseUser> CourseUsers { get; set; }

		[ScriptIgnore]
		public virtual ICollection<CourseUserGroup> CourseUserGroups { get; set; }

		/*
		public List<User> FacultyUsers {
			get {
				return CourseUsers
							.Where(cu => cu.CourseUserRole == CourseUserRole.Faculty)
							.Select(cu => cu.User)
							.ToList();
			}
		}

		public string FacultyUsersFormatted {
			get {
				return string.Join(", ", FacultyUsers.Select(u => u.LastName).ToArray());
			}
		}
		

		public List<User> FacultyUsers {
			get {
				return new DidacheDb()
							.CourseUsers
								.Where(cu => cu.CourseID == CourseID && cu.CourseUserRole == CourseUserRole.Faculty)
								.Select(cu => cu.User)
								.ToList();
			}
		}
		*/

		public string FacultyLastNames {
			get {
				return 
					string.Join(", ", 
								CourseUsers
									.Where(cu => cu.CourseID == CourseID && cu.RoleID == (int) CourseUserRole.Faculty)
									.Select(cu => cu.User.LastName)
									.ToArray()
								);
				
			}
		}

		public string FacultyFullNames {
			get {
				List<User> faculty = CourseUsers
										.Where(cu => cu.CourseID == CourseID && cu.RoleID == (int)CourseUserRole.Faculty)
										.Select(cu => cu.User)
										.ToList();
				switch (faculty.Count) {
					case 0:
						return "(no faculty member)";
					case 1:
						return faculty[0].FormattedName;
					case 2:
						return faculty[0].FormattedName + " &amp; " + faculty[1].FormattedName;
					default:
						return string.Join(", ", faculty.GetRange(0, faculty.Count-1).Select(u => u.FormattedName).ToArray()) + ", and " + faculty.Last().FormattedName;

				}

			}
		}


		public override string ToString() {
			return CourseCode + " - " + Name;
		}
	}



}
