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
	public class SelectionItemPattern : BasePattern
	{
		public struct SelectionItemPatternInformation
		{
			public bool IsSelected {
				get {
					throw new NotImplementedException ();
				}
			}
			
			public AutomationElement SelectionContainer {
				get {
					throw new NotImplementedException ();
				}
			}
		}
		
		internal SelectionItemPattern ()
		{
		}

		public SelectionItemPatternInformation Cached {
			get {
				throw new NotImplementedException ();
			}
		}

		public SelectionItemPatternInformation Current {
			get {
				throw new NotImplementedException ();
			}
		}

		public void Select ()
		{
			throw new NotImplementedException ();
		}

		public void AddToSelection ()
		{
			throw new NotImplementedException ();
		}

		public void RemoveFromSelection ()
		{
			throw new NotImplementedException ();
		}

		public static readonly AutomationPattern Pattern;

		public static readonly AutomationProperty IsSelectedProperty;

		public static readonly AutomationProperty SelectionContainerProperty;

		public static readonly AutomationEvent ElementAddedToSelectionEvent;

		public static readonly AutomationEvent ElementRemovedFromSelectionEvent;

		public static readonly AutomationEvent ElementSelectedEvent;
	}
}
