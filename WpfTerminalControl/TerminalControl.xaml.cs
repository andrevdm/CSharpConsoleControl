using System;
using System.Collections.Generic;
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
		private TerminalController m_terminal;
		private const int TerminalFontSize = 12;
		private readonly double m_charWidth;
		private readonly double m_charHeight;

		public TerminalControl()
		{
			InitializeComponent();

			IsTabStop = true;

			m_culture = CultureInfo.GetCultureInfo( "en-us" );
			m_typeface = new Typeface( "Courier New" );

			var measure = new FormattedText( "0", m_culture, FlowDirection.LeftToRight, m_typeface, 12, Brushes.Black );
			m_charWidth = measure.WidthIncludingTrailingWhitespace;
			m_charHeight = measure.Height;
		}

		protected override void OnInitialized( EventArgs e )
		{
			base.OnInitialized( e );

			var prompt = new Span( "test> ", Colours.Blue );
			var promptWrap = new Span( "      ", Colours.Blue );
			var promptOutput = new Span( " ", Colours.Blue );
			m_terminal = new TerminalController( this, new SizeD( m_charWidth, m_charHeight ), int.MaxValue, prompt, promptWrap, promptOutput );

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
									TerminalFontSize,
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

			var state = TerminalKeyModifiers.None;

			switch( e.Key )
			{
				case Key.End:
					Terminal.ControlKeyPressed( TerminalKey.End, state );
					break;

				case Key.Home:
					Terminal.ControlKeyPressed( TerminalKey.Home, state );
					break;

				case Key.Left:
					Terminal.ControlKeyPressed( TerminalKey.Left, state );
					break;

				case Key.Up:
					Terminal.ControlKeyPressed( TerminalKey.Up, state );
					break;

				case Key.Right:
					Terminal.ControlKeyPressed( TerminalKey.Right, state );
					break;

				case Key.Down:
					Terminal.ControlKeyPressed( TerminalKey.Down, state );
					break;

				case Key.Insert:
					Terminal.ControlKeyPressed( TerminalKey.Insert, state );
					break;

				case Key.Delete:
					Terminal.ControlKeyPressed( TerminalKey.Delete, state );
					break;
			}

			m_terminalCanvas.InvalidateVisual();
		}

		private void m_scrollbar_ValueChanged( object sender, RoutedPropertyChangedEventArgs<double> e )
		{
		}

		private void OnHexCanvasRender( object sender, RenderEventArgs e )
		{
			Draw( e.DrawingContext );
		}

		private void OnHexCanvasRenderSizeChanged( object sender, RenderSizeChangedEventArgs e )
		{
			m_terminal.CharsPerLine = (int)(m_terminalCanvas.ActualWidth / m_charWidth);
			m_terminalCanvas.InvalidateVisual();
		}

		private void OnHexMouseLeftButtonDown( object sender, MouseButtonEventArgs e )
		{
			base.OnMouseLeftButtonDown( e );

			//m_terminalCanvas.InvalidateVisual();
		}

		private void Draw( DrawingContext ctx )
		{
			ctx.DrawRectangle( Background, null, new Rect( 0, 0, m_terminalCanvas.ActualWidth, m_terminalCanvas.ActualHeight ) );

			var info = m_terminal.GetCurrentPageDrawingInfo( (int)(m_terminalCanvas.ActualHeight / m_charHeight) );

			DrawCursor( ctx, info.CursorPosition );
			DrawLines( ctx, info.Lines );
		}

		private void DrawCursor( DrawingContext ctx, CursorPosition position )
		{
			ctx.DrawRectangle( Brushes.Yellow, null, new Rect( position.X * m_charWidth, position.Y * m_charWidth, m_charWidth, m_charHeight ) );
		}

		private void DrawLines( DrawingContext ctx, IEnumerable<Line> lines )
		{
			double top = 0;

			foreach( Line line in lines )
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
						TerminalFontSize,
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

		public TerminalController Terminal { get { return m_terminal; } }
	}
}
