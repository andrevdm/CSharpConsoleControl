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

		public Span( Span copy )
		{
			#region param checks
			if( copy == null )
			{
				throw new ArgumentNullException( "copy" );
			}	
			#endregion

			Text = copy.Text;
			IsPrompt = copy.IsPrompt;
			BackgroundColour = copy.BackgroundColour;
			ForegroundColour = copy.ForegroundColour;
		}

		public Colour BackgroundColour { get; private set; }
		public Colour ForegroundColour { get; private set; }
		public string Text { get; set; }
		public bool IsPrompt { get; set; }
	}
}