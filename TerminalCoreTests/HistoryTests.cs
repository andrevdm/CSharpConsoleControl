using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TerminalCore;
using TerminalCore.Model;

namespace TerminalCoreTests
{
   [TestClass]
   public class HistoryTests
   {
      private readonly string m_prompt = "   > ";
      private readonly string m_promptWrap = "......";
      private readonly string m_promptOutput = "___";
      private readonly string m_promptOutputWrap = "****";

      private readonly Span m_promptSpan;
      private readonly Span m_promptWrapSpan;
      private readonly Span m_promptOutputSpan;
      private readonly Span m_promptOutputWrapSpan;

      public HistoryTests()
      {
         m_promptSpan = new Span( m_prompt );
         m_promptWrapSpan = new Span( m_promptWrap );
         m_promptOutputSpan = new Span( m_promptOutput );
         m_promptOutputWrapSpan = new Span( m_promptOutputWrap );
      }

      [TestMethod]
      public void EscClearsCurrentLine()
      {
         var view = new DummyTerminalView( s => new SizeD( s.Length, 1 ) );
         var terminal = new TerminalController( view, new SizeD( 1, 1 ), int.MaxValue, m_promptSpan, m_promptWrapSpan, m_promptOutputSpan, m_promptOutputWrapSpan );

         terminal.CharsPerLine = m_prompt.Length + 5;

         "abcdefghlakdgj;lkdjglkjd;lgkj;dslkjg;lds".ForEach( c => terminal.CharTyped( c ) );
         terminal.CharTyped( '\x1B' );

         DrawingInfo info = terminal.GetCurrentPageDrawingInfo( 4 );
         var lines = CoreTestHelpers.GetLinesAsText( info );

         Assert.AreEqual( 1, lines.Count, "Incorrect number of lines" );
         Assert.AreEqual( m_prompt, lines[ 0 ], "Incorrect input line 0" );
      }

      [TestMethod]
      public void UpShowsPreviousInHistory()
      {
         var view = new DummyTerminalView( s => new SizeD( s.Length, 1 ) );
         var terminal = new TerminalController( view, new SizeD( 1, 1 ), int.MaxValue, m_promptSpan, m_promptWrapSpan, m_promptOutputSpan, m_promptOutputWrapSpan );

         terminal.LineEntered += ( s, lea ) => terminal.WriteOutput( "out: " + lea.Line );
         terminal.CharsPerLine = m_prompt.Length + 5;

         "aa\r".ForEach( c => terminal.CharTyped( c ) );
         "bb\r".ForEach( c => terminal.CharTyped( c ) );
         "cc".ForEach( c => terminal.CharTyped( c ) ); //current line
         terminal.ControlKeyPressed( TerminalKey.Up, TerminalKeyModifiers.None );

         DrawingInfo info = terminal.GetCurrentPageDrawingInfo( 50 );

         var lines = CoreTestHelpers.GetLinesAsText( info );

         Assert.AreEqual( m_prompt + "bb", lines[ lines.Count - 1 ], "Last line should now show previously entered text" );
      }

      [TestMethod]
      public void UpShowsPreviousInHistory_CantGoPastFirst()
      {
         var view = new DummyTerminalView( s => new SizeD( s.Length, 1 ) );
         var terminal = new TerminalController( view, new SizeD( 1, 1 ), int.MaxValue, m_promptSpan, m_promptWrapSpan, m_promptOutputSpan, m_promptOutputWrapSpan );

         terminal.LineEntered += ( s, lea ) => terminal.WriteOutput( "out: " + lea.Line );
         terminal.CharsPerLine = m_prompt.Length + 5;

         "aa\r".ForEach( c => terminal.CharTyped( c ) );
         "bb\r".ForEach( c => terminal.CharTyped( c ) );
         "cc\r".ForEach( c => terminal.CharTyped( c ) );
         "dd".ForEach( c => terminal.CharTyped( c ) ); //current line

         terminal.ControlKeyPressed( TerminalKey.Up, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Up, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Up, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Up, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Up, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Up, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Up, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Up, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Up, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Up, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Up, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Up, TerminalKeyModifiers.None );

         DrawingInfo info = terminal.GetCurrentPageDrawingInfo( 50 );

         var lines = CoreTestHelpers.GetLinesAsText( info );

         Assert.AreEqual( m_prompt + "aa", lines[ lines.Count - 1 ], "Last line should now show first command" );
      }

      [TestMethod]
      public void DownShowsNextInHistory()
      {
         var view = new DummyTerminalView( s => new SizeD( s.Length, 1 ) );
         var terminal = new TerminalController( view, new SizeD( 1, 1 ), int.MaxValue, m_promptSpan, m_promptWrapSpan, m_promptOutputSpan, m_promptOutputWrapSpan );

         terminal.LineEntered += ( s, lea ) => terminal.WriteOutput( "out: " + lea.Line );
         terminal.CharsPerLine = m_prompt.Length + 5;

         "aa\r".ForEach( c => terminal.CharTyped( c ) );
         "bb\r".ForEach( c => terminal.CharTyped( c ) );
         "cc\r".ForEach( c => terminal.CharTyped( c ) );
         "dd".ForEach( c => terminal.CharTyped( c ) ); //current line

         terminal.ControlKeyPressed( TerminalKey.Up, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Up, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Up, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Down, TerminalKeyModifiers.None );

         DrawingInfo info = terminal.GetCurrentPageDrawingInfo( 50 );

         var lines = CoreTestHelpers.GetLinesAsText( info );

         Assert.AreEqual( m_prompt + "bb", lines[ lines.Count - 1 ], "Incorrect history item shown" );
      }

      [TestMethod]
      public void DownShowsNextInHistory_CantGoPastLast()
      {
         var view = new DummyTerminalView( s => new SizeD( s.Length, 1 ) );
         var terminal = new TerminalController( view, new SizeD( 1, 1 ), int.MaxValue, m_promptSpan, m_promptWrapSpan, m_promptOutputSpan, m_promptOutputWrapSpan );

         terminal.LineEntered += ( s, lea ) => terminal.WriteOutput( "out: " + lea.Line );
         terminal.CharsPerLine = m_prompt.Length + 5;

         "aa\r".ForEach( c => terminal.CharTyped( c ) );
         "bb\r".ForEach( c => terminal.CharTyped( c ) );
         "cc\r".ForEach( c => terminal.CharTyped( c ) );
         "dd".ForEach( c => terminal.CharTyped( c ) ); //current line

         terminal.ControlKeyPressed( TerminalKey.Up, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Up, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Down, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Down, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Down, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Down, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Down, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Down, TerminalKeyModifiers.None );
         terminal.ControlKeyPressed( TerminalKey.Down, TerminalKeyModifiers.None );

         DrawingInfo info = terminal.GetCurrentPageDrawingInfo( 50 );

         var lines = CoreTestHelpers.GetLinesAsText( info );

         Assert.AreEqual( m_prompt + "cc", lines[ lines.Count - 1 ], "Incorrect history item shown" );
      }
   }
}
