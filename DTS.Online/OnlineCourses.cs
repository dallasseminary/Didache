using System;
using System.Web;
using System.Net;
using System.Xml;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Didache;


namespace DTS.Online {

	public class OnlineCourses {
		
		private static string[] _blockedCourses = new string[] {"BE000","DEMO","DEMO1","DEMO2","MISC","PM101-v2","PM101_v2","PM001","POM1001","RS000","WM001", "BE105",  "BC101"}; //"RS101",
		private static string[] _languagesList = new string[] {"en-US","zh-CN","zh-TW", "es-ES"};
		private static string _defaultLanguage = "en-US";


		// not in groups
		public static List<OnlineCourseGroup> GetCourseGroups() {
			List<OnlineCourseGroup> courseGroups = new List<OnlineCourseGroup>();
			
			
			courseGroups.Add(new OnlineCourseGroup("Biblical Studies"));
			courseGroups.Add(new OnlineCourseGroup("Languages"));
			courseGroups.Add(new OnlineCourseGroup("Theological Studies"));
			courseGroups.Add(new OnlineCourseGroup("Practical Ministry"));

			List<OnlineCourse> courses = GetCourses();
			foreach (OnlineCourse course in courses) {
				string dept = course.CourseCode.Substring(0,2);
				
				switch (dept) {
					case "BE":
						courseGroups[0].Courses.Add(course);
						break;
					case "NT":
					case "OT":
						courseGroups[1].Courses.Add(course);
						break;	
					case "ST":
					case "HT":
						courseGroups[2].Courses.Add(course);
						break;			
						
					case "CE":
					case "PM":
					case "WM":
					case "SL":
					case "RS":
					default:
						courseGroups[3].Courses.Add(course);
						break;																		
											
				
				}
			
			}
			
			// sort all the lists
			
			
			return courseGroups;
		}
		
		// not in groups
		public static List<OnlineCourse> GetCourses() {
			List<OnlineCourse> courses = new List<OnlineCourse>();
			
			// get directores
			string coursePath = Settings.PlayerFilesLocation;

			string[] coursesList = Directory.GetDirectories(coursePath);
			
			for (int i=0; i<coursesList.Length; i++) {
				// get only directory name
				string path = coursesList[i];
				coursesList[i] = path.Substring(path.LastIndexOf("\\")+1);
			}		
			
			// load the names in
			foreach (string courseCode in coursesList) {

				// skip the blocked ones
				bool okayToAdd = true;
				foreach (string blockedCode in _blockedCourses) {
					if (courseCode.ToUpper().Trim() == blockedCode.ToUpper().Trim()) {
						okayToAdd = false;
						break;
					}
				}
				
				if (!okayToAdd)
					continue;

				OnlineCourse course = GetCourse(courseCode);
				if (course != null)
					courses.Add(course);
			}		
			
			
			
			return courses;
		}
		
		// single
		public static OnlineCourse GetCourse(string courseCode) {	
			
			OnlineCourse course = new OnlineCourse(courseCode);
			bool languageSuccess = false;
			string coursePath = Settings.PlayerFilesLocation;
			
			// try possible languages
			foreach (string languageCode in _languagesList)
			{
				XmlDocument languageSpecificCourseInfoDoc = null;
				
				// check to ensure there is a video file there
				string courseLanguagePath = coursePath + courseCode + "/titles/" + languageCode + ".xml";

				try {
				
					languageSpecificCourseInfoDoc = new XmlDocument();
					languageSpecificCourseInfoDoc.Load(courseLanguagePath);

					string courseName = languageSpecificCourseInfoDoc.SelectSingleNode("course").Attributes["name"].Value;
					
					OnlineCourseData courseD = new OnlineCourseData(courseCode, languageCode, courseName);
					
					course.Languages.Add(languageCode, courseD);
					
					languageSuccess	= true;
				} catch (Exception e) {					
				}
				
			}		
			
			if (languageSuccess) {
				return course;
			} else {
				return null;
			}
		}
		
		public static List<OnlineCourseUnit> GetCourseUnits(string courseCode) {
			return GetCourseUnits(courseCode, _defaultLanguage);
		}
		
		public static List<OnlineCourseUnit> GetCourseUnits(string courseCode, string language) {
			List<OnlineCourseUnit> units = new List<OnlineCourseUnit>();
			
			bool viewAll = (HttpContext.Current.Request.QueryString["admin"] + "" == "true" || HttpContext.Current.Request.QueryString["alumni"] + "" == "true");
			
			// my.dts.edu course units
			List<Unit> courseUnits = null;
			User user = Users.GetLoggedInUser();		
			if (user != null) {
				// get the courseID by looking at all the user's classes and matching the courseID
				List<Course> usersCourses = Courses.GetUsersRunningCourses(user.UserID, CourseUserRole.Student); //(int) UserRole.Student);
				//Course course = usersCourses.SingleOrDefault(uc => uc.CourseCode == courseCode);

				Course course = MatchCourseCode(courseCode, usersCourses);  usersCourses.SingleOrDefault(uc => uc.CourseCode == courseCode);

				if (course != null)
					courseUnits = course.Units.ToList();
			}
			
			// LOAD videoXmlPath		
			string videoXmlPath = Settings.PlayerFilesLocation + courseCode + @"\titles\en-US.xml";

			XmlDocument videosXmlDoc = new XmlDocument();					
			if (System.IO.File.Exists(videoXmlPath)) {
				
				// load local file

				XmlTextReader xmlTextReader = new XmlTextReader(videoXmlPath);
				videosXmlDoc.Load(xmlTextReader);
			} else {
				throw new Exception("Can't find " + videoXmlPath);
			}
			XmlNode rootNode = videosXmlDoc.SelectSingleNode("course");		
			
			
			// remove inactive units for logged in students, or visitors
			if (viewAll) {
				// don't make any changes
			} else {
			
				List<XmlNode> nodesToRemove = new List<XmlNode>();
					
				// for people with units, we'll need to check the active status	
				if (courseUnits != null) {
					
					foreach (XmlNode unitNode in rootNode) {
						int unitNumber = Convert.ToInt32(unitNode.Attributes["number"].Value);
						
						// go through the my.dts units
						foreach (Unit unit in courseUnits) {
							if (unit.SortOrder == unitNumber && !unit.IsActive) {
								nodesToRemove.Add(unitNode);				
							}
						}														
					}	
					
				// everyone else only gets 2
				} else {
									
					foreach (XmlNode unitNode in rootNode) {
						int unitNumber = Convert.ToInt32(unitNode.Attributes["number"].Value);
						if (unitNumber > 2)
							nodesToRemove.Add(unitNode);					
					}		
				
				}
				
				// remove all the inactive units
				foreach (XmlNode nodeToRemove in nodesToRemove) {
					rootNode.RemoveChild(nodeToRemove);
				}	
			}
			
			// create objects	
			foreach (XmlNode unitNode in rootNode) {
				int unitNumber = Convert.ToInt32(unitNode.Attributes["number"].Value);
				string unitName = unitNode.Attributes["name"].Value;
				
				OnlineCourseUnit unit = new OnlineCourseUnit(courseCode, unitNumber, unitName);
				units.Add(unit);
				
				// get videos!!	
				foreach (XmlNode videoNode in unitNode) {
					int videoNumber = Convert.ToInt32(videoNode.Attributes["number"].Value);
					string videoName = videoNode.Attributes["name"].Value;
					string speaker = videoNode.Attributes["speaker"].Value;
					string duration = videoNode.Attributes["duration"].Value;
					
					OnlineCourseVideo video = new OnlineCourseVideo(courseCode, unitNumber, videoNumber, videoName, duration, speaker);
					unit.Videos.Add(video);
				}
								
			}			
			
			
			return units;
		}

		private static Course MatchCourseCode(string courseCode, List<Course> coursesList) {

			List<Course> coursesThatMightMatch = new List<Course>();

			foreach (Course course in coursesList) {
				string generateCode = (course.CourseCode + ((course.VersionNumber > 1) ? "v" + course.VersionNumber.ToString() : "") ).Trim();
			
				if ( generateCode.ToUpper().Trim() == courseCode.ToUpper().Trim())
					coursesThatMightMatch.Add(course);
			}
			
			// if there are no courses, return -1
			if (coursesThatMightMatch.Count == 0) {
				return null;
				
			// if there's just one, then return that one
			} else if (coursesThatMightMatch.Count == 1) {
				return coursesThatMightMatch[0];
			
			// there's more than one, then try to find the OL one
			} else if (coursesThatMightMatch.Count > 1) {
				foreach (Course course in coursesThatMightMatch) {
					if (course.Section == "OL")
						return course;
				
				}
			
				// if none are "OL", just try the first one
				return coursesThatMightMatch[0];
			
			}
			
			return null;	
		}

	}



	public class OnlineCourse {
		public OnlineCourse(string code) {
			_code = code;
			_languages = new Dictionary<String, OnlineCourseData>();
		}

		private string _code;
		private Dictionary<String, OnlineCourseData> _languages;
		
		public string CourseCode
		{
			get { return _code; }
			set { _code = value; }
		}
		
		public Dictionary<String, OnlineCourseData> Languages
		{
			get { return _languages; }
		} 
		
		
		public OnlineCourseData DefaultCourseData {
			get {
				if (Languages["en-US"] != null) {
					return Languages["en-US"];
				} else {
					string firstKey = "";
					foreach (string key in Languages.Keys) {
						firstKey = key;
						break;
					}				
					
					return Languages[firstKey];	
				}
			}	
		}		
	}

	public class OnlineCourseData {
		public OnlineCourseData(string code, string language, string title) {
			_code = code;
			_title = title;
			_language = language;
			_units = null;
		}

		private string _code;
		private string _title;
		private string _language;
		private List<OnlineCourseUnit> _units;
		
		public string CourseCode {
			get { return _code; }
			set { _code = value; }
		}
	 
		public string Title {
			get { return _title; }
			set { _title = value; }
		} 
		
		public string Language {
			get { return _language; }
			set { _language = value; }
		}     

		public List<OnlineCourseUnit> Units {
			get {
				if (_units == null)
					_units = OnlineCourses.GetCourseUnits(_code,_language);
							
				return _units;
			}    
		}
		
		   
	}


	public class OnlineCourseUnit {

		public OnlineCourseUnit(string courseCode, int unitNumber, string unitName) {
			_courseCode = courseCode;
			_unitNumber = unitNumber;
			_unitName = unitName;
			_videos = new List<OnlineCourseVideo>();
		}

		private string _courseCode;
		private string _unitName;
		private int _unitNumber;
		private List<OnlineCourseVideo> _videos;
	   
		public string CourseCode {
			get { return _courseCode; }
			set { _courseCode = value; }
		}     
		public string UnitName {
			get { return _unitName; }
			set { _unitName = value; }
		}
		public int UnitNumber {
			get { return _unitNumber; }
			set { _unitNumber = value; }
		}    
		
		public List<OnlineCourseVideo> Videos {
			get { return _videos; }
		}    
	}

	public class OnlineCourseVideo {

		public OnlineCourseVideo(string courseCode, int unitNumber, int videoNumber, string videoName, string duration, string speaker) {
			_courseCode = courseCode;
			_unitNumber = unitNumber;
			_videoNumber = videoNumber;
			_videoName = videoName;
			_duration = duration;
			_speaker = speaker;
		}

		private string _courseCode;
		private int _unitNumber;
		private int _videoNumber;
		private string _videoName;
		private string _duration;
		private string _speaker;
	   
		public string CourseCode {
			get { return _courseCode; }
			set { _courseCode = value; }
		}  
		public int UnitNumber {
			get { return _unitNumber; }
			set { _unitNumber = value; }
		}  
		public int VideoNumber {
			get { return _videoNumber; }
			set { _videoNumber = value; }
		}  
		public string VideoName {
			get { return _videoName; }
			set { _videoName = value; }
		}
		public string Duration {
			get { return _duration; }
			set { _duration = value; }
		} 
		public string Speaker {
			get { return _speaker; }
			set { _speaker = value; }
		} 		
		public string FormattedDuration {
			get { 
				string duration = _duration.Replace("00:","");
				if (duration.IndexOf(".") > -1) 
					duration = duration.Substring(0,duration.IndexOf("."));
				
				return duration;
			}
		}        
	}

	public class OnlineCourseGroup {

		public OnlineCourseGroup(string name) {
			_name = name;
			courses = new List<OnlineCourse>();

		}


		private string _name;
		private List<OnlineCourse> courses;
	   
		public string Name {
			get { return _name; }
			set { _name = value; }
		}
		
		public List<OnlineCourse> Courses {
			get { return courses; }
		}    
	}


}