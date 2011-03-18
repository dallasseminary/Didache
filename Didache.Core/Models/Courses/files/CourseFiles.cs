using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Didache  {
	public class CourseFiles {
		public static List<CourseFileGroup> GetCourseFileGroups(int courseID) {
			return new DidacheDb()
				.CourseFileGroups
					.Include("CourseFileAssociations").Include("CourseFiles")
				.Where(g => g.CourseID == courseID).ToList();
		}
	}
}
