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

		public UserLine( UserLine copy )
		{
			#region param checks
			if( copy == null )
			{
				throw new ArgumentNullException( "copy" );
			}
			#endregion

			CachedWrapAt = -1;
			IsOutput = copy.IsOutput;

			foreach( var span in copy.Spans )
			{
				Spans.Add( new Span( span ) );
			}
		}

		public bool HasUserText()
		{
			for( int i = 0; i < Spans.Count; ++ i )
			{
				if( (i == 0) && (Spans[ i ].IsPrompt) )
				{
					continue;
				}

				if( !string.IsNullOrEmpty( Spans[ i ].Text ) )
				{
					return true;
				}
			}

			return false;
		}

		public int CachedWrapAt{ get; set; }
		public IList<CachedLine> CachedLines { get; set; }
	}
}