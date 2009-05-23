using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TerminalCore.Model
{
	public abstract class Line
	{
		protected Line()
		{
			Spans = new List<Span>();
		}

		public string ToString( bool includePrompt )
		{
			var str = new StringBuilder();

			for( int i = 0; i < Spans.Count; ++i )
			{
				if( (!includePrompt) && (i == 0) && Spans[ i ].IsPrompt )
				{
					continue;
				}

				str.Append( Spans[ i ].Text );
			}

			return str.ToString();
		}

		public override string ToString()
		{
			return ToString( false );
		}
		
		public IList<Span> Spans { get; private set; }
		public bool IsOutput { get; set; }
	}
}