using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using TerminalCore;
using TerminalCore.Model;

using SizeF = System.Drawing.SizeF;

namespace WinFormsTerminalControl
{
	public partial class TerminalControl : UserControl, ITerminalView
	{
		protected TerminalController m_terminal;

		public TerminalControl()
		{
			InitializeComponent();

			SetStyle( ControlStyles.DoubleBuffer, true );
			SetStyle( ControlStyles.UserPaint, true );
			SetStyle( ControlStyles.AllPaintingInWmPaint, true );

			m_terminal = new TerminalController( this, "tst> " );
			Font = FontFromSpanFont( m_terminal.DefaultSpanFont );
		}
		/*private void MeasureFont()
		{
			using( Graphics g = CreateGraphics() )
			{
				SizeF size = g.MeasureString( "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", Font );
				m_charWidth = size.Width / 400F;
				m_charHeight = size.Height;
			}
		}*/

		private void TerminalControl_Paint( object sender, PaintEventArgs e )
		{
			float top = 0;

			foreach( var para in m_terminal.GetParagraphs() )
			{
				float left = 0;
				float maxHeight = 0;

				foreach( Span span in para.Spans )
				{
					Font font = span.Font != null ? FontFromSpanFont( span.Font ) : Font;

					SizeF fontSize = e.Graphics.MeasureString( span.Text, font );

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
							font,
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

		private Font FontFromSpanFont( SpanFont spanFont )
		{
			return new Font( spanFont.TypeFace, SizeFromSpanFontSize( spanFont.Size ), FontStyleFromSpanFont( spanFont ) );
		}

		private float SizeFromSpanFontSize( float size )
		{
			//Windows Forms font size = WPF font size * 72.0 / 96.0.
			return (size / 96.0F) * 72.0F;
		}

		private FontStyle FontStyleFromSpanFont( SpanFont spanFont )
		{
			FontStyle style = FontStyle.Regular;

			if( (spanFont.Style & SpanFontStyle.Bold) != 0 )
				style &= FontStyle.Bold;

			if( (spanFont.Style & SpanFontStyle.Italic) != 0 )
				style &= FontStyle.Italic;

			if( (spanFont.Style & SpanFontStyle.Underline) != 0 )
				style &= FontStyle.Underline;

			return style;
		}

		private Color ColorFromSpanColour( Colour colour )
		{
			return Color.FromArgb( colour.Alpha, colour.Red, colour.Green, colour.Blue );
		}

		public TerminalCore.Model.SizeF MeasureText( string text, SpanFont font )
		{
			throw new NotImplementedException();
		}
	}
}
