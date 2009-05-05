using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TerminalCore
{
	[Flags]
	public enum TerminalKeyModifier
	{
		None = 0,
		Control = 1,
		Alt = 2,
		Shift = 4
	}
}
