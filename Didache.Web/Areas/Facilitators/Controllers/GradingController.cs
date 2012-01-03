using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ionic.Zip;
using System.IO;
using Didache;
using System.Text.RegularExpressions;
using Didache;
using Didache.Models;

namespace Didache.Web.Areas.Facilitators.Controllers
{

	[AdminFacultyFacilitator]
    public class GradingController : Controller
    {
        //
        // GET: /Facilitators/Grading/

		DidacheDb db = new DidacheDb();
		EntityObjectSerializer serializer = new EntityObjectSerializer();

        public ActionResult Index(string slug) {
			Course course = Courses.GetCourseBySlug(slug);

            return View(course);
        }

		public ActionResult StudentList(string slug) {
			Course course = Courses.GetCourseBySlug(slug);

			List<CourseUser> users = db.CourseUsers
										.Include("User")
								.Where(cu => cu.CourseID == course.CourseID && cu.RoleID == (int) CourseUserRole.Student)
								.OrderBy(cu => cu.GroupID)
									.ThenBy(cu => cu.User.LastName)
								.ToList(); 
			ViewBag.Users = users;

			return View(course);
		}


		public ActionResult TaskList(string slug) {
			Course course = Courses.GetCourseBySlug(slug);

			List<Unit> units = db.Units
									.Include("Tasks")
								.Where(u => u.CourseID == course.CourseID)
								.OrderBy (u=> u.SortOrder)
								.ToList();

			foreach (Unit unit in units) {
				unit.Tasks = unit.Tasks.Where(t => t.IsActive).ToList();
			}

			ViewBag.Units = units;
			ViewBag.CourseUserGroups = db.CourseUserGroups.Where(cug => cug.CourseID == course.CourseID)
								.OrderBy(cug => cug.Name)
								.ToList();

			return View(course);
		}

		public ActionResult EnterTaskGrades(string slug, int id) {
			Course course = Courses.GetCourseBySlug(slug);
			Task task = db.Tasks.Find(id);

			List<UserTaskData> taskData = db.UserTasks
											.Include("User")
											.Include("StudentFile")
											.Include("GradedFile")
											.Where(utd => utd.CourseID == course.CourseID && utd.TaskID == task.TaskID && utd.Task.IsActive)
											.ToList();

			List<CourseUser> users = db.CourseUsers
										.Include("User")
								.Where(cu => cu.CourseID == course.CourseID && cu.RoleID == (int)CourseUserRole.Student)
								.OrderBy(cu => cu.GroupID)
									.ThenBy(cu => cu.User.LastName)
								.ToList();


			ViewBag.CourseUsers = users;
			ViewBag.Task = task;
			ViewBag.UserTaskData = taskData;

			return View(course);
		}

		[HttpPost]
		public ActionResult EnterTaskGrades(string slug, int id, int[] grades) {
			Course course = Courses.GetCourseBySlug(slug);
			Task task = db.Tasks.Find(id);

			// store stuff

			return RedirectToAction("EnterTaskGrades");
		}

		public ActionResult UserTasks(string slug, int id) {
			Course course = Courses.GetCourseBySlug(slug);
			User user = db.Users.Find(id);

			List<UserTaskData> taskData = db.UserTasks
											.Include("Unit")
											.Include("Task")
											//.Include("StudentFile")
											//.Include("GradedFile")
											.Where(utd => utd.CourseID == course.CourseID && utd.UserID == user.UserID && utd.Task.IsActive)
											.OrderBy(utd => utd.Unit.SortOrder)
												.ThenBy(utd => utd.Task.SortOrder)
											.ToList();
		

			ViewBag.User = user;
			ViewBag.UserTaskData = taskData;

			return View(course);
		}



		public ActionResult UserInteractions(string slug, int id, int id2) {

			Course course = Courses.GetCourseBySlug(slug);
			Task task = db.Tasks.Find(id);
			Unit unit = task.Unit;
			User user = db.Users.Find(id2);

			// get the interactions and display?

			// threads for the unit, including posts
			List<InteractionThread> threads = Interactions.GetInteractionThreads(id);						
			List<InteractionPost> userReplies = new List<InteractionPost>();

			// remove posts not by the user or thread starter
			foreach (InteractionThread thread in threads) {
				if (thread.UserID == id2) {
					// keep all the posts when the thread was started by the user in question

					userReplies.AddRange(thread.Posts.Where(p => p.PostID != thread.Posts.First().PostID && p.UserID == id2));
				} else {

					// make sure this user made at least on post in the thread
					if (thread.Posts.Count(p => p.UserID == id2) > 0) {
						// keep the posts by the user and starter
						thread.Posts = thread.Posts.Where(p => p.UserID ==  thread.UserID || p.UserID == id2).ToList();

						userReplies.AddRange(thread.Posts.Where(p => p.UserID == id2));
					} else {
						thread.Posts = new List<InteractionPost>();
					}
				}
			}

			// remove remaining threads wihtout posts
			threads = threads.Where(t => t.Posts.Count > 0).ToList();

			ViewBag.TotalReplies = userReplies.Count;
			ViewBag.TotalWords = userReplies.Sum(p => Utility.WordCount(p.PostContentFormatted));
			ViewBag.Task = task;
			ViewBag.Course = course;
			ViewBag.Unit= unit;		
			ViewBag.User = user;

			return View(threads);
		}
		
		public ActionResult RemoveStudentFile(string slug, int id, int? id2) {

			UserTaskData userTaskData = db.UserTasks.SingleOrDefault(utd => utd.TaskID == id && utd.UserID == id2);

			// is this a post?
			if (userTaskData.PostID > 0) {

				// get the thread
				InteractionPost post = db.InteractionPosts.Find(userTaskData.PostID);
				InteractionThread thread = db.InteractionThreads.Include("posts").Where(t => t.ThreadID == post.ThreadID).SingleOrDefault();

				if (thread.Posts.Count == 1) {
					db.Database.ExecuteSqlCommand("DELETE FROM oe_interactions_threads WHERE threadid = {0}", thread.ThreadID);
					db.Database.ExecuteSqlCommand("DELETE FROM oe_interactions_posts WHERE postid = {0}", post.PostID);
					//db.InteractionPosts.Remove(post);
					//db.InteractionThreads.Remove(thread);

				} else {
					thread.IsDeleted = true;
				}


				userTaskData.PostID = 0;
				
			}


			userTaskData.StudentFileID = 0;
			userTaskData.StudentSubmitDate = null;
			userTaskData.TaskCompletionStatus = TaskCompletionStatus.NotStarted;

			db.SaveChanges();

			// email student and grader
			
			return Redirect("/grading/" + slug + "/entertaskgrades/" + id);
		}

		public ActionResult RemoveGraderFile(string slug, int id, int? id2) {

			UserTaskData userTaskData = db.UserTasks.SingleOrDefault(utd => utd.TaskID == id && utd.UserID == id2);

			userTaskData.GraderFileID = 0;
			userTaskData.GraderSubmitDate = null;

			db.SaveChanges();

			// email student and grader

			return Redirect("/grading/" + slug + "/entertaskgrades/" + id);
		}


		public ActionResult DownloadFiles(string slug, int id, int? id2, bool excludeUngraded = false) {

			Course course = Courses.GetCourseBySlug(slug);
			Task task = db.Tasks.Find(id);
			Unit unit= task.Unit;
			CourseUserGroup group = null;

			List<UserTaskData> userTasks = db.UserTasks.Where(utd => utd.TaskID == id && utd.StudentFileID > 0).ToList();

			if (id2.HasValue) {

				// get members of groups
				group = db.CourseUserGroups.Find(id2);
				List<int> groupUserIDs = db.CourseUsers.Where(cu => cu.GroupID == id2).Select(u => u.UserID).ToList();
				userTasks = userTasks.Where(utd => groupUserIDs.Contains(utd.UserID)).ToList();
				
			}


			ZipFile zip = new ZipFile();

			foreach (UserTaskData utd in userTasks) {

				// get all or get just ungraded
				if (!excludeUngraded || utd.GraderFileID == 0) {

					if (System.IO.File.Exists(utd.StudentFile.PhysicalPath)) {

						ZipEntry entry = zip.AddFile(utd.StudentFile.PhysicalPath);
						// customize file
						entry.FileName = StudentFile.GetFriendlyFilename(course, unit, task, utd.User, utd.StudentFile.Filename);
					}
				}
			}

			return new ZipFileResult(zip, course.CourseCode + course.Section + "-Unit" + unit.SortOrder + "-" + task.Name.Replace(" ","-") + (group != null ? "-" + group.Name.Replace(" ","-") : "") + ".zip");
		}

		[HttpPost]
		public ActionResult UpdateUserTaskNumericGrade(string slug, int id, int id2, string numericGrade) {

			User user = Users.GetLoggedInUser();
			UserTaskData userTask = db.UserTasks.SingleOrDefault(utd => utd.UserID == id2 && utd.TaskID == id);

			if (userTask != null) {

				userTask.GraderUserID = user.UserID;

				// enter grade
				Double numericGradeNum = -1;
				if (Double.TryParse(numericGrade, out numericGradeNum)) {

					if (userTask.NumericGrade != numericGradeNum) {
						userTask.GraderSubmitDate = DateTime.Now;
					}

					userTask.NumericGrade = numericGradeNum;

				}
				else {
					if (userTask.NumericGrade != numericGradeNum) {
						userTask.GraderSubmitDate = DateTime.Now;
					}

					userTask.NumericGrade = null;
				}
		

				db.SaveChanges();
			}

			// when a grade is changed via ajax
			return Json(new { success = true });
		}

		[HttpPost]
		public ActionResult UpdateUserTaskStatus(string slug, int id, int id2, string taskStatus) {

			User user = Users.GetLoggedInUser();
			UserTaskData userTask = db.UserTasks.SingleOrDefault(utd => utd.UserID == id2 && utd.TaskID == id);

			if (userTask != null) {

				userTask.GraderUserID = user.UserID;

				// set status
				int taskStatusInt = -1;
				if (Int32.TryParse(taskStatus, out taskStatusInt)) {
					TaskCompletionStatus completionStatus = (TaskCompletionStatus)taskStatusInt;

					// if it wasn't complete before
					if (completionStatus == TaskCompletionStatus.Completed && userTask.TaskStatus != taskStatusInt) {
						userTask.StudentSubmitDate = DateTime.Now;
						userTask.GraderSubmitDate = DateTime.Now;
					}

					userTask.TaskStatus = taskStatusInt;
				}

				db.SaveChanges();
			}

			// when a grade is changed via ajax
			return Json(new { success = true });
		}

		[HttpPost]
		public ActionResult UploadGraderFile(string slug, int id, int id2) {

			User user = Users.GetLoggedInUser();
			UserTaskData userTask = db.UserTasks.SingleOrDefault(utd => utd.UserID == id2 && utd.TaskID == id);

			if (userTask != null) {

				

				// is there a file?
				Guid uniqueID = Guid.NewGuid();
				string originalFilename = "";
				string originalExtension = "";
				int fileID = 0;
				HttpPostedFileBase httpFile = null;

				// save file
				if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0) {
					httpFile = Request.Files[0];

					originalFilename = Path.GetFileName(httpFile.FileName);
					originalExtension = Path.GetExtension(httpFile.FileName);

					string filePath = Path.Combine(Settings.GradedFilesLocation, uniqueID.ToString() + originalExtension);
					httpFile.SaveAs(filePath);

					GradedFile gradedFile = new GradedFile();
					gradedFile.UserID = user.UserID;
					gradedFile.UniqueID = uniqueID;
					gradedFile.ContentType = httpFile.ContentType;
					gradedFile.Length = httpFile.ContentLength;
					gradedFile.Filename = originalFilename;
					gradedFile.UploadedDate = DateTime.Now;

					// save the file to the database
					db.GradedFiles.Add(gradedFile);
					db.SaveChanges();

					// update teh File ID and grader userid
					userTask.GraderUserID = user.UserID;
					userTask.GraderFileID = gradedFile.FileID;
					db.SaveChanges();
				}

				
			}
			
			// when a file is uploaded
			return RedirectToAction("EnterTaskGrades", new { slug = slug, id = id });
			
		}

		/*
		[HttpPost]
		public ActionResult UpdateUserTask(string slug, int id, int id2, string numericGrade, string taskStatus, bool isAsync) {

			User user = Users.GetLoggedInUser();
			UserTaskData userTask = db.UserTasks.SingleOrDefault(utd => utd.UserID == id2 && utd.TaskID == id);

			if (userTask != null) {

				userTask.GraderUserID = user.UserID;

				// enter grade
				int numericGradeInt = -1;
				if (Int32.TryParse(numericGrade, out numericGradeInt)) {
					
					if (userTask.NumericGrade != numericGradeInt) {
						userTask.GraderSubmitDate = DateTime.Now;
					}
					
					userTask.NumericGrade = numericGradeInt;

				} else {
					if (userTask.NumericGrade != numericGradeInt) {
						userTask.GraderSubmitDate = DateTime.Now;
					}
					
					userTask.NumericGrade = null;
				}

				// set status
				int taskStatusInt = -1;
				if (Int32.TryParse(taskStatus, out taskStatusInt)) {
					TaskCompletionStatus completionStatus = (TaskCompletionStatus)taskStatusInt;
					
					// if it wasn't complete before
					if (completionStatus == TaskCompletionStatus.Completed && userTask.TaskStatus != taskStatusInt) {
						userTask.StudentSubmitDate = DateTime.Now;
						userTask.GraderSubmitDate = DateTime.Now;
					}
					
					userTask.TaskStatus = taskStatusInt;
				}

				// is there a file?
				Guid uniqueID = Guid.NewGuid();
				string originalFilename = "";
				string originalExtension = "";
				int fileID = 0;
				HttpPostedFileBase httpFile = null;

				// save file
				if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0) {
					httpFile = Request.Files[0];

					originalFilename = Path.GetFileName(httpFile.FileName);
					originalExtension = Path.GetExtension(httpFile.FileName);

					string filePath = Path.Combine(Settings.GradedFilesLocation, uniqueID.ToString() + originalExtension);
					httpFile.SaveAs(filePath);

					GradedFile gradedFile = new GradedFile();
					gradedFile.UserID = user.UserID;
					gradedFile.UniqueID = uniqueID;
					gradedFile.ContentType = httpFile.ContentType;
					gradedFile.Length = httpFile.ContentLength;
					gradedFile.Filename = originalFilename;
					gradedFile.UploadedDate = DateTime.Now;
					
					db.GradedFiles.Add(gradedFile);
					db.SaveChanges();

					fileID = gradedFile.FileID;

					userTask.GraderFileID = fileID;
				}

				db.SaveChanges();				
			}



			if (isAsync) {
				
				// when a grade is changed via ajax
				return Json(new {success = true });

			} else {

				// when a file is uploaded
				return RedirectToAction("EnterTaskGrades", new { slug = slug, id = id });
			}
		}
		*/

		[HttpPost]
		public ActionResult MassUpload(string slug, int id) {

			

			if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0) {

				User user = Users.GetLoggedInUser();
				
				HttpPostedFileBase httpFile = Request.Files[0];
				
				Guid tempID = Guid.NewGuid();
				string tempPath = Path.Combine(Settings.GradedFilesLocation, tempID.ToString());

				httpFile.SaveAs(tempPath);

				ZipFile zipFile = ZipFile.Read(tempPath);
				foreach (ZipEntry e in zipFile) {
					

					// get task and student ID
					string gradedFilename = e.FileName;
					int userID = 0;
					int taskID = 0;
					Match match = Regex.Match(gradedFilename, @"\[(\d+),(\d+)\]");

					if (match.Success && Int32.TryParse(match.Groups[1].Value, out taskID) && Int32.TryParse(match.Groups[2].Value, out userID)) {

						UserTaskData userData = db.UserTasks.SingleOrDefault(utd => utd.UserID == userID && utd.TaskID == taskID);

						Guid uniqueID = Guid.NewGuid();
						string originalExtension = Path.GetExtension(gradedFilename);
						string filePath = Path.Combine(Settings.GradedFilesLocation, uniqueID.ToString() + originalExtension);

						// create a new grader file
						GradedFile gradedFile = new GradedFile();
						gradedFile.UserID = (user != null) ? user.UserID : 0;
						gradedFile.UniqueID = uniqueID;
						gradedFile.ContentType = httpFile.ContentType;
						gradedFile.Length = httpFile.ContentLength;
						gradedFile.Filename = gradedFilename;
						gradedFile.UploadedDate = DateTime.Now;

						db.GradedFiles.Add(gradedFile);
						db.SaveChanges();

						// record to task
						userData.GraderFileID = gradedFile.FileID;
						userData.GraderSubmitDate = DateTime.Now;
						db.SaveChanges();


						// save this file?
						FileStream fileStream = new FileStream(filePath, FileMode.CreateNew);
						e.Extract(fileStream);
						fileStream.Close();

					}

				}

			}

			return RedirectToAction("EnterTaskGrades", new { slug = slug, id = id });
		}


		public ActionResult Email(string slug) {
			Course course = Courses.GetCourseBySlug(slug, false);
			User user = Users.GetLoggedInUser();

			// is the user a grader of this course?
			CourseUserGroup graderGroup = course.CourseUserGroups.Where(cug => cug.FacilitatorUserID ==  user.UserID).FirstOrDefault();


			ContactStudentsModel model = new ContactStudentsModel();

			model.SendACopy = true;
			model.Course = course;
			model.Students = db.CourseUsers
										.Include("Group")
										.Include("User")
										.Where(cu => cu.CourseID == course.CourseID && cu.RoleID == (int) CourseUserRole.Student)
										.OrderBy(cu => cu.GroupID)
											.ThenBy(cu => cu.User.LastName)
										.ToList();

			model.SelectedGroupID = (graderGroup != null) ? graderGroup.GroupID : -1;
			model.Subject = "";
			model.EmailText = "";

			return View(model);
		}

		[HttpPost]
		public ActionResult Email(string slug, ContactStudentsModel model) {

			Course course = Courses.GetCourseBySlug(slug);
			User user = Users.GetLoggedInUser();

			if (ModelState.IsValid) {

				// get the right users
				List<User> users = null;
				if (model.SelectedGroupID == -1) {
					users = course.CourseUsers.Where(cu=> cu.RoleID == (int) CourseUserRole.Student).Select(cu => cu.User).ToList();
				} else {
					users = course.CourseUsers.Where(cu => cu.GroupID == model.SelectedGroupID).Select(cu => cu.User).ToList();
				}

				if (model.SendACopy) {
					users.Add(user);
				}

				// send email to all people
				foreach (User student in users) {
					Emails.EnqueueEmail(user.Email, student.Email, model.Subject, model.EmailText, true);
				}

				


				return View("EmailSuccess", course);
			} else {
				model.Course = course;

				return View(model);
			}


			
		}

		public ActionResult GetUserTaskData(string slug, int id, int id2) {
			UserTaskData userTaskData = db.UserTasks
											.Include("User")
											.Include("Task")
											.Where(utd => utd.UserID == id2 && utd.TaskID == id)
											.SingleOrDefault();

			return Json(serializer.Serialize(userTaskData), JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public ActionResult SendGradedEmail(string slug, int id, int id2, string emailText) {
			
			User graderUser = Users.GetLoggedInUser();

			UserTaskData userTaskData = db.UserTasks
											.Include("User")
											.Include("Task")
											.Where(utd => utd.UserID == id2 && utd.TaskID == id)
											.SingleOrDefault();

			userTaskData.GraderComments = emailText;
			userTaskData.GraderUserID = graderUser.UserID;
			userTaskData.GradeSentDate = DateTime.Now;

			db.SaveChanges();

			// send email
			string subject = Emails.FormatEmail(Didache.Resources.emails.gradedtask_subject, userTaskData.Course, null, userTaskData.Task, null, null, null, null, null);
			string body = Emails.FormatEmail(Didache.Resources.emails.gradedtask_body, userTaskData.Course, null, userTaskData.Task, graderUser, null, userTaskData, null, null);

			Emails.EnqueueEmail(graderUser.Email, userTaskData.User.Email, subject, body, false);

			return Json(new {success= true}, JsonRequestBehavior.AllowGet);
		}

    }
}
