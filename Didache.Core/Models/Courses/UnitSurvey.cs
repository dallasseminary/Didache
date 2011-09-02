using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Didache {
	public class UnitSurvey {

		[Key]
		public int SurveyID { get; set; }

		public int CourseID { get; set; }
		public int UnitID { get; set; }
		public int UserID { get; set; }

		public DateTime DateStamp{ get; set; }
		public int DevoValue{ get; set; }
		public int ReadingClarity{ get; set; }
		public int ReadingValue{ get; set; }
		public int VideoClarity{ get; set; }
		public int VideoTechnical{ get; set; }
		public int VideoValue{ get; set; }
		public int VideoSlidesValue{ get; set; }
		public int VideoTranscripts{ get; set; }
		public int LearningClarity{ get; set; }
		public int LearningValue{ get; set; }
		public int LearningDriven{ get; set; }
		public int InteractionClarity{ get; set; }
		public int InteractionValue{ get; set; }
		public int NavigationLayout{ get; set; }
		public int NavigationEase{ get; set; }
		public int NavigationLabels{ get; set; }
		public int ServiceFacilitators{ get; set; }
		public int ServiceTechnical{ get; set; }

		[MaxLength]
		public string Comments { get; set; }

	}
}
