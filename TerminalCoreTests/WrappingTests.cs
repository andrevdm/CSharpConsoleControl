using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TerminalCore;
using TerminalCore.Model;

namespace TerminalCoreTests
{
   [TestClass]
   public class WrappingTests
   {
      private readonly string m_prompt = "   > ";
      private readonly string m_promptWrap = "......";
      private readonly string m_promptOutput = "___";
      private readonly string m_promptOutputWrap = "****";

      private readonly Span m_promptSpan;
      private readonly Span m_promptWrapSpan;
      private readonly Span m_promptOutputSpan;
      private readonly Span m_promptOutputWrapSpan;

      public WrappingTests()
      {
         m_promptSpan = new Span( m_prompt );
         m_promptWrapSpan = new Span( m_promptWrap );
         m_promptOutputSpan = new Span( m_promptOutput );
         m_promptOutputWrapSpan = new Span( m_promptOutputWrap );
      }

      [TestMethod]
      public void SingleLine()
      {
         var view = new DummyTerminalView( s => new SizeD( s.Length, 1 ) );
         var terminal = new TerminalController( view, new SizeD( 1, 1 ), int.MaxValue, m_promptSpan, m_promptWrapSpan, m_promptOutputSpan, m_promptOutputWrapSpan );

         terminal.CharsPerLine = 10;

         "ab".ForEach( c => terminal.CharTyped( c ) );

         DrawingInfo info = terminal.GetCurrentPageDrawingInfo( 5 );

         var lines = CoreTestHelpers.GetLinesAsText( info );

         Assert.AreEqual( 1, lines.Count, "Incorrect number of lines" );
         Assert.AreEqual( m_prompt + "ab", lines[ 0 ] );
      }

      [TestMethod]
      public void AfterReturnTypeOutputAndNextInputLineShow()
      {
         var view = new DummyTerminalView( s => new SizeD( s.Length, 1 ) );
         var terminal = new TerminalController( view, new SizeD( 1, 1 ), int.MaxValue, m_promptSpan, m_promptWrapSpan, m_promptOutputSpan, m_promptOutputWrapSpan );

         terminal.LineEntered += (s, lea) => terminal.WriteOutput( "out: " + lea.Line );
         terminal.CharsPerLine = 10;

         "ab\r".ForEach( c => terminal.CharTyped( c ) );

         DrawingInfo info = terminal.GetCurrentPageDrawingInfo( 5 );

         var lines = CoreTestHelpers.GetLinesAsText( info );

         Assert.AreEqual( 3, lines.Count, "Incorrect number of lines" );
         Assert.AreEqual( m_prompt + "ab", lines[ 0 ], "Incorrect input line" );
         Assert.AreEqual( m_promptOutput + "out: ab", lines[ 1 ], "Incorrect outline" );
         Assert.AreEqual( m_prompt, lines[ 2 ], "Incorrect next input line" );
      }

      [TestMethod]
      public void InputWrapping()
      {
         var view = new DummyTerminalView( s => new SizeD( s.Length, 1 ) );
         var terminal = new TerminalController( view, new SizeD( 1, 1 ), int.MaxValue, m_promptSpan, m_promptWrapSpan, m_promptOutputSpan, m_promptOutputWrapSpan );

         terminal.CharsPerLine = m_prompt.Length + 5;

         "abcdefghijk".ForEach( c => terminal.CharTyped( c ) );

         DrawingInfo info = terminal.GetCurrentPageDrawingInfo( 5 );

         var lines = CoreTestHelpers.GetLinesAsText( info );

         Assert.AreEqual( 3, lines.Count, "Incorrect number of lines" );
         Assert.AreEqual( m_prompt + "abcde", lines[ 0 ], "Incorrect input line" );
         Assert.AreEqual( m_promptWrap + "fghi", lines[ 1 ], "Incorrect input line" );
         Assert.AreEqual( m_promptWrap + "jk", lines[ 2 ], "Incorrect input line" );
      }

      [TestMethod]
      public void InputAndOutputWrapping()
      {
         var view = new DummyTerminalView( s => new SizeD( s.Length, 1 ) );
         var terminal = new TerminalController( view, new SizeD( 1, 1 ), int.MaxValue, m_promptSpan, m_promptWrapSpan, m_promptOutputSpan, m_promptOutputWrapSpan );

         terminal.LineEntered += ( s, lea ) => terminal.WriteOutput( "out: " + lea.Line );
         terminal.CharsPerLine = m_prompt.Length + 5;

         "abcdefghijk\r".ForEach( c => terminal.CharTyped( c ) );

         DrawingInfo info = terminal.GetCurrentPageDrawingInfo( 50 );

         var lines = CoreTestHelpers.GetLinesAsText( info );

         Assert.AreEqual( 7, lines.Count, "Incorrect number of lines" );
         Assert.AreEqual( m_prompt + "abcde", lines[ 0 ], "Incorrect input line 0" );
         Assert.AreEqual( m_promptWrap + "fghi", lines[ 1 ], "Incorrect input line 1" );
         Assert.AreEqual( m_promptWrap + "jk", lines[ 2 ], "Incorrect input line 2" );

         Assert.AreEqual( m_promptOutput + "out: ab", lines[ 3 ], "Incorrect output line 3" );
         Assert.AreEqual( m_promptOutputWrap + "cdefgh", lines[ 4 ], "Incorrect output line 4" );
         Assert.AreEqual( m_promptOutputWrap + "ijk", lines[ 5 ], "Incorrect output line 5" );

         Assert.AreEqual( m_prompt, lines[ 6 ], "Incorrect input line 6" );
      }

      public TestContext TestContext { get; set; }
   }

}
