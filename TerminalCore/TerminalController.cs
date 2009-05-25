using System;
using System.Linq;
using System.Collections.Generic;

using TerminalCore.Model;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace TerminalCore
{
	public class TerminalController
	{
		public event EventHandler<LineEventArgs> LineEntered = delegate { };
		public event EventHandler<CharEventArgs> ControlCharEntered = delegate { };

		private readonly ITerminalView m_view;
		private readonly LinkedList<UserLine> m_lines = new LinkedList<UserLine>();
		private UserLine m_currentLine;
		private LinkedListNode<UserLine> m_historyItem;
		private int m_maxHistoryItems = 300;
		private int m_offsetInText;

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

		public DrawingInfo GetCurrentPageDrawingInfo( int rowsOnPage )
		{
			var lines = new List<CachedLine>( GetLinesToDrawOnCurrentPageReverse( rowsOnPage ).Reverse() );

			CursorPosition cursorPos = GetCursorPosition( lines );

			return new DrawingInfo( lines, cursorPos );
		}

		private CursorPosition GetCursorPosition( IList<CachedLine> lines )
		{
			int x = Prompt.Text.Length;
			int y = lines.Count - 1;

			if( lines.Count > 0 )
			{
				int offset = -m_offsetInText;
				var realLine = lines[ y ].RealLine;
				var realLineLen = lines[ y ].RealLine.ToString().Length;
				int charsPerLine = realLine.CachedLines[ 0 ].ToString( false ).Length;

				if( charsPerLine > 0 )
				{
					if( m_offsetInText != 0 )
					{
						y = lines.Count - realLine.CachedLines.Count;
						y += ((realLineLen - offset) / charsPerLine);
						//y = Math.Min( y, realLine.CachedLines.Count - 1 );

						x = ((realLineLen - offset) % charsPerLine);
					}
					else
					{
						x = lines[ lines.Count - 1 ].ToString().Length;
					}

					x += (y > 0) ? PromptWrap.Text.Length : Prompt.Text.Length;
				}
			}

			return new CursorPosition( x, y );
		}

		private IEnumerable<CachedLine> GetLinesToDrawOnCurrentPageReverse( int rowsOnPage )
		{
			int rowsReturned = 0;

			LinkedListNode<UserLine> current = m_lines.First;

			UpdateLineCache( m_currentLine );

			foreach( var cachedLine in m_currentLine.CachedLines.Reverse() )
			{
				if( rowsReturned >= rowsOnPage )
				{
					break;
				}

				yield return cachedLine;
				rowsReturned++;
			}

			while( (current != null) && (rowsReturned < rowsOnPage) )
			{
				UserLine line = current.Value;

				if( (line.CachedLines == null) || (line.CachedLines.Count == 0) || (line.CachedWrapAt != CharsPerLine) )
				{
					UpdateLineCache( line );
					Debug.Assert( line.CachedLines != null );
				}

				foreach( var cachedLine in line.CachedLines.Reverse() )
				{
					if( rowsReturned >= rowsOnPage )
					{
						break;
					}

					yield return cachedLine;
					rowsReturned++;
				}

				current = current.Next;
			}
		}

		public void CharTyped( char character )
		{
			ResetHistoryNavigation();

			switch( character )
			{
				case (char)13: //return
					ReturnPressed();
					break;

				case (char)27: //escape
					ClearCurrentLine();
					ResetColumn();
					break;

				case '\b': //backspace
					BackspacePressed();
					break;

				default:
					AppendCharToCurrentSpan( character );
					break;
			}
		}

		public void ControlKeyPressed( TerminalKey key, TerminalKeyModifiers state )
		{
			switch( key )
			{
				case TerminalKey.Up:
					ResetColumn();
					NavigationUpInHistory();
					break;

				case TerminalKey.Down:
					ResetColumn();
					NavigationDownInHistory();
					break;

				case TerminalKey.End:
					MoveToEnd();
					ResetHistoryNavigation();
					break;

				case TerminalKey.Home:
					MoveToBegining();
					ResetHistoryNavigation();
					break;

				case TerminalKey.Left:
					MoveLeft();
					ResetHistoryNavigation();
					break;

				case TerminalKey.Right:
					MoveRight();
					ResetHistoryNavigation();
					break;

				case TerminalKey.Insert:
					ResetHistoryNavigation();
					break;

				case TerminalKey.Delete:
					DeletePressed();
					ResetHistoryNavigation();
					break;

				default:
					ResetHistoryNavigation();
					break;
			}
		}

		private void MoveToBegining()
		{
			var span = m_currentLine.LastUserSpan;
			m_offsetInText = -span.Text.Length;
		}

		private void MoveToEnd()
		{
			m_offsetInText = 0;
		}

		private void ResetColumn()
		{
			m_offsetInText = 0;
		}

		private void MoveLeft()
		{
			var span = m_currentLine.LastUserSpan;
			m_offsetInText = Math.Max( -span.Text.Length, m_offsetInText - 1 );
		}

		private void MoveRight()
		{
			m_offsetInText = Math.Min( 0, m_offsetInText + 1 );
		}

		private void ResetHistoryNavigation()
		{
			m_historyItem = null;
		}

		private void NavigationDownInHistory()
		{
			if( m_historyItem == null )
			{
				return;
			}

			LinkedListNode<UserLine> at = m_historyItem.Previous;

			if( at == null )
			{
				return;
			}

			while( at.Value.IsOutput )
			{
				at = at.Previous;

				if( at == null )
				{
					return;
				}
			}

			m_historyItem = at;
			ShowHistoryItem();
		}

		private void NavigationUpInHistory()
		{
			LinkedListNode<UserLine> at = (m_historyItem == null) ?
				m_lines.First :
				m_historyItem.Next;

			if( at == null )
			{
				return;
			}

			while( at.Value.IsOutput )
			{
				at = at.Next;

				if( at == null )
				{
					return;
				}
			}

			m_historyItem = at;
			ShowHistoryItem();
		}

		private void ShowHistoryItem()
		{
			m_currentLine = new UserLine( m_historyItem.Value );
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
			ResetHistoryNavigation();
			var outputLines = Regex.Split( text, "(\r\n)|\r|\n" );

			foreach( string outputLine in outputLines )
			{
				var line = new UserLine();
				line.IsOutput = true;

				line.Spans.Add( PromptOutput );

				var span = new Span( outputLine, foregroundColour, backgroundColour );
				line.Spans.Add( span );

				UpdateMaxHistoryItems();

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
			if( !char.IsControl( c ) )
			{
				if( m_offsetInText == 0 )
				{
					m_currentLine.LastUserSpan.Text += c;
				}
				else
				{
					var txt = m_currentLine.LastUserSpan.Text;
					var len = txt.Length;
					m_currentLine.LastUserSpan.Text = txt.Substring( 0, len + m_offsetInText ) + c + txt.Substring( len + m_offsetInText );
				}

				m_currentLine.CachedLines = null;
			}
			else
			{
				ControlCharEntered( this, new CharEventArgs( c ) );
			}
		}

		private void DeletePressed()
		{
			string text = m_currentLine.LastUserSpan.Text;

			if( text.Length > 0 )
			{
				int offset = text.Length + m_offsetInText;

				if( offset == 0 )
				{
					text = text.Substring( 1 );
					m_offsetInText++;
				}
				else
				{
					if( offset < text.Length )
					{
						text = text.Substring( 0, offset ) + text.Substring( offset + 1 );
						m_offsetInText++;
					}
				}

				m_currentLine.LastUserSpan.Text = text;
			}
		}

		private void BackspacePressed()
		{
			string text = m_currentLine.LastUserSpan.Text;

			if( text.Length > 0 )
			{
				if( m_offsetInText == 0 )
				{
					text = text.Substring( 0, text.Length - 1 );
				}
				else
				{
					int offset = text.Length + m_offsetInText;

					if( offset > 0 )
					{
						text = text.Substring( 0, offset - 1 ) + text.Substring( offset );
					}
				}

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

			foreach( var origSpan in line.Spans )
			{
				if( cachedLine == null )
				{
					cachedLine = new CachedLine( line );
					line.CachedLines.Add( cachedLine );
				}

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
							cachedLine = new CachedLine( line );
							line.CachedLines.Add( cachedLine );

							cachedLine.Spans.Add( PromptWrap );
							len += PromptWrap.Text.Length;
						}
					}
				}
			}
		}

		private void UpdateMaxHistoryItems()
		{
			while( (m_lines.Count > 0) && (m_lines.Count > m_maxHistoryItems) )
			{
				m_lines.RemoveLast();
			}
		}

		public int MaxHistoryItems
		{
			get { return m_maxHistoryItems; }
			set
			{
				m_maxHistoryItems = value;
				UpdateMaxHistoryItems();
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