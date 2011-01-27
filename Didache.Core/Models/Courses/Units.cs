using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Didache {
	public class Units {
		public static List<Unit> GetCourseUnits(int courseID) {
			return new DidacheDb().Units
				.Where(u => u.CourseID == courseID)
				.OrderBy(u => u.SortOrder).ThenBy(u => u.StartDate)
				.ToList();
		}
	}
}
