using System;

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
