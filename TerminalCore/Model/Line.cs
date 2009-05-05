using System.Collections.Generic;

namespace TerminalCore.Model
{
	public abstract class Line
	{
		protected Line()
		{
			Spans = new List<Span>();
		}
		
		public List<Span> Spans { get; private set; }
	}
}