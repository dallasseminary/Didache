﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<ul id="course-tabs">
	<li>
		<a href="/courses/<%= ViewContext.RouteData.Values["slug"] %>/dashboard" <%= ViewContext.RouteData.Values["action"].ToString() == "dashboard" ? "class=\"selected\"" : "" %>>
			<span><%= Resources.labels.coursetabs_dashboard %></span>
			<dfn><%= Resources.labels.coursetabs_dashboard_desc %></dfn>
		</a>
	</li>
	<li>
		<a href="/courses/<%= ViewContext.RouteData.Values["slug"] %>/schedule" <%= ViewContext.RouteData.Values["action"].ToString() == "schedule" ? "class=\"selected\"" : "" %>>
			<span>Schedule</span>
			<dfn>Get to Work</dfn>
		</a>
	</li>
	<li>
		<a href="/courses/<%= ViewContext.RouteData.Values["slug"] %>/files" <%= ViewContext.RouteData.Values["action"].ToString() == "files" ? "class=\"selected\"" : "" %>>
			<span>Files</span>
			<dfn>Downloads, Syllabi, etc.</dfn>
		</a>
	</li>
	<li>
		<a href="/courses/<%= ViewContext.RouteData.Values["slug"] %>/roster" <%= ViewContext.RouteData.Values["action"].ToString() == "roster" ? "class=\"selected\"" : "" %>>
			<span>Roster</span>
			<dfn>Meet your classmates</dfn>
		</a>
	</li>
	<li>
		<a href="/courses/<%= ViewContext.RouteData.Values["slug"] %>/assignments" <%= ViewContext.RouteData.Values["action"].ToString() == "assignments" ? "class=\"selected\"" : "" %>>
			<span>Assignments</span>
			<dfn>Your returned work</dfn>
		</a>
	</li>
	<li>
		<a href="/courses/<%= ViewContext.RouteData.Values["slug"] %>/discussion" <%=  HttpContext.Current.Request.Url.ToString().IndexOf("/discussion") > 0 ? "class=\"selected\"" : "" %>>
			<span>Discussion</span>
			<dfn>Message board</dfn>
		</a>
	</li>
</ul>