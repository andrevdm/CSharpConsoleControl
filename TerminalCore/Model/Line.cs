using System.Collections.Generic;

namespace TerminalCore.Model
{
	public class Line
	{
		public Line()
		{
			Spans = new List<Span>();
		}

		public List<Span> Spans { get; private set; }
	}
}