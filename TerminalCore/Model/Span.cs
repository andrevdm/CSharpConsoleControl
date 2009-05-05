using System;

namespace TerminalCore.Model
{
	public class Span
	{
		public Span( SpanType spanType, string text )
			: this( spanType, text, null, null )
		{
		}

		public Span( SpanType spanType, string text, Colour foregroundColour )
			: this( spanType, text, foregroundColour, null )
		{
		}

		public Span( SpanType spanType, string text, Colour foregroundColour, Colour backgroundColour )
		{
			#region param checks
			if( text == null )
			{
				throw new ArgumentNullException( "text" );
			}
			#endregion

			SpanType = spanType;
			Text = text;
			ForegroundColour = foregroundColour;
			BackgroundColour = backgroundColour;
		}

		public SpanType SpanType { get; private set; }
		public Colour BackgroundColour { get; private set; }
		public Colour ForegroundColour { get; private set; }
		public string Text { get; set; }
	}

	public class PromptSpan : Span
	{
		public PromptSpan( string text )
			: base( SpanType.Prompt, text )
		{
		}

		public PromptSpan( string text, Colour foregroundColour )
			: base( SpanType.Prompt, text, foregroundColour )
		{
		}

		public PromptSpan( string text, Colour foregroundColour, Colour backgroundColour )
			: base( SpanType.Prompt, text, foregroundColour, backgroundColour )
		{
		}
	}

	public class PromptWrapSpan : Span
	{
		public PromptWrapSpan( string text )
			: base( SpanType.PromptWrap, text )
		{
		}

		public PromptWrapSpan( string text, Colour foregroundColour )
			: base( SpanType.PromptWrap, text, foregroundColour )
		{
		}

		public PromptWrapSpan( string text, Colour foregroundColour, Colour backgroundColour )
			: base( SpanType.PromptWrap, text, foregroundColour, backgroundColour )
		{
		}
	}

	public class InputSpan : Span
	{
		public InputSpan( string text )
			: base( SpanType.Input, text )
		{
		}

		public InputSpan( string text, Colour foregroundColour )
			: base( SpanType.Input, text, foregroundColour )
		{
		}

		public InputSpan( string text, Colour foregroundColour, Colour backgroundColour )
			: base( SpanType.Input, text, foregroundColour, backgroundColour )
		{
		}
	}

	public class OutputSpan : Span
	{
		public OutputSpan( string text )
			: base( SpanType.Output, text )
		{
		}

		public OutputSpan( string text, Colour foregroundColour )
			: base( SpanType.Output, text, foregroundColour )
		{
		}

		public OutputSpan( string text, Colour foregroundColour, Colour backgroundColour )
			: base( SpanType.Output, text, foregroundColour, backgroundColour )
		{
		}
	}
}