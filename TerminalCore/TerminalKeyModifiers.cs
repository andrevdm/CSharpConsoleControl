using System;

namespace TerminalCore
{
	[Flags]
	public enum TerminalKeyModifiers
	{
		None = 0,
		Control = 1,
		Alt = 2,
		Shift = 4
	}
}
