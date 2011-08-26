using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Didache {
	public class Settings {
		public static string CourseFilesLocation {
			get {
				return System.Configuration.ConfigurationManager.AppSettings["CourseFilesLocation"];
			}
		}
		public static string StudentFilesLocation {
			get {
				return System.Configuration.ConfigurationManager.AppSettings["StudentFilesLocation"];
			}
		}
		public static string GradedFilesLocation {
			get {
				return System.Configuration.ConfigurationManager.AppSettings["GradedFilesLocation"];
			}
		}

		public static string PlayerFilesLocation {
			get {
				return System.Configuration.ConfigurationManager.AppSettings["PlayerFilesLocation"];
			}
		}

		public static bool RequireSsl {
			get {
				return System.Configuration.ConfigurationManager.AppSettings["RequireSsl"] == "true" ;
			}
		}

	}
}
