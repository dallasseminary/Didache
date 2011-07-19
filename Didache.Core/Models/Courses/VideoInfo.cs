using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Didache {

	public class VideoInfo {
		public int SortOrder { get; set; }
		public string Title { get; set; }
		public string UnitTaskInfo { get; set; }
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
