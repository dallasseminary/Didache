using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace Didache {
	public class Emails {

		public static void SendEmail(string fromEmail, string toEmail, string subject, string body) {
			SendEmail(fromEmail, new List<string>() { toEmail }, subject, body, false);
		}

		public static void SendEmail(string fromEmail, string toEmail, string subject, string body, bool isHtml) {
			SendEmail(fromEmail, new List<string>() { toEmail }, subject, body, isHtml);
		}

		public static void SendEmail(string fromEmail, List<string> toEmails, string subject, string body) {
			SendEmail(fromEmail, toEmails, subject, body, false);
		}

		public static void SendEmail(string fromEmail, List<string> toEmails, string subject, string body, bool isHtml) {

			SmtpClient client = new SmtpClient();

			MailMessage message = new MailMessage();
			message.From = new MailAddress(fromEmail);
			foreach (String email in toEmails)
				message.To.Add(new MailAddress(email));
			
			
			message.Subject = "[DTS Online] " + subject;
			message.BodyEncoding = Encoding.UTF8;
			message.SubjectEncoding = Encoding.UTF8;
			message.IsBodyHtml = isHtml;
			message.Body = body;

			try {
				client.Send(message);
			} catch {
				System.IO.File.AppendAllText( System.Web.HttpContext.Current.Server.MapPath("~/tools/email.txt"), fromEmail + "\t" + toEmails[0]+ "\t" + subject+ "\n" + body + "\n==================\n"); 
			}
		}

		public static void EnqueueEmail(string fromEmail, string toEmail, string subject, string body, bool isHtml) {
			
			// TODO: make this faster by queueing for a later thread
			
			SendEmail(fromEmail, toEmail,  subject,  body,  isHtml);
		}

		public static string FormatEmail(string text, Course course, Unit unit, Task task, User fromUser, User toUser, UserTaskData userTaskData, InteractionPost interactionPost, Post forumPost) {

			if (course != null) {
				text = text.Replace("{course-code}", course.CourseCode + course.Section);
				text = text.Replace("{course-name}", course.Name);
			}

			if (fromUser != null) {
				text = text.Replace("{fromuser-secureformattedname}", fromUser.SecureFormattedName);
				text = text.Replace("{fromuser-secureshortname}", fromUser.SecureShortName);
			}

			if (toUser != null) {
				text = text.Replace("{touser-secureformattedname}", toUser.SecureFormattedName);
				text = text.Replace("{touser-secureshortname}", toUser.SecureShortName);
			}

			if (task != null) {
				text = text.Replace("{task-name}", task.Name);
			}

			if (userTaskData != null) {
				text = text.Replace("{usertaskdata-numericgrade}", userTaskData.NumericGrade.HasValue ? userTaskData.NumericGrade.Value.ToString() : "(none)");
				text = text.Replace("{usertaskdata-studentcomments}", userTaskData.StudentComments);
				text = text.Replace("{usertaskdata-gradercomments}", userTaskData.GraderComments);
				text = text.Replace("{usertaskdata-gradedfile-url}", userTaskData.GradedFile != null ? "https://online.dts.edu" + userTaskData.GradedFile.FileUrl : "");
				text = text.Replace("{usertaskdata-studentfile-url}", userTaskData.StudentFile != null ? "https://online.dts.edu" + userTaskData.StudentFile.FileUrl : "");
			}

			if (interactionPost != null) {
				text = text.Replace("{interactionpost-postcontent}", interactionPost.PostContent);
				text = text.Replace("{interactionpost-posturl}", "https://online.dts.edu" + interactionPost.PostUrl);
			}

			return text;
		}
	}
}
