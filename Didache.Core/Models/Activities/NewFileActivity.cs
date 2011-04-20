﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Didache {
	public class NewFileActivity : ActivityStreamItemBase {

		public int FileID { get; set; }
		public CourseFile CourseFile { get; set; }

		public override string FormatActivity() {
			return User.FormattedName + " uploaded <a href=\"" + CourseFile.FileUrl + "\">" + CourseFile.Filename + "</a>";
		}
	}
}
