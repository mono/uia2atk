// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//
// Copyright (c) 2019 AxxonSoft (http://axxonsoft.com)
//
// Authors:
//   Nikita Voronchev <nikita.voronchev@ru.axxonsoft.com>
//

using System;

namespace System.Windows.Automation
{
	public static class LegacyIAccessiblePatternIdentifiers
	{
#region Constructor
		private const int PatternId = 10018;
		private const int ChildIdPropertyId = 30091;
		private const int DefaultActionPropertyId = 30100;
		private const int DescriptionPropertyId = 30094;
		private const int HelpPropertyId = 30097;
		private const int KeyboardShortcutPropertyId = 30098;
		private const int NamePropertyId = 30092;
		private const int RolePropertyId = 30095;
		private const int StatePropertyId = 30096;
		private const int ValuePropertyId = 30093;

		static LegacyIAccessiblePatternIdentifiers ()
		{
			Pattern =
				new AutomationPattern (PatternId,
					"LegacyIAccessiblePatternIdentifiers.Pattern");

			ChildIdProperty =
				new AutomationProperty (ChildIdPropertyId,
					"LegacyIAccessiblePatternIdentifiers.ChildIdProperty");

			DefaultActionProperty =
				new AutomationProperty (DefaultActionPropertyId,
					"LegacyIAccessiblePatternIdentifiers.DefaultActionProperty");

			DescriptionProperty =
				new AutomationProperty (DescriptionPropertyId,
					"LegacyIAccessiblePatternIdentifiers.DescriptionProperty");

			HelpProperty =
				new AutomationProperty (HelpPropertyId,
					"LegacyIAccessiblePatternIdentifiers.HelpProperty");

			KeyboardShortcutProperty =
				new AutomationProperty (KeyboardShortcutPropertyId,
					"LegacyIAccessiblePatternIdentifiers.KeyboardShortcutProperty");

			NameProperty =
				new AutomationProperty (NamePropertyId,
					"LegacyIAccessiblePatternIdentifiers.NameProperty");

			RoleProperty =
				new AutomationProperty (RolePropertyId,
					"LegacyIAccessiblePatternIdentifiers.RoleProperty");

			StateProperty =
				new AutomationProperty (StatePropertyId,
					"LegacyIAccessiblePatternIdentifiers.StateProperty");

			ValueProperty =
				new AutomationProperty (ValuePropertyId,
					"LegacyIAccessiblePatternIdentifiers.ValueProperty");
		}

#endregion

#region Public Fields

		public static readonly AutomationPattern Pattern;
		public static readonly AutomationProperty ChildIdProperty;
		public static readonly AutomationProperty DefaultActionProperty;
		public static readonly AutomationProperty DescriptionProperty;
		public static readonly AutomationProperty HelpProperty;
		public static readonly AutomationProperty KeyboardShortcutProperty;
		public static readonly AutomationProperty NameProperty;
		public static readonly AutomationProperty RoleProperty;
		public static readonly AutomationProperty StateProperty;
		public static readonly AutomationProperty ValueProperty;

#endregion
	}
}
