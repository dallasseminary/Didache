﻿<%@ Control Language="C#"  %>

<nav id="dts-common-menu">
	<div class="container_12">
		<div class="grid_6">
			<div id="dts-sites">
				<b>DTS Website</b>
				<a href="http://library.dts.edu/">Library</a>
				<a href="http://bookcenter.dts.edu/">Book Center</a>
				<a href="http://campus.dts.edu/">CampusNet</a>
				<a href="http://alumni.dts.edu/">Alumni</a>
				<a href="http://my.dts.edu/">MyDTS</a>				
			</div>
		</div>
		<div class="grid_6">
			<div id="dts-account">				
<%if (Request.IsAuthenticated) { %>
        <b><%: Page.User.Identity.Name %></b> <%: Html.ActionLink("Log Off", "LogOff", "Account") %>
<% } else { %> 
        [ <%: Html.ActionLink("Log On", "LogOn", "Account") %> ]
<% } %>
							
			</div>
		</div>			
	</div>
</nav>