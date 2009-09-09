
using System;

namespace Application
{
	class DummyEntryPoint
	{
		static int Main ()
		{
			//be sure we're referencing moon's corlib
			new System.Security.SecurityCriticalAttribute ();

			//be sure we're referencing MoonAtkBridge
			Moonlight.AtkBridge.AutomationBridge.CreateAutomationBridge ().GetAccessibleHandle ();

			//be sure we're referencing private MoonAtkBridge's API used by moonlight
			return Moonlight.AtkBridge.AutomationBridge.IsAccessibilityEnabled () ? 0 : 1;
		}
	}
}
