using System;

namespace TerminalCore.Model
{
	public class Span
	{
		public Span( string text )
			: this( text, null, null )
		{
		}

		public Span( string text, Colour foregroundColour )
			: this( text, foregroundColour, null )
		{
		}

		public Span( string text, Colour foregroundColour, Colour backgroundColour )
		{
			#region param checks
			if( text == null )
			{
				throw new ArgumentNullException( "text" );
			}
			#endregion

			Text = text;
			ForegroundColour = foregroundColour;
			BackgroundColour = backgroundColour;
		}

		public Colour BackgroundColour { get; private set; }
		public Colour ForegroundColour { get; private set; }
		public string Text { get; set; }
	}
}