using System;
using System.Collections.Generic;

using TerminalCore.Model;

namespace TerminalCore
{
	public class TerminalController
	{
		private readonly ITerminalView m_view;
		private readonly LinkedList<Line> m_lines = new LinkedList<Line>();
		private Line m_currentLine;

		public TerminalController( ITerminalView view, Span prompt )
			: this( view, prompt, Colours.White, Colours.Black, new SpanFont( "Courier New", SpanFontStyle.Normal, 12 ) )
		{
		}

		public TerminalController(
			ITerminalView view,
			Span prompt,
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

			ClearCurrentLine();
		}

		public IEnumerable<Line> GetLines()
		{
			//TODO remove
			//------------------------------------
			var p = new Line();
			p.Spans.Add( Prompt );

			p.Spans.Add( new Span(
								"abcdefg",
								new SpanFont( "Consolas", SpanFontStyle.Normal, 14 ),
								new Colour( 0, 0, 255 ),
								new Colour( 255, 255, 255 ) ) );

			yield return p;

			p = new Line();
			p.Spans.Add( Prompt );

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
			//------------------------------------

			foreach( var line in m_lines )
			{
				yield return line;
			}

			yield return m_currentLine;
		}

		public void CharTyped( char c )
		{
			switch( c )
			{
				case (char)0x13:
					ReturnPressed();
					break;

				case (char)27:
					ClearCurrentLine();
					break;

				default:
					AppendCharToCurrentSpan( c );
					break;
			}
		}

		private void ClearCurrentLine()
		{
			m_currentLine = new Line();
			m_currentLine.Spans.Add( Prompt );
		}

		private void AppendCharToCurrentSpan( char c )
		{
			if( char.IsLetterOrDigit( c ) || char.IsWhiteSpace( c ) )
			{
				m_currentLine.LastUserSpan.Text += c;
			}
		}

		private void ReturnPressed()
		{
			if( m_currentLine.HasUserText() )
			{
				m_lines.AddFirst( m_currentLine );
			}

			ClearCurrentLine();
		}

		private Span Prompt { get; set; }
		public Colour DefaultForegroundColour { get; private set; }
		public Colour DefaultBackgroundColour { get; private set; }
		public SpanFont DefaultSpanFont { get; private set; }
	}
}