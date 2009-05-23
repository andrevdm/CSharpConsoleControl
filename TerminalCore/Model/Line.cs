using System;
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

		public Span LastUserSpan
		{
			get
			{
				//There must be a prompt
				if( Spans.Count == 0 )
				{
					throw new Exception( "Line is missing a prompt" );
				}

				//Add an input span if there is not one
				if( Spans.Count == 1 )
				{
					Spans.Add( new Span( "" ) );
				}

				return Spans[ Spans.Count - 1 ];
			}
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