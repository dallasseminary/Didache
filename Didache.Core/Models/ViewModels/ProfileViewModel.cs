using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Didache.Models {
	public class ProfileViewModel {
		public User User { get; set; }
		public List<User> CommonUserRelationships { get; set; }
		public List<CarsCourse> CommonCarsCourses { get; set; }

		public UserRelationship ViewerRelationshipToUser { get; set; }


		public List<CarsRelationship> Spouses { get; set; }
		public List<FamilyMember> Children { get; set; }
	}
}
