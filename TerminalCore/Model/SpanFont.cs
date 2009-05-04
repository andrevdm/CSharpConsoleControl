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

		/// <summary>
		/// Expressed as one ninety-sixth of an inch.
		/// For WPF use as is.
		/// For WinForms use (Size / 96.0F) * 72.0F;
		/// </summary>
		public float Size { get; private set; }
	}
}