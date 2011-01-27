using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Didache {
	public class Sessions {

		public static List<Session> GetSessions() {
			return new DidacheDb().Sessions.OrderByDescending(s => s.StartDate).ToList();
		}

	}
}
