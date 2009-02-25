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
// Copyright (c) 2008 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//  Sandy Armstrong <sanfordarmstrong@gmail.com>
// 

using System;

namespace System.Windows.Automation
{
	public class WindowPattern : BasePattern
	{
		public struct WindowPatternInformation
		{
			public bool CanMaximize {
				get {
					throw new NotImplementedException ();
				}
			}
			
			public bool CanMinimize {
				get {
					throw new NotImplementedException ();
				}
			}
			
			public bool IsModal {
				get {
					throw new NotImplementedException ();
				}
			}
			
			public bool IsTopmost {
				get {
					throw new NotImplementedException ();
				}
			}
			
			public WindowVisualState WindowVisualState {
				get {
					throw new NotImplementedException ();
				}
			}
			
			public WindowInteractionState WindowInteractionState {
				get {
					throw new NotImplementedException ();
				}
			}
		}
		
		internal WindowPattern ()
		{
		}

		public WindowPatternInformation Cached {
			get {
				throw new NotImplementedException ();
			}
		}

		public WindowPatternInformation Current {
			get {
				throw new NotImplementedException ();
			}
		}

		public bool WaitForInputIdle (int milliseconds)
		{
			throw new NotImplementedException ();
		}

		public void Close ()
		{
			throw new NotImplementedException ();
		}

		public void SetWindowVisualState (WindowVisualState state)
		{
			throw new NotImplementedException ();
		}

		public static readonly AutomationPattern Pattern;

		public static readonly AutomationProperty CanMaximizeProperty;

		public static readonly AutomationProperty CanMinimizeProperty;

		public static readonly AutomationProperty IsModalProperty;

		public static readonly AutomationProperty IsTopmostProperty;

		public static readonly AutomationProperty WindowVisualStateProperty;

		public static readonly AutomationProperty WindowInteractionStateProperty;

		public static readonly AutomationEvent WindowOpenedEvent;

		public static readonly AutomationEvent WindowClosedEvent;
	}
}
