using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TerminalCore;
using TerminalCore.Model;

namespace TerminalCoreTests
{
   [TestClass]
   public class ScrollingTests
   {
      private readonly string m_prompt = "   > ";
      private readonly string m_promptWrap = "......";
      private readonly string m_promptOutput = "___";
      private readonly string m_promptOutputWrap = "****";

      private readonly Span m_promptSpan;
      private readonly Span m_promptWrapSpan;
      private readonly Span m_promptOutputSpan;
      private readonly Span m_promptOutputWrapSpan;

      public ScrollingTests()
      {
         m_promptSpan = new Span( m_prompt );
         m_promptWrapSpan = new Span( m_promptWrap );
         m_promptOutputSpan = new Span( m_promptOutput );
         m_promptOutputWrapSpan = new Span( m_promptOutputWrap );
      }

      [TestMethod]
      public void AutoScrollShowsOnlyCurrentPageOfData()
      {
         var view = new DummyTerminalView( s => new SizeD( s.Length, 1 ) );
         var terminal = new TerminalController( view, new SizeD( 1, 1 ), int.MaxValue, m_promptSpan, m_promptWrapSpan, m_promptOutputSpan, m_promptOutputWrapSpan );

         terminal.LineEntered += ( s, lea ) => terminal.WriteOutput( "out: " + lea.Line );
         terminal.CharsPerLine = m_prompt.Length + 5;

         "abcdefgh\r".ForEach( c => terminal.CharTyped( c ) );

         DrawingInfo info = terminal.GetCurrentPageDrawingInfo( 4 );

         var lines = CoreTestHelpers.GetLinesAsText( info );

         Assert.AreEqual( 4, lines.Count, "Incorrect number of lines" );
         Assert.AreEqual( m_promptWrap + "fgh", lines[ 0 ], "Incorrect input line 1" );
         Assert.AreEqual( m_promptOutput + "out: ab", lines[ 1 ], "Incorrect output line 2" );
         Assert.AreEqual( m_promptOutputWrap + "cdefgh", lines[ 2 ], "Incorrect output line 3" );
         Assert.AreEqual( m_prompt, lines[ 3 ], "Incorrect input line 4" );
      }

      [TestMethod]
      public void Todo()
      {
         //TODO
         Assert.Fail( "Scrolling not supported yet. Test GetCurrentPageDrawingInfo after scrolling up and down off page" );
      }
   }
}
