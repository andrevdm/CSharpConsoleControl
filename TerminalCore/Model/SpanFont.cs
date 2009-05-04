using System;

namespace TerminalCore.Model
{
	public class SpanFont
	{
		public SpanFont( string typeFace, SpanFontStyle style, float size )
		{
			#region param checks
			if( typeFace == null )
			{
				throw new ArgumentNullException( "typeFace" );
			}
			#endregion

			TypeFace = typeFace;
			Style = style;
			Size = size;
		}

		public string TypeFace { get; private set; }
		public SpanFontStyle Style { get; private set; }
		public float Size { get; private set; }
	}
}