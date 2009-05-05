namespace TerminalCore.Model
{
	public class SizeD
	{
		public SizeD( double width, double height )
		{
			Width = width;
			Height = height;
		}

		public double Width { get; private set; }
		public double Height { get; private set; }
	}
}