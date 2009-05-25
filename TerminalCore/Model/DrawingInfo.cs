using System;
using System.Collections.Generic;

namespace TerminalCore.Model
{
	public class DrawingInfo
	{
		public DrawingInfo( IEnumerable<CachedLine> lines, CursorPosition cursorPosition )
		{
			#region param checks
			if( lines == null )
			{
				throw new ArgumentNullException( "lines" );
			}
			
			if( cursorPosition == null )
			{
				throw new ArgumentNullException( "cursorPosition" );
			}
			#endregion

			Lines = lines;
			CursorPosition = cursorPosition;
		}

		public IEnumerable<CachedLine> Lines { get; private set; }
		public CursorPosition CursorPosition { get; private set; }
	}
}
