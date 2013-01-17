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
	"community/Classmates",
	new { controller = "Community", action = "Classmates" }
);

			context.MapRoute(
				"Community_Friends",
				"community/Friends",
				new { controller = "Community", action = "Friends" }
			);


			context.MapRoute(
				"Community_feed",
				"community/feed",
				new { controller = "Community", action = "feed" }
			);




			// must be LAST
			context.MapRoute(
				"Community_name",
				"community/{name}",
				new { controller = "Community", action = "Display" }
			);




			// START: Groups
			context.MapRoute(
					"groups_home",
					"groups",
					new { controller = "Group", action = "Index" }
				);

			context.MapRoute(
				"groups_create",
				"groups/edit/{id}",
				new { controller = "Group", action = "Edit", id = UrlParameter.Optional }
			);

			context.MapRoute(
				"groups_view",
				"groups/{id}",
				new { controller = "Group", action = "View" }
			);

			// END: Groups
		

		}
	}
}
