using System;
using System.Collections.Generic;

using TerminalCore.Model;
using System.Text.RegularExpressions;

namespace TerminalCore
{
	public class TerminalController
	{
		public event EventHandler<LineEventArgs> LineEntered = delegate { };
		public event EventHandler<CharEventArgs> ControlCharEntered = delegate { };

		private readonly ITerminalView m_view;
		private readonly LinkedList<UserLine> m_lines = new LinkedList<UserLine>();
		private UserLine m_currentLine;

		public TerminalController( ITerminalView view, SizeD charSize, int charsPerLine, Span prompt, Span promptWrap, Span promptOutput )
			: this( view, charSize, charsPerLine, prompt, promptWrap, promptOutput, Colours.White, Colours.Black )
		{
		}

		public TerminalController(
			ITerminalView view,
			SizeD charSize,
			int charsPerLine,
			Span prompt,
			Span promptWrap,
			Span promptOutput,
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

			if( promptWrap == null )
			{
				throw new ArgumentNullException( "promptWrap" );
			}

			if( promptOutput == null )
			{
				throw new ArgumentNullException( "promptOutput" );
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
			PromptOutput = promptOutput;
			DefaultBackgroundColour = defaultBackgroundColour;
			DefaultForegroundColour = defaultForegroundColour;

			Prompt.IsPrompt = true;
			promptWrap.IsPrompt = true;
			promptOutput.IsPrompt = true;

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
				case (char)13: //return
					ReturnPressed();
					break;

				case (char)27: //escape
					ClearCurrentLine();
					break;

				case '\b': //backspace
					BackspacePressed();
					break;

				default:
					AppendCharToCurrentSpan( c );
					break;
			}
		}

		public void WriteOutput( string text )
		{
			WriteOutput( text, DefaultForegroundColour, DefaultBackgroundColour );
		}

		public void WriteOutput( string text, Colour foregroundColour )
		{
			WriteOutput( text, foregroundColour, DefaultBackgroundColour );
		}

		public void WriteOutput( string text, Colour foregroundColour, Colour backgroundColour )
		{
			var outputLines = Regex.Split( text, "(\r\n)|\r|\n" );

			foreach( string outputLine in outputLines )
			{
				var line = new UserLine();
				line.IsOutput = true;

				line.Spans.Add( PromptOutput );

				var span = new Span( outputLine, foregroundColour, backgroundColour );
				line.Spans.Add( span );

				m_lines.AddFirst( line );
			}

			m_currentLine = new UserLine();
		}

		private void ClearCurrentLine()
		{
			m_currentLine = new UserLine();
			m_currentLine.Spans.Add( Prompt );
		}

		private void AppendCharToCurrentSpan( char c )
		{
			if( !char.IsControl( c ) /*char.IsLetterOrDigit( c ) || char.IsWhiteSpace( c )*/ )
			{
				m_currentLine.LastUserSpan.Text += c;
				m_currentLine.CachedLines = null;
			}
			else
			{
				ControlCharEntered( this, new CharEventArgs( c ) );
			}
		}

		private void BackspacePressed()
		{
			string text = m_currentLine.LastUserSpan.Text;

			if( text.Length > 0 )
			{
				text = text.Substring( 0, text.Length - 1 );
				m_currentLine.LastUserSpan.Text = text;
			}
		}

		private void ReturnPressed()
		{
			if( m_currentLine.HasUserText() )
			{
				m_lines.AddFirst( m_currentLine );

				LineEntered( this, new LineEventArgs( m_currentLine.ToString() ) );
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

						var cachedSpan = new Span( substring, origSpan.ForegroundColour, origSpan.BackgroundColour );
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
		private Span Prompt { get; set; }
		private Span PromptWrap { get; set; }
		private Span PromptOutput { get; set; }
		public Colour DefaultForegroundColour { get; private set; }
		public Colour DefaultBackgroundColour { get; private set; }
	}
}