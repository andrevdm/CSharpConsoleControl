using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TerminalCore;
using TerminalCore.Model;

namespace TerminalCoreTests
{
   [TestClass]
   public class NavigationTests
   {
      private readonly string m_prompt = "   > ";
      private readonly string m_promptWrap = "......";
      private readonly string m_promptOutput = "___";
      private readonly string m_promptOutputWrap = "****";

      private readonly Span m_promptSpan;
      private readonly Span m_promptWrapSpan;
      private readonly Span m_promptOutputSpan;
      private readonly Span m_promptOutputWrapSpan;

      public NavigationTests()
      {
         m_promptSpan = new Span( m_prompt );
         m_promptWrapSpan = new Span( m_promptWrap );
         m_promptOutputSpan = new Span( m_promptOutput );
         m_promptOutputWrapSpan = new Span( m_promptOutputWrap );
      }

      [TestMethod]
      public void Left()
      {
         var view = new DummyTerminalView( s => new SizeD( s.Length, 1 ) );
         var terminal = new TerminalController( view, new SizeD( 1, 1 ), int.MaxValue, m_promptSpan, m_promptWrapSpan, m_promptOutputSpan, m_promptOutputWrapSpan );

         terminal.LineEntered += ( s, lea ) => terminal.WriteOutput( "out: " + lea.Line );
         terminal.CharsPerLine = m_prompt.Length + 5;

         "abcd".ForEach( c => terminal.CharTyped( c ) );
         terminal.ControlKeyPressed( TerminalKey.Left, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Left, TerminalKeyModifiers.None );

         DrawingInfo info = terminal.GetCurrentPageDrawingInfo( 50 );

         var lines = CoreTestHelpers.GetLinesAsText( info, true );
         Assert.AreEqual( m_prompt + "ab|cd", lines[ lines.Count - 1 ], "Left navigation failed" );
      }

      [TestMethod]
      public void Right()
      {
         var view = new DummyTerminalView( s => new SizeD( s.Length, 1 ) );
         var terminal = new TerminalController( view, new SizeD( 1, 1 ), int.MaxValue, m_promptSpan, m_promptWrapSpan, m_promptOutputSpan, m_promptOutputWrapSpan );

         terminal.LineEntered += ( s, lea ) => terminal.WriteOutput( "out: " + lea.Line );
         terminal.CharsPerLine = m_prompt.Length + 5;

         "abcd".ForEach( c => terminal.CharTyped( c ) );
         terminal.ControlKeyPressed( TerminalKey.Left, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Left, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Right, TerminalKeyModifiers.None );

         DrawingInfo info = terminal.GetCurrentPageDrawingInfo( 50 );

         var lines = CoreTestHelpers.GetLinesAsText( info, true );
         Assert.AreEqual( m_prompt + "abc|d", lines[ lines.Count - 1 ], "Left navigation failed" );
      }

      [TestMethod]
      public void LeftCantGoPastStartOfCommand()
      {
         var view = new DummyTerminalView( s => new SizeD( s.Length, 1 ) );
         var terminal = new TerminalController( view, new SizeD( 1, 1 ), int.MaxValue, m_promptSpan, m_promptWrapSpan, m_promptOutputSpan, m_promptOutputWrapSpan );

         terminal.LineEntered += ( s, lea ) => terminal.WriteOutput( "out: " + lea.Line );
         terminal.CharsPerLine = m_prompt.Length + 5;

         "abcd".ForEach( c => terminal.CharTyped( c ) );
         terminal.ControlKeyPressed( TerminalKey.Left, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Left, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Left, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Left, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Left, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Left, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Left, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Left, TerminalKeyModifiers.None );

         DrawingInfo info = terminal.GetCurrentPageDrawingInfo( 50 );

         var lines = CoreTestHelpers.GetLinesAsText( info, true );
         Assert.AreEqual( m_prompt + "|abcd", lines[ lines.Count - 1 ], "Left navigation failed" );
      }

      [TestMethod]
      public void RightCantGoPastEndOfCommand()
      {
         var view = new DummyTerminalView( s => new SizeD( s.Length, 1 ) );
         var terminal = new TerminalController( view, new SizeD( 1, 1 ), int.MaxValue, m_promptSpan, m_promptWrapSpan, m_promptOutputSpan, m_promptOutputWrapSpan );

         terminal.LineEntered += ( s, lea ) => terminal.WriteOutput( "out: " + lea.Line );
         terminal.CharsPerLine = m_prompt.Length + 5;

         "abcd".ForEach( c => terminal.CharTyped( c ) );
         terminal.ControlKeyPressed( TerminalKey.Left, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Right, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Right, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Right, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Right, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Right, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Right, TerminalKeyModifiers.None );

         DrawingInfo info = terminal.GetCurrentPageDrawingInfo( 50 );

         var lines = CoreTestHelpers.GetLinesAsText( info, true );
         Assert.AreEqual( m_prompt + "abcd|", lines[ lines.Count - 1 ], "Left navigation failed" );
      }

      [TestMethod]
      public void LeftThroughWrap()
      {
         var view = new DummyTerminalView( s => new SizeD( s.Length, 1 ) );
         var terminal = new TerminalController( view, new SizeD( 1, 1 ), int.MaxValue, m_promptSpan, m_promptWrapSpan, m_promptOutputSpan, m_promptOutputWrapSpan );

         terminal.LineEntered += ( s, lea ) => terminal.WriteOutput( "out: " + lea.Line );
         terminal.CharsPerLine = m_prompt.Length + 5;

         "abcdefghijk".ForEach( c => terminal.CharTyped( c ) );
         terminal.ControlKeyPressed( TerminalKey.Left, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Left, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Left, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Left, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Left, TerminalKeyModifiers.None );

         DrawingInfo info = terminal.GetCurrentPageDrawingInfo( 50 );

         var lines = CoreTestHelpers.GetLinesAsText( info, true );

         Assert.AreEqual( 3, lines.Count, "Incorrect number of lines" );
         Assert.AreEqual( m_prompt + "abcde", lines[ 0 ], "Incorrect input line 0" );
         Assert.AreEqual( m_promptWrap + "f|ghi", lines[ 1 ], "Incorrect input line 1" );
         Assert.AreEqual( m_promptWrap + "jk", lines[ 2 ], "Incorrect input line 2" );
      }

      [TestMethod]
      public void RightThroughWrap()
      {
         var view = new DummyTerminalView( s => new SizeD( s.Length, 1 ) );
         var terminal = new TerminalController( view, new SizeD( 1, 1 ), int.MaxValue, m_promptSpan, m_promptWrapSpan, m_promptOutputSpan, m_promptOutputWrapSpan );

         terminal.LineEntered += ( s, lea ) => terminal.WriteOutput( "out: " + lea.Line );
         terminal.CharsPerLine = m_prompt.Length + 5;

         "abcdefghijk".ForEach( c => terminal.CharTyped( c ) );
         terminal.ControlKeyPressed( TerminalKey.Left, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Left, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Left, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Left, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Left, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Left, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Left, TerminalKeyModifiers.None );

         terminal.ControlKeyPressed( TerminalKey.Right, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Right, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Right, TerminalKeyModifiers.None );

         DrawingInfo info = terminal.GetCurrentPageDrawingInfo( 50 );

         var lines = CoreTestHelpers.GetLinesAsText( info, true );

         Assert.AreEqual( 3, lines.Count, "Incorrect number of lines" );
         Assert.AreEqual( m_prompt + "abcde", lines[ 0 ], "Incorrect input line 0" );
         Assert.AreEqual( m_promptWrap + "fg|hi", lines[ 1 ], "Incorrect input line 1" );
         Assert.AreEqual( m_promptWrap + "jk", lines[ 2 ], "Incorrect input line 2" );
      }

      [TestMethod]
      public void Todo()
      {
         //TODO
         Assert.Fail( "Test navigation" );
      }
   }
}
