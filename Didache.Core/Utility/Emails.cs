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
	}
}
