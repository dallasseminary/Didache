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
		JoinGroup = 6,
		MakeNewComment = 7,
		UpdateSettings = 8,
		SimpleSearch = 9,
		AdvancedSearch = 10,
		ScheduleSearch = 11,
		ViewProfile = 12,
		LeaveGroup = 13,
		CreateGroup = 14,
		MessageToGroup = 15
	}
}