using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
	}
}
