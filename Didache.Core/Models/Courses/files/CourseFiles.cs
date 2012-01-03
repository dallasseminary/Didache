using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;

namespace Didache  {
	public class CourseFiles {
		public static List<CourseFileGroup> GetCourseFileGroups(int courseID) {
			List<CourseFileGroup> groups = new DidacheDb()
				.CourseFileGroups
					.Include("CourseFileAssociations.CourseFile.User")
				.Where(g => g.CourseID == courseID)
				.OrderBy(g => g.SortOrder)
				.ToList();

			foreach (CourseFileGroup group in groups) {
				List<CourseFileAssociation> associations = group.CourseFileAssociations.ToList();

				associations.Sort(delegate(CourseFileAssociation a, CourseFileAssociation b) {
					return a.SortOrder.CompareTo(b.SortOrder);
				});

				group.CourseFileAssociations = associations;
			}


			return groups;
		}

		public static StudentFile SaveStudentFile(int userID, UserTaskData userData, HttpPostedFileBase file) {
			DidacheDb db = new DidacheDb();
			
			StudentFile studentFile = null;

			if (file.ContentLength > 0) {		



				// setup data
				studentFile = new StudentFile();
				studentFile.UserID = userID;
				studentFile.UploadedDate = DateTime.Now;
				studentFile.UniqueID = Guid.NewGuid();
				studentFile.ContentType = file.ContentType;
				studentFile.Length = file.ContentLength;

				// setup physical file
				string extension = Path.GetExtension(file.FileName);
				string filenameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
				studentFile.Filename = Path.GetFileName(file.FileName);
				string savePath = Path.Combine(Settings.StudentFilesLocation, studentFile.UniqueID.ToString() + extension);

				// check file type
				if (!String.IsNullOrWhiteSpace(userData.Task.FileTypesAllowed)) {



					// fail?
				}


				file.SaveAs(savePath);

				// save file info to DB
				db.StudentFiles.Add(studentFile);
				db.SaveChanges();
			}

			return studentFile;
		}		

		public static CourseFileAssociation GetCourseSyllabus(int courseID) {
			return new DidacheDb().CourseFileAssociations.Where(cfa => 
							cfa.CourseFileGroup.CourseID == courseID && 
							cfa.IsActive == true &&
							cfa.CourseFile.Title.Contains("Syllabus"))
								.OrderBy(cfa=>cfa.SortOrder)
								.FirstOrDefault();
		}

	}
}