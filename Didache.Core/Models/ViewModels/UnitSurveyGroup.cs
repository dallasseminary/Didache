using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Didache.Models {
	public class UnitSurveyGroup {
		public Unit Unit { get; set; }
		public List<UnitSurvey> UnitSurveys { get; set; }
	}
}
