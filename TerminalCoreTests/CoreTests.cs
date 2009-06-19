using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TerminalCore;
using TerminalCore.Model;

namespace TerminalCoreTests
{
   [TestClass]
   public class CoreTests
   {
      [TestMethod]
      public void SingleLine()
      {
         string prompt = "   > ";
         string promptWrap = "     ";
         string promptOutput = "  ";
         string promptOutputWrap = "****";

         var view = new DummyTerminalView( s => new SizeD( s.Length, 1 ) );

         var terminal = new TerminalController(
             view, 
             new SizeD( 1, 1 ),
             int.MaxValue,
             new Span( prompt ),
             new Span( promptWrap ),
             new Span( promptOutput ),
             new Span( promptOutputWrap ) );

         terminal.CharsPerLine = 10;

         "ab".ForEach( c => terminal.CharTyped( c ) );

         DrawingInfo info = terminal.GetCurrentPageDrawingInfo( 5 );

         var lines = GetLinesAsText( info );

         Assert.AreEqual( 1, lines.Count, "Incorrect number of lines" );
         Assert.AreEqual( prompt + "ab", lines[ 0 ] );
      }

      [TestMethod]
      public void AfterReturnTypeOutputAndNextInputLineShow()
      {
         string prompt = "   > ";
         string promptWrap = "     ";
         string promptOutput = "  ";
         string promptOutputWrap = "****";

         var view = new DummyTerminalView( s => new SizeD( s.Length, 1 ) );

         var terminal = new TerminalController(
             view,
             new SizeD( 1, 1 ),
             int.MaxValue,
             new Span( prompt ),
             new Span( promptWrap ),
             new Span( promptOutput ),
             new Span( promptOutputWrap ) );

         terminal.LineEntered += (s, lea) => terminal.WriteOutput( "out: " + lea.Line );
         terminal.CharsPerLine = 10;

         "ab\r".ForEach( c => terminal.CharTyped( c ) );

         DrawingInfo info = terminal.GetCurrentPageDrawingInfo( 5 );

         var lines = GetLinesAsText( info );

         Assert.AreEqual( 3, lines.Count, "Incorrect number of lines" );
         Assert.AreEqual( prompt + "ab", lines[ 0 ], "Incorrect input line" );
         Assert.AreEqual( promptOutput + "out: ab", lines[ 1 ], "Incorrect outline" );
         Assert.AreEqual( prompt, lines[ 2 ], "Incorrect next input line" );
      }

      [TestMethod]
      public void InputWrapping()
      {
         string prompt = "   > ";
         string promptWrap = "......";
         string promptOutput = "___";
         string promptOutputWrap = "****";

         var view = new DummyTerminalView( s => new SizeD( s.Length, 1 ) );

         var terminal = new TerminalController(
             view,
             new SizeD( 1, 1 ),
             int.MaxValue,
             new Span( prompt ),
             new Span( promptWrap ),
             new Span( promptOutput ),
             new Span( promptOutputWrap ) );

         terminal.CharsPerLine = prompt.Length + 5;

         "abcdefghijk".ForEach( c => terminal.CharTyped( c ) );

         DrawingInfo info = terminal.GetCurrentPageDrawingInfo( 5 );

         var lines = GetLinesAsText( info );

         Assert.AreEqual( 3, lines.Count, "Incorrect number of lines" );
         Assert.AreEqual( prompt + "abcde", lines[ 0 ], "Incorrect input line" );
         Assert.AreEqual( promptWrap + "fghi", lines[ 1 ], "Incorrect input line" );
         Assert.AreEqual( promptWrap + "jk", lines[ 2 ], "Incorrect input line" );
      }

      [TestMethod]
      public void InputAndOutputWrapping()
      {
         string prompt = "   > ";
         string promptWrap = "......";
         string promptOutput = "___";
         string promptOutputWrap = "****";

         var view = new DummyTerminalView( s => new SizeD( s.Length, 1 ) );

         var terminal = new TerminalController(
             view,
             new SizeD( 1, 1 ),
             int.MaxValue,
             new Span( prompt ),
             new Span( promptWrap ),
             new Span( promptOutput ),
             new Span( promptOutputWrap ) );

         terminal.LineEntered += ( s, lea ) => terminal.WriteOutput( "out: " + lea.Line );
         terminal.CharsPerLine = prompt.Length + 5;

         "abcdefghijk\r".ForEach( c => terminal.CharTyped( c ) );

         DrawingInfo info = terminal.GetCurrentPageDrawingInfo( 50 );

         var lines = GetLinesAsText( info );

         Assert.AreEqual( 7, lines.Count, "Incorrect number of lines" );
         Assert.AreEqual( prompt + "abcde", lines[ 0 ], "Incorrect input line 0" );
         Assert.AreEqual( promptWrap + "fghi", lines[ 1 ], "Incorrect input line 1" );
         Assert.AreEqual( promptWrap + "jk", lines[ 2 ], "Incorrect input line 2" );

         Assert.AreEqual( promptOutput + "out: ab", lines[ 3 ], "Incorrect output line 3" );
         Assert.AreEqual( promptOutputWrap + "cdefgh", lines[ 4 ], "Incorrect output line 4" );
         Assert.AreEqual( promptOutputWrap + "ijk", lines[ 5 ], "Incorrect output line 5" );
         
         Assert.AreEqual( prompt, lines[ 6 ], "Incorrect input line 6" );
      }

      private static List<string> GetLinesAsText( DrawingInfo info )
      {
         var lines = new List<string>();

         foreach( var line in info.Lines )
         {
            string lineText = "";
            foreach( var span in line.Spans )
            {
               lineText += span.Text;
            }

            lines.Add( lineText );
         }

         return lines;
      }

      public TestContext TestContext { get; set; }
   }

   static class CoreTestExtensionMethods
   {
      public static void ForEach( this string text, Action<char> act )
      {
         foreach( char c in text )
         {
            act( c );
         }
      }
   }
}
