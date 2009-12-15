// Permission is hereby granted, free of charge, to any person obtaining 
// a copy of this software and associated documentation files (the 
// "Software"), to deal in the Software without restriction, including 
// without limitation the rights to use, copy, modify, merge, publish, 
// distribute, sublicense, and/or sell copies of the Software, and to 
// permit persons to whom the Software is furnished to do so, subject to 
// the following conditions: 
//  
// The above copyright notice and this permission notice shall be 
// included in all copies or substantial portions of the Software. 
//  
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION 
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
// 
// Copyright (c) 2009 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//  Sandy Armstrong <sanfordarmstrong@gmail.com>
//  Mike Gorse <mgorse@novell.com>
// 

using System;
using Mono.UIAutomation.Source;

namespace System.Windows.Automation
{
	public class WindowPattern : BasePattern
	{
		public struct WindowPatternInformation
		{
			private bool cache;
			private WindowPattern pattern;

			internal WindowPatternInformation (WindowPattern pattern, bool cache)
			{
				this.pattern = pattern;
				this.cache = cache;
			}

			public bool CanMaximize {
				get { return (bool) pattern.element.GetPropertyValue (CanMaximizeProperty, cache); }
			}

			public bool CanMinimize {
				get { return (bool) pattern.element.GetPropertyValue (CanMinimizeProperty, cache); }
			}

			public bool IsModal {
				get { return (bool) pattern.element.GetPropertyValue (IsModalProperty, cache); }
			}

			public bool IsTopmost {
				get { return (bool) pattern.element.GetPropertyValue (IsTopmostProperty, cache); }
			}

			public WindowVisualState WindowVisualState {
				get { return (WindowVisualState) pattern.element.GetPropertyValue (WindowVisualStateProperty, cache); }
			}

			public WindowInteractionState WindowInteractionState {
				get { return (WindowInteractionState) pattern.element.GetPropertyValue (WindowInteractionStateProperty, cache); }
			}
		}

		private AutomationElement element;
		private bool cached;
		private WindowPatternInformation currentInfo;
		private WindowPatternInformation cachedInfo;

		internal WindowPattern ()
		{
		}

		internal WindowPattern (IWindowPattern source, AutomationElement element, bool cached)
		{
			this.element = element;
			this.cached = cached;
			this.Source = source;
			currentInfo = new WindowPatternInformation (this, false);
			if (cached)
				cachedInfo = new WindowPatternInformation (this, true);
		}

		internal IWindowPattern Source { get; private set; }

		public WindowPatternInformation Cached {
			get {
				if (!cached)
					throw new InvalidOperationException ("Cannot request a property or pattern that is not cached");
				return cachedInfo;
			}
		}

		public WindowPatternInformation Current {
			get {
				return currentInfo;
			}
		}

		public bool WaitForInputIdle (int milliseconds)
		{
			return Source.WaitForInputIdle (milliseconds);
		}

		public void Close ()
		{
			Source.Close ();
		}

		public void SetWindowVisualState (WindowVisualState state)
		{
			Source.SetWindowVisualState (state);
		}

		public static readonly AutomationPattern Pattern =
			WindowPatternIdentifiers.Pattern;

		public static readonly AutomationProperty CanMaximizeProperty =
			WindowPatternIdentifiers.CanMaximizeProperty;

		public static readonly AutomationProperty CanMinimizeProperty =
			WindowPatternIdentifiers.CanMinimizeProperty;

		public static readonly AutomationProperty IsModalProperty =
			WindowPatternIdentifiers.IsModalProperty;

		public static readonly AutomationProperty IsTopmostProperty =
			WindowPatternIdentifiers.IsTopmostProperty;

		public static readonly AutomationProperty WindowVisualStateProperty =
			WindowPatternIdentifiers.WindowVisualStateProperty;

		public static readonly AutomationProperty WindowInteractionStateProperty =
			WindowPatternIdentifiers.WindowInteractionStateProperty;

		public static readonly AutomationEvent WindowOpenedEvent =
			WindowPatternIdentifiers.WindowOpenedEvent;

		public static readonly AutomationEvent WindowClosedEvent =
			WindowPatternIdentifiers.WindowClosedEvent;
	}
}
