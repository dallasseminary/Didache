using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using Didache.Models;

namespace Didache.Web.Areas.Admin.Controllers {

	[AdminAndBuilder]
	public class DiscussionGroupsController : Controller {
		//
		// GET: /Admin/Users/
		DidacheDb db = new DidacheDb();


		public ActionResult Index() {
			List<DiscussionGroup> groups = db.DiscussionGroups
												.Include("GroupMembers")
												.OrderBy(g => g.GroupTypeID)
												.ToList();

			return View(groups);
		}

		public ActionResult Edit(int? id) {
			DiscussionGroup group = (id.HasValue) ? db.DiscussionGroups.Find(id.Value) : null;

			return View(group);
		}

		[HttpPost]
		public ActionResult Edit(DiscussionGroup model) {
			if (model.GroupID > 0) {
				DiscussionGroup group = db.DiscussionGroups.Find(model.GroupID);

				UpdateModel(group);

				db.SaveChanges();
			} else {
				DiscussionGroup group = new DiscussionGroup();

				UpdateModel(group);

				db.DiscussionGroups.Add(group);

				db.SaveChanges();
			}

			return RedirectToAction("Index");
		}

	}
}
