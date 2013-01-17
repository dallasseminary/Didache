using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Didache {
	public enum CourseUserRole {
		Student = 5,
		//Grader = 2, // no longer used
		Faculty = 3,
		Editor = 4,
		Faciliator = 8,
		All = 0
	}
}
