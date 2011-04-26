using System.Web.Mvc;

namespace Didache.Web.Areas.Admin {
	public class AdminAreaRegistration : AreaRegistration {
		public override string AreaName {
			get {
				return "Admin";
			}
		}

		public override void RegisterArea(AreaRegistrationContext context) {
			context.MapRoute(
				"Admin_grading",
				"admin/grading/{id}/{action}",
				new { controller = "Grading", action = "Index" }
			);

			context.MapRoute(
				"Admin_default",
				"admin/{controller}/{action}/{id}",
				new { controller = "Admin", action = "Index", id = UrlParameter.Optional }
			);

			context.MapRoute(
				"Admin_default_2",
				"admin/{controller}/{action}/{id}/{id2}",
				new { controller = "Admin", action = "Index", id = UrlParameter.Optional, id2 = UrlParameter.Optional }
			);
		}
	}
}
