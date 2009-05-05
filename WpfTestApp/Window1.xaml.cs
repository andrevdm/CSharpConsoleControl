using System;
using System.Collections.Generic;
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

namespace WpfTestApp
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();

			Terminal.LineEntered += Terminal_LineEntered;
		}

		private void Terminal_LineEntered( object sender, LineEventArgs e )
		{
			Terminal.WriteOutput( "Echo\r\n'" + e.Line + "\n'" );
			m_terminal.InvalidateVisual();
		}

		public TerminalController Terminal { get { return m_terminal.Terminal; } }
	}
}
