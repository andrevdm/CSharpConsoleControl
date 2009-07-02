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
      public void Insert()
      {
         var view = new DummyTerminalView( s => new SizeD( s.Length, 1 ) );
         var terminal = new TerminalController( view, new SizeD( 1, 1 ), int.MaxValue, m_promptSpan, m_promptWrapSpan, m_promptOutputSpan, m_promptOutputWrapSpan );

         terminal.LineEntered += ( s, lea ) => terminal.WriteOutput( "out: " + lea.Line );
         terminal.CharsPerLine = m_prompt.Length + 5;

         "abcde".ForEach( c => terminal.CharTyped( c ) );
         terminal.ControlKeyPressed( TerminalKey.Left, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Left, TerminalKeyModifiers.None );
         terminal.CharTyped( '1' );
         terminal.CharTyped( '2' );

         DrawingInfo info = terminal.GetCurrentPageDrawingInfo( 50 );

         var lines = CoreTestHelpers.GetLinesAsText( info );
         Assert.AreEqual( 2, lines.Count, "Incorrect number of lines" );
         Assert.AreEqual( m_prompt + "abc12", lines[ 0 ], "Line 0 invalid" );
         Assert.AreEqual( m_promptWrap + "de", lines[ 1 ], "Line 1 invalid" );
      }

      public void Delete()
      {
         var view = new DummyTerminalView( s => new SizeD( s.Length, 1 ) );
         var terminal = new TerminalController( view, new SizeD( 1, 1 ), int.MaxValue, m_promptSpan, m_promptWrapSpan, m_promptOutputSpan, m_promptOutputWrapSpan );

         terminal.LineEntered += ( s, lea ) => terminal.WriteOutput( "out: " + lea.Line );
         terminal.CharsPerLine = m_prompt.Length + 5;

         "abcdefg".ForEach( c => terminal.CharTyped( c ) );
         terminal.ControlKeyPressed( TerminalKey.Left, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Left, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Left, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Delete, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Delete, TerminalKeyModifiers.None );

         DrawingInfo info = terminal.GetCurrentPageDrawingInfo( 50 );

         var lines = CoreTestHelpers.GetLinesAsText( info );
         Assert.AreEqual( 1, lines.Count, "Incorrect number of lines" );
         Assert.AreEqual( m_prompt + "abcdg", lines[ 0 ], "Line 0 invalid" );
      }
   }
}
