using System.Collections.Generic;

namespace TerminalCore.Model
{
	public class Line
	{
		public Line()
		{
			Spans = new List<Span>();
		}

		public bool HasText()
		{
			foreach( var span in Spans )
			{
				if( !string.IsNullOrEmpty( span.Text ) )
				{
					return true;
				}
			}

			return false;
		}

		public Span LastSpan
		{
			get 
			{
				if( Spans.Count == 0 )
				{
					Spans.Add( new Span( "" ) );
				}

				return Spans[ Spans.Count - 1 ];
			}
		}

		public List<Span> Spans { get; private set; }
	}
}