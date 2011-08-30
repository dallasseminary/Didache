<%@ WebHandler Language="C#" Class="UserCourses" %>

using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using DTS.Online;
using Didache;

public class UserCourses : IHttpHandler {
    
	public void ProcessRequest (HttpContext context) {
 
    	
		// get a list of languages
		ArrayList languageList = new ArrayList();
		XmlDocument languagesDoc = new XmlDocument();	
		XmlTextReader xmlTextReader = new XmlTextReader(context.Server.MapPath("~/player/languages.xml"));
		languagesDoc.Load(xmlTextReader);
		xmlTextReader.Close();
		XmlNode root = languagesDoc.SelectSingleNode("languageSettings");
        
		foreach (XmlNode languageNode in root.ChildNodes)           
		{
			if (languageNode.NodeType != XmlNodeType.Comment)
			//context.Response.Write(languageNode.Name);
			languageList.Add(languageNode.Attributes["code"].Value);
		}    	
    	
    	
		// START: document
		context.Response.ContentType = "text/xml";


		XmlTextWriter writer = new XmlTextWriter(context.Response.OutputStream, Encoding.UTF8);
		writer.Formatting = Formatting.Indented;
		
		writer.WriteStartDocument();
		writer.WriteStartElement("courses");

			
			string[] blockedCourses = new string[] {"BE000","DEMO","DEMO1","DEMO2","MISC","PM101-v2","PM101_v2","PM001","POM1001","RS000","WM001", "BE105",  "BC101"}; //"RS101",
			
			string coursePath = Didache.Settings.PlayerFilesLocation;
			
			string[] coursesList = Directory.GetDirectories(coursePath);
			
			for (int i=0; i<coursesList.Length; i++) {
				string path = coursesList[i];
				
				string code = path.Substring(path.LastIndexOf("\\")+1);
				
				
				
				//if (okayToAdd)
					coursesList[i] = code;			
			}
			
	
			foreach (string courseCode in coursesList) {
				//string courseCode = course.CourseMaster.DepartmentCode + course.CourseMaster.Number.ToString();
				

				// skip the blocked ones
				bool okayToAdd = true;
				foreach (string blockedCode in blockedCourses) {
					if (courseCode.ToUpper().Trim() == blockedCode.ToUpper().Trim()) {
						okayToAdd = false;
						break;
					}
				}
				
				if (!okayToAdd)
					continue;

				writer.WriteStartElement("course");
				writer.WriteAttributeString("code", courseCode);


				// try possible languages
				foreach (string languageCode in languageList)
				{
					XmlDocument languageSpecificCourseInfoDoc = null;
					
					// check to ensure there is a video file there
					string courseLanguagePath = coursePath + courseCode + "/titles/" + languageCode + ".xml";

					try {
					
						languageSpecificCourseInfoDoc = new XmlDocument();
						languageSpecificCourseInfoDoc.Load(courseLanguagePath);

						
						writer.WriteStartElement("language");

						writer.WriteAttributeString("code", languageCode);

						// load the names from other XML file
						XmlNode courseRoot = languageSpecificCourseInfoDoc.SelectSingleNode("course");
						string courseName = courseRoot.Attributes["name"].Value;

						writer.WriteAttributeString("name", courseName);

						writer.WriteEndElement();
						
						
					} catch (Exception e) {
						// the language doesn't exist
						
						/*
						writer.WriteStartElement("language");

						writer.WriteAttributeString("lan", languageCode);

						writer.WriteAttributeString("url", courseLanguageUrl);
						
						writer.WriteAttributeString("error", e.ToString());

						writer.WriteEndElement();
						*/
						
					}
					
				}

				writer.WriteEndElement();

			}


		
		
		// close list
		writer.WriteEndElement();
		writer.WriteEndDocument();
		writer.Close();	
    }

 
    public bool IsReusable {
        get {
            return false;
        }
    }

}