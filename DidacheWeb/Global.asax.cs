using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Didache {
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class DidacheMvcApplication : System.Web.HttpApplication {
		public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
			filters.Add(new HandleErrorAttribute());
		}

		public static void RegisterRoutes(RouteCollection routes) {
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			/*
			routes.MapRoute(
				"Courses", // Route name
				"courses/{courseid}/{action}/{id}", // URL with parameters
				new { controller = "Courses", action = "Dashboard", id = UrlParameter.Optional } // Parameter defaults
			);

			routes.MapRoute(
				"Admin", // Route name
				"admin/{action}/{id}", // URL with parameters
				new { controller = "Admin", action = "Dashboard", id = UrlParameter.Optional } // Parameter defaults
			);
			 * */


			routes.MapRoute(
				"Default", // Route name
				"{controller}/{action}/{id}", // URL with parameters
				new { controller = "Main", action = "Index", id = UrlParameter.Optional } // Parameter defaults
			);

		}

		protected void Application_Start() {
			AreaRegistration.RegisterAllAreas();

			RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterRoutes(RouteTable.Routes);

			// fixes a caching problem on attributes like [AllowHtml]
			ModelMetadataProviders.Current = new DataAnnotationsModelMetadataProvider();
		}
	}
}