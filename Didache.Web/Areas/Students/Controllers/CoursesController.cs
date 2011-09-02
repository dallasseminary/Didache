using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Text;
using Ionic.Zip;

namespace Didache.Web.Areas.Students.Controllers
{
	
    public class CoursesController : Controller
    {
		DidacheDb db = new DidacheDb();

		[Authorize]
		public ActionResult Index() {

			List<Course> courses = null;
			User profile = Users.GetLoggedInUser();

			if (profile != null)
				courses = Didache.Courses.GetUsersCourses(profile.UserID, CourseUserRole.Student);
			else
				courses = new List<Course>();

			return View(courses.Where(c=>c.IsActive).ToList());
		}

		[Authorize]
		public ActionResult Dashboard(string slug) {

			Course course = Didache.Courses.GetCourseBySlug(slug);

			return View(course);
		}

		[Authorize]
		public ActionResult Tools(string slug) {

			Course course = Didache.Courses.GetCourseBySlug(slug);

			return View(course);
		}

		[Authorize]
		public ActionResult Schedule(string slug, int? id) {

			
			Course course = Didache.Courses.GetCourseBySlug(slug);
			User user = Users.GetLoggedInUser();
			List<Unit> units = Didache.Units.GetCourseUnits(course.CourseID);
			Unit currentUnit = null;
			List<UserTaskData> userTasks = null;			

			// (1) figure out which Unit to show (either by ID or date)
			if (id.HasValue) {
				// pick unit specified in URL
				currentUnit = units.AsQueryable().SingleOrDefault(u => u.UnitID == id.Value);
			} else {
				// pick current URL by date
				currentUnit = units.AsQueryable().SingleOrDefault(u => u.StartDate <= DateTime.Now && u.EndDate >= DateTime.Now);
			}

			// fallback to first unit
			if (currentUnit == null && units.Count > 0) {
				currentUnit = units[0];
			}


			// (2) get tasks
			if (currentUnit != null) {

				CourseUser thisUser = db.CourseUsers.SingleOrDefault(cu => cu.UserID == user.UserID && cu.RoleID == (int)CourseUserRole.Student && cu.CourseID == course.CourseID);

				userTasks = Tasks.GetUserTaskDataInUnit(currentUnit.UnitID, user.UserID, (thisUser==null));
			}


			// (3) all other data
			// TODO: more efficient
			
			ViewBag.AllUserTasks = db.UserTasks.Where(ut => ut.UserID == user.UserID && ut.CourseID == course.CourseID).ToList();
            ViewBag.UserGroups = Didache.Courses.GetCourseUserGroups(course.CourseID);
			ViewBag.Units = units;
			ViewBag.CurrentUnit = currentUnit;
			ViewBag.UserTasks = userTasks;

			return View(course);
		}

		[Authorize]
		public ActionResult Files(string slug) {

			Course course = Didache.Courses.GetCourseBySlug(slug);

			ViewBag.CourseFileGroups = CourseFiles.GetCourseFileGroups(course.CourseID);

			return View(course);
		}

		[Authorize]
		public ActionResult Roster(string slug) {

			//List<CourseUser> users = Didache.Courses.GetUsersInCourse(Didache.Courses.GetCourseBySlug(slug).CourseID, CourseUserRole.Student);

			//List<CourseUserGroup> users = Didache.Courses.GetUsersInCourse(Didache.Courses.GetCourseBySlug(slug).CourseID, CourseUserRole.Student);

			Course course = Didache.Courses.GetCourseBySlug(slug);

			ViewBag.UserGroups = Didache.Courses.GetCourseUserGroups(course.CourseID);

			return View(course);
		}

		[Authorize]
		public ActionResult Assignments(string slug) {

			User user = Users.GetLoggedInUser();
            Course course = Didache.Courses.GetCourseBySlug(slug);

            //ViewBag.CourseFileGroups = CourseFiles.GetCourseFileGroups(course.CourseID);
            //ViewBag.StudentFiles = db.StudentFiles;


			ViewBag.AllUserTasks = db.UserTasks
										.Include("Task.Unit")
										.Include("StudentFile")
										.Include("GradedFile")
										.Where(ut => ut.UserID == user.UserID && ut.CourseID == course.CourseID)
										.OrderBy(ut => ut.Unit.SortOrder)
											.ThenBy(ut => ut.Task.SortOrder)
										.ToList();


			return View(course);
		}

		[Authorize]
		public ActionResult CourseFile(int id, string filename) {
			
			CourseFile file = new DidacheDb().CourseFiles.Find(id);

			//string basePath = Settings.CourseFilesLocation;
			//string path = System.IO.Path.Combine(basePath, file.Filename);
			string path = file.PhysicalPath;

			if (System.IO.File.Exists(path)) {

				return File(path, file.ContentType);	
			} else {
				return HttpNotFound("Cannot find: " + file.Filename + " at " + file.PhysicalPath);
			}
		}

		[Authorize]
		public ActionResult StudentFile(int id, string filename) {

			StudentFile file = new DidacheDb().StudentFiles.Find(id);

			string path = file.PhysicalPath;

			if (System.IO.File.Exists(path)) {

				return File(path, file.ContentType);
			} else {
				return HttpNotFound("Cannot find: " + file.Filename);
			}
		}

		[Authorize]
		public ActionResult GradedFile(int id, string filename) {

			GradedFile file = new DidacheDb().GradedFiles.Find(id);

			string basePath = Settings.GradedFilesLocation;
			string path = System.IO.Path.Combine(basePath, file.StoredFilename);

			if (System.IO.File.Exists(path)) {

				return File(path, file.ContentType);
			} else {
				return HttpNotFound("Cannot find: " + file.Filename);
			}
		}

		[Authorize]
		public ActionResult DownloadAll(string slug) {

			Course course = Didache.Courses.GetCourseBySlug(slug);

			List<CourseFileGroup> groups = CourseFiles.GetCourseFileGroups(course.CourseID);

		
			ZipFile zip = new ZipFile();

			foreach (CourseFileGroup group in groups) {

				foreach (CourseFileAssociation cfa in group.CourseFileAssociations) {

					if (cfa.IsActive && System.IO.File.Exists(cfa.CourseFile.PhysicalPath)) {

						ZipEntry entry = zip.AddFile(cfa.CourseFile.PhysicalPath, "");
						try {
							entry.FileName = cfa.CourseFile.Filename;
						} catch {
							System.IO.FileInfo info = new System.IO.FileInfo(cfa.CourseFile.Filename);
							entry.FileName = info.Name + "-" + cfa.CourseFile.UniqueID.ToString() + info.Extension ;
						}
					}
				}
			}

			return new ZipFileResult(zip, slug + "-files.zip");
		}

		//
		// GET: /Feeds/
		public List<VideoInfo> GetVideoInfo(Course course, int userID) {
			List<VideoInfo> videosList = new List<VideoInfo>();

			string externalThumbBase = "http://dtsoe.s3.amazonaws.com"; // "http://oefiles.dts.edu";
			string xmlPath = Settings.PlayerFilesLocation + course.CourseCode + (course.VersionNumber > 1 ? "v" + course.VersionNumber : "") + @"\titles\en-US.xml";
			User user = Users.GetUser(userID);


			// load XML
			int selectedUnitNumber = -1;

			if (System.IO.File.Exists(xmlPath)) {
				System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
				doc.Load(xmlPath);

				Random random = new Random();


				System.Xml.XmlNodeList unitNodes = null;
				if (selectedUnitNumber > 0)
					unitNodes = doc.SelectNodes("//unit[@number=" + selectedUnitNumber + "]");
				else
					unitNodes = doc.SelectNodes("//unit");

				foreach (XmlNode uNode in unitNodes) {
					int unitNumber = 0;
					Int32.TryParse(uNode.Attributes["number"].Value, out unitNumber);
					System.Xml.XmlNodeList videoNodes = doc.SelectNodes("//unit[@number=" + unitNumber + "]/video");

					foreach (System.Xml.XmlNode vNode in videoNodes) {
						int videoNumber = 0;
						Int32.TryParse(vNode.Attributes["number"].Value, out videoNumber);

						videosList.Add(new VideoInfo {
							SortOrder = videoNumber++,
							UnitTaskInfo = "Unit " + unitNumber + ". Task " + videoNumber + ". ",
							CourseCode = course.CourseCode,
							UnitNumber = unitNumber,
							VideoNumber = videoNumber,
							Title = vNode.Attributes["name"].Value,
							Duration = vNode.Attributes["duration"].Value,
							Speaker = vNode.Attributes["speaker"].Value,
							VideoUrl = String.Format("{0}/{1}/{2}_u{3}_v{4}.mp4",
								externalThumbBase,
								course.CourseCode.ToLower() + (course.VersionNumber > 1 ? "v" + course.VersionNumber : ""),
								course.CourseCode.ToUpper() + (course.VersionNumber > 1 ? "v" + course.VersionNumber : ""),
								unitNumber.ToString().PadLeft(3, '0'),
								videoNumber.ToString().PadLeft(3, '0')),
							
							SlidesUrl = String.Format("https://my.dts.edu/download/slides/{0}-{1}-{2}-{3}-{4}-{5}.pdf",
								course.CourseID,
								course.CourseCode.ToLower() + (course.VersionNumber > 1 ? "v" + course.VersionNumber : ""),
								unitNumber,
								videoNumber,
								user.Language,
								user.UserID),

							TranscriptUrl = String.Format("https://my.dts.edu/download/transcript/{0}-{1}-{2}-{3}-{4}-{5}.pdf", 
								course.CourseID, 
								course.CourseCode.ToLower() + (course.VersionNumber > 1 ? "v" + course.VersionNumber : ""), 
								unitNumber, 
								videoNumber,
								user.Language,
								user.UserID),


							ThumbnailFilename = String.Format("{0}_u{1}_v{2}_thumb.jpg",
								course.CourseCode.ToUpper() + (course.VersionNumber > 1 ? "v" + course.VersionNumber : ""),
								unitNumber.ToString().PadLeft(3, '0'),
								videoNumber.ToString().PadLeft(3, '0')),

							//ThumbnailUrl = String.Format("{0}/{1}/videos/{2}_u{3}_v{4}_thumb.jpg", locaThumbBase, Model.Course.CourseCode.ToLower(), Model.Course.CourseCode.ToUpper(), Model.Unit.UnitNumber.ToString().PadLeft(3, '0'), vNode.Attributes["number"].Value.ToString().PadLeft(3, '0')),
							ThumbnailUrl = String.Format("{0}/{1}/{2}_u{3}_v{4}_thumb.jpg",
								externalThumbBase,
								course.CourseCode.ToLower() + (course.VersionNumber > 1 ? "v" + course.VersionNumber : ""),
								course.CourseCode.ToUpper() + (course.VersionNumber > 1 ? "v" + course.VersionNumber : ""),
								unitNumber.ToString().PadLeft(3, '0'),
								videoNumber.ToString().PadLeft(3, '0')),

							PercentComplete = random.Next(100)
						});
					}
				}
			}
			else {
				//Response.Write("can't find: " + xmlPath);	
			}


			return videosList;

		}



		public ActionResult iCal(string slug, string type, int userID, string extension) {

			DidacheDb db = new DidacheDb();

			Course course = Didache.Courses.GetCourseBySlug(slug);
			User user = Users.GetUser(userID);

			if (user == null)
				return HttpNotFound();

			List<UserTaskData> userTask = db.UserTasks
											.Include("Task.Unit")
											.Where(ut => ut.UserID == user.UserID && ut.CourseID == course.CourseID)
											.OrderBy(t => t.Task.Unit.SortOrder)
												.ThenBy(t => t.Task.SortOrder)
											.ToList();

			List<ICalEvent> iEvents = new List<ICalEvent>();

			foreach (UserTaskData task in userTask) {
				DateTime dueDate;

				if (task.Task.DueDate.HasValue)
					dueDate = task.Task.DueDate.Value;
				else
					dueDate = task.Task.Unit.EndDate;

				iEvents.Add(new ICalEvent() {
					UID = task.TaskID.ToString(),
					Summary = task.Task.Name + " (Unit " + task.Unit.SortOrder.ToString() + ")" +  ((task.TaskCompletionStatus == TaskCompletionStatus.Completed) ? " [completed]" : ""),
					Description = Utility.StripHtml(task.Task.Instructions),
					Location = "",
					//StartUtc = dueDate.ToUniversalTime(),
					//EndUtc = dueDate.ToUniversalTime()

					StartUtc = dueDate,
					EndUtc = dueDate

				});
			}

			return new ICalEventResult(
						course.ToString() + " Tasks",
						"Dallas Theological Seminary",
						"", //"DTS-" + course.CourseCode + course.Section + ".ics",
						iEvents);

		}

		public ActionResult FeedOld(string slug, int userID, string type, string extension) {

			DidacheDb db = new DidacheDb();
			Course course = Didache.Courses.GetCourseBySlug(slug);
			User user = Users.GetUser(userID);

			if (user == null)
				return HttpNotFound();

			List<SyndicationItem> items = new List<SyndicationItem>();


			// get current and next two units for this course
			DateTime unitStartDate = DateTime.Now.AddDays(21);
			List<Unit> units = db.Units.Where(u => u.CourseID == course.CourseID && u.StartDate <= unitStartDate).ToList();

			// get all vidoes
			List<VideoInfo> videoInfos = GetVideoInfo(course, userID);

			foreach (Unit unit in units) {

				foreach (VideoInfo videoInfo in videoInfos.Where(vi=> vi.UnitNumber == unit.SortOrder)) {
					
					// VIDEO
					SyndicationItem item = new SyndicationItem(
							videoInfo.Title,
							videoInfo.Title,
							new Uri("http://online.dts.edu/courses/" + course.Slug), // new Uri(Utility.BaseFullUrl + post.PostUrl),
							String.Format("c{0}u{1}v{2}", videoInfo.CourseCode, videoInfo.UnitNumber, videoInfo.VideoNumber),
							DateTime.Now // need to hook to unit start date
						);

					long videoLength = 10000;
					item.Links.Add(
						SyndicationLink.CreateMediaEnclosureLink(new Uri(videoInfo.VideoUrl), "video/mp4", videoLength)
					
						//SyndicationLink.CreateMediaEnclosureLink(new Uri("http://online.dts.edu/" + String.Format("download/video/{0}-{1}-{2}-{3}-{4}.mp4", course.CourseID, course.CourseCode, videoInfo.UnitNumber, videoInfo.VideoNumber, userID)), "video/mp4", videoLength)
					);

					items.Add(item);
				}
			}

			SyndicationFeed feed = new SyndicationFeed(
										course.ToString() + " :: RSS",
										"Video, Slides, and Transcripts",
										new Uri("http://online.dts.edu/"),
										items);



			return new SyndicationResult(feed, extension);
		}

		public ActionResult Feed(string slug, int userID, string type, string extension) {

			DidacheDb db = new DidacheDb();
			Course course = Didache.Courses.GetCourseBySlug(slug);
			User user = Users.GetUser(userID);

			if (user == null)
				return HttpNotFound();

			// BEING XML/RSS
			XmlTextWriter writer = new XmlTextWriter(Response.OutputStream, Encoding.UTF8);
			Response.ContentType = "application/rss+xml";
			Response.ContentEncoding = Encoding.UTF8;
			writer.WriteStartDocument();

			// write RSS 2.0 starter
			writer.WriteStartElement("rss");
			writer.WriteAttributeString("version", "2.0");
			writer.WriteAttributeString("xmlns", "itunes", null, "http://www.itunes.com/dtds/podcast-1.0.dtd");
			writer.WriteStartElement("channel");

			writer.WriteElementString("title", "DTS-" + course.CourseCode + course.Section + "-" + course.Name);
			writer.WriteElementString("link", "https://online.dts.edu/courses/" + course.Slug);
			writer.WriteElementString("description", course.Description);
			writer.WriteElementString("language", user.Language);
			writer.WriteElementString("docs", "http://blogs.law.harvard.edu/tech/rss");
			writer.WriteElementString("copyright", "This work is copyright 2004-" + DateTime.Now.Year + " by Dallas Theological Seminary and the individual speakers.");
			//writer.WriteElementString("lastBuildDate", mediaItems[0].OnlineDate.ToString("ddd, d MMM yyyy hh:mm:ss") + " CST");
			writer.WriteElementString("webMaster", "webmaster@dts.edu (Dallas Theological Seminary)");
			writer.WriteElementString("category", "Religion");



			// get current and next two units for this course
			DateTime unitStartDate = DateTime.Now.AddDays(21);
			//List<Unit> units = db.Units.Where(u => u.CourseID == course.CourseID && u.StartDate <= unitStartDate).ToList();
			List<Unit> units = db.Units.Where(u => u.CourseID == course.CourseID).ToList();

			// get all vidoes
			List<VideoInfo> videoInfos = GetVideoInfo(course, userID);

			foreach (Unit unit in units) {

				foreach (VideoInfo videoInfo in videoInfos.Where(vi => vi.UnitNumber == unit.SortOrder)) {

					//string videoName = videoInfo.Title;
					//int videoNumber = videoInfo.SortOrder;
					//string duration = videoInfo.Duration;


					// video
					writer.WriteStartElement("item");

					writer.WriteElementString("title", String.Format("{0} unit {1} video {2}", course.CourseCode, unit.SortOrder, videoInfo.SortOrder));
					writer.WriteElementString("link", "https://my.dts.edu/courses/" + course.Slug + "/schedule/" + unit.UnitID.ToString());
					writer.WriteElementString("description", videoInfo.Title);

					writer.WriteElementString("author", videoInfo.Speaker);
					writer.WriteElementString("pubDate", unit.StartDate.AddMinutes(videoInfo.SortOrder).ToString("ddd, d MMM yyyy hh:mm:ss") + " CST"); //"DDD, dd MMM yyyy HH:mm:ss"

					// VIDEO
					writer.WriteStartElement("enclosure");
					writer.WriteAttributeString("url", videoInfo.VideoUrl);
					//writer.WriteAttributeString("length", mediaItem.VideoBytesLength.ToString());
					writer.WriteAttributeString("type", "video/mp4");
					writer.WriteEndElement(); // enclosure


					writer.WriteStartElement("guid");
					writer.WriteAttributeString("isPermaLink", "false");
					writer.WriteString(String.Format("video-c{0}u{1}v{2}", course.CourseCode, unit.SortOrder, videoInfo.SortOrder));
					writer.WriteEndElement(); // guid

					writer.WriteStartElement("itunes", "author", null);
					writer.WriteString(videoInfo.Speaker);
					writer.WriteEndElement();
					writer.WriteStartElement("itunes", "subtitle", null);
					writer.WriteString(videoInfo.Title);
					writer.WriteEndElement();
					writer.WriteStartElement("itunes", "summary", null);
					writer.WriteString(videoInfo.Title);
					writer.WriteEndElement();
					writer.WriteStartElement("itunes", "duration", null);
					writer.WriteString(videoInfo.Duration);
					writer.WriteEndElement();


					writer.WriteEndElement(); // item


					/*
					// PDF transcript
					writer.WriteStartElement("item");

					writer.WriteElementString("title", String.Format("{0} unit {1} video {2} transcript", course.CourseCode, unit.SortOrder, videoInfo.SortOrder));
					writer.WriteElementString("link", "https://online.dts.edu/courses/" + course.Slug + "/schedule/" + unit.UnitID.ToString());
					writer.WriteElementString("description", videoInfo.Title);

					writer.WriteElementString("author", videoInfo.Speaker);
					writer.WriteElementString("pubDate", unit.StartDate.AddMinutes(videoInfo.SortOrder).ToString("ddd, d MMM yyyy hh:mm:ss") + " CST"); //"DDD, dd MMM yyyy HH:mm:ss"

					// PDF transcript
					writer.WriteStartElement("enclosure");
					writer.WriteAttributeString("url", videoInfo.TranscriptUrl);
					writer.WriteAttributeString("type", "application/pdf");
					writer.WriteEndElement(); // enclosure  

					writer.WriteStartElement("guid");
					writer.WriteAttributeString("isPermaLink", "false");
					writer.WriteString(String.Format("transcript-c{0}u{1}v{2}", course.CourseCode, unit.SortOrder, videoInfo.SortOrder));
					writer.WriteEndElement(); // guid

					writer.WriteStartElement("itunes", "author", null);
					writer.WriteString(videoInfo.Speaker);
					writer.WriteEndElement();
					writer.WriteStartElement("itunes", "subtitle", null);
					writer.WriteString(videoInfo.Title);
					writer.WriteEndElement();
					writer.WriteStartElement("itunes", "summary", null);
					writer.WriteString(videoInfo.Title);
					writer.WriteEndElement();
					writer.WriteStartElement("itunes", "duration", null);
					writer.WriteString(videoInfo.Duration);
					writer.WriteEndElement();

					writer.WriteEndElement(); // item    



					// PDF Slides
					writer.WriteStartElement("item");

					writer.WriteElementString("title", String.Format("{0} unit {1} video {2} slides", course.CourseCode, unit.SortOrder, videoInfo.SortOrder));
					writer.WriteElementString("link", "https://online.dts.edu/courses/" + course.Slug + "/schedule/" + unit.UnitID.ToString());
					writer.WriteElementString("description", videoInfo.Title);

					writer.WriteElementString("author", videoInfo.Speaker);
					writer.WriteElementString("pubDate", unit.StartDate.AddMinutes(videoInfo.SortOrder).ToString("ddd, d MMM yyyy hh:mm:ss") + " CST"); //"DDD, dd MMM yyyy HH:mm:ss"


					// PDF slides
					writer.WriteStartElement("enclosure");
					writer.WriteAttributeString("url", videoInfo.SlidesUrl);
					writer.WriteAttributeString("type", "application/pdf");
					writer.WriteEndElement(); // enclosure  

					writer.WriteStartElement("guid");
					writer.WriteAttributeString("isPermaLink", "false");
					writer.WriteString(String.Format("slides-c{0}u{1}v{2}", course.CourseCode, unit.SortOrder, videoInfo.SortOrder));
					writer.WriteEndElement(); // guid

					writer.WriteStartElement("itunes", "author", null);
					writer.WriteString(videoInfo.Speaker);
					writer.WriteEndElement();
					writer.WriteStartElement("itunes", "subtitle", null);
					writer.WriteString(videoInfo.Title);
					writer.WriteEndElement();
					writer.WriteStartElement("itunes", "summary", null);
					writer.WriteString(videoInfo.Title);
					writer.WriteEndElement();
					writer.WriteStartElement("itunes", "duration", null);
					writer.WriteString(videoInfo.Duration);
					writer.WriteEndElement();
					

					writer.WriteEndElement(); // item   
					  
					 */
				}
			}


			writer.WriteEndElement(); // channel
			writer.WriteEndElement(); // rss
			writer.WriteEndDocument();
			writer.Close();

			return null;


		}


		/*
		public ActionResult iCalUnits(string slug) {
			Course course = Didache.Courses.GetCourseBySlug(slug);
		}
		*/
    }


}
