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
using Mono.UIAutomation.Source;

namespace System.Windows.Automation
{
	public class LegacyIAccessiblePattern : BasePattern
	{
		public struct LegacyIAccessiblePatternInformation
		{
			private bool cache;
			private LegacyIAccessiblePattern pattern;

			internal LegacyIAccessiblePatternInformation (LegacyIAccessiblePattern pattern, bool cache)
			{
				this.pattern = pattern;
				this.cache = cache;
			}
			public int ChildId
			{
				get { return (int)pattern.element.GetPropertyValue (ChildIdProperty, cache); }
			}

			public string DefaultAction
			{
				get { return (string)pattern.element.GetPropertyValue (DefaultActionProperty, cache); }
			}

			public string Description
			{
				get { return (string)pattern.element.GetPropertyValue (DescriptionProperty, cache); }
			}

			public string Help
			{
				get { return (string)pattern.element.GetPropertyValue (HelpProperty, cache); }
			}

			public string KeyboardShortcut
			{
				get { return (string)pattern.element.GetPropertyValue (KeyboardShortcutProperty, cache); }
			}

			public string Name
			{
				get { return (string)pattern.element.GetPropertyValue (NameProperty, cache); }
			}

			public int Role
			{
				// CLS-compliantment requires signed int instead of UInt32
				get
				{
					return (int)pattern.element.GetPropertyValue (RoleProperty, cache);
				}
			}

			public int State
			{
				// CLS-compliantment requires signed int instead of UInt32
				get { return (int)pattern.element.GetPropertyValue (StateProperty, cache); }
			}

			public string Value
			{
				get { return (string)pattern.element.GetPropertyValue (ValueProperty, cache); }
			}
		}

		public void DoDefaultAction ()
		{
			Source.DoDefaultAction ();
		}

		private AutomationElement element;
		private bool cached;
		private LegacyIAccessiblePatternInformation currentInfo;
		private LegacyIAccessiblePatternInformation cachedInfo;

		internal LegacyIAccessiblePattern ()
		{
		}

		internal LegacyIAccessiblePattern (ILegacyIAccessiblePattern source, AutomationElement element, bool cached)
		{
			this.element = element;
			this.cached = cached;
			this.Source = source;
			currentInfo = new LegacyIAccessiblePatternInformation (this, false);
			if (cached)
				cachedInfo = new LegacyIAccessiblePatternInformation (this, true);
		}

		internal ILegacyIAccessiblePattern Source { get; private set; }

		public LegacyIAccessiblePatternInformation Cached
		{
			get {
				if (!cached)
					throw new InvalidOperationException ("Cannot request a property or pattern that is not cached");
				return cachedInfo;
			}
		}

		public LegacyIAccessiblePatternInformation Current
		{
			get {
				return currentInfo;
			}
		}

		public static readonly AutomationPattern Pattern =
			LegacyIAccessiblePatternIdentifiers.Pattern;

		public static readonly AutomationProperty ChildIdProperty =
			LegacyIAccessiblePatternIdentifiers.ChildIdProperty;

		public static readonly AutomationProperty DefaultActionProperty =
			LegacyIAccessiblePatternIdentifiers.DefaultActionProperty;

		public static readonly AutomationProperty DescriptionProperty =
			LegacyIAccessiblePatternIdentifiers.DescriptionProperty;

		public static readonly AutomationProperty HelpProperty =
			LegacyIAccessiblePatternIdentifiers.HelpProperty;

		public static readonly AutomationProperty KeyboardShortcutProperty =
			LegacyIAccessiblePatternIdentifiers.KeyboardShortcutProperty;

		public static readonly AutomationProperty NameProperty =
			LegacyIAccessiblePatternIdentifiers.NameProperty;

		public static readonly AutomationProperty RoleProperty =
			LegacyIAccessiblePatternIdentifiers.RoleProperty;

		public static readonly AutomationProperty StateProperty =
			LegacyIAccessiblePatternIdentifiers.StateProperty;

		public static readonly AutomationProperty ValueProperty =
			LegacyIAccessiblePatternIdentifiers.ValueProperty;
	}
}
