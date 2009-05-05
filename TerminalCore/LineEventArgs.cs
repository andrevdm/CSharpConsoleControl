using System;

namespace TerminalCore
{
	public class LineEventArgs : EventArgs
	{
		public LineEventArgs( string line )
		{
			#region param checks
			if( line == null )
			{
				throw new ArgumentNullException( "line" );
			}
			#endregion

			Line = line;
		}

		public string Line { get; private set; }
	}
}
