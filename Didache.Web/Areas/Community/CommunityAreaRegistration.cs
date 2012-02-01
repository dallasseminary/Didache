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
				"Community_Search",
				"community/search",
				new { controller = "Community", action = "search" }
			);

			context.MapRoute(
				"Community_Classmates",
				"community/classmates",
				new { controller = "Community", action = "Classmates" }
			);


			context.MapRoute(
				"Community_feed",
				"community/feed",
				new { controller = "Community", action = "feed" }
			);

			context.MapRoute(
				"Community_name",
				"community/{name}",
				new { controller = "Community", action = "Display" }
			);
		}
	}
}
