using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Didache {
	public class Markdown {
		public static string Transform(string input) {
			//MarkdownSharp.MarkdownOptions mdo = new MarkdownSharp.MarkdownOptions();
			//mdo.AutoHyperlink = true;
			//mdo.AutoNewlines = true;
			MarkdownSharp.Markdown md = new MarkdownSharp.Markdown(); //mdo);
			md.AutoHyperlink = true;

			return md.Transform(input);
		}

	}
}
