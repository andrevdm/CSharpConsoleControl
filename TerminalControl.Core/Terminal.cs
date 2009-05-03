using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TerminalControl.Core
{
	public class Terminal
	{
		public Terminal( string prompt )
		{
			#region param checks
			if( prompt == null )
			{
				throw new ArgumentNullException( "prompt" );
			}
			#endregion

			Prompt = prompt;
		}

		public string Prompt { get; private set; }
	}
}
