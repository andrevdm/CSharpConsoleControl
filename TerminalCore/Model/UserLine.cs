using System;
using System.Collections.Generic;

namespace TerminalCore.Model
{
	public class UserLine : Line
	{
		public UserLine()
		{
			CachedWrapAt = -1;
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
					throw new Exception( "Line is missing a prompt" );
				}

				//Add an input span if there is not one
				if( Spans.Count == 1 )
				{
					Spans.Add( new InputSpan( "" ) );
				}

				return Spans[ Spans.Count - 1 ];
			}
		}

		public int CachedWrapAt{ get; set; }
		public List<CachedLine> CachedLines { get; set; }
	}
}