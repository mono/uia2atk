
using System;

namespace Application
{
	class DummyEntryPoint
	{
		static void Main ()
		{
			//be sure we're referencing moon's corlib
			new System.Security.SecurityCriticalAttribute ();

			//be sure we're referencing MoonAtkBridge
			Moonlight.AtkBridge.AutomationBridge.CreateAutomationBridge ().GetAccessibleHandle ();
		}
	}
}
