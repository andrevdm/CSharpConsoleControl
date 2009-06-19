using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerminalCore;
using TerminalCore.Model;

namespace TerminalCoreTests
{
   public class DummyTerminalView : ITerminalView
   {
      private readonly Func<string, SizeD> m_measureText;

      public DummyTerminalView( Func<string, SizeD> measureText )
      {
         m_measureText = measureText;
      }

      public SizeD MeasureText( string text )
      {
         return m_measureText( text );
      }
   }
}
