using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.Mvc;
using System.IO;
using System.Xml;

namespace Didache.TaskControls {
	public class VideoListing : ViewUserControl<UserTaskData> {

		public List<VideoInfo> Videos;

		protected void Page_Load()
		{
			Videos = new List<VideoInfo>();

			string locaThumbBase = "/supportfiles";
			string externalThumbBase = "http://oefiles.dts.edu";

			// load XML
			string xmlPath = System.Web.HttpContext.Current.Server.MapPath(String.Format("~/supportfiles/{0}/Titles/en-US.xml", Model.Task.Course.CourseCode));

			if (File.Exists(xmlPath))
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(xmlPath);

				Random random = new Random();

				// pull videos
				XmlNodeList videoNodes = doc.SelectNodes("//unit[@number=" + Model.Task.Unit.SortOrder + "]/video");
				foreach (XmlNode vNode in videoNodes)
				{

					Videos.Add(new VideoInfo
								{
									Title = vNode.Attributes["name"].Value,
									Duration = vNode.Attributes["duration"].Value,
									VideoUrl = String.Format("{0}/{1}/{2}_u{3}_v{4}.mp4", externalThumbBase, Model.Task.Course.CourseCode.ToLower(), Model.Task.Course.CourseCode.ToUpper(), Model.Task.Unit.SortOrder.ToString().PadLeft(3, '0'), vNode.Attributes["number"].Value.ToString().PadLeft(3, '0')),

									ThumbnailFilename = String.Format("{0}_u{1}_v{2}_thumb.jpg", Model.Task.Course.CourseCode.ToUpper(), Model.Task.Unit.SortOrder.ToString().PadLeft(3, '0'), vNode.Attributes["number"].Value.ToString().PadLeft(3, '0')),

									//ThumbnailUrl = String.Format("{0}/{1}/videos/{2}_u{3}_v{4}_thumb.jpg", locaThumbBase, Model.Course.CourseCode.ToLower(), Model.Course.CourseCode.ToUpper(), Model.Unit.UnitNumber.ToString().PadLeft(3, '0'), vNode.Attributes["number"].Value.ToString().PadLeft(3, '0')),
									ThumbnailUrl = String.Format("{0}/{1}/{2}_u{3}_v{4}_thumb.jpg", externalThumbBase, Model.Task.Course.CourseCode.ToLower(), Model.Task.Course.CourseCode.ToUpper(), Model.Task.Unit.SortOrder.ToString().PadLeft(3, '0'), vNode.Attributes["number"].Value.ToString().PadLeft(3, '0')),
									PercentComplete = random.Next(100)
								});
				}
			}
		}
	}

	public class VideoInfo {
		public int SortOrder { get; set; }
		public string Title { get; set; }
		public string Duration { get; set; }
		public string ThumbnailUrl { get; set; }
		public string ThumbnailFilename { get; set; }
		public string VideoUrl { get; set; }
		public int PercentComplete { get; set; }
		public string FormattedDuration {
			get {
				string[] parts = Duration.Split(':');
				return String.Format("{0} minutes", Int32.Parse(parts[1]));
				//return new TimeSpan(0, Int32.Parse(parts[1]), 0).ToString();
			}
		}

		public int Minutes {
			get {
				string[] parts = Duration.Split(':');
				return Int32.Parse(parts[1]);
			}
		}

		public int Seconds {
			get {
				string[] parts = Duration.Split(':');
				return Int32.Parse(parts[2]);
			}
		}
	}
}