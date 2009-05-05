using System.Text;
using System.Collections.Generic;

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

			for( int i = 0; i < Spans.Count; ++i )
			{
				if( (i == 0) && Spans[ i ].IsPrompt )
				{
					continue;
				}

				str.Append( Spans[ i ].Text );
			}

			return str.ToString();
		}
		
		public List<Span> Spans { get; private set; }
		public bool IsOutput { get; set; }
	}
}