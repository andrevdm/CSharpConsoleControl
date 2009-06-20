using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerminalCore.Model;

namespace TerminalCoreTests
{
   class CoreTestHelpers
   {
      public static List<string> GetLinesAsText( DrawingInfo info )
      {
         return GetLinesAsText( info, false );
      }

      public static List<string> GetLinesAsText( DrawingInfo info, bool drawCursor )
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

         if( drawCursor )
         {
            string lastLine = lines[ lines.Count - 1 ];
            lastLine = lastLine.Substring( 0, (int)info.CursorPosition.X ) + "|" + lastLine.Substring( (int)info.CursorPosition.X );
            lines[ lines.Count - 1 ] = lastLine;
         }

         return lines;
      }
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
