using System;

namespace TerminalCore.Model
{
	public class CachedLine : Line
	{
		public CachedLine( UserLine realLine )
		{
			#region param checks
			if( realLine == null )
			{
				throw new ArgumentNullException( "realLine" );
			}
			#endregion

			RealLine = realLine;
		}

		public UserLine RealLine { get; private set; }
	}
}