<%@ Page Language="C#" MasterPageFile="Mobile.Master" EnableViewState="False" %>
<%@ Import Namespace="DTS.Online" %>

<script runat="server">
void Page_Load() {
	
	OnlineCourse course = OnlineCourses.GetCourse(Request["course"]+"");
	
	CourseName.Text = course.CourseCode + ": " + course.DefaultCourseData.Title;
	
	int unitNumber = 0;
	if (Int32.TryParse(Request["unit"]+"", out unitNumber) && course.DefaultCourseData.Units.Count >= unitNumber) {
		
		UnitName.Text = "Unit " + unitNumber.ToString() + ": " + course.DefaultCourseData.Units[unitNumber-1].UnitName;
		
		VideosRepeater.DataSource = course.DefaultCourseData.Units[unitNumber-1].Videos;
		VideosRepeater.DataBind();			
	}
	
	
		
}

</script>
<asp:Content ContentPlaceHolderID="MainContent" runat="ServeR">
 
<div data-role="page"> 
 
	<div data-role="header"> 
		<h1><%= Request["Course"] %> Unit <%= Request["unit"] %></h1> 
	</div><!-- /header --> 
 
	<div data-role="content"> 
	
		<h3><asp:literal id="CourseName" runat="Server" /></h3>
		<h4><asp:literal id="UnitName" runat="Server" /></h4>	
		
		<ul data-role="listview" data-inset="true">
<asp:Repeater ID="VideosRepeater" runat="server">
	<ItemTemplate>		
		<li>
			<a href="video.aspx?course=<%= Request["Course"] %>&unit=<%= Request["unit"] %>&video=<%# Container.ItemIndex+1 %>">
				<h3><%# Container.ItemIndex+1 %>. <%# Eval("VideoName") %></h3>
				<p><%# Eval("Speaker") %></p> 
				<p class="ui-li-aside"><strong><%# Eval("FormattedDuration") %></strong></p> 
			</a>
		</li>				
	</ItemTemplate>
</asp:Repeater>	
		</ul>
		
		
	</div><!-- /content --> 
	
</div><!-- /page --> 
 
</asp:Content>