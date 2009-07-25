using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace CfTerminalControl
{
	public partial class CtrlDoubleBuffer : UserControl
	{
		public event PaintEventHandler OnPaintBuffered = delegate { };

		public CtrlDoubleBuffer()
		{
			InitializeComponent();

			EnableDoubleBuffering = true;
		}

		protected override void OnPaintBackground( PaintEventArgs e )
		{
			//Dont do a PaintBackground when not in design mode. This prevents flickering
			if( (Site != null) && (Site.DesignMode) )
			{
				base.OnPaintBackground( e );
			}
		}

		protected override void OnPaint( PaintEventArgs e )
		{
			base.OnPaint( e );

			if( EnableDoubleBuffering )
			{
				using( Bitmap backBuffer = new Bitmap( Width, Height ) )
				{
					using( Graphics g = Graphics.FromImage( backBuffer ) )
					{
						OnPaintBuffered( this, new PaintEventArgs( g, this.ClientRectangle ) );
					}

					e.Graphics.DrawImage( backBuffer, 0, 0 );
				}
			}
			else
			{
				OnPaintBuffered( this, new PaintEventArgs( e.Graphics, this.ClientRectangle ) );
			}
		}

		public bool EnableDoubleBuffering { get; set; }
	}
}
