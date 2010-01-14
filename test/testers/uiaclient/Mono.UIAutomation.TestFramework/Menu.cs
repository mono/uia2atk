
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Automation;

namespace Mono.UIAutomation.TestFramework
{
	
	
	public class Menu : Element
	{
		public static readonly ControlType UIAType = ControlType.Menu;
		
		public Menu(AutomationElement elm)
			: base (elm)
		{
			
		}
		
		// Perform "Click" action.
		public void Click ()
		{
			Click (true);
		}

		public void Click (bool log)
		{
			if (log == true)
				procedureLogger.Action (string.Format ("Click {0}.", this.Name));

			InvokePattern ip = (InvokePattern) element.GetCurrentPattern (InvokePattern.Pattern);
			ip.Invoke ();
		}
	}
}
