using System.Web.Mvc;

namespace Didache.Web.Areas.Students {
	public class StudentsAreaRegistration : AreaRegistration {
		public override string AreaName {
			get {
				return "Students";
			}
		}

		public override void RegisterArea(AreaRegistrationContext context) {
			// special one for just /courses/
			context.MapRoute(
				"Courses_home",
				"courses",
				new { controller = "Courses", action = "Index" }
			);

			// discussion
			context.MapRoute(
				"Courses_TaskInfo",
				"courses/api/{action}/{userid}/{taskid}",
				new { controller = "Api", action = "taskstatus", userid = UrlParameter.Optional, taskid = UrlParameter.Optional }
			);


			// discussion
			context.MapRoute(
				"Courses_Discussion",
				"courses/{slug}/discussion/{action}/{id}",
				new { controller = "Discussion", action = "Index", id = UrlParameter.Optional }
			);


			// main six tabs
			context.MapRoute(
				"Courses",
				"courses/{slug}/{action}/{id}",
				new { controller = "Courses", action = "Schedule", id = UrlParameter.Optional }
			);
		}
	}
}
