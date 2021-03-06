﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Globalization;

namespace Didache.Web {
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : System.Web.HttpApplication {
		public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
			filters.Add(new HandleErrorAttribute());
		}

		public static void RegisterRoutes(RouteCollection routes) {
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");




			string[] staticPages = new string[] {"about","tour","help","resources", "indexfeed", "posts", "faq"};

			foreach (string page in staticPages) {
				routes.MapRoute(
					"static_" + page, // Route name
					page, // URL with parameters
					new { controller = "Home", action = page } // Parameter defaults
					, new string[] { "Didache.Web.Controllers" }
				);
			}

			routes.MapRoute(
				"ViewPost", // Route name
				"post/{id}", // URL with parameters
				new { controller = "Home", action = "Post" } // Parameter defaults
				, new string[] { "Didache.Web.Controllers" }
			);
			

			routes.MapRoute(
				"Default", // Route name
				"{controller}/{action}/{id}", // URL with parameters
				new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
				, new string[] { "Didache.Web.Controllers" }
			);

		}

		protected void Application_AcquireRequestState(Object sender, EventArgs e) {
			/* 
			 * // is user logged in?
			if (User.Identity.IsAuthenticated) {
				User user = Users.GetLoggedInUser();
				SetLanguage(user.Language);
				HttpContext.Current.Request.Cookies.Add(new HttpCookie("HowLangSet", "database"));
			} else {
				// check if there is a value in the cookiez
				// this is for visitor who set via the drop down list
				HttpCookie langCookie = HttpContext.Current.Request.Cookies["Language"];
				if (langCookie != null && langCookie.Value != null) {
					// use cookie
					SetLanguage(langCookie.Value);
					HttpContext.Current.Request.Cookies.Add(new HttpCookie("HowLangSet", "cookie"));
				} else {
					// use default value from browser
					string lang = HttpContext.Current.Request.UserLanguages[0];
					SetLanguage(lang);
					HttpContext.Current.Request.Cookies.Add(new HttpCookie("HowLangSet", "browser"));
				}
			}
			 * */

			string language = Users.GetUserLanguage();
			string setMethod = Users.GetUserLanguageSetMethod();
			SetLanguage(language);
			HttpContext.Current.Request.Cookies.Add(new HttpCookie("HowLangSet", setMethod));

			//base.BeginRequest(sender, e);
		}

		private void SetLanguage(string lang) {
			//System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(lang);
			System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(lang);
			HttpContext.Current.Request.Cookies.Add(new HttpCookie("CurrentLanguage", lang));
		}


		protected void Application_Start() {
			AreaRegistration.RegisterAllAreas();

			RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterRoutes(RouteTable.Routes);

			// fixes a caching problem on attributes like [AllowHtml]
			ModelMetadataProviders.Current = new DataAnnotationsModelMetadataProvider();

			// String.Empty instead of null for empty form fields
			ModelBinders.Binders.DefaultBinder = new EmptyStringModelBinder();
		}

		protected void Application_BeginRequest(Object sender, EventArgs e) {


			if (Settings.RequireSsl && !HttpContext.Current.Request.IsSecureConnection) {
				Response.Redirect("https://" + Request.ServerVariables["HTTP_HOST"] + HttpContext.Current.Request.RawUrl);
			}
		}
	}


	public class EmptyStringModelBinder : DefaultModelBinder {
		public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
			bindingContext.ModelMetadata.ConvertEmptyStringToNull = false;
			return base.BindModel(controllerContext, bindingContext);
		}
	}
}