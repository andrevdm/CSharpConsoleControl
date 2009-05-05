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
		private readonly Typeface m_typeface;
		private readonly CultureInfo m_culture;
		private readonly TerminalController m_terminal;
		private const int m_fontSize = 12;
		private readonly double m_charWidth;
		private readonly double m_charHeight;

		public TerminalControl()
		{
			InitializeComponent();

			IsTabStop = true;

			var measure = new FormattedText( "0", m_culture, FlowDirection.LeftToRight, m_typeface, 12, Brushes.Black );
			m_charWidth = measure.WidthIncludingTrailingWhitespace;
			m_charHeight = measure.Height;

			var prompt = new PromptSpan( "test> ", Colours.Blue );
			var promptWrap = new PromptWrapSpan( "    > ", Colours.Blue );
			int charsPerLine = (int)(Width / m_charWidth);
			m_terminal = new TerminalController( this, new SizeD( m_charWidth, m_charHeight ), charsPerLine, prompt, promptWrap );

			m_culture = CultureInfo.GetCultureInfo( "en-us" );
			m_typeface = new Typeface( "Courier New" );

			Background = new SolidColorBrush( ColorFromSpanColour( m_terminal.DefaultBackgroundColour ) );
			Foreground = new SolidColorBrush( ColorFromSpanColour( m_terminal.DefaultForegroundColour ) );
		}

		public SizeD MeasureText( string text )
		{
			FormattedText formattedText = new FormattedText(
									text,
									m_culture,
									FlowDirection.LeftToRight,
									m_typeface,
									m_fontSize,
									Brushes.Black );

			return new SizeD( formattedText.WidthIncludingTrailingWhitespace, formattedText.Height );
		}

		protected override void OnTextInput( TextCompositionEventArgs e )
		{
			base.OnTextInput( e );

			if( !string.IsNullOrEmpty( e.Text ) )
			{
				m_terminal.CharTyped( e.Text[ 0 ] );
			}

			m_terminalCanvas.InvalidateVisual();
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

			foreach( Line line in m_terminal.GetLines() )
			{
				double left = 0;
				double maxHeight = 0;

				foreach( Span span in line.Spans )
				{
					var fgBrush = span.ForegroundColour != null ? new SolidColorBrush( ColorFromSpanColour( span.ForegroundColour ) ) : Foreground;

					FormattedText formattedText = new FormattedText(
						span.Text,
						m_culture,
						FlowDirection.LeftToRight,
						m_typeface,
						m_fontSize,
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

		private Color ColorFromSpanColour( Colour spanColour )
		{
			return Color.FromArgb( spanColour.Alpha, spanColour.Red, spanColour.Green, spanColour.Blue );
		}
	}
}
