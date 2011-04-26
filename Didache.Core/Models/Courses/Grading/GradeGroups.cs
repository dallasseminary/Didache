using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Didache {
	public class GradeGroups {

		public static List<GradeGroup> GetCourseGradeGroups(int courseID) {

			List<GradeGroup> gradeGroups = (new DidacheDb())
					.GradeGroups
						.Include("GradeItems.Task")
					.Where(gg => gg.CourseID == courseID)
					.OrderBy(gg => gg.SortOrder)
					.ToList();

			foreach (GradeGroup group in gradeGroups) {
				List<GradeItem> gradeItems = group.GradeItems.ToList();

				gradeItems.Sort(delegate(GradeItem a, GradeItem b) {
					return a.SortOrder.CompareTo(b.SortOrder);
				});

				group.GradeItems = gradeItems;
			}

			return gradeGroups;
		}
	}
}
