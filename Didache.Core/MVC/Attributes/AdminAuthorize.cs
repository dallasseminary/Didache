using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Didache {
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class AdminOnlyAttribute : AuthorizeAttribute {
		public AdminOnlyAttribute() {
			Roles = UserRoles.Administrator;
		}
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class AdminAndBuilderAttribute : AuthorizeAttribute {
		public AdminAndBuilderAttribute() {
			Roles = UserRoles.Administrator + "," + UserRoles.Builder;
		}
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class AdminBuilderFacilitatorAttribute : AuthorizeAttribute {
		public AdminBuilderFacilitatorAttribute() {
			Roles = UserRoles.Administrator + "," + UserRoles.Builder + "," + UserRoles.Facilitator;
		}
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class FacilitatorOnlyAttribute : AuthorizeAttribute {
		public FacilitatorOnlyAttribute() {
			Roles = UserRoles.Facilitator;
		}
	}
}
