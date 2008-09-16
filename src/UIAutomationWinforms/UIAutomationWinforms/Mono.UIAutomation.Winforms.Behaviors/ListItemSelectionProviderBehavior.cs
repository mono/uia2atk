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
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;

namespace Mono.UIAutomation.Winforms.Behaviors
{
	
	internal class ListItemSelectionProviderBehavior 
		: ProviderBehavior, ISelectionItemProvider
	{
		
		#region Constructors

		public ListItemSelectionProviderBehavior (ListItemProvider provider)
			: base (provider)
		{
			this.item_provider = provider;
		}
		
		#endregion
		
		#region IProviderBehavior Interface

		public override AutomationPattern ProviderPattern { 
			get { return SelectionItemPatternIdentifiers.Pattern; }
		}		

		public override void Connect (Control control)
		{
		}		
		
		public override void Disconnect (Control control)
		{
			//TODO: Should support all the following events:
//			Provider.SetEvent (ProviderEventType.SelectionItemElementSelectedEvent, 
//			                   null);
//			Provider.SetEvent (ProviderEventType.SelectionItemElementAddedEvent, 
//			                   null);
//			Provider.SetEvent (ProviderEventType.SelectionItemElementRemovedEvent, 
//			                   null);
//			Provider.SetEvent (ProviderEventType.SelectionItemIsSelected, 
//			                   null);
//			Provider.SetEvent (ProviderEventType.SelectionItemSelectionContainer,
//			                   null);
		}

		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == SelectionItemPatternIdentifiers.IsSelectedProperty.Id)
				return IsSelected;
			else if (propertyId == SelectionItemPatternIdentifiers.SelectionContainerProperty.Id)
				return SelectionContainer;
			else
				return null;
		}
		
		#endregion

		#region ISelectionItemProvider Interface
		
		public void AddToSelection ()
		{
			if (IsSelected == true)
				return;
			
			bool multipleSelection = 
				(bool) item_provider.ListProvider.GetPropertyValue (SelectionPatternIdentifiers.CanSelectMultipleProperty.Id);
			
			if (multipleSelection == false) {
				if (item_provider.ListProvider.SelectedItemsCount > 0)
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
			
			if (item_provider.ListProvider.Control.InvokeRequired == true) {
				item_provider.ListProvider.Control.BeginInvoke (new MethodInvoker (RemoveFromSelection));
				return;
			}
			
			bool multipleSelection = 
				(bool) item_provider.ListProvider.GetPropertyValue (SelectionPatternIdentifiers.CanSelectMultipleProperty.Id);
			bool selectionRequired =
				(bool) item_provider.ListProvider.GetPropertyValue (SelectionPatternIdentifiers.IsSelectionRequiredProperty.Id);

			if (multipleSelection == false && selectionRequired == true 
			    && item_provider.ListProvider.SelectedItemsCount > 0) 
				throw new InvalidOperationException ();	
			else if (multipleSelection == true && selectionRequired == true 
			         && item_provider.ListProvider.SelectedItemsCount == 1)
				throw new InvalidOperationException ();	
			else {
				item_provider.ListProvider.UnselectItem (item_provider);

				//TODO: Would be great if this code is refactored to use an Event
				if (AutomationInteropProvider.ClientsAreListening) {
					AutomationEvent automationEvent;

					if (item_provider.ListProvider.SelectedItemsCount == 1)
						automationEvent = SelectionItemPatternIdentifiers.ElementSelectedEvent;
					else 
						automationEvent = SelectionItemPatternIdentifiers.ElementRemovedFromSelectionEvent;
	
					AutomationEventArgs args = new AutomationEventArgs (automationEvent);
					AutomationInteropProvider.RaiseAutomationEvent (automationEvent, 
					                                                Provider, args);
				}

			}
		}

		public void Select ()
		{
			if (IsSelected == true)
				return;
			
			if (item_provider.ListProvider.Control.InvokeRequired == true) {
				item_provider.ListProvider.Control.BeginInvoke (new MethodInvoker (Select));
				return;
			}
			
			item_provider.ListProvider.SelectItem (item_provider);
			
			//TODO: Would be great if this code is refactored to use an Event
			if (AutomationInteropProvider.ClientsAreListening) {
				AutomationEvent automationEvent;

				if (item_provider.ListProvider.SelectedItemsCount == 1)
					automationEvent = SelectionItemPatternIdentifiers.ElementSelectedEvent;
				else 
					automationEvent = SelectionItemPatternIdentifiers.ElementAddedToSelectionEvent;

				AutomationEventArgs args = new AutomationEventArgs (automationEvent);
				AutomationInteropProvider.RaiseAutomationEvent (automationEvent, 
				                                                Provider, args);
			}
		}

		public bool IsSelected {
			get { return item_provider.ListProvider.IsItemSelected (item_provider); }
		}

		public IRawElementProviderSimple SelectionContainer {
			get { return item_provider.ListProvider; }
		}

		#endregion
		
		#region Private Fields
		
		private ListItemProvider item_provider;
		
		#endregion
		
	}
}
