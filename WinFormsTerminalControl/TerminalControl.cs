using System;
using System.Drawing;
using System.Drawing.Text;
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

			SetStyle( ControlStyles.DoubleBuffer, true );
			SetStyle( ControlStyles.UserPaint, true );
			SetStyle( ControlStyles.AllPaintingInWmPaint, true );
		}

		private void TerminalControl_Load( object sender, EventArgs e )
		{
			MeasureFont();

			var prompt = new Span( "test> ", Colours.Blue );
			var promptWrap = new Span( "      ", Colours.Blue );
			int charsPerLine = (int)(Width / m_charWidth);
			m_terminal = new TerminalController( this, new SizeD( m_charWidth, m_charHeight ), charsPerLine, prompt, promptWrap );
		}

		private void MeasureFont()
		{
			using( Graphics g = CreateGraphics() )
			{
				SizeF size = MeasureString( g, "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", Font );
				m_charWidth = size.Width / 400F;
				m_charHeight = size.Height;
			}
		}

		protected override void OnKeyPress( KeyPressEventArgs e )
		{
			base.OnKeyPress( e );

			m_terminal.CharTyped( e.KeyChar );
			Invalidate( true );
		}

		private void TerminalControl_Paint( object sender, PaintEventArgs e )
		{
			float top = 0;

			foreach( var line in m_terminal.GetLines() )
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

					left += fontSize.Width;
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

		private void TerminalControl_Resize( object sender, EventArgs e )
		{
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
	}
}
