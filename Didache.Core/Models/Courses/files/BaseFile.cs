using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Script.Serialization;
using System.Runtime.Serialization;

namespace Didache {
	public class BaseFile {
		[Key]
		public int FileID { get; set; }
		public Guid UniqueID { get; set; }
		public string ContentType { get; set; }
		public string Filename { get; set; }
		public int Length { get; set; }
		public int UserID { get; set; }
		public DateTime UploadedDate { get; set; }

		[ScriptIgnore]
		public virtual User User { get; set; }

		public string StoredFilename {
			get {
				return UniqueID.ToString() + new System.IO.FileInfo(Filename).Extension;
			}
		}

		public string FileType {
			get {
				string ext = (Filename.IndexOf(".") > -1) ? Filename.Substring(Filename.LastIndexOf(".") + 1) : "";
				string fileType = "";

				switch (ext.ToLower()) {
					default:
						fileType = "unknown";
						break;
					case "doc":
					case "docx":
						fileType = "word";
						break;
					case "pdf":
						fileType = "pdf";
						break;
					case "xls":
					case "xlsx":
						fileType = "excel";
						break;
					case "ppt":
					case "pttx":
						fileType = "powerpoint";
						break;
					case "txt":
						fileType = "text";
						break;
					case "jpg":
					case "png":
					case "gif":
					case "jpeg":
					case "jpe":
						fileType = "image";
						break;
					case "mp4":
					case "avi":
					case "ogv":
					case "3gp":
					case "mpeg":
					case "mov":
						fileType = "video";
						break;
					case "mp3":
					case "wav":
						fileType = "audio";
                        break;
                    case "zip":
                    case "rar":
                    case "7z":
                    case "7zip":
                    case "tar":
                    case "gz":
                        fileType = "compress";
                        break;
				}


				return fileType;
			}
		}

		public string FormattedLength {
			get {
				return FormatBytes(Length);
			}
		}

		private string FormatBytes(int bytes) {
			const int scale = 1024;
			string[] orders = new string[] { "GB", "MB", "KB", "Bytes" };
			long max = (long)Math.Pow(scale, orders.Length - 1);

			foreach (string order in orders) {
				if (bytes > max)
					return string.Format("{0:##.##} {1}", decimal.Divide(bytes, max), order);

				max /= scale;
			}
			return "0 Bytes";
		}

		public virtual string PhysicalPath { 
			get { 
				
				
				return ""; 
			} 
		}

	}
}
