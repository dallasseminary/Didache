using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Web.Script.Serialization;
using Didache;


namespace Didache.Web.Areas.Admin.Controllers
{
    public class CoursesController : Controller
    {
		private DidacheDb db = new DidacheDb();
		private EntityObjectSerializer serializer = new EntityObjectSerializer();
		
		//
        // GET: /Admin/Courses/

        public ActionResult Index()
        {
			List<Course> courses = Didache.Courses.GetCurrentlyRunningCourses();

			return View("List", courses);
        }

		public ActionResult BySession(int id) {

			List<Course> courses = Didache.Courses.GetCoursesBySession(id);

			return View("List", courses);
		}


		public ActionResult Units(int id) {

			List<Unit> units = Didache.Courses.GetCourseUnits(id);
			ViewBag.Course = Courses.GetCourse(id);

			return View(units);
		}


		public ActionResult Grading(int id) {

			ViewBag.GradeGroups = Didache.GradeGroups.GetCourseGradeGroups(id);
			ViewBag.Tasks = Courses.GetTasks(id);

			return View(Courses.GetCourse(id));
		}


		public ActionResult CourseEditor(int? id) {

			ViewBag.Sessions = db.Sessions.OrderByDescending(s => s.StartDate).ToList();
			ViewBag.Campuses = db.Campuses.OrderBy(s => s.Name).ToList();

			Course course = (id == null) ? null : db.Courses.Find(id);

			return View(course);
		}

		[HttpPost]
		[ValidateInput(false)]
		public ActionResult UpdateCourse(Course model) {
			if (model.CourseID > 0) {
				// EDIT MODE
				try {
					model = db.Courses.Find(model.CourseID);

					UpdateModel(model);

					db.SaveChanges();

					return Json(new { success = true, action = "edit", course = serializer.Serialize(model) });
				} catch (Exception ex) {

					ModelState.AddModelError("", "Edit Failure, see inner exception");

					Response.StatusCode = 500;
					return Json(new { success = false, action="edit", message = ex.ToString(), errors = GetErrors() });
				}
			} else {
				// ADD MODE
				if (ModelState.IsValid) {
					db.Courses.Add(model);
					db.SaveChanges();
					return Json(new { success = true, action = "add", course = serializer.Serialize(model) });
				} else {
					
					Response.StatusCode = 500;
					return Json(new { success = false, action = "add", message = "Invalid new model", errors = GetErrors() });
				}
			}
		}

		Dictionary<string,string[]> GetErrors() {
			return ModelState.ToDictionary(
						kvp => kvp.Key,
						kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
					);
		}

		public ActionResult Users(int id) {
			ViewBag.CourseUserGroups = db.CourseUserGroups.Where(cug => cug.CourseID == id).ToList();
			return View(Courses.GetCourse(id));
		}

		public ActionResult UpdateUserInCourse(int userID, int courseID, int roleID, int groupID, bool remove) {

			if (remove) {
				CourseUsers.RemoveUserFromCourse(courseID, userID, (CourseUserRole)roleID);
			} else {
				CourseUsers.AddUserToCourse(courseID, userID, groupID, (CourseUserRole)roleID);
			}

			// get it
			CourseUser courseUser = db.CourseUsers
											.Include("User")
											.SingleOrDefault(cu => cu.UserID == userID && cu.CourseID == courseID && cu.RoleID == roleID);

			return Json(serializer.Serialize(courseUser));
		}


		public ActionResult UserGroups(int id) {

			return View(Courses.GetCourse(id));
		}

		[HttpPost]
		public ActionResult UpdateUserGroup(CourseUserGroup model) {

			if (model.GroupID > 0) {
				// EDIT MODE
				try {
					model = db.CourseUserGroups.Find(model.GroupID);

					UpdateModel(model);

					db.SaveChanges();

					return Json(new { success = true, action = "edit", model = serializer.Serialize(model) });
				}
				catch (Exception ex) {

					ModelState.AddModelError("", "Edit Failure, see inner exception");

					Response.StatusCode = 500;
					return Json(new { success = false, action = "edit", message = ex.ToString(), errors = GetErrors() });
				}
			}
			else {
				// ADD MODE
				if (ModelState.IsValid) {
					db.CourseUserGroups.Add(model);
					db.SaveChanges();
					return Json(new { success = true, action = "add", model = serializer.Serialize(model) });
				}
				else {

					Response.StatusCode = 500;
					return Json(new { success = false, action = "add", message = "Invalid new model", errors = GetErrors() });
				}
			}
		}

		/*
		[HttpPost]
		public ActionResult UpdateTask(int id) {

			var didacheDb = new DidacheDb();

			var reader = new JsonFx.Json.JsonReader();
			dynamic output = reader.Read(HttpUtility.UrlDecode(Request.Form.ToString()));


			Task task = didacheDb.Tasks.Find(output.taskid);
			task.Name = output.name;
			DateTime dueDate = DateTime.MinValue;
			if (DateTime.TryParse(output.duedate, out dueDate)) {
				task.DueDate = dueDate;
			}

			didacheDb.SaveChanges();

			return Json(new {success= true});
		}
		*/

		[HttpPost]
		//[ValidateInput(false)]
		public ActionResult UpdateTask(Task model) {

			if (model.TaskID > 0) {
				// EDIT MODE
				try {
					model = db.Tasks.Find(model.TaskID);

					UpdateModel(model);

					db.SaveChanges();

					// reciprocate
					if (model.RelatedTaskID > 0) {
						Task relatedTask = db.Tasks.Find(model.RelatedTaskID);
						if (relatedTask != null) {
							relatedTask.RelatedTaskID = model.TaskID;
							db.SaveChanges();
						}
					}

					
					return Json(new { success = true, task = serializer.Serialize(model) });
				} catch (Exception ex) {

					Response.StatusCode = 500;
					return Json(new { success = false, task = serializer.Serialize(model), message = "Update error", errors = GetErrors() });
				}				
			} else {

				// ADD MODE
				if (ModelState.IsValid) {
					db.Tasks.Add(model);
					db.SaveChanges();

					// reciprocate
					if (model.RelatedTaskID > 0) {
						Task relatedTask = db.Tasks.Find(model.RelatedTaskID);
						if (relatedTask != null) {
							relatedTask.RelatedTaskID = model.TaskID;
							db.SaveChanges();
						}
					}

					// create user tasks
					List<CourseUser> users = db.CourseUsers.Where(cu => cu.CourseID == model.CourseID && cu.RoleID == (int) CourseUserRole.Student).ToList();
					foreach (CourseUser user in users) {

						UserTaskData utd = new UserTaskData() {
							TaskID = model.TaskID,
							CourseID = model.CourseID,
							UnitID = model.UnitID,
							GraderComments = "",
							GraderSubmitDate = null,
							GraderUserID = 0,
							GradeStatus = 0,
							LetterGrade = "",
							NumericGrade = null,
							StudentComments = "",
							StudentSubmitDate = null,
							TaskCompletionStatus = TaskCompletionStatus.NotStarted,
							TaskData = "",
							GraderFileID = 0,
							PostID = 0,
							StudentFileID = 0,
							UserID = user.UserID
						};

						db.UserTasks.Add(utd);

					}

					db.SaveChanges();

					return Json(new { success = true, task = serializer.Serialize(model) });
				} else {
					Response.StatusCode = 500;
					return Json(new { success = false, task = serializer.Serialize(model), message = "Add error", errors = GetErrors() });
				}				
			}
		}

		[HttpPost]
		//[ValidateInput(false)]
		public ActionResult UpdateUnit(Unit model) {

			if (model.UnitID > 0) {
				// EDIT MODE
				try {
					model = db.Units.Find(model.UnitID);

					UpdateModel(model);

					db.SaveChanges();

					return Json(new { success = true, unit = serializer.Serialize(model) });
				}
				catch (Exception ex) {

					Response.StatusCode = 500;
					return Json(new { success = false, unit = serializer.Serialize(model), message = "Update error", errors = GetErrors() });
				}
			}
			else {

				// ADD MODE
				if (ModelState.IsValid) {
					db.Units.Add(model);
					db.SaveChanges();

					return Json(new { success = true, unit = serializer.Serialize(model) });
				}
				else {
					Response.StatusCode = 500;
					return Json(new { success = false, unit = serializer.Serialize(model), message = "Add error", errors = GetErrors() });
				}
			}
		}


		[HttpPost]
		public ActionResult UpdateUnitSorting(int id) {

			var didacheDb = new DidacheDb();

			string data = HttpUtility.UrlDecode(Request.Form.ToString());

			//dynamic newValue = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Dynamic.>(data);
			var reader = new JsonFx.Json.JsonReader();
			dynamic output = reader.Read(data);

			foreach (var unitInfo in output) {

				// get and update the unit
				Unit unit = didacheDb.Units.Find(unitInfo.unitid);
				unit.SortOrder = unitInfo.sortorder;


				foreach (var taskInfo in unitInfo.tasks) {
					// get and update the task
					Task task = didacheDb.Tasks.Find(taskInfo.taskid);					
					task.SortOrder = taskInfo.sortorder;				
	
				}

			}

			string errorMessage = "";
			bool success = false;

			try {
				didacheDb.SaveChanges();
				success = true;
			} catch {
				var entries = didacheDb.GetValidationErrors();
				foreach (var entry in entries) {
					errorMessage += "[" + entry.Entry.Entity.ToString() + "]\n";
					foreach (var error in entry.ValidationErrors) {
						errorMessage += error.PropertyName + " = " + error.ErrorMessage + "; ";
					}
				}
			}

			
			return Json(new { success = success, errorMessage = errorMessage});
		}


		public ActionResult Files(int id) {

			ViewBag.CourseFileGroups = CourseFiles.GetCourseFileGroups(id);
			Course course = Courses.GetCourse(id);

			return View(course);


		}

		[HttpPost]
		public ActionResult AddFileGroup(int id) {

			var didacheDb = new DidacheDb();

			// get data
			var reader = new JsonFx.Json.JsonReader();
			dynamic json = reader.Read(HttpUtility.UrlDecode(Request.Form.ToString()));

			CourseFileGroup group = new CourseFileGroup();
			group.CourseID = id;
			group.SortOrder = 9999;
			group.Name = json.name;

			didacheDb.CourseFileGroups.Add(group);
			didacheDb.SaveChanges();


			return Json(new {groupid = group.GroupID, name=json.name, courseid= id});
		}


		[HttpPost]
		public ActionResult FileUpload(int id) {

			// get groupID from form data JSON
			int courseID = id;
			int groupID = 0;			
			string data = Request.Form["groupid"];
			Int32.TryParse(data, out groupID);

			// prep objects
			Guid uniqueID = Guid.NewGuid();
			string originalFilename = "";
			string originalExtension = "";
			string title = "";
			int fileID = 0;
			HttpPostedFileBase file = null;

			CourseFileAssociation cfa = null;
			CourseFile courseFile = null;

			// save file
			if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0) {
				file = Request.Files[0];

				originalFilename = Path.GetFileName(file.FileName);
				originalExtension = Path.GetExtension(file.FileName);
				title = originalFilename.Replace(originalExtension, ""); ;

				string filePath = Path.Combine(HttpContext.Server.MapPath("~/uploads"), uniqueID.ToString() + originalExtension);
				file.SaveAs(filePath);

				courseFile = new CourseFile();
				courseFile.UserID = Didache.Users.GetLoggedInUser().UserID;
				courseFile.UniqueID = uniqueID;
				courseFile.ContentType = file.ContentType;
				courseFile.Length = file.ContentLength;
				courseFile.Filename = originalFilename;
				courseFile.UploadedDate = DateTime.Now;
				courseFile.Title = title;
				courseFile.Description = "";

				db.CourseFiles.Add(courseFile);				
				db.SaveChanges();


				// create association
				cfa = new CourseFileAssociation();
				cfa.FileID = courseFile.FileID;
				cfa.GroupID = groupID;
				cfa.SortOrder = 999;
				cfa.DateAdded = DateTime.Now;

				db.CourseFileAssociations.Add(cfa);
				db.SaveChanges();


			}

			/*
			return Json(serializer.Serialize(new {
				success = (file!= null),
				cfa = cfa
			}));
			*/

			return Json(serializer.Serialize(cfa));

			// do processing
			object returnObject = new {
				success = (file != null),
				fileid = fileID,
				filelength = (file != null) ? file.ContentLength : 0,
				filename = originalFilename,
				title = title,
				user = Didache.Users.GetLoggedInUser().FullName
			};

			return Json(returnObject);
		}

		[HttpPost]
		public ActionResult UpdateFileSorting(int id) {

			var didacheDb = new DidacheDb();

			string data = HttpUtility.UrlDecode( Request.Form.ToString() );

			//dynamic newValue = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Dynamic.>(data);
			var reader = new JsonFx.Json.JsonReader();
			dynamic output = reader.Read(data);


			// get groups
			List<CourseFileGroup> groups = db.CourseFileGroups.Where(cfg => cfg.CourseID == id).ToList();
			List<int> groupIDs = groups.Select(cfg => cfg.GroupID).ToList();
			// get files
			List<CourseFileAssociation> courseFiles = db.CourseFileAssociations.Where(cfa => groupIDs.Contains(cfa.GroupID)).ToList();

			foreach (var groupInfo in output) {
				CourseFileGroup fileGroup = groups.SingleOrDefault(cfg => cfg.GroupID == groupInfo.groupid);
				fileGroup.SortOrder = groupInfo.sortorder;

				foreach (var fileInfo in groupInfo.files) {
					CourseFileAssociation currentCourseFile = courseFiles.SingleOrDefault(cfa => cfa.FileID == fileInfo.fileid);

					// has the group changed?
					if (currentCourseFile.GroupID != fileGroup.GroupID) {
						
						// create a new one with the new group
						CourseFileAssociation newCourseFile = new CourseFileAssociation() {
							FileID = currentCourseFile.FileID,
							GroupID = fileGroup.GroupID,
							DateAdded = currentCourseFile.DateAdded,
							SortOrder = fileInfo.sortorder
						};

						db.CourseFileAssociations.Remove(currentCourseFile);
						db.CourseFileAssociations.Add(newCourseFile);

					}
					else {
						// update current one
						currentCourseFile.SortOrder = fileInfo.sortorder;
					}
				
				}
			}
			db.SaveChanges();

			/*
			foreach (var groupInfo in output) {

				// get and update the group
				CourseFileGroup fileGroup = didacheDb.CourseFileGroups.Find(groupInfo.groupid);
				fileGroup.SortOrder = groupInfo.sortorder;
				//fileGroup.Name = groupInfo.name;
				

				// TEST 2: get all existing, update, remove missing


				// get exsiting files
				List<CourseFileAssociation> courseFiles = didacheDb.CourseFileAssociations.Where(cfa => cfa.GroupID == fileGroup.GroupID).ToList();
				

				foreach (var fileInfo in groupInfo.files) {
					// find the file
					CourseFileAssociation cfa = courseFiles.Find(c => c.FileID == fileInfo.fileid);
					if (cfa != null) {
						// update
						cfa.SortOrder = fileInfo.sortorder;
						// add to change list
						//changedFiles.Add(cfa);
						courseFiles.Remove(cfa);
					} else {
						cfa = new CourseFileAssociation();
						cfa.GroupID = fileGroup.GroupID;
						cfa.FileID = fileInfo.fileid;
						cfa.SortOrder = fileInfo.sortorder;
						didacheDb.CourseFileAssociations.Add(cfa);
					}
				}

				// remove all remaining files
				foreach (CourseFileAssociation notUpdated in courseFiles) {
					didacheDb.CourseFileAssociations.Remove(notUpdated);
				}

			}
			*/

			didacheDb.SaveChanges();


			// need to deserialize this and update all the groups and files

			return Json(new { success= true});
		}

		[HttpPost]
		[ValidateInput(false)]
		public ActionResult UpdateCourseFileGroup(CourseFileGroup model) {
			if (model.GroupID > 0) {
				// EDIT MODE
				try {
					model = db.CourseFileGroups.Find(model.GroupID);

					UpdateModel(model);

					db.SaveChanges();

					return Json(new { success = true, action = "edit", courseFileGroup = serializer.Serialize(model) });
				}
				catch (Exception ex) {

					ModelState.AddModelError("", "Edit Failure, see inner exception");

					Response.StatusCode = 500;
					return Json(new { success = false, action = "edit", message = ex.ToString(), errors = GetErrors() });
				}
			}
			else {
				// ADD MODE
				if (ModelState.IsValid) {
					db.CourseFileGroups.Add(model);
					db.SaveChanges();
					return Json(new { success = true, action = "add", courseFileGroup = serializer.Serialize(model) });
				}
				else {

					Response.StatusCode = 500;
					return Json(new { success = false, action = "add", message = "Invalid new model", errors = GetErrors() });
				}
			}
		}

		[HttpPost]
		public ActionResult UpdateUserGroupSorting(int id) {

			var didacheDb = new DidacheDb();

			string data = HttpUtility.UrlDecode(Request.Form.ToString());

			//dynamic newValue = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Dynamic.>(data);
			var reader = new JsonFx.Json.JsonReader();
			dynamic output = reader.Read(data);


			// get groups
			List<CourseUserGroup> groups = db.CourseUserGroups.Where(cfg => cfg.CourseID == id).ToList();
			List<int> groupIDs = groups.Select(cfg => cfg.GroupID).ToList();
			
			// get files
			List<CourseUser> courseUsers = db.CourseUsers.Where(cu => groupIDs.Contains(cu.GroupID)).ToList();

			foreach (var groupInfo in output) {
				CourseUserGroup userGroup = groups.SingleOrDefault(cug => cug.GroupID == groupInfo.groupid);

				foreach (var userInfo in groupInfo.users) {
					CourseUser courseUser = courseUsers.SingleOrDefault(cu => cu.UserID == userInfo.userid);

					// has the group changed?
					if (courseUser.GroupID != userGroup.GroupID) {

						// create a new one with the new group
						CourseUser newCourseFile = new CourseUser() {
							UserID = courseUser.UserID,
							GroupID = userGroup.GroupID,
							RoleID = (int)CourseUserRole.Student
						};

						db.CourseUsers.Remove(courseUser);
						db.CourseUsers.Add(newCourseFile);

					}
					else {
						// do nothing since we don't care about the order
					}

				}
			}
			db.SaveChanges();

			/*
			foreach (var groupInfo in output) {

				// get and update the group
				CourseFileGroup fileGroup = didacheDb.CourseFileGroups.Find(groupInfo.groupid);
				fileGroup.SortOrder = groupInfo.sortorder;
				//fileGroup.Name = groupInfo.name;
				

				// TEST 2: get all existing, update, remove missing


				// get exsiting files
				List<CourseFileAssociation> courseFiles = didacheDb.CourseFileAssociations.Where(cfa => cfa.GroupID == fileGroup.GroupID).ToList();
				

				foreach (var fileInfo in groupInfo.files) {
					// find the file
					CourseFileAssociation cfa = courseFiles.Find(c => c.FileID == fileInfo.fileid);
					if (cfa != null) {
						// update
						cfa.SortOrder = fileInfo.sortorder;
						// add to change list
						//changedFiles.Add(cfa);
						courseFiles.Remove(cfa);
					} else {
						cfa = new CourseFileAssociation();
						cfa.GroupID = fileGroup.GroupID;
						cfa.FileID = fileInfo.fileid;
						cfa.SortOrder = fileInfo.sortorder;
						didacheDb.CourseFileAssociations.Add(cfa);
					}
				}

				// remove all remaining files
				foreach (CourseFileAssociation notUpdated in courseFiles) {
					didacheDb.CourseFileAssociations.Remove(notUpdated);
				}

			}
			*/

			didacheDb.SaveChanges();


			// need to deserialize this and update all the groups and files

			return Json(new { success = true });
		}



		/*
		public ActionResult Unit(int? id, int? courseID) {
			Unit unit = db.Units.SingleOrDefault(u => u.UnitID == id);
			if (unit == null)
				unit = new Unit();

			return View(unit);
		}

		[HttpPost]
		//[ValidateInput(false)]
		public ActionResult Unit(Unit model) {

			if (model.UnitID > 0) {
				// EDIT MODE
				try {
					model = db.Units.Find(model.UnitID);

					UpdateModel(model);

					db.SaveChanges();

					//return RedirectToAction("units", new { id = model.CourseID });
					return View("DialogClose");
				} catch (Exception ex) {
					ModelState.AddModelError("", "Edit Failure, see inner exception: " + ex.ToString());
				}

				return View(model);
			} else {

				// ADD MODE
				if (ModelState.IsValid) {
					db.Units.Add(model);
					db.SaveChanges();
					//return RedirectToAction("units", new { id = model.CourseID });
				} else {
					//return View(model);
				}

				return View("DialogClose");
			}

		}

		public ActionResult Tasks(int id) {

			List<Task> tasks = Didache.Tasks.GetUnitTasks(id);

			return View(tasks);
		}


		
		public ActionResult Task(int? id, int? unitID) {
			Task task = db.Tasks.SingleOrDefault(t => t.TaskID == id);
			if (task == null)
				task = new Task();

			return View(task);
		}
		
		[HttpPost]
		//[ValidateInput(false)]
		public ActionResult Task(Task model) {

			if (model.TaskID > 0) {
				// EDIT MODE
				try {
					model = db.Tasks.Find(model.TaskID);

					UpdateModel(model);

					db.SaveChanges();

					//return RedirectToAction("tasks", new { id = model.UnitID });					
					return View("DialogClose");
				} catch (Exception ex) {
					ModelState.AddModelError("", "Edit Failure, see inner exception: " + ex.ToString());
				}

				return View(model);
			} else {

				// ADD MODE
				if (ModelState.IsValid) {
					db.Tasks.Add(model);
					db.SaveChanges();
					//return RedirectToAction("tasks", new { id = model.UnitID });
				} else {
					//return View(model);
				}

				return View("DialogClose");
			}

		}

		
		public ActionResult Edit(int? id)
		{
			Course course = db.Courses.SingleOrDefault(s => s.CourseID == id);
			if (course == null)
				course = new Course() { CourseID = 0 };
			return View(course);
		}

		[HttpPost]
		[ValidateInput(false)]
		public ActionResult Edit(Course model)
		{
			if (model.CourseID > 0)
			{
				// EDIT MODE
				try
				{
					model = db.Courses.Find(model.CourseID);

					UpdateModel(model);

					db.SaveChanges();

					//return RedirectToAction("Courses", new { id = model.ID });
					return RedirectToAction("Index");
				}
				catch (Exception)
				{
					ModelState.AddModelError("", "Edit Failure, see inner exception");
				}
				return View(model);
			}
			else
			{
				// ADD MODE
				if (ModelState.IsValid)
				{
					db.Courses.Add(model);
					db.SaveChanges();
					return RedirectToAction("Index");
				}
				else
				{
					return View(model);
				}
			}
		}
		 * */

    }
}
