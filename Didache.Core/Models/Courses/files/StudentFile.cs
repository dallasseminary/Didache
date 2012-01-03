using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.IO;
using System.Text.RegularExpressions;

namespace Didache {

	public class StudentFile : BaseFile {
		public string FileUrl {
			get {
				return "/courses/studentfile/" + FileID + "/" + EncodedFilename;
			}
		}

		public string PhysicalPath {
			get {
				return Path.Combine(Settings.StudentFilesLocation, UniqueID + Path.GetExtension(Filename));
			}
		}

		public static string GetFriendlyFilename(Course course, Unit unit, Task task, User user, string filename) {
			return Regex.Replace(course.CourseCode + course.Section + 
				"-Unit" + unit.SortOrder + "-" + 
				task.Name.Replace(" ", "-") + "-" +
				user.FullName.Replace(" ", "-"), @"[\*\!\$\%\?;:]", "")
				+ "[" + task.TaskID + "," + user.UserID + "]" + Path.GetExtension(filename);
		}

	}
}
