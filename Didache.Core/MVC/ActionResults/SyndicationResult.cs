using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web.Mvc;
using System.ServiceModel.Syndication;

namespace Didache {

	public class SyndicationResult : ActionResult {
		public SyndicationFeed Feed { get; set; }
		public string Type { get; set; }

		public SyndicationResult() { }

		public SyndicationResult(SyndicationFeed feed, string type) {
			this.Feed = feed;
			this.Type = type;
		}

		public override void ExecuteResult(ControllerContext context) {

			SyndicationFeedFormatter formatter = null;

			switch (Type) {
				default:
				case "rss":
					context.HttpContext.Response.ContentType = "application/rss+xml";
					formatter = new Rss20FeedFormatter(this.Feed);


					break;
				case "atom":
					context.HttpContext.Response.ContentType = "application/atom+xml";
					formatter = new Atom10FeedFormatter(this.Feed);

					break;
			}

			using (XmlWriter writer = XmlWriter.Create(context.HttpContext.Response.Output)) {
				formatter.WriteTo(writer);
			}
		}
	}
}
