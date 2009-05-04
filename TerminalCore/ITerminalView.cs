using TerminalCore.Model;

namespace TerminalCore
{
	public interface ITerminalView
	{
		SizeF MeasureText( string text, SpanFont font );
	}
}