namespace TerminalCore.Model
{
	public class SizeF
	{
		public SizeF( float width, float height )
		{
			Width = width;
			Height = height;
		}

		public float Width { get; private set; }
		public float Height { get; private set; }
	}
}