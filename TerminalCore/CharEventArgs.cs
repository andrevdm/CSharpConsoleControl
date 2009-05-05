using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TerminalCore
{
	public class CharEventArgs : EventArgs
	{
		public CharEventArgs( char c )
		{
			Char = c;
		}

		public Char Char { get; private set; }
	}
}
