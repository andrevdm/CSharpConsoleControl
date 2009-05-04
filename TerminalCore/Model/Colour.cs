namespace TerminalCore.Model
{
	public class Colour
	{
		public Colour( byte red, byte green, byte blue )
			: this( 255, red, green, blue )
		{

		}

		public Colour( byte alpha, byte red, byte green, byte blue )
		{
			Alpha = alpha;
			Red = red;
			Green = green;
			Blue = blue;
		}

		public byte Alpha { get; set; }
		public byte Red { get; set; }
		public byte Green { get; set; }
		public byte Blue { get; set; }
	}

	public static class Colours
	{
		static Colours()
		{
			Black = new Colour( 0, 0, 0 );
			White = new Colour( 255, 255, 255 );
			Red = new Colour( 255, 0, 0 );
			Green = new Colour( 0, 255, 0 );
			Blue = new Colour( 0, 0, 255 );
		}

		public static Colour Black { get; private set; }
		public static Colour White { get; private set; }
		public static Colour Red { get; private set; }
		public static Colour Green { get; private set; }
		public static Colour Blue { get; private set; }
	}
}