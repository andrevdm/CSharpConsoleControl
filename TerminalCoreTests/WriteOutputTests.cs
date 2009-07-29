using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TerminalCore;
using TerminalCore.Model;

namespace TerminalCoreTests
{
   [TestClass]
   public class WriteOutputTests
   {
      private readonly string m_prompt = "   > ";
      private readonly string m_promptWrap = "......";
      private readonly string m_promptOutput = "___";
      private readonly string m_promptOutputWrap = "****";

      private readonly Span m_promptSpan;
      private readonly Span m_promptWrapSpan;
      private readonly Span m_promptOutputSpan;
      private readonly Span m_promptOutputWrapSpan;

      public WriteOutputTests()
      {
         m_promptSpan = new Span( m_prompt );
         m_promptWrapSpan = new Span( m_promptWrap );
         m_promptOutputSpan = new Span( m_promptOutput );
         m_promptOutputWrapSpan = new Span( m_promptOutputWrap );
      }

      [TestMethod]
      public void WriteOutput()
      {
         var view = new DummyTerminalView( s => new SizeD( s.Length, 1 ) );
         var terminal = new TerminalController( view, new SizeD( 1, 1 ), int.MaxValue, m_promptSpan, m_promptWrapSpan, m_promptOutputSpan, m_promptOutputWrapSpan );

         terminal.CharsPerLine = 10;

         "ab\r".ForEach( c => terminal.CharTyped( c ) );
         terminal.WriteOutput( "123" );
         "cd".ForEach( c => terminal.CharTyped( c ) );

         DrawingInfo info = terminal.GetCurrentPageDrawingInfo( 5 );

         var lines = CoreTestHelpers.GetLinesAsText( info );

         Assert.AreEqual( 3, lines.Count, "Incorrect number of lines" );
         Assert.AreEqual( m_prompt + "ab", lines[ 0 ] );
         Assert.AreEqual( m_promptOutput + "123", lines[ 1 ] );
         Assert.AreEqual( m_prompt + "cd", lines[ 2 ] );
      }

      [TestMethod]
      public void WriteOutputWithNewLine()
      {
         var view = new DummyTerminalView( s => new SizeD( s.Length, 1 ) );
         var terminal = new TerminalController( view, new SizeD( 1, 1 ), int.MaxValue, m_promptSpan, m_promptWrapSpan, m_promptOutputSpan, m_promptOutputWrapSpan );

         terminal.CharsPerLine = 10;

         "ab\r".ForEach( c => terminal.CharTyped( c ) );
         terminal.WriteOutput( "1\r\n2" );
         "cd".ForEach( c => terminal.CharTyped( c ) );

         DrawingInfo info = terminal.GetCurrentPageDrawingInfo( 5 );

         var lines = CoreTestHelpers.GetLinesAsText( info );

         Assert.AreEqual( 4, lines.Count, "Incorrect number of lines" );
         Assert.AreEqual( m_prompt + "ab", lines[ 0 ] );
         Assert.AreEqual( m_promptOutput + "1", lines[ 1 ] );
         Assert.AreEqual( m_promptOutput + "2", lines[ 2 ] );
         Assert.AreEqual( m_prompt + "cd", lines[ 3 ] );
      }

      public TestContext TestContext { get; set; }
   }
}
