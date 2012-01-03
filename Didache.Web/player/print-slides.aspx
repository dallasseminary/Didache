<%@ Page Language="C#" EnableViewState="false" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="System.Xml" %>
<%@ Import Namespace="DTS.Online" %>

<script runat="server">
void Page_Load() {

	string courseCode = Request["course"] + "";
	string unitNumber = Request["unit"] + "";
	string videoNumber = Request["video"] + "";
	string language = Request["language"] + "";

	if (String.IsNullOrEmpty(language)) {
		language = "en-US";
	}


	// get course info for headers
	DTS.Online.OnlineCourse course = DTS.Online.OnlineCourses.GetCourse(courseCode);

	// write slides
	if (String.IsNullOrEmpty(videoNumber)) {
		OnlineCourseUnit unit = DTS.Online.OnlineCourses.GetCourseUnits(courseCode).SingleOrDefault(u => u.UnitNumber.ToString() == unitNumber);
		if (unit != null) {
			foreach (OnlineCourseVideo video in unit.Videos) {
				Output.Text += WriteSlides(course, courseCode, unit.UnitNumber.ToString(), video.VideoNumber.ToString(), language);
			}
		}
	} else {
		Output.Text = WriteSlides(course, courseCode, unitNumber, videoNumber, language);
	}
}

string WriteSlides(OnlineCourse course, string courseCode, string unitNumber, string videoNumber, string language) {


	string slidesUrlBase = "/playerfiles/" + courseCode + "/slides/" + language + "/";

	XmlDocument slidesDoc = null;
	XmlDocument videoInfoDoc = null;

	// construct paths
	string slidesXmlPath = Didache.Settings.PlayerFilesLocation + courseCode + Path.DirectorySeparatorChar + "Slides" + Path.DirectorySeparatorChar + language + Path.DirectorySeparatorChar + courseCode + "_u" + unitNumber.PadLeft(3, '0') + "_v" + videoNumber.PadLeft(3, '0') + "_slides.xml";
	string videoInfoXmlPath = Didache.Settings.PlayerFilesLocation + courseCode + Path.DirectorySeparatorChar + "Titles" + Path.DirectorySeparatorChar + language + ".xml";
	
	// load local xml
	videoInfoDoc = LoadLocalXmlDocument(videoInfoXmlPath);
	slidesDoc = LoadLocalXmlDocument(slidesXmlPath);

	if (videoInfoDoc == null || slidesDoc == null)
		return "";
	

	// LOAD: video info
	XmlNode videoRoot = videoInfoDoc.SelectSingleNode("course");

	string unitTitle = videoRoot.ChildNodes[Convert.ToInt32(unitNumber)-1].Attributes["name"].Value;
	string videoTitle = videoRoot.ChildNodes[Convert.ToInt32(unitNumber)-1].ChildNodes[Convert.ToInt32(videoNumber)-1].Attributes["name"].Value;


	string pageTitle = courseCode.ToUpper() + " Unit " + unitNumber.ToString() + " - " + unitTitle + "<br /> Video " + videoNumber.ToString() + " - " + videoTitle;
	StringBuilder sb = new StringBuilder();


	sb.AppendLine("<h1 class=\"title\">" + pageTitle + "</h1>\n");

	//LOAD: slides
	XmlNode slidesRoot = slidesDoc.SelectSingleNode("slides");
	if (slidesRoot == null) {
		slidesRoot = slidesDoc.SelectSingleNode("slideCues");
	}

	sb.AppendLine("<table class=\"SlideTable\">\n");

	// spin through the slides
	int i = 1;
	for (i=0; i<slidesRoot.ChildNodes.Count; i++) {
	
		string slide1filename = "";
		string slide2filename = "";
      
		XmlNode unitNode = slidesRoot.ChildNodes[i];
		
		slide1filename = unitNode.Attributes["slideFileName"].Value;
		if (slide1filename == string.Empty)
			slide1filename = courseCode + "_u" + unitNumber.PadLeft(3, '0') + "_v" + videoNumber.PadLeft(3, '0') + "_s" + (i+1).ToString().PadLeft(3, '0') + ".jpg";
			
		// there is at least one more
		if (i < slidesRoot.ChildNodes.Count - 1) {
			unitNode = slidesRoot.ChildNodes[i + 1];

			// the slide on the right slide of the table
			slide2filename = unitNode.Attributes["slideFileName"].Value;
			if (slide2filename == string.Empty)
				slide2filename = courseCode + "_u" + unitNumber.PadLeft(3, '0') + "_v" + videoNumber.PadLeft(3, '0') + "_s" + (i + 2).ToString().PadLeft(3, '0') + ".jpg";
		}
	
		sb.AppendLine(@"
			<tr> 
				<td>
					" + String.Format("<img src=\"{0}\" style=\"width:3in;height:2.25in;\" />", ResolveUrl(slidesUrlBase + slide1filename)) + @"
				</td>
				<td>
					" + ((slide2filename != "") ? String.Format("<img src=\"{0}\" style=\"width:3in;height:2.25in;\" />", ResolveUrl(slidesUrlBase + slide2filename)) : "") + @"
				</td>
			</tr>");

		if ((i+1) % 6 == 0 && (i+1) < slidesRoot.ChildNodes.Count) {
			sb.AppendLine("</table>\n" +
						"<h1 class=\"title BreakBefore\">" + pageTitle + "</h1>\n" + 
						"<table class=\"SlideTable\">\n");
		}
		
		if (slide2filename != "")
      i++;
		
	}
	sb.AppendLine("</table>");

	return sb.ToString();
}
XmlDocument LoadRemoteXmlDocument(string url) {
	XmlDocument doc = null;

	// load the remote URL xml file
	try {
		WebRequest webRequest = WebRequest.Create(url);
		WebResponse webResponse = webRequest.GetResponse();
	
		// stream into XML doc
		StreamReader streamReader = new StreamReader(webResponse.GetResponseStream());
		doc = new XmlDocument();
		doc.LoadXml(streamReader.ReadToEnd());
		// close and dispose
		streamReader.Close();	
	} catch {
		
		Output.Text = "Sorry, the slides for this video are not yet ready. We apologize for the delay.";
		
	}



	return doc;
}


XmlDocument LoadLocalXmlDocument(string xmlPath) {
	XmlTextReader xmlReader = null;
	XmlDocument xmlDoc = null;

	if (!File.Exists(xmlPath))
		return null;
	
	try {
		xmlReader = new XmlTextReader(xmlPath);

		xmlDoc = new XmlDocument();
		xmlDoc.Load(xmlReader);
	}
	catch (Exception e) {
		Response.Write("error loading: " + xmlPath + ":: " + e.ToString());
		Response.End();
		return null;
	}
	finally {
		if (xmlReader != null)
			xmlReader.Close();
	}
	return xmlDoc;
}		
</script>

<!DOCTYPE html />
<html>
<head>
	<style>
	body {
		padding: 0px;
		margin: auto;
		text-align: center;
		font-family: Times New Roman;
		font-size: 11pt;
		color: #111;	
	}
	form {
		padding: 0px;
	}
	.PageTitle {
		font-size: 12pt;
		font-family: arial, tahoma, san serif;
	
	}
	.SlideTable {
		margin: auto;
		text-align: left;
	}
	.SlideTable td {
		padding: .25in;
	}
	.BreakBefore {
		page-break-before: always;
	}
	textarea {
		overflow: auto;
		font-family: arial;
		font-size: 12pt;
	}
	h1 {
		padding: 0;
		margin: 0;
		font-size: 1.1em;
	}

	#footer {
		text-align: center;
		border-top: solid 1px #000;
	}

	</style>
</head>
<body>
<form runat="server">

	<div id="container">
	
		<asp:literal id="Output" runat="Server" />

		<div id="footer">
			&copy; <%= DateTime.Now.Year %> Dallas Theological Seminary
		</div>
	</div>

</form>
</body>
</html>