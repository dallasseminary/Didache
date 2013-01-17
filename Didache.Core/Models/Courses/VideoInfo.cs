using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Didache {

	public class VideoInfo {
		public int SortOrder { get; set; }
		public string Title { get; set; }
		public string Speaker { get; set; }
		public string UnitTaskInfo { get; set; }
		public string Duration { get; set; }
		public string ThumbnailUrl { get; set; }
		public string ThumbnailFilename { get; set; }
		public int PercentComplete { get; set; }

		public string CourseCode  { get; set;}
		public int UnitNumber { get; set;}
		public int VideoNumber { get; set;}

		public string WatchUrl {get; set;}
		public string VideoUrl { get; set; }
		public string TranscriptUrl { get; set; }
		public string SlidesUrl { get; set; }
		


		public string FormattedDuration {
			get {
				//string[] parts = Duration.Split(':');
				return String.Format("{0} minutes", Minutes);
				//return new TimeSpan(0, Int32.Parse(parts[1]), 0).ToString();
			}
		}

		public Double TotalSeconds {
			get {

				return (Minutes * 60) + Seconds;
				//string[] parts = Duration.Split(':');
				//if (parts.Length == 2)
				//return (Double.Parse(parts[1]) * 60) + (parts.Length >= 2 ? Double.Parse(parts[2]) : 0);
			}
		}

		public int Minutes {
			get {
				string[] parts = Duration.Split(':');
				int minutes = 0;

				if (parts.Length == 2) {
					Int32.TryParse(parts[0], out minutes);
				} else if (parts.Length == 3) {
					Int32.TryParse(parts[1], out minutes);
	
				}

				return minutes;
			}
		}

		public int Seconds {
			get {
				string[] parts = Duration.Split(':');
				int minutes = 0;

				if (parts.Length == 2) {
					Int32.TryParse(parts[1], out minutes);
				} else if (parts.Length == 3) {
					Int32.TryParse(parts[2], out minutes);
				}

				return minutes;
			}
		}
	}
}
