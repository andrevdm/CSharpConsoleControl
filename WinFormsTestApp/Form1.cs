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
using System.IO;

namespace WinFormsTestApp
{
   public partial class Form1 : Form
   {
      private InteractiveInterpreter2 m_boo;
      private DummyTextWriter m_writer = new DummyTextWriter();

      public Form1()
      {
         InitializeComponent();
      }

      protected override void OnLoad( EventArgs e )
      {
         base.OnLoad( e );

         m_boo = new InteractiveInterpreter2();
         m_boo.RememberLastValue = true;

         m_writer.OnWrite = s => m_terminal.Terminal.WriteOutput( s );
         Console.SetOut( m_writer );

         m_terminal.Terminal.LineEntered += Terminal_LineEntered;
         m_terminal.Terminal.ControlCharEntered += Terminal_ControlCharEntered;
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
                  m_terminal.Terminal.WriteOutput( m_boo.LastValue.ToString(), Colours.Green );
               }
            }
            else
            {
               m_terminal.Terminal.WriteOutput( ctx.Errors[ 0 ].Message, Colours.Red );
            }
         }
         catch( Exception ex )
         {
            m_terminal.Terminal.WriteOutput( ex.Message, Colours.Red );
         }
      }

      class DummyTextWriter : TextWriter
      {
         public Action<string> OnWrite { get; set; }

         public override void Write( char[] buffer, int index, int count )
         {
            var txt = new StringWriter();
            txt.Write( buffer, index, count );

            if( OnWrite != null )
            {
               OnWrite( txt.ToString() );
            }
         }

         public override Encoding Encoding { get { return Encoding.ASCII; } }
      }
   }
}
