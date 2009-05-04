using System;
using System.Collections.Generic;

using TerminalCore.Model;

namespace TerminalCore
{
	public class TerminalController
	{
		private readonly ITerminalView m_view;

		public TerminalController( ITerminalView view, string prompt )
			: this( view, prompt, Colours.White, Colours.Black, new SpanFont( "Courier New", SpanFontStyle.Normal, 12 ) )
		{
		}

		public TerminalController(
			ITerminalView view,
			string prompt,
			Colour defaultForegroundColour,
			Colour defaultBackgroundColour,
			SpanFont defaultFont )
		{
			m_view = view;

			#region param checks
			if( view == null )
			{
				throw new ArgumentNullException( "view" );
			}

			if( prompt == null )
			{
				throw new ArgumentNullException( "prompt" );
			}

			if( defaultBackgroundColour == null )
			{
				throw new ArgumentNullException( "defaultBackgroundColour" );
			}

			if( defaultForegroundColour == null )
			{
				throw new ArgumentNullException( "defaultForegroundColour" );
			}

			if( defaultFont == null )
			{
				throw new ArgumentNullException( "defaultFont" );
			}
			#endregion

			Prompt = prompt;
			DefaultBackgroundColour = defaultBackgroundColour;
			DefaultForegroundColour = defaultForegroundColour;
			DefaultSpanFont = defaultFont;
		}

		public IEnumerable<Paragraph> GetParagraphs()
		{
			var p = new Paragraph();
			p.Spans.Add( new Span( Prompt ) );

			p.Spans.Add( new Span(
								"abcdefg",
								new SpanFont( "Consolas", SpanFontStyle.Normal, 14 ),
								new Colour( 0, 0, 255 ),
								new Colour( 255, 255, 255 ) ) );

			yield return p;

			p = new Paragraph();
			p.Spans.Add( new Span( Prompt ) );

			p.Spans.Add( new Span(
								"abcdefg",
								new SpanFont( "Consolas", SpanFontStyle.Italic | SpanFontStyle.Bold, 10 ),
								new Colour( 0, 255, 0 ),
								new Colour( 100, 100, 100 ) ) );

			p.Spans.Add( new Span(
								"hij dd",
								new SpanFont( "Consolas", SpanFontStyle.Normal, 10 ),
								new Colour( 0, 255, 0 ),
								new Colour( 0, 0, 0 ) ) );


			yield return p;
		}

		public string Prompt { get; private set; }
		public Colour DefaultForegroundColour { get; private set; }
		public Colour DefaultBackgroundColour { get; private set; }
		public SpanFont DefaultSpanFont { get; private set; }
	}
}