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
//	Mario Carrion <mcarrion@novell.com>
// 

using System;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;
using System.Windows;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;

namespace Mono.UIAutomation.Winforms.Behaviors
{

	public class ListBoxItemSelectionProviderBehavior 
		: ListBoxItemProviderBehavior, ISelectionItemProvider
	{

#region Constructor

		public ListBoxItemSelectionProviderBehavior (ListBoxItemProvider provider)
			: base (provider)
		{
		}
		
#endregion
		
#region IProviderBehavior Interface

		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == SelectionItemPatternIdentifiers.IsSelectedProperty.Id)
				return IsSelected;
			else if (propertyId == SelectionItemPatternIdentifiers.SelectionContainerProperty.Id)
				return SelectionContainer;
			else
				return base.GetPropertyValue (propertyId);
		}

		public override AutomationPattern ProviderPattern { 
			get { return SelectionItemPatternIdentifiers.Pattern; }
		}
		
#endregion
		
#region ISelectionItemProvider Interface

		public void AddToSelection ()
		{
			if (IsSelected)
				return;
			
			bool multipleSelection = 
				(bool) ListBoxProvider.GetPropertyValue (SelectionPatternIdentifiers.CanSelectMultipleProperty.Id);
			
			if (multipleSelection == false) {
				if (ListBoxControl.SelectedItems.Count > -1)
					throw new InvalidOperationException ();
				else
					Select ();
			} else
				Select ();
		}

		public void RemoveFromSelection ()
		{
			if (IsSelected == false)
				return;
			
			bool multipleSelection = 
				(bool) ListBoxProvider.GetPropertyValue (SelectionPatternIdentifiers.CanSelectMultipleProperty.Id);
			bool selectionRequired =
				(bool) ListBoxProvider.GetPropertyValue (SelectionPatternIdentifiers.IsSelectionRequiredProperty.Id);

			if (multipleSelection == false && selectionRequired == true 
			    && ListBoxControl.SelectedItems.Count > 0) 
				throw new InvalidOperationException ();	
			else if (multipleSelection == true && selectionRequired == true 
			         && ListBoxControl.SelectedItems.Count == 1)
				throw new InvalidOperationException ();	
			else {
				ListBoxControl.SetSelected (ListBoxItemProvider.Index, false);

				//TODO: Would be great if this code is refactored to use an Event
				if (AutomationInteropProvider.ClientsAreListening) {
					AutomationEvent automation_event;
					if (ListBoxControl.SelectedItems.Count == 1)
						automation_event = SelectionItemPatternIdentifiers.ElementSelectedEvent;
					else 
						automation_event = SelectionItemPatternIdentifiers.ElementRemovedFromSelectionEvent;
	
					AutomationEventArgs args = new AutomationEventArgs (automation_event);
					AutomationInteropProvider.RaiseAutomationEvent (automation_event, 
					                                                Provider, args);
				}

			}
		}

		public void Select ()
		{
			if (IsSelected)
				return;
			
			ListBoxControl.SetSelected (ListBoxItemProvider.Index, true);
			
			//TODO: Would be great if this code is refactored to use an Event
			if (AutomationInteropProvider.ClientsAreListening) {
				AutomationEvent automation_event;

				if (ListBoxControl.SelectedItems.Count == 1)
					automation_event = SelectionItemPatternIdentifiers.ElementSelectedEvent;
				else 
					automation_event = SelectionItemPatternIdentifiers.ElementAddedToSelectionEvent;

				AutomationEventArgs args = new AutomationEventArgs (automation_event);
				AutomationInteropProvider.RaiseAutomationEvent (automation_event, 
				                                                Provider, args);
			}
		}

		public bool IsSelected {
			get {
				return ListBoxControl.SelectedIndices.Contains (ListBoxItemProvider.Index);
			}
		}

		public IRawElementProviderSimple SelectionContainer {
			get { return ListBoxProvider; }
		}

#endregion 		


	}
}
