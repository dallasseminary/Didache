using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Didache {
	public class ICalEventResult : ActionResult {

		private List<ICalEvent> _events = null;
		private string _filename = null;
		private string _title = null;
		private string _id = null;


		public ICalEventResult(string title, string id, string filename, List<ICalEvent> events) {
			_events = events;
			_filename = filename;
			_title = title;
			_id = id;
		}

		public override void ExecuteResult(ControllerContext context) {
			StringBuilder sb = new StringBuilder();

			sb.AppendLine("BEGIN:VCALENDAR");
			sb.AppendLine("VERSION:2.0");

			if (!String.IsNullOrEmpty(_title)) {
				sb.AppendLine("X-WR-CALNAME:" + _title);
			}

			if (!String.IsNullOrEmpty(_id)) {
				sb.AppendLine("PRODID:-//" + _id + "//NONSGML v1.0//EN");
			}

			sb.AppendLine("CALSCALE:GREGORIAN");
			sb.AppendLine("METHOD:PUBLISH");


			if (_events != null && _events.Count > 0) {
				foreach (ICalEvent iEvent in _events) {
					sb.AppendLine(String.Format("DTSTART:{0}",iEvent.StartUtc.ToUniversalTime().ToString("yyyymmddThhiissZ")));
					sb.AppendLine(String.Format("DTEND:{0}", iEvent.EndUtc.ToUniversalTime().ToString("yyyymmddThhiissZ")));
					sb.AppendLine(String.Format("SUMMARY:{0}", iEvent.Summary));
					sb.AppendLine(String.Format("DESCRIPTION:{0}",iEvent.Description));
					sb.AppendLine(String.Format("LOCATION:{0}", iEvent.Location));
				}
			}

			sb.AppendLine("END:VCALENDAR");

            context.HttpContext.Response.ContentType = "text/calendar";
			if (!String.IsNullOrEmpty(_filename)) {
				context.HttpContext.Response.AddHeader("Content-disposition", String.Format("attachment; filename={0}", _filename));
			}
            context.HttpContext.Response.Write(sb.ToString());
			context.HttpContext.Response.End();
		}
	}

	public class ICalEvent {
		public DateTime StartUtc { get; set;}
        public DateTime EndUtc { get; set;}
        public string Summary { get; set;}
        public string Description  { get; set;}
        public string Location  { get; set;}
	}
}
