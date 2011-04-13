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
				"community",
				new { controller = "Community", action = "Index" }
			);

			context.MapRoute(
				"Community_name",
				"community/{name}",
				new { controller = "Community", action = "Display" }
			);
		}
	}
}
