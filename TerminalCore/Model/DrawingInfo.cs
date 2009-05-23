using System;
using System.Collections.Generic;

namespace TerminalCore.Model
{
	public class DrawingInfo
	{
		public DrawingInfo( IEnumerable<Line> lines, CursorPosition cursorPosition )
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

		public IEnumerable<Line> Lines { get; private set; }
		public CursorPosition CursorPosition { get; private set; }
	}
}
