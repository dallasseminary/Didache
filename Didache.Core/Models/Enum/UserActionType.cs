using System;
using System.Data;
using System.Configuration;


namespace Didache {
	public enum UserActionType {
		Register = 0,
		BecomeClassmates = 1,
		UpdateContactInformation = 2,
		UpdatePicture = 3,
		MakeNewPost = 4,
		LeaveMessage = 5,
		JoinDiscussionGroup = 6,
		MakeNewComment = 7,
		UpdateSettings = 8,
		SimpleSearch = 9,
		AdvancedSearch = 10,
		ScheduleSearch = 11,
		ViewProfile = 12,
		LeaveDiscussionGroup = 13,
		CreateDiscussionGroup = 14,
		MessageToDiscussionGroup = 15
	}
}