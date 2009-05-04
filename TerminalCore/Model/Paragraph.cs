using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TerminalCore.Model
{
	public class Paragraph
	{
		public Paragraph()
		{
			Spans = new List<Span>();
		}

		public List<Span> Spans { get; private set; }
	}
}