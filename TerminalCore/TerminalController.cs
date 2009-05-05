using System;
using System.Collections.Generic;

using TerminalCore.Model;

namespace TerminalCore
{
	public class TerminalController
	{
		private readonly ITerminalView m_view;
		private readonly LinkedList<UserLine> m_lines = new LinkedList<UserLine>();
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
			LinkedListNode<UserLine> current = m_lines.Last;

			while( current != null )
			{
				UserLine line = current.Value;

				if( (line.CachedLines == null) || (line.CachedLines.Count == 0) || (line.CachedWrapAt != CharsPerLine) )
				{
					UpdateLineCache( line );
				}

				foreach( var cachedLine in line.CachedLines )
				{
					yield return cachedLine;
				}

				current = current.Previous;
			}

			UpdateLineCache( m_currentLine );

			foreach( var cachedLine in m_currentLine.CachedLines )
			{
				yield return cachedLine;
			}
		}

		public void CharTyped( char c )
		{
			switch( c )
			{
				case (char)13:
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
				m_currentLine.CachedLines = null;
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

		private void UpdateLineCache( UserLine line )
		{
			line.CachedLines = new List<CachedLine>();
			line.CachedWrapAt = CharsPerLine;

			int len = 0;

			CachedLine cachedLine = null;
			bool firstLine = true;

			foreach( var origSpan in line.Spans )
			{
				if( cachedLine == null )
				{
					cachedLine = new CachedLine();
					line.CachedLines.Add( cachedLine );

					if( !firstLine )
					{
						cachedLine.Spans.Add( PromptWrap );
						len += PromptWrap.Text.Length;
					}
				}

				firstLine = false;

				if( (origSpan.Text.Length + len) < CharsPerLine )
				{
					cachedLine.Spans.Add( origSpan );
					len += origSpan.Text.Length;
				}
				else
				{
					string spanText = origSpan.Text;

					while( spanText.Length > 0 )
					{
						string substring = spanText.Substring( 0, Math.Min( CharsPerLine - len, spanText.Length ) );
						spanText = spanText.Substring( Math.Min( CharsPerLine - len, spanText.Length ) );
						len = 0;

						var cachedSpan = new Span( origSpan.SpanType, substring, origSpan.ForegroundColour, origSpan.BackgroundColour );
						cachedLine.Spans.Add( cachedSpan );

						if( spanText.Length > 0 )
						{
							cachedLine = new CachedLine();
							line.CachedLines.Add( cachedLine );

							cachedLine.Spans.Add( PromptWrap );
							len += PromptWrap.Text.Length;
						}
					}
				}
			}
		}

		public int CharsPerLine { get; set; }
		public SizeD CharSize { get; private set; }
		private PromptSpan Prompt { get; set; }
		private PromptWrapSpan PromptWrap { get; set; }
		public Colour DefaultForegroundColour { get; private set; }
		public Colour DefaultBackgroundColour { get; private set; }
	}
}