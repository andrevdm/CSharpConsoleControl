using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TerminalCore
{
	public class TerminalController
	{
		private readonly ITerminalView m_view;

		public TerminalController( ITerminalView view, string prompt )
		{
			m_view = view;

			#region param checks
			if( view == null )
			{
				throw new ArgumentNullException( "view" );
			}

			if( prompt == null )
			{
				throw new ArgumentNullException( "prompt" );
			}
			#endregion

			Prompt = prompt;
		}

		public string Prompt { get; private set; }
	}
}