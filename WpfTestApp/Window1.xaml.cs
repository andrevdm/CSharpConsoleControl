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

using Boo.Lang.Compiler;

using TerminalCore;
using TerminalCore.Model;
using System.Diagnostics;
using System.Threading;

using Boo.Lang.Interpreter;

namespace WpfTestApp
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		private InteractiveInterpreter2 m_boo;

		public Window1()
		{
			InitializeComponent();

			Terminal.LineEntered += Terminal_LineEntered;
		}

		protected override void OnInitialized( EventArgs e )
		{
			base.OnInitialized( e );

			m_boo = new InteractiveInterpreter2();
			m_boo.RememberLastValue = true;
		}


		private void Terminal_LineEntered( object sender, LineEventArgs e )
		{
			try
			{
				m_boo.Eval( e.Line );
				Terminal.WriteOutput( m_boo.LastValue.ToString(), Colours.Green );
			}
			catch( Exception ex )
			{
				Terminal.WriteOutput( ex.Message, Colours.Red );
			}
		}

		private TerminalController Terminal { get { return m_terminal.Terminal; } }
	}
}
