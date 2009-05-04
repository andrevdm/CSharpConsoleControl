using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using TerminalCore;
using TerminalCore.Model;

namespace WpfTerminalControl
{
	/// <summary>
	/// Interaction logic for TerminalControl.xaml
	/// </summary>
	public partial class TerminalControl : UserControl, ITerminalView
	{
		protected readonly Typeface m_typeface;
		protected readonly CultureInfo m_culture;
		protected readonly TerminalController m_terminal;

		public TerminalControl()
		{
			InitializeComponent();

			IsTabStop = true;

			m_terminal = new TerminalController( this, "tst> " );

			m_culture = CultureInfo.GetCultureInfo( "en-us" );
			m_typeface = new Typeface( m_terminal.DefaultSpanFont.TypeFace );

			Background = new SolidColorBrush( ColorFromSpanColour( m_terminal.DefaultBackgroundColour ) );
			Foreground = new SolidColorBrush( ColorFromSpanColour( m_terminal.DefaultForegroundColour ) );
		}

		public SizeF MeasureText( string text, SpanFont font )
		{
			/*Typeface t = new Typeface( 
				new FontFamily( font.FontFamily.Name ), 
				FontStyles.Normal, 
				FontWeights.Bold, 
				FontStretches.Normal );

			//Windows Forms font size = WPF font size * 72.0 / 96.0.
			float fontSize = font.Size / (72.0F / 96.0F);

			FormattedText measure = new FormattedText( 
				text, 
				m_culture, 
				FlowDirection.LeftToRight, 
				m_typeface,
				fontSize, 
				Brushes.Black );

			m_charWidth = measure.WidthIncludingTrailingWhitespace;
			m_charHeight = measure.Height;*/

			throw new NotImplementedException();
		}

		protected override void OnPreviewKeyDown( KeyEventArgs e )
		{
			base.OnPreviewKeyDown( e );
		}

		private void m_scrollbar_ValueChanged( object sender, RoutedPropertyChangedEventArgs<double> e )
		{
		}

		public void OnHexCanvasRender( object sender, RenderEventArgs e )
		{
			Draw( e.DrawingContext );
		}

		public void OnHexCanvasRenderSizeChanged( object sender, RenderSizeChangedEventArgs e )
		{
			m_terminalCanvas.InvalidateVisual();
		}

		public void OnHexMouseLeftButtonDown( object sender, MouseButtonEventArgs e )
		{
			base.OnMouseLeftButtonDown( e );

			//m_terminalCanvas.InvalidateVisual();
		}

		private void Draw( DrawingContext ctx )
		{
			ctx.DrawRectangle( Background, null, new Rect( 0, 0, m_terminalCanvas.ActualWidth, m_terminalCanvas.ActualHeight ) );

			double top = 0;

			foreach( Paragraph para in m_terminal.GetParagraphs() )
			{
				double left = 0;
				double maxHeight = 0;

				foreach( Span span in para.Spans )
				{
					var fgBrush = span.ForegroundColour != null ? new SolidColorBrush( ColorFromSpanColour( span.ForegroundColour ) ) : Foreground;

					var font = span.Font ?? m_terminal.DefaultSpanFont;

					FormattedText formattedText = new FormattedText(
						span.Text,
						m_culture,
						FlowDirection.LeftToRight,
						TypefaceFromSpanFont( font ),
						font.Size,
						fgBrush );


					if( span.BackgroundColour != null )
					{
						var bgBrush = span.BackgroundColour != null ? new SolidColorBrush( ColorFromSpanColour( span.BackgroundColour ) ) : Background;
						
						ctx.DrawRectangle(
							bgBrush,
							null,
							new Rect( left, top, formattedText.WidthIncludingTrailingWhitespace, formattedText.Height ) );
					}

					ctx.DrawText( formattedText, new Point( left, top ) );

					left += formattedText.WidthIncludingTrailingWhitespace;
					maxHeight = Math.Max( maxHeight, formattedText.Height );
				}

				top += maxHeight;
			}
		}

		private Typeface TypefaceFromSpanFont( SpanFont spanFont )
		{
			return new Typeface(
				new FontFamily( spanFont.TypeFace ),
				FontStyleFromSpanFont( spanFont ),
				FontWeightFromSpanFont( spanFont ),
				FontStretches.Normal
				);
		}

		private FontWeight FontWeightFromSpanFont( SpanFont spanFont )
		{
			return ((spanFont.Style & SpanFontStyle.Bold) != 0) ? FontWeights.Bold : FontWeights.Normal;
		}

		private FontStyle FontStyleFromSpanFont( SpanFont spanFont )
		{
			return ((spanFont.Style & SpanFontStyle.Italic) != 0) ? FontStyles.Italic : FontStyles.Normal;
		}

		private Color ColorFromSpanColour( Colour spanColour )
		{
			return Color.FromArgb( spanColour.Alpha, spanColour.Red, spanColour.Green, spanColour.Blue );
		}
	}
}
