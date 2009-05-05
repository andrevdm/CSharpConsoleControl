using System.Collections.Generic;
using System.Text;

namespace TerminalCore.Model
{
	public abstract class Line
	{
		protected Line()
		{
			Spans = new List<Span>();
		}

		public override string ToString()
		{
			var str = new StringBuilder();

			Spans.ForEach( s => str.Append( s.Text ) );

			return str.ToString();
		}
		
		public List<Span> Spans { get; private set; }
	}
}