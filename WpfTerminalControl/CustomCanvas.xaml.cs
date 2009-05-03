using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfTerminalControl
{
	/// <summary>
	/// Interaction logic for CustomCanvas.xaml
	/// </summary>
	public partial class CustomCanvas : UserControl
	{
		public event EventHandler<RenderEventArgs> OnCustomRender = delegate { };
		public event EventHandler<RenderSizeChangedEventArgs> OnCustomRenderSizeChanged = delegate { };

		static CustomCanvas()
		{
			DefaultStyleKeyProperty.OverrideMetadata( typeof( CustomCanvas ), new FrameworkPropertyMetadata( typeof( CustomCanvas ) ) );
		}

		public CustomCanvas()
		{
			InitializeComponent();

			Focusable = true;
			IsTabStop = true;
		}

		protected override void OnRender( DrawingContext drawingContext )
		{
			base.OnRender( drawingContext );
			OnCustomRender( this, new RenderEventArgs { DrawingContext = drawingContext } );
		}

		protected override void OnRenderSizeChanged( SizeChangedInfo sizeInfo )
		{
			base.OnRenderSizeChanged( sizeInfo );
			OnCustomRenderSizeChanged( this, new RenderSizeChangedEventArgs { SizeInfo = sizeInfo } );
		}
	}

	public class RenderEventArgs : EventArgs
	{
		public DrawingContext DrawingContext { get; set; }
	}

	public class RenderSizeChangedEventArgs : EventArgs
	{
		public SizeChangedInfo SizeInfo { get; set; }
	}
}
