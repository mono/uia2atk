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
//      Calvin Gaisford <calvinrg@gmail.com>
// 

using System;

namespace System.Windows.Automation
{
#region delegates
	public delegate void AutomationEventHandler( object sender, AutomationEventArgs e);
	
	public delegate void AutomationPropertyChangedEventHandler( object sender, AutomationPropertyChangedEventArgs e);
	
	public delegate void StructureChangedEventHandler( object sender, StructureChangedEventArgs e);
#endregion
	
	public class AutomationEvent : AutomationIdentifier
	{
#region Internal Constructor
		
		internal AutomationEvent (int id, string programmaticName) :
			base (id, programmaticName)
		{
		}
		
#endregion
		
#region Public Static Methods
		
		public static AutomationEvent LookupById (int id)
		{
			if (id == InvokePatternIdentifiers.InvokedEvent.Id)
				return InvokePatternIdentifiers.InvokedEvent;
			else if (id == AutomationElementIdentifiers.AsyncContentLoadedEvent.Id)
				return AutomationElementIdentifiers.AsyncContentLoadedEvent;
			else if (id == AutomationElementIdentifiers.AutomationFocusChangedEvent.Id)
				return AutomationElementIdentifiers.AutomationFocusChangedEvent;
			else if (id == AutomationElementIdentifiers.AutomationPropertyChangedEvent.Id)
				return AutomationElementIdentifiers.AutomationPropertyChangedEvent;
			else if (id == AutomationElementIdentifiers.LayoutInvalidatedEvent.Id)
				return AutomationElementIdentifiers.LayoutInvalidatedEvent;
			else if (id == AutomationElementIdentifiers.MenuClosedEvent.Id)
				return AutomationElementIdentifiers.MenuClosedEvent;
			else if (id == AutomationElementIdentifiers.MenuOpenedEvent.Id)
				return AutomationElementIdentifiers.MenuOpenedEvent;
			else if (id == AutomationElementIdentifiers.StructureChangedEvent.Id)
				return AutomationElementIdentifiers.StructureChangedEvent;
			else if (id == AutomationElementIdentifiers.ToolTipClosedEvent.Id)
				return AutomationElementIdentifiers.ToolTipClosedEvent;
			else if (id == AutomationElementIdentifiers.ToolTipOpenedEvent.Id)
				return AutomationElementIdentifiers.ToolTipOpenedEvent;
			else if (id == SelectionItemPatternIdentifiers.ElementAddedToSelectionEvent.Id)
				return SelectionItemPatternIdentifiers.ElementAddedToSelectionEvent;
			else if (id == SelectionItemPatternIdentifiers.ElementRemovedFromSelectionEvent.Id)
				return SelectionItemPatternIdentifiers.ElementRemovedFromSelectionEvent;
			else if (id == SelectionItemPatternIdentifiers.ElementSelectedEvent.Id)
				return SelectionItemPatternIdentifiers.ElementSelectedEvent;
			else if (id == SelectionPatternIdentifiers.InvalidatedEvent.Id)
				return SelectionPatternIdentifiers.InvalidatedEvent;
			else if (id == TextPatternIdentifiers.TextChangedEvent.Id)
				return TextPatternIdentifiers.TextChangedEvent;
			else if (id == TextPatternIdentifiers.TextSelectionChangedEvent.Id)
				return TextPatternIdentifiers.TextSelectionChangedEvent;
			else if (id == WindowPatternIdentifiers.WindowClosedEvent.Id)
				return WindowPatternIdentifiers.WindowClosedEvent;
			else if (id == WindowPatternIdentifiers.WindowOpenedEvent.Id)
				return WindowPatternIdentifiers.WindowOpenedEvent;
			else
				return null;
		}
		
#endregion
	}
}
