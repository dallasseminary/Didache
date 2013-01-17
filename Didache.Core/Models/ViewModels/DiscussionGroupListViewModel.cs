using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Didache.Models {
	public class DiscussionGroupListViewModel {
		public List<DiscussionGroup> AllGroups { get; set; }
		public List<DiscussionGroup> UsersGroups { get; set; }
	}
}
