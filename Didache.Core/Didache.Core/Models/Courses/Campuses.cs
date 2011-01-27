using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Didache {
	public class Campuses {

		public static List<Campus> GetCampuses() {
			return new DidacheDb().Campuses.OrderBy(c => c.SortOrder).ToList();
		}

	}
}
