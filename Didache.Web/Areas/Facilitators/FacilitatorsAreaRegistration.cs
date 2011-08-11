using System.Web.Mvc;

namespace Didache.Web.Areas.Facilitators {
	public class FacilitatorsAreaRegistration : AreaRegistration {
		public override string AreaName {
			get {
				return "Facilitators";
			}
		}

		public override void RegisterArea(AreaRegistrationContext context) {
			
			// grading home
			context.MapRoute(
				"Grading_default",
				"grading/{sessionID}",
				new { controller = "Home", action = "Index", sessionID = UrlParameter.Optional }
			);

			// task view
			context.MapRoute(
				"Grading_parts",
				"grading/{slug}/{action}/{id}/{id2}",
				new { controller = "Grading", action = "Index", id = UrlParameter.Optional, id2 = UrlParameter.Optional }
			);


		}
	}
}
