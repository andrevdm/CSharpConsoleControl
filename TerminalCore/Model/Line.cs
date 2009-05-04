using System.Collections.Generic;

namespace TerminalCore.Model
{
	public class Line
	{
		public Line()
		{
			Spans = new List<Span>();
		}

		public bool HasUserText()
		{
			for( int i = 1; i < Spans.Count; ++ i )
			{
				if( !string.IsNullOrEmpty( Spans[ i ].Text ) )
				{
					return true;
				}
			}

			return false;
		}

		public Span LastUserSpan
		{
			get 
			{
				//There must be a prompt
				if( Spans.Count == 0 )
				{
					Spans.Add( new Span( ">" ) );
				}

				//Add the user span if there is not one
				if( Spans.Count == 1 )
				{
					Spans.Add( new Span( "" ) );
				}

				return Spans[ Spans.Count - 1 ];
			}
		}

		public List<Span> Spans { get; private set; }
	}
}