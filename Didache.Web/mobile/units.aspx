<%@ Page Language="C#" MasterPageFile="Mobile.Master" EnableViewState="False" %>
<%@ Import Namespace="DTS.Online" %>

<script runat="server">
void Page_Load() {

	OnlineCourse course = OnlineCourses.GetCourse(Request["course"]+"");
	
	CourseName.Text = course.CourseCode + ": " + course.DefaultCourseData.Title;

	UnitsRepeater.DataSource = course.DefaultCourseData.Units;
	UnitsRepeater.DataBind();		
		
}

</script>
<asp:Content ContentPlaceHolderID="MainContent" runat="ServeR">

 
<div data-role="page"> 
 
	<div data-role="header"> 
		<h1><%= Request["Course"] %></h1> 
	</div><!-- /header --> 
 
	<div data-role="content"> 
	
		<h3><asp:literal id="CourseName" runat="Server" /></h3>
		
		<ul data-role="listview" data-inset="true">
<asp:Repeater ID="UnitsRepeater" runat="server" enableviewstate="false">
	<ItemTemplate>		
		<li>
			<a href="videos.aspx?course=<%# Eval("CourseCode") %>&unit=<%# Eval("UnitNumber") %><%= (Request["admin"] +"" != "") ? "&admin=" + Request["admin"] : "" %>">
				<h3>Unit <%# Eval("UnitNumber") %></h3>
				<p><%# Eval("UnitName") %></p>	
				<span class="ui-li-count"><%# Eval("Videos.Count") %></span> 
			</a>
			
			<asp:Repeater DataSource='<%# Eval("Videos") %>' runat="server" visible="false">
				<headertemplate>
					<ul>
				</headertemplate>
				<ItemTemplate>		
					<li>
						<a href="video.aspx?course=<%= Request["Course"] %>&unit=<%= Request["unit"] %>&video=<%# Container.ItemIndex+1 %>">
							<h3><%# Container.ItemIndex+1 %>. <%# Eval("VideoName") %></h3>
							<p><%# Eval("Speaker") %></p> 
							<p class="ui-li-aside"><strong><%# Eval("FormattedDuration") %></strong></p> 
						</a>
					</li>				
				</ItemTemplate>
				<footertemplate>
				</ul>
				</footertemplate>				
			</asp:Repeater>	
							
		</li>				
	</ItemTemplate>
</asp:Repeater>	
		</ul>
		
		
	</div><!-- /content --> 
	
</div><!-- /page --> 
 
</asp:Content>