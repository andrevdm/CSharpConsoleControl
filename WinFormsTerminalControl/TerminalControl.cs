using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using TerminalCore;
using TerminalCore.Model;

using SizeF = System.Drawing.SizeF;

namespace WinFormsTerminalControl
{
	public partial class TerminalControl : UserControl, ITerminalView
	{
		private TerminalController m_terminal;
		private float m_charWidth;
		private float m_charHeight;

		public TerminalControl()
		{
			InitializeComponent();
			SelectionColour = Color.FromArgb( 89, 119, 77 );

			SetStyle( ControlStyles.DoubleBuffer, true );
			SetStyle( ControlStyles.UserPaint, true );
			SetStyle( ControlStyles.AllPaintingInWmPaint, true );
			
         MeasureFont();

			var prompt = new Span( "test> ", Colours.Blue );
			var promptWrap = new Span( "      ", Colours.Blue );
			var promptOutput = new Span( " ", Colours.Blue );
         var promptOutputWrap = new Span( " ", Colours.Blue );
         int charsPerLine = (int)(Width / m_charWidth);
         
         m_terminal = new TerminalController( this, new SizeD( m_charWidth, m_charHeight ), charsPerLine, prompt, promptWrap, promptOutput, promptOutputWrap );
		}

		private void MeasureFont()
		{
			string measure = new string( '0', 3000 );

			using( Graphics g = CreateGraphics() )
			{
				SizeF size = MeasureString( g, measure, Font );
				m_charWidth = size.Width / measure.Length;
				m_charHeight = size.Height;
			}
		}

		protected override void OnKeyUp( KeyEventArgs e )
		{
			base.OnKeyUp( e );

			var state = TerminalKeyModifiers.None;

			switch( e.KeyCode )
			{
				case Keys.End:
					m_terminal.ControlKeyPressed( TerminalKey.End, state );
					break;

				case Keys.Home:
					m_terminal.ControlKeyPressed( TerminalKey.Home, state );
					break;

				case Keys.Left:
					m_terminal.ControlKeyPressed( TerminalKey.Left, state );
					break;

				case Keys.Up:
					m_terminal.ControlKeyPressed( TerminalKey.Up, state );
					break;

				case Keys.Right:
					m_terminal.ControlKeyPressed( TerminalKey.Right, state );
					break;

				case Keys.Down:
					m_terminal.ControlKeyPressed( TerminalKey.Down, state );
					break;

				case Keys.Insert:
					m_terminal.ControlKeyPressed( TerminalKey.Insert, state );
					break;

				case Keys.Delete:
					m_terminal.ControlKeyPressed( TerminalKey.Delete, state );
					break;
			}

			Invalidate( true );
		}

		protected override void OnKeyPress( KeyPressEventArgs e )
		{
			base.OnKeyPress( e );

			m_terminal.CharTyped( e.KeyChar );
			Invalidate( true );
		}

      protected override void OnPaint( PaintEventArgs e )
      {
         base.OnPaint( e );
			
         var info = m_terminal.GetCurrentPageDrawingInfo( (int)(Height / m_charHeight) );

			DrawCursor( e, info.CursorPosition );
			DrawLines( e, info.Lines );
		}

		private void DrawCursor( PaintEventArgs e, CursorPosition position )
		{
			using( var brush = new SolidBrush( SelectionColour ) )
			{
				e.Graphics.FillRectangle( brush, (float)(position.X * m_charWidth), (float)(position.Y * m_charHeight), m_charWidth, m_charHeight );
			}
		}

		private void DrawLines( PaintEventArgs e, IEnumerable<CachedLine> lines )
		{
			float top = 0;

			foreach( var line in lines )
			{
				float left = 0;
				float maxHeight = 0;

				foreach( Span span in line.Spans )
				{
					SizeF fontSize = MeasureString( e.Graphics, span.Text, Font );

					if( span.BackgroundColour != null )
					{
						using( Brush bgBrush = new SolidBrush( ColorFromSpanColour( span.BackgroundColour ) ) )
						{
							e.Graphics.FillRectangle( bgBrush, left, top, fontSize.Width, fontSize.Height );
						}
					}

					using( Brush fgBrush = new SolidBrush( ColorFromSpanColour( span.ForegroundColour ?? m_terminal.DefaultForegroundColour ) ) )
					{
						e.Graphics.DrawString(
							span.Text,
							Font,
							fgBrush,
							left,
							top );
					}

					left += span.Text.Length * m_charWidth;
					maxHeight = fontSize.Height;
				}

				top += maxHeight;
			}
		}

		private Color ColorFromSpanColour( Colour colour )
		{
			return Color.FromArgb( colour.Alpha, colour.Red, colour.Green, colour.Blue );
		}

		public SizeD MeasureText( string text )
		{
			using( var g = CreateGraphics() )
			{
				SizeF size = MeasureString( g, text, Font );

				return new SizeD( size.Width, size.Height );
			}
		}

      protected override void OnResize( EventArgs e )
      {
         base.OnResize( e );

         if( m_terminal != null )
			{
				m_terminal.CharsPerLine = (int)(Width / m_charWidth);
				Invalidate( true );
			}
		}

		/// <summary>
		/// MeasureString lies. This method trys to get a more accurate string size.
		/// From: http://www.codeproject.com/KB/GDI-plus/measurestring.aspx?fid=3655&select=1682461#xx1682461xx
		/// </summary>
		/// <param name="g"></param>
		/// <param name="text"></param>
		/// <param name="font"></param>
		/// <returns></returns>
		private SizeF MeasureString( Graphics g, string text, Font font )
		{
			SizeF fontSizeDouble = g.MeasureString( text + text, font );
			SizeF fontSizeSingle = g.MeasureString( text, font );
			return new SizeF( fontSizeDouble.Width - fontSizeSingle.Width + 1.0f, fontSizeSingle.Height );
		}

		public Color SelectionColour { get; set; }
		public TerminalController Terminal { get { return m_terminal; } }
	}
}
