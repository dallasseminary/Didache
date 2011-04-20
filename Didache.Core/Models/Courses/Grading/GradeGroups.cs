using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Didache {
	public class GradeGroups {

		public static List<GradeGroup> GetCourseGradeGroups(int courseID) {

			return (new DidacheDb())
					.GradeGroups
						.Include("GradeItems.Task")
					.Where(gg => gg.CourseID == courseID)
					.OrderBy(gg => gg.SortOrder)
					.ToList();

		}
	}
}
