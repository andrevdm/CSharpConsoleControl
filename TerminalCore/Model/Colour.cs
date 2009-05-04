using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TerminalCore.Model
{
	public class Colour
	{
		public Colour( int alpha, int red, int green, int blue )
		{
			Alpha = alpha;
			Red = red;
			Green = green;
			Blue = blue;
		}

		public int Alpha { get; set; }
		public int Red { get; set; }
		public int Green { get; set; }
		public int Blue { get; set; }
	}
}