using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Automation;

namespace Mono.UIAutomation.TestFramework
{
	class UIAutomationMetadata
	{
		public static string GetControlTypeName (ControlType ctype)
		{
			string fullName = ctype.ProgrammaticName;
			int lastDotIndex = fullName.LastIndexOf ('.');
			return fullName.Substring (lastDotIndex + 1);
		}

		public static string GetElementDesc (AutomationElement element)
		{
			const int MAX_NAME_DISPLAY_LEN = 32;
			string name = element.Current.Name;
			if (name.Length > MAX_NAME_DISPLAY_LEN) {
				// Sometimes the name could be super long, e.g. for a RichTextBox, the name will be its whole content.
				name = name.Substring (0, MAX_NAME_DISPLAY_LEN) + "...";
			}
			
			return string.Format ("\"{0}\" {1}",
				name, GetControlTypeName (element.Current.ControlType));
		}
	}
}