using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using TerminalCore;
using TerminalCore.Model;

using Brushes = System.Windows.Media.Brushes;
using Point = System.Windows.Point;

namespace WpfTerminalControl
{
	/// <summary>
	/// Interaction logic for TerminalControl.xaml
	/// </summary>
	public partial class TerminalControl : UserControl, ITerminalView
	{
		protected readonly Typeface m_typeface;
		protected readonly CultureInfo m_culture;
		protected readonly double m_charWidth;
		protected readonly double m_charHeight;
		protected readonly TerminalController m_terminal;

		public TerminalControl()
		{
			InitializeComponent();

			Background = Brushes.Black;
			Foreground = Brushes.WhiteSmoke;

			IsTabStop = true;

			m_culture = CultureInfo.GetCultureInfo( "en-us" );
			m_typeface = new Typeface( "Courier New" );

			FormattedText measure = new FormattedText( "0", m_culture, FlowDirection.LeftToRight, m_typeface, 12, Brushes.Black );
			m_charWidth = measure.WidthIncludingTrailingWhitespace;
			m_charHeight = measure.Height;

			m_terminal = new TerminalController( this, "tst> " );
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

			var formattedText = new FormattedText(
									m_terminal.Prompt,
									m_culture,
									FlowDirection.LeftToRight,
									m_typeface,
									12,
									Foreground );

			ctx.DrawText( formattedText, new Point( 0 * m_charWidth, 0 * m_charHeight ) );
		}
	}
}
