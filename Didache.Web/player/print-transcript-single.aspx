<%@ Page Language="C#" EnableViewState="false" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="System.Xml" %>

<script runat="server">
void Page_Load() {

	// look for local files? or remote?
	bool useLocalFiles = true;
	
	string remoteUrl = Request.QueryString["RemoteUrl"] as string;
	string appRoot = Request.PhysicalApplicationPath;
	
	string language= Request["language"]+"";
	string courseCode = Request["course"]+"";
	string unitNumber = Request["unit"]+"";
	string videoNumber = Request["video"]+"";

	if (String.IsNullOrEmpty(language)) {
		language = "en-US";
	}

	try {
		XmlDocument videoInfoDoc = null;

		
		//if (Directory.Exists(appRoot + Path.DirectorySeparatorChar + courseCode)) {
		if (useLocalFiles)
		{

			// construct paths to XML documents
			string videoInfoXmlPath = Didache.Settings.PlayerFilesLocation + courseCode + Path.DirectorySeparatorChar + "titles" + Path.DirectorySeparatorChar + language + ".xml";
			
			// load local xml
			videoInfoDoc = LoadLocalXmlDocument(videoInfoXmlPath);
			
		}
		else
		{
			// load unit/video info file and main nodes
			string videoInfoXmlUrl = remoteUrl + courseCode + "/titles/" + language + ".xml";
			videoInfoDoc = LoadRemoteXmlDocument(videoInfoXmlUrl);
		}
	
		XmlNode coureInfoRoot = videoInfoDoc.SelectSingleNode("course");
		XmlNode unitNode = coureInfoRoot.ChildNodes[Convert.ToInt32(unitNumber) - 1];
		
		// get the title of the course and title of the unit              
		CourseTitle.Text = coureInfoRoot.Attributes["code"].Value + ". " + coureInfoRoot.Attributes["name"].Value;
		UnitTitle.Text = "Unit " + unitNumber + ". " + unitNode.Attributes["name"].Value;
		

		// create a list of the videos we want to use.		
		string[] videoNumbers = null;

		// single video transcript
		if (videoNumber != null)
		{
			videoNumbers = new string[] { videoNumber };
		}
		else
		{
			videoNumbers = new string[unitNode.ChildNodes.Count];

			for (int i = 0; i < unitNode.ChildNodes.Count; i++)
			{
				videoNumbers[i] = (i+1).ToString();
			}
		}

		

		// create Video list for databinding

		ArrayList transcriptsList = new ArrayList();

		foreach (string video in videoNumbers)
		{
			try
			{
				// get title
				string videoTitle = unitNode.ChildNodes[Convert.ToInt32(video) - 1].Attributes["name"].Value;
				string speaker = unitNode.ChildNodes[Convert.ToInt32(video) - 1].Attributes["speaker"].Value;

				// get the transcript
				XmlDocument transcriptDoc;
				
				if (useLocalFiles) {
					string transcriptXmlPath = Didache.Settings.PlayerFilesLocation + courseCode + "/transcripts/" + language + "/" + courseCode + "_u" + unitNumber.PadLeft(3, '0') + "_v" + video.PadLeft(3, '0') + "_transcript.xml";
					transcriptDoc = LoadLocalXmlDocument(transcriptXmlPath);	
				} else {
					string transcriptXmlUrl = remoteUrl + courseCode + "/transcripts/" + language + "/" + courseCode + "_u" + unitNumber.PadLeft(3, '0') + "_v" + video.PadLeft(3, '0') + "_transcript.xml";
					transcriptDoc = LoadRemoteXmlDocument(transcriptXmlUrl);				
				}
				

				if (transcriptDoc != null)
				{
					XmlNode transcriptRoot = transcriptDoc.SelectSingleNode("transcript");

					XmlNodeList transcriptNodes = transcriptRoot.ChildNodes;

					transcriptsList.Add(new Transcript(Convert.ToInt32(video), videoTitle, speaker, transcriptNodes));
				}
				else
				{
					transcriptsList.Add(new Transcript(Convert.ToInt32(video), "MISSING: " + videoTitle, speaker, null));
				}
			}
			catch (Exception ex) 
			{
				transcriptsList.Add(new Transcript(Convert.ToInt32(video), "ERROR: " + ex.ToString(), "", null));
			}
		}

		TranscriptRepeater.DataSource = transcriptsList;
		TranscriptRepeater.DataBind();	

	} catch (Exception e) {

		Response.Write(e);
		Message.Text = "There was an error loading this transcript. Please alert <a href=\"mailto:babegg@dts.edu?subject=bad transcript: " + courseCode + " unit " + unitNumber + " video " + videoNumber + "\">Bob Abegg</a>";

	}
}
    
    
XmlDocument LoadRemoteXmlDocument(string url) {
    XmlDocument doc = null;
	
	// load the remote URL xml file
    try
    {
        WebRequest webRequest = WebRequest.Create(url);
        WebResponse webResponse = webRequest.GetResponse();

        // stream into XML doc
        StreamReader streamReader = new StreamReader(webResponse.GetResponseStream());
        doc = new XmlDocument();
        doc.LoadXml(streamReader.ReadToEnd());

        // close and dispose
        streamReader.Close();
    }
    catch
    {
        //Response.Write("error getting: " + url);
        //Response.End();            
    }

	return doc;
}	

	
XmlDocument LoadLocalXmlDocument(string xmlPath) {
	XmlTextReader xmlReader = null;
	XmlDocument xmlDoc = null;

	try {
		xmlReader = new XmlTextReader(xmlPath);

		xmlDoc = new XmlDocument();
		xmlDoc.Load(xmlReader);
	}
	catch (Exception e) {
		// Response.Write("error loading: " + xmlPath + ":: " + e.ToString());
		// Response.End();
		return null;
	}
	finally {
		if (xmlReader != null)
			xmlReader.Close();
	}
	return xmlDoc;
}

    public class Transcript
    {
        public Transcript(int number, string videoTitle, string speaker, XmlNodeList transcriptNodeList)
        {
            this.number = number;
            this.videoTitle = videoTitle;
            this.speaker = speaker;
            this.transcriptNodeList = transcriptNodeList;
        }

        private int number;
        private string videoTitle;
        private string speaker;
        private XmlNodeList transcriptNodeList;

        public int Number
        {
            get { return number; }
            set { number = value; }
        }        
                
        public string VideoTitle {
            get { return videoTitle; }
            set { videoTitle = value; }
        }
        
        public string Speaker {
            get { return speaker; }
            set { speaker = value; }
        }        

        public XmlNodeList TranscriptNodeList
        {
            get { return transcriptNodeList; }
            set { transcriptNodeList = value; }
        }        
                
    }
    
    
</script>

<!DOCTYPE html>
<html>
<head>
<title><asp:Literal id="PageTitle" runat="server" /></title>
<style type="text/css">
body {
	padding: 0;
	margin: 0;
	font-family: Times New Roman;
	line-height: 1.5em;
	font-size: 14pt;
	color: #111;	
	
}
form {
	padding: 0px;
}
p {
	line-height: 1.4em;
}
h1, h2, h3 {
	margin-top: .2em;
	margin-bottom: .2em;
}
h1 {
	font-size: 1.4em;
}
h2 {
	font-size: 1.2em;
}

.header {
	border-bottom: solid 1px #000;
}
h3 {
	float: left;
	margin: 0;
}
h4 {
	float: right;
	margin: 0;
	font-style: italic;
}
.content {
	clear: both;
}


#container {
	text-align: left;
	max-width: 800px;
	padding: 0 0.25in;
	margin: .25in auto;
}


#header {

}
#header .speaker {
	float: right;
}
#footer {
	text-align:center;
	border-top: solid 1px #000;	
}
.clear {
	clear:both;
}
</style>
</head>
<body>
<form id="Form1" runat="server">

	<div id="container">
	    
	  <div id="header">			
	    <h1><asp:Literal id="CourseTitle" runat="server" /> </h1>
	    <h2><asp:Literal id="UnitTitle" runat="server" /></h2>
		</div>
				
		<asp:Repeater id="TranscriptRepeater" runat="Server" >
		
			<ItemTemplate>
						
				<div class="header">
					<h3>Video <%# ((Transcript)Container.DataItem).Number %>. <%# ((Transcript)Container.DataItem).VideoTitle %></h3>
					<h4><%# ((Transcript)Container.DataItem).Speaker %></h4>
					<div class="clear"></div>
				</div>
				
				<div class="content">
				
					<asp:Repeater id="TranscriptTextRepeater" DataSource='<%# ((Transcript)Container.DataItem).TranscriptNodeList %>' runat="Server" EnableViewState="false" >
						
						<HeaderTemplate><p></HeaderTemplate>
						
						<ItemTemplate>
								<%# ((XmlNode) Container.DataItem).Attributes["text"].Value %><%# ( ((XmlNode) Container.DataItem).Attributes["breakAfter"] != null) ? "</p><p>" : "" %>
						</ItemTemplate>
							
						<FooterTemplate><p></FooterTemplate>
					</asp:Repeater>
				
				</div>
			
			</ItemTemplate>
		</asp:Repeater>
		
		<asp:Literal id="Message" runat="Server" />

		<div id="footer">
			&copy; <%= DateTime.Now.Year %> Dallas Theological Seminary
		</div>
	</div>

</form>
</body>
</html>