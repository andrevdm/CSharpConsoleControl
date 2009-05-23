namespace TerminalCore.Model
{
	public class CursorPosition
	{
		public CursorPosition( double x, double y )
		{
			X = x;
			Y = y;
		}

		public double X { get; set; }
		public double Y { get; set; }
	}
}
