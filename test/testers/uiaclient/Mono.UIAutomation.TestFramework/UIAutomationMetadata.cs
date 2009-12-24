using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Automation;

namespace Mono.UIAutomation.TestFramework
{
	class UIAutomationMetadata
	{
		protected ControlType ctype;
		public static string GetControlTypeName (ControlType ctype)
		{
			string fullName = ctype.ProgrammaticName;
			int lastDotIndex = fullName.LastIndexOf ('.');
			return fullName.Substring (lastDotIndex + 1);
		}
	}
}
