<%@ Page Language="C#" MasterPageFile="Mobile.Master" EnableViewState="False" %>
<%@ Import Namespace="DTS.Online" %>

<script runat="server">
void Page_Load() {

	OnlineCourse course = OnlineCourses.GetCourse(Request["course"]+"");
	
	CourseName.Text = course.CourseCode + ": " + course.DefaultCourseData.Title;
	
	int unitNumber = 0;
	int videoNumber = 0;
	if ( 
		Int32.TryParse(Request["unit"]+"", out unitNumber) && 
		Int32.TryParse(Request["video"]+"", out videoNumber) && 
		course.DefaultCourseData.Units.Count >= unitNumber  &&
		course.DefaultCourseData.Units[unitNumber-1].Videos.Count >= videoNumber ) {
		
		
		UnitName.Text = "Unit " + unitNumber.ToString() + ": " + course.DefaultCourseData.Units[unitNumber-1].UnitName;
		VideoName.Text = "Video " + videoNumber.ToString() + ": " + course.DefaultCourseData.Units[unitNumber-1].Videos[videoNumber-1].VideoName + 
			" (" + course.DefaultCourseData.Units[unitNumber-1].Videos[videoNumber-1].FormattedDuration + ")";
	}
			
		
}

string GetCourseString() {
	return Request["Course"].ToString().ToLower() + "/" + Request["Course"].ToString().ToUpper().Replace("V2","v2").Replace("V3","v3") + "_u" + Request["Unit"].ToString().PadLeft(3,'0') + "_v" + Request["Video"].ToString().PadLeft(3,'0');
}

</script>
<asp:Content ContentPlaceHolderID="MainContent" runat="ServeR">
 
<div data-role="page"> 
 
	<div data-role="header"> 
		<h1><%= Request["Course"] %> Video <%= Request["unit"] %>-<%= Request["video"] %></h1> 
	</div><!-- /header --> 
 
	<div data-role="content"> 
	
		<h3><asp:literal id="CourseName" runat="Server" /></h3>
		<h4><asp:literal id="UnitName" runat="Server" /></h4>
	
		<p><asp:literal id="VideoName" runat="server" /></p>
		
		<div data-role="controlgroup">
			<a href="https://dtsoe.s3.amazonaws.com/<%= GetCourseString() %>_lo.mp4" data-role="button">Download Video (low)</a>
			<a href="https://dtsoe.s3.amazonaws.com/<%= GetCourseString() %>.mp4" data-role="button">Download Video (high)</a>
			<a href="https://dtsoe.s3.amazonaws.com/<%= GetCourseString() %>.mp3" data-role="button">Download Audio</a>
		</div>
		<div data-role="controlgroup">
			<a href="slides.aspx?course=<%= Request["course"]  %>&unit=<%= Request["unit"]  %>&video=<%= Request["video"]  %>" data-role="button">Download Slides</a>
			<a href="transcript.aspx?course=<%= Request["course"]  %>&unit=<%= Request["unit"]  %>&video=<%= Request["video"]  %>" data-role="button">Download Transcript</a>
		</div>				
		
	</div><!-- /content --> 
	
</div><!-- /page --> 

</asp:Content>