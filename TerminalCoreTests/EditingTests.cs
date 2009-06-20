using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TerminalCore;
using TerminalCore.Model;


namespace TerminalCoreTests
{
   [TestClass]
   public class EditingTests
   {
      private readonly string m_prompt = "   > ";
      private readonly string m_promptWrap = "......";
      private readonly string m_promptOutput = "___";
      private readonly string m_promptOutputWrap = "****";

      private readonly Span m_promptSpan;
      private readonly Span m_promptWrapSpan;
      private readonly Span m_promptOutputSpan;
      private readonly Span m_promptOutputWrapSpan;

      public EditingTests()
      {
         m_promptSpan = new Span( m_prompt );
         m_promptWrapSpan = new Span( m_promptWrap );
         m_promptOutputSpan = new Span( m_promptOutput );
         m_promptOutputWrapSpan = new Span( m_promptOutputWrap );
      }

      [TestMethod]
      public void Backspace()
      {
         var view = new DummyTerminalView( s => new SizeD( s.Length, 1 ) );
         var terminal = new TerminalController( view, new SizeD( 1, 1 ), int.MaxValue, m_promptSpan, m_promptWrapSpan, m_promptOutputSpan, m_promptOutputWrapSpan );

         terminal.LineEntered += ( s, lea ) => terminal.WriteOutput( "out: " + lea.Line );
         terminal.CharsPerLine = m_prompt.Length + 5;

         "abcd".ForEach( c => terminal.CharTyped( c ) );
         terminal.CharTyped( '\b' );
         terminal.CharTyped( '\b' );

         DrawingInfo info = terminal.GetCurrentPageDrawingInfo( 50 );

         var lines = CoreTestHelpers.GetLinesAsText( info );
         Assert.AreEqual( m_prompt + "ab", lines[ lines.Count - 1 ], "Backspace should have deleted the text" );
      }

      [TestMethod]
      public void BackspaceThroughWrappedText()
      {
         var view = new DummyTerminalView( s => new SizeD( s.Length, 1 ) );
         var terminal = new TerminalController( view, new SizeD( 1, 1 ), int.MaxValue, m_promptSpan, m_promptWrapSpan, m_promptOutputSpan, m_promptOutputWrapSpan );

         terminal.LineEntered += ( s, lea ) => terminal.WriteOutput( "out: " + lea.Line );
         terminal.CharsPerLine = m_prompt.Length + 5;

         "12345ab".ForEach( c => terminal.CharTyped( c ) );
         terminal.CharTyped( '\b' );
         terminal.CharTyped( '\b' );
         terminal.CharTyped( '\b' );

         DrawingInfo info = terminal.GetCurrentPageDrawingInfo( 50 );

         var lines = CoreTestHelpers.GetLinesAsText( info );
         Assert.AreEqual( m_prompt + "1234", lines[ 0 ], "Backspace should have deleted the text" );
      }

      [TestMethod]
      public void Todo()
      {
         //TODO
         Assert.Fail( "Test editing: ins, delete etc." );
      }
   }
}
