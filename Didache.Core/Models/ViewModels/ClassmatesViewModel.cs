using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Didache.Models {
	public class ClassmatesViewModel {
		public List<User> ApprovedUsers { get; set; }
		public List<User> PendingUsers { get; set; }
	}
}
