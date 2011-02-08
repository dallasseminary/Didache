using System.Web.Mvc;

namespace Didache.Web.Areas.Profiles {
	public class ProfilesAreaRegistration : AreaRegistration {
		public override string AreaName {
			get {
				return "Profiles";
			}
		}

		public override void RegisterArea(AreaRegistrationContext context) {
			context.MapRoute(
				"Profiles_default",
				"profiles",
				new { controller="Profiles", action = "Index" }
			);

			context.MapRoute(
				"Profiles_edit",
				"profiles/edit",
				new { controller = "Profiles", action = "Edit" }
			);

			context.MapRoute(
				"Profiles_name",
				"profiles/{name}",
				new { controller = "Profiles", action = "Display" }
			);
		}
	}
}
