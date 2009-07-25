using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CfTerminalControl;

namespace CfTestApp
{
   public partial class Form1 : Form
   {
      private TerminalControl m_terminal;

      public Form1()
      {
         InitializeComponent();

         m_terminal = new TerminalControl();
         m_terminal.Dock = DockStyle.Fill;
         Controls.Add( m_terminal );
      }
   }
}