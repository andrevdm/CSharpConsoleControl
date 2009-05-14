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
		private int m_inputColumn;
		private int m_maxHistoryItems = 300;

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

		public IEnumerable<Line> GetLinesToDrawOnCurrentPage( int rowsOnPage )
		{
			return GetLinesToDrawOnCurrentPageReverse( rowsOnPage ).Reverse();
		}

		private IEnumerable<Line> GetLinesToDrawOnCurrentPageReverse( int rowsOnPage )
		{
			int rowsReturned = 0;

			LinkedListNode<UserLine> current = m_lines.First;

			UpdateLineCache( m_currentLine );

			foreach( var cachedLine in m_currentLine.CachedLines )
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

				foreach( var cachedLine in line.CachedLines )
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

		public void CharTyped( char c )
		{
			ResetHistoryNavigation();

			switch( c )
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
					ResetColumn();
					AppendCharToCurrentSpan( c );
					break;
			}
		}

		public void ControlKeyPressed( TerminalKey key, TerminalKeyModifier state )
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
					ResetHistoryNavigation();
					break;

				default:
					ResetHistoryNavigation();
					break;
			}
		}

		private void MoveToBegining()
		{
			//TODO
		}

		private void MoveToEnd()
		{
			//TODO
		}

		private void ResetColumn()
		{
			m_inputColumn = 0;
		}

		private void MoveLeft()
		{
			//TODO m_inputColumn++;
		}

		private void MoveRight()
		{
			//TODO m_inputColumn--;
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

			foreach( var origSpan in line.Spans )
			{
				if( cachedLine == null )
				{
					cachedLine = new CachedLine();
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
							cachedLine = new CachedLine();
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