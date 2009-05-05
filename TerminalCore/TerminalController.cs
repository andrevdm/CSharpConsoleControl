using System;
using System.Collections.Generic;

using TerminalCore.Model;

namespace TerminalCore
{
	public class TerminalController
	{
		private readonly ITerminalView m_view;
		private readonly LinkedList<Line> m_lines = new LinkedList<Line>();
		private UserLine m_currentLine;

		public TerminalController( ITerminalView view, SizeD charSize, int charsPerLine, PromptSpan prompt, PromptWrapSpan promptWrap )
			: this( view, charSize, charsPerLine, prompt, promptWrap, Colours.White, Colours.Black )
		{
		}

		public TerminalController(
			ITerminalView view,
			SizeD charSize,
			int charsPerLine,
			PromptSpan prompt,
			PromptWrapSpan promptWrap,
			Colour defaultForegroundColour,
			Colour defaultBackgroundColour )
		{
			m_view = view;

			#region param checks
			if( view == null )
			{
				throw new ArgumentNullException( "view" );
			}

			if( charSize == null )
			{
				throw new ArgumentNullException( "charSize" );
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
			#endregion

			Prompt = prompt;
			CharSize = charSize;
			CharsPerLine = charsPerLine;
			PromptWrap = promptWrap;
			DefaultBackgroundColour = defaultBackgroundColour;
			DefaultForegroundColour = defaultForegroundColour;

			ClearCurrentLine();
		}

		public IEnumerable<Line> GetLines()
		{
			//TODO Have a cached List<Lines> on each Line with an associated width. If the views' width is different then recalc wrap, else just return lines.

			foreach( Line line in m_lines )
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
			m_currentLine = new UserLine();
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

		public int CharsPerLine { get; set; }
		public SizeD CharSize { get; private set; }
		private PromptSpan Prompt { get; set; }
		private PromptWrapSpan PromptWrap { get; set; }
		public Colour DefaultForegroundColour { get; private set; }
		public Colour DefaultBackgroundColour { get; private set; }
	}
}