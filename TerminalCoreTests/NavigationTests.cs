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

         "abcdefghijk\r".ForEach( c => terminal.CharTyped( c ) );
         //TODO terminal.ControlKeyPressed( TerminalKey.Left, TerminalKeyModifiers.None );

         DrawingInfo info = terminal.GetCurrentPageDrawingInfo( 50 );

         var lines = CoreTestHelpers.GetLinesAsText( info, true );

         Assert.AreEqual( 7, lines.Count, "Incorrect number of lines" );
         Assert.AreEqual( m_prompt + "abcde", lines[ 0 ], "Incorrect input line 0" );
         Assert.AreEqual( m_promptWrap + "fghi", lines[ 1 ], "Incorrect input line 1" );
         Assert.AreEqual( m_promptWrap + "jk", lines[ 2 ], "Incorrect input line 2" );
         Assert.AreEqual( m_promptOutput + "out: ab", lines[ 3 ], "Incorrect output line 3" );
         Assert.AreEqual( m_promptOutputWrap + "cdefgh", lines[ 4 ], "Incorrect output line 4" );
         Assert.AreEqual( m_promptOutputWrap + "ijk", lines[ 5 ], "Incorrect output line 5" );
         Assert.AreEqual( m_prompt, lines[ 6 ], "Incorrect input line 6" );
      }

      [TestMethod]
      public void RightThroughWrap()
      {
         var view = new DummyTerminalView( s => new SizeD( s.Length, 1 ) );
         var terminal = new TerminalController( view, new SizeD( 1, 1 ), int.MaxValue, m_promptSpan, m_promptWrapSpan, m_promptOutputSpan, m_promptOutputWrapSpan );

         terminal.LineEntered += ( s, lea ) => terminal.WriteOutput( "out: " + lea.Line );
         terminal.CharsPerLine = m_prompt.Length + 5;

         "abcdefghijk\r".ForEach( c => terminal.CharTyped( c ) );
         //TODO terminal.ControlKeyPressed( TerminalKey.Left, TerminalKeyModifiers.None );

         DrawingInfo info = terminal.GetCurrentPageDrawingInfo( 50 );

         var lines = CoreTestHelpers.GetLinesAsText( info, true );

         Assert.AreEqual( 7, lines.Count, "Incorrect number of lines" );
         Assert.AreEqual( m_prompt + "abcde", lines[ 0 ], "Incorrect input line 0" );
         Assert.AreEqual( m_promptWrap + "fghi", lines[ 1 ], "Incorrect input line 1" );
         Assert.AreEqual( m_promptWrap + "jk", lines[ 2 ], "Incorrect input line 2" );
         Assert.AreEqual( m_promptOutput + "out: ab", lines[ 3 ], "Incorrect output line 3" );
         Assert.AreEqual( m_promptOutputWrap + "cdefgh", lines[ 4 ], "Incorrect output line 4" );
         Assert.AreEqual( m_promptOutputWrap + "ijk", lines[ 5 ], "Incorrect output line 5" );
         Assert.AreEqual( m_prompt, lines[ 6 ], "Incorrect input line 6" );
      }

      [TestMethod]
      public void Todo()
      {
         //TODO
         Assert.Fail( "Test navigation" );
      }
   }
}
