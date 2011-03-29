using System.Web.Mvc;

namespace Didache.Web.Areas.Community {
	public class CommunityAreaRegistration : AreaRegistration {
		public override string AreaName {
			get {
				return "Community";
			}
		}

		public override void RegisterArea(AreaRegistrationContext context) {
			context.MapRoute(
				"Community_default",
				"profiles",
				new { controller = "Community", action = "Index" }
			);

			context.MapRoute(
				"Community_edit",
				"Community/edit",
				new { controller = "Community", action = "Edit" }
			);

			context.MapRoute(
				"Community_name",
				"Community/{name}",
				new { controller = "Community", action = "Display" }
			);
		}
	}
}
