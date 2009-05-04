using System;
using System.Drawing;
using System.Windows.Forms;

using TerminalCore;
using TerminalCore.Model;

using SizeF = System.Drawing.SizeF;

namespace WinFormsTerminalControl
{
	public partial class TerminalControl : UserControl, ITerminalView
	{
		private readonly TerminalController m_terminal;

		public TerminalControl()
		{
			InitializeComponent();

			SetStyle( ControlStyles.DoubleBuffer, true );
			SetStyle( ControlStyles.UserPaint, true );
			SetStyle( ControlStyles.AllPaintingInWmPaint, true );

			m_terminal = new TerminalController( this, "tst> " );
			Font = FontFromSpanFont( m_terminal.DefaultSpanFont );
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
					Font font = span.Font != null ? FontFromSpanFont( span.Font ) : Font;

					//SizeF fontSize = TextRenderer.MeasureText( span.Text, font );
					//SizeF fontSize = e.Graphics.MeasureString( span.Text, font );
					SizeF fontSize = MeasureDisplayStringWidth( e.Graphics, span.Text, font );

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

		/// <summary>
		/// From: http://www.codeproject.com/KB/GDI-plus/measurestring.aspx
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="text"></param>
		/// <param name="font"></param>
		/// <returns></returns>
		private static SizeF MeasureDisplayStringWidth( Graphics graphics, string text, Font font )
		{
			StringFormat format = new StringFormat();
			RectangleF rect = new RectangleF( 0, 0, 1000, 1000 );
			CharacterRange[] ranges = { new CharacterRange( 0, text.Length ) };

			format.SetMeasurableCharacterRanges( ranges );
			format.FormatFlags = StringFormatFlags.MeasureTrailingSpaces;

			Region[] regions = graphics.MeasureCharacterRanges( text, font, rect, format );
			rect = regions[ 0 ].GetBounds( graphics );

			return new SizeF( rect.Right + 1.0f, rect.Bottom + 1.0f );
		}
	}
}
