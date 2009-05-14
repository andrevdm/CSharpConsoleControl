using System;
using System.Windows;

using Boo.Lang.Compiler;

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
			Terminal.ControlCharEntered += Terminal_ControlCharEntered;
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
				CompilerContext ctx = m_boo.Eval( e.Line );

				if( ctx.Errors.Count == 0 )
				{
					if( m_boo.LastValue != null )
					{
						Terminal.WriteOutput( m_boo.LastValue.ToString(), Colours.Green );
					}
				}
				else
				{
					Terminal.WriteOutput( ctx.Errors[ 0 ].Message, Colours.Red );
				}
			}
			catch( Exception ex )
			{
				Terminal.WriteOutput( ex.Message, Colours.Red );
			}
		}

		private void Terminal_ControlCharEntered( object sender, CharEventArgs e )
		{
			if( e.Char == '\t' )
			{
				//m_boo.SuggestCodeCompletion(  )
				//TODO suggest completion
			}
		}


		private TerminalController Terminal { get { return m_terminal.Terminal; } }
	}
}
