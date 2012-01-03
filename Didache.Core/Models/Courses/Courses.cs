using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace Didache  {
	public class Courses {

		private static string _courseById = "course-id-{0}";
		private static string _courseBySlug = "course-slug-{0}";

		public static Course GetCourse(int courseID) {
			return GetCourse(courseID, true);
		}

		public static Course GetCourse(int courseID, bool useCache) {

			string key = string.Format(_courseById, courseID);
			Course course = (HttpContext.Current != null) ? HttpContext.Current.Cache[key] as Course : null;

			if (course == null || !useCache) {
				course = new DidacheDb().Courses.SingleOrDefault(c => c.CourseID == courseID);

				if (course != null)
					HttpContext.Current.Cache.Add(key, course, null, Cache.NoAbsoluteExpiration, new TimeSpan(1, 0, 0), CacheItemPriority.Default, null);
			}


			return course;
		}

		public static Course GetCourseBySlug(string slug) {
			return GetCourseBySlug(slug, true);
		}

		public static Course GetCourseBySlug(string slug, bool useCache) {


			string key = string.Format(_courseBySlug, slug);
			Course course = (HttpContext.Current != null) ? HttpContext.Current.Cache[key] as Course : null;

			if (course == null || !useCache) {
				//string[] parts = slug.Split(new char[] { '-' });
				//string sessionInfo = parts[0].Trim().ToUpper();
				//string courseCode = parts[1].Trim().ToUpper();

				int firstDash = slug.IndexOf("-");
				string sessionInfo = slug.Substring(0, firstDash);
				string courseCode = slug.Substring(firstDash+1);


				var db = new DidacheDb();

				//Session session = db.Sessions.SingleOrDefault(s => s.SessionCode == sessionCode);
				//Course course = session.Courses.SingleOrDefault(c => c.CourseCode + c.Section == courseCode);

				int sessionYear = 0;
				string sessionCode = "";

				if (sessionInfo.Length >= 5) {
					if (Int32.TryParse(sessionInfo.Substring(sessionInfo.Length - 4), out sessionYear)) {
						sessionCode = sessionInfo.Substring(0, sessionInfo.Length - 4);
					}
				} else {
					throw new Exception("Your session is invalid bro");
				}

				course = db.Courses
									.Include("Session")
									.Include("Campus")
									.SingleOrDefault(c => c.CourseCode + c.Section == courseCode && c.Session.SessionCode == sessionCode && c.Session.SessionYear == sessionYear);

				if (course != null) 
					HttpContext.Current.Cache.Add(key, course, null, Cache.NoAbsoluteExpiration, new TimeSpan(1, 0, 0), CacheItemPriority.Default, null);
			}


			return course;

		}

		public static List<Course> GetCoursesBySession(int sessionID) {
			return new DidacheDb().Courses
					.Where(c => c.SessionID == sessionID)
					.OrderBy(c => c.CourseCode)
						.ThenBy(c => c.Section)
						.ThenBy(c => c.StartDate)
					.ToList();
		}

		public static List<Course> GetCurrentlyRunningCourses() {			
			return new DidacheDb().Courses
					.Where(c => c.StartDate < DateTime.Now && c.EndDate > DateTime.Now)
					.OrderByDescending(c => c.Session.StartDate)
						.ThenBy(c => c.CourseCode)
						.ThenBy(c => c.Section)
						.ThenBy(c => c.StartDate)
					.ToList();		
		}

		public static List<CourseUser> GetUsersInCourse(int courseID) {
			return new DidacheDb().CourseUsers
				.Include("User")
				.Where(cu => cu.CourseID == courseID)
				.OrderBy(cu => cu.User.LastName)
				//.OrderBy(cu => cu.RoleID)
				//	.ThenBy(cu => cu.GroupID)
				//	.ThenBy(cu => cu.User.LastName)
				.ToList();
		}

		public static List<CourseUser> GetUsersInCourse(int courseID, CourseUserRole courseUserRole) {
			return new DidacheDb().CourseUsers
				.Include("User")
				.Where(cu => cu.CourseID == courseID && cu.RoleID == (int)courseUserRole)
				.OrderBy(cu => cu.RoleID)
					.ThenBy(cu => cu.GroupID)
					.ThenBy(cu => cu.User.LastName)
				.ToList();
		}

		public static List<Course> GetUsersCourses(CourseUserRole roleID) {
			return GetUsersCourses(Users.GetLoggedInUser().UserID, roleID);
		}

		public static List<Course> GetUsersCourses(int userID, CourseUserRole roleID) {
			return new DidacheDb().Courses
				.Where(c => c.CourseUsers.Any(cu => cu.UserID == userID && cu.RoleID == (int)roleID))
				.OrderByDescending(c => c.Session.StartDate)
				.ToList();
		}

		public static List<Course> GetUsersRunningCourses(CourseUserRole roleID) {
			User profile = Users.GetLoggedInUser();

			if (profile != null)
				return GetUsersRunningCourses(profile.UserID, roleID);
			else
				return null;
		}

		public static List<Course> GetUsersRunningCourses(CourseUserRole roleID, CourseUserRole roleID2) {
			User profile = Users.GetLoggedInUser();

			if (profile != null)
				return GetUsersRunningCourses(profile.UserID, roleID, roleID2);
			else
				return null;
		}

		public static List<Course> GetUsersRunningCourses(int userID, CourseUserRole roleID) {

			string key = String.Format("coursess-userrunning-{0}-{1}", userID, roleID);
			List<Course> courses = (HttpContext.Current != null) ? HttpContext.Current.Cache[key] as List<Course> : null;

			bool ignoreCache = true;

			if (courses == null || ignoreCache) {


				DateTime targetStartDate = DateTime.Now.AddDays(7);
				DateTime targetEndDate = DateTime.Now.AddDays(-7);

				courses = new DidacheDb().Courses
					.Where(c => c.CourseUsers.Any(cu => cu.UserID == userID &&
														cu.RoleID == (int)roleID) &&
														c.StartDate <= targetStartDate &&
														c.EndDate >= targetEndDate)
					.OrderByDescending(c => c.StartDate)
					.ToList();

				if (roleID == CourseUserRole.Student)
					courses = courses.Where(c => c.IsActive).ToList();

				HttpContext.Current.Cache.Add(key, courses, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, 3, 0), CacheItemPriority.Default, null);
			}

			return courses;
		}

		public static List<Course> GetUsersRunningCourses(int userID, CourseUserRole roleID, CourseUserRole roleID2) {

			List<Course> courses = GetUsersRunningCourses(userID, roleID);
			List<Course> courses2 = GetUsersRunningCourses(userID, roleID2);

			foreach (Course course in courses2) {
				if (courses.Count(c => c.CourseID == course.CourseID) == 0) {
					courses.Add(course);
				}
			}


			return courses;
		}



		public static List<CourseUserGroup> GetCourseUserGroups(int courseID) {
			List<CourseUserGroup> userGroups = new DidacheDb().CourseUserGroups
				.Include("Students.User")
                //.Include("Facilitator")
				.Where(g => g.CourseID == courseID)
				.ToList();

			foreach (CourseUserGroup group in userGroups) {
				List<CourseUser> users = group.Students.ToList();
				users.Sort(delegate(CourseUser a, CourseUser b) { return a.User.FormattedNameLastFirst.CompareTo(b.User.FormattedNameLastFirst); });
				group.Students = users;
			}

			return userGroups;
		}

		public static List<CourseUser> GetUsersInGroup(int groupID) {
			List<CourseUser> users = new DidacheDb().CourseUsers
				.Include("User")
				.Where(cu => cu.GroupID == groupID)
				.ToList();

			return users;
		}

		/*
		public static CourseUserGroup GetCoursesInGroup(int userID) {
			CourseUserGroup group = new DidacheDb().CourseUserGroups
				.Where(cu => cu.GroupID == groupID)
				.ToList();

			return users;
		}
		 **/

		public static List<Unit> GetCourseUnitsWithTasks(int courseID) {
			List<Unit> units = new DidacheDb().Units
				.Include("Tasks")
				.Where(u => u.CourseID == courseID)
				.OrderBy(u => u.SortOrder)
				.ToList();

			foreach (Unit unit in units) {
				unit.Tasks = unit.Tasks.ToList().OrderBy(t => t.SortOrder).ToList();
			}

			return units;
		}

		public static List<Unit> GetCourseUnits(int courseID) {
			return new DidacheDb().Units
				.Where(u => u.CourseID == courseID)
				.OrderBy(u => u.SortOrder)
				.ToList();
		}


		public static List<UserTaskData> GetImportantTasksForUser() {

			User user = Users.GetLoggedInUser();

			if (user != null) {
				return new DidacheDb().UserTasks
					.Include("Course")
					.Where(utd =>
							utd.UserID == user.UserID &&
							utd.Task.Priority > 1 &&
							utd.Task.DueDate != null &&
							utd.TaskStatus == 0)
					.OrderBy(utd =>
							utd.Task.DueDate)
					.ToList();
			} else {
				return null;
			}
		}

		public static List<UserTaskData> GetImportantTasksForUser( int courseID) {

			User user = Users.GetLoggedInUser();

			if (user != null) {
				return new DidacheDb().UserTasks
					.Where(utd =>
							utd.UserID == user.UserID &&
							utd.Task.Priority > 1 &&
							utd.Task.IsActive == true && 
							utd.Task.DueDate != null &&
							utd.TaskStatus == 0 && 
							utd.CourseID == courseID)
					.OrderBy(utd =>
							utd.Task.DueDate)
					.ToList();
			}
			else {
				return null;
			}
		}


		public static List<Task> GetTasks(int courseID) {
			return new DidacheDb()
				.Tasks
				.Where(t => t.Unit.CourseID == courseID)
				.OrderBy(t => t.Unit.SortOrder)
					.ThenBy(t => t.SortOrder)
				.ToList();
		}

		public static Course CloneCourse(int courseID, int sessionID, DateTime startDate, string courseCode, string section) {

			DidacheDb db = new DidacheDb();

			Course oldCourse = db.Courses
												.Include("Units.Tasks")
												.Include("CourseFileGroups.CourseFileAssociations")
												.SingleOrDefault(c => c.CourseID == courseID);

			int daysToShift = (startDate - oldCourse.StartDate).Days;

			Course newCourse = new Course() {
				SessionID = sessionID,
				CampusID = oldCourse.CampusID,
				IsActive = oldCourse.IsActive,
				CourseCode = !String.IsNullOrWhiteSpace(courseCode) ? courseCode : oldCourse.CourseCode,
				Name = oldCourse.Name,
				Section = !String.IsNullOrWhiteSpace(section) ? section : oldCourse.Section,
				StartDate = oldCourse.StartDate.AddDays(daysToShift),
				EndDate = oldCourse.EndDate.AddDays(daysToShift),
				Description = oldCourse.Description
			};

			db.Courses.Add(newCourse);
			db.SaveChanges();

			/*
			} catch (DbEntityValidationException dbEx) {
				string errors = "";
				
				foreach (var validationErrors in dbEx.EntityValidationErrors) {
					foreach (var validationError in validationErrors.ValidationErrors) {
						//System.Web.HttpContext.Current.Trace.Warn("Property: {0} Error: {1}", validationError.PropertyName, dbEx);
						errors += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage) + "; ";
					}
				}

				throw new Exception(errors);
			}
			*/

			foreach (Unit oldUnit in oldCourse.Units) {
				Unit newUnit = new Unit() {
					CourseID = newCourse.CourseID,
					IsActive = oldUnit.IsActive,
					SortOrder = oldUnit.SortOrder,
					Name = oldUnit.Name,
					StartDate = oldUnit.StartDate.AddDays(daysToShift),
					EndDate = oldUnit.EndDate.AddDays(daysToShift),
					Instructions = oldUnit.Instructions
				};

				db.Units.Add(newUnit);
				db.SaveChanges();

				Dictionary<int, int> taskMap = new Dictionary<int, int>();
				List<Task> newTasks = new List<Task>();

				foreach (Task oldTask in oldUnit.Tasks) {
					Task newTask = new Task() {
						CourseID = newUnit.CourseID,
						UnitID = newUnit.UnitID,
						IsActive = oldTask.IsActive,
						SortOrder = oldTask.SortOrder,
						Name = oldTask.Name,
						DueDate = null,
						FileTypesAllowed = oldTask.FileTypesAllowed,
						InstructionsAvailableDate = null,
						IsSkippable = oldTask.IsSkippable,
						Priority = oldTask.Priority,
						RelatedTaskID = oldTask.RelatedTaskID,
						SubmissionAvailableDate = null,
						TaskID = oldTask.TaskID,
						TaskTypeName = oldTask.TaskTypeName,
						Instructions = oldTask.Instructions
					};

					if (oldTask.DueDate.HasValue)
						newTask.DueDate = oldTask.DueDate.Value.AddDays(daysToShift);

					if (oldTask.SubmissionAvailableDate.HasValue)
						newTask.SubmissionAvailableDate = oldTask.SubmissionAvailableDate.Value.AddDays(daysToShift);

					if (oldTask.InstructionsAvailableDate.HasValue)
						newTask.InstructionsAvailableDate = oldTask.InstructionsAvailableDate.Value.AddDays(daysToShift);

					db.Tasks.Add(newTask);

					db.SaveChanges();

					// store to remap the tasks below
					newTasks.Add(newTask);
					taskMap.Add(oldTask.TaskID, newTask.TaskID);
				}
			
				// go back and remap the related tasks
				List<int> newTaskIds = taskMap.Values.ToList();
				

				foreach(Task newTask in newTasks) {
					if (newTask.RelatedTaskID > 0 && taskMap.ContainsKey(newTask.RelatedTaskID)) {
						newTask.RelatedTaskID = taskMap[newTask.RelatedTaskID];
					}
				}

				db.SaveChanges();
			}

			// FILES
			foreach (CourseFileGroup oldGroup in oldCourse.CourseFileGroups) {
				CourseFileGroup newGroup = new CourseFileGroup() {
					CourseID = newCourse.CourseID,
					Name = oldGroup.Name,
					SortOrder = oldGroup.SortOrder
				};

				db.CourseFileGroups.Add(newGroup);
				db.SaveChanges();

				foreach (CourseFileAssociation oldFile in oldGroup.CourseFileAssociations) {
					CourseFileAssociation newFile = new CourseFileAssociation() {
						GroupID = newGroup.GroupID,
						FileID = oldFile.FileID,
						DateAdded = newCourse.StartDate,
						IsActive = oldFile.IsActive,
						SortOrder = oldFile.SortOrder
					};

					db.CourseFileAssociations.Add(newFile);
				}

				db.SaveChanges();

			}

			return newCourse;
		}


		public static bool IsUserFaculty(int courseID, int userID) {
			return IsUserInCourseRole(courseID, userID, CourseUserRole.Faculty);
		}
		public static bool IsUserFacilitator(int courseID, int userID) {
			return IsUserInCourseRole(courseID, userID, CourseUserRole.Faciliator);
		}
		public static bool IsUserInCourseRole(int courseID, int userID, CourseUserRole courseUserRole) {
			string key = string.Format("courserole-{0}-{1}-{2}", courseID, userID, courseUserRole);
			bool isInRole = false;

			if (System.Web.HttpContext.Current.Cache[key] != null) {
				isInRole = (bool)System.Web.HttpContext.Current.Cache[key];
			} else {
				isInRole = new DidacheDb().CourseUsers.Count(cu => cu.UserID == userID && cu.CourseID == courseID && cu.RoleID == (int)courseUserRole) > 0;
				System.Web.HttpContext.Current.Cache.Insert(key, isInRole, null, DateTime.Now.AddMinutes(30), Cache.NoSlidingExpiration);
			}

			return isInRole;

		}
	
	}
}
