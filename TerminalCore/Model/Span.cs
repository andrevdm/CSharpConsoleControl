using System;

namespace TerminalCore.Model
{
	public class Span
	{
		public Span( string text, SpanFont font, Colour foregroundColour, Colour backgroundColour )
		{
			#region param checks
			if( text == null )
			{
				throw new ArgumentNullException( "text" );
			}

			if( font == null )
			{
				throw new ArgumentNullException( "font" );
			}
			#endregion

			Text = text;
			Font = font;
			ForegroundColour = foregroundColour;
			BackgroundColour = backgroundColour;
		}

		public SpanFont Font { get; private set; }
		public Colour BackgroundColour { get; private set; }
		public Colour ForegroundColour { get; private set; }
		public string Text { get; private set; }
	}
}