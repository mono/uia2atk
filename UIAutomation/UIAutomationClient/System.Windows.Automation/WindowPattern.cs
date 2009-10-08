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
			internal WindowPatternInformation (WindowProperties properties)
			{
				CanMaximize = properties.CanMaximize;
				CanMinimize = properties.CanMinimize;
				IsModal = properties.IsModal;
				IsTopmost = properties.IsTopmost;
				WindowVisualState = properties.WindowVisualState;
				WindowInteractionState = properties.WindowInteractionState;
			}

			public bool CanMaximize {
					get; private set;
			}

			public bool CanMinimize {
				get; private set;
			}

			public bool IsModal {
				get; private set;
			}

			public bool IsTopmost {
				get; private set;
			}

			public WindowVisualState WindowVisualState {
				get; private set;
			}

			public WindowInteractionState WindowInteractionState {
				get; private set;
			}
		}

		private IWindowPattern source;

		internal WindowPattern ()
		{
		}

		internal WindowPattern (IWindowPattern source)
		{
			this.source = source;
		}

		public WindowPatternInformation Cached {
			get {
				throw new NotImplementedException ();
			}
		}

		public WindowPatternInformation Current {
			get {
				return new WindowPatternInformation (source.Properties);
			}
		}

		public bool WaitForInputIdle (int milliseconds)
		{
			throw new NotImplementedException ();
		}

		public void Close ()
		{
			source.Close ();
		}

		public void SetWindowVisualState (WindowVisualState state)
		{
			source.SetWindowVisualState (state);
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
