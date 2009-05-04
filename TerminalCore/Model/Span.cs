using System;

namespace TerminalCore.Model
{
	public class Span
	{
		public Span( string text )
			: this( text, null, null, null )
		{
		}

		public Span( string text, SpanFont font )
			: this( text, font, null, null )
		{
		}

		public Span( string text, SpanFont font, Colour foregroundColour, Colour backgroundColour )
		{
			#region param checks
			if( text == null )
			{
				throw new ArgumentNullException( "text" );
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
		public string Text { get; set; }
	}
}