using System;
using System.Windows;

using TerminalCore;
using TerminalCore.Model;

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

				if( m_boo.LastValue != null )
				{
					Terminal.WriteOutput( m_boo.LastValue.ToString(), Colours.Green );
				}
			}
			catch( Exception ex )
			{
				Terminal.WriteOutput( ex.Message, Colours.Red );
			}
		}

		private TerminalController Terminal { get { return m_terminal.Terminal; } }
	}
}
