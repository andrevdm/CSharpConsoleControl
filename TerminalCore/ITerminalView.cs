using TerminalCore.Model;

namespace TerminalCore
{
	public interface ITerminalView
	{
		SizeD MeasureText( string text );
	}
}