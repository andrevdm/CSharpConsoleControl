using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Boo.Lang.Compiler;
using Boo.Lang.Interpreter;

using TerminalCore;
using TerminalCore.Model;

namespace WinFormsTestApp
{
	public partial class Form1 : Form
	{
		private InteractiveInterpreter2 m_boo;

		public Form1()
		{
			InitializeComponent();
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

			m_boo = new InteractiveInterpreter2();
			m_boo.RememberLastValue = true;

			m_terminal.Controller.LineEntered += Terminal_LineEntered;
			m_terminal.Controller.ControlCharEntered += Terminal_ControlCharEntered;
		}

		private void Terminal_ControlCharEntered( object sender, CharEventArgs e )
		{
			if( e.Char == '\t' )
			{
				//m_boo.SuggestCodeCompletion(  )
				//TODO suggest completion
			}
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
						m_terminal.Controller.WriteOutput( m_boo.LastValue.ToString(), Colours.Green );
					}
				}
				else
				{
					m_terminal.Controller.WriteOutput( ctx.Errors[ 0 ].Message, Colours.Red );
				}
			}
			catch( Exception ex )
			{
				m_terminal.Controller.WriteOutput( ex.Message, Colours.Red );
			}
		}
	}
}
