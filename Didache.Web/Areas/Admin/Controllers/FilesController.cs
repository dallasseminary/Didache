using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Didache.Web.Areas.Admin.Controllers
{
    public class FilesController : Controller
    {
        //
        // GET: /Admin/Files/

		DidacheDb db = new DidacheDb();

        public ActionResult Index(string title, int? userID, int? courseID)
        {

			List<CourseFile> filesList = null;

			if (!String.IsNullOrEmpty(title) || userID != null || courseID != null) {

				var fileQuery = db.CourseFiles
									.Include("User")
									.Include("CourseFileAssociations.CourseFileGroup.Course")
									.AsQueryable();

				if (!String.IsNullOrEmpty(title)) {
					fileQuery = fileQuery.Where(f => f.Title.Contains(title) || f.Filename.Contains(title));
				}

				if (userID != null && userID > 0) {
					fileQuery = fileQuery.Where(f => f.UserID == userID);
				}

				if (courseID != null && courseID > 0) {

					fileQuery = fileQuery.Where(f => 
													db.CourseFileAssociations
														.Where(cfa => db.CourseFileGroups
																			.Where(cfg => cfg.CourseID == courseID)
																			.Select(cfg => cfg.GroupID).Contains(cfa.GroupID))
														.Select(cfa => cfa.FileID)
														.Contains(f.FileID)
													);
				}

				filesList = fileQuery.ToList();
				
			}
			
			//db.Database.ExecuteSqlCommand
			//db.Courses.SqlQuery("ADFDS").ToList();

			// USERS dropdown
			var usersWithFiles = db.Users
								.Where(u => db.CourseFiles.Select(f => f.UserID).Contains(u.UserID))
								.OrderBy(u => u.LastName)
								.ToList();

			usersWithFiles.Insert(0, new Didache.User() { UserID = 0, LastName = "-- none --" });
			ViewBag.UserSelectList = new SelectList(usersWithFiles, "UserID", "FormattedNameLastFirst", userID); ;
			
			// Courses DropDown
			var courses = db.Courses
							.Include("Session")
							.OrderByDescending(c => c.Session.StartDate)
								.ThenBy(c => c.CourseCode)
								.ThenBy(c => c.Section)
							.ToList();
			//ViewBag.CourseGroups = new Didache.HtmlHelpers
			List<Didache.HtmlHelpers.GroupedSelectListItem> courseGroupList = courses.AsQueryable().Select(c => new Didache.HtmlHelpers.GroupedSelectListItem() { 
												Text = c.CourseCode + c.Section,
												Value = c.CourseID.ToString(),
												Selected = c.CourseID == courseID,
												GroupName = c.Session.SessionCode + c.Session.SessionYear,
												GroupKey = c.SessionID.ToString()}).ToList();
			courseGroupList.Insert(0, new HtmlHelpers.GroupedSelectListItem() { GroupKey = "0", GroupName = "None", Text = "None", Value = "0" });

			ViewBag.CourseGroupList = courseGroupList;

			//ViewBag.Courses = 
 
			//Didache.HtmlHelpers.GroupedSelectListItem s = new HtmlHelpers.GroupedSelectListItem() { 

			return View(filesList);
        }


		public ActionResult Delete(int id) {
			CourseFile file = db.CourseFiles.Find(id);
	
			db.CourseFiles.Remove(file);
			db.SaveChanges();

			return RedirectToAction("Index");
		}


		public ActionResult Edit(int id) {
			CourseFile file = db.CourseFiles.Find(id);

			return View(file);
		}

		[HttpPost]
		public ActionResult Edit(int id, FormCollection collection) {
			CourseFile file = db.CourseFiles.Find(id);

			UpdateModel(file);

			if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0) {
				// save it!
				Request.Files[0].SaveAs(file.PhysicalPath);
				file.ContentType = Request.Files[0].ContentType;
				file.Length = Request.Files[0].ContentLength;
			}

			db.SaveChanges();

			return View(file);

			//return RedirectToAction("Index");
		}

    }


}
