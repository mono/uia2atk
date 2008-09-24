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

namespace Mono.UIAutomation.Winforms.Behaviors.ListItem
{
	//NOTE: 
	//     About exceptions thrown: http://msdn.microsoft.com/en-us/library/ms749016.aspx
	internal class SelectionItemProviderBehavior 
		: ProviderBehavior, ISelectionItemProvider
	{
		
		#region Constructors

		public SelectionItemProviderBehavior (ListItemProvider provider)
			: base (provider)
		{
			this.itemProvider = provider;
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
			Provider.SetEvent (ProviderEventType.SelectionItemPatternElementSelectedEvent, 
			                   null);
			Provider.SetEvent (ProviderEventType.SelectionItemPatternElementAddedEvent, 
			                   null);
			Provider.SetEvent (ProviderEventType.SelectionItemPatternElementRemovedEvent, 
			                   null);
			Provider.SetEvent (ProviderEventType.SelectionItemPatternIsSelectedProperty, 
			                   null);
			Provider.SetEvent (ProviderEventType.SelectionItemPatternSelectionContainerProperty,
			                   null);
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
				(bool) itemProvider.ListProvider.GetPropertyValue (SelectionPatternIdentifiers.CanSelectMultipleProperty.Id);
			
			if (multipleSelection == false) {
				if (itemProvider.ListProvider.SelectedItemsCount > 0)
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
			
			if (itemProvider.ListProvider.Control.InvokeRequired == true) {
				itemProvider.ListProvider.Control.BeginInvoke (new MethodInvoker (RemoveFromSelection));
				return;
			}
			
			bool multipleSelection = 
				(bool) itemProvider.ListProvider.GetPropertyValue (SelectionPatternIdentifiers.CanSelectMultipleProperty.Id);
			bool selectionRequired =
				(bool) itemProvider.ListProvider.GetPropertyValue (SelectionPatternIdentifiers.IsSelectionRequiredProperty.Id);

			if (multipleSelection == false && selectionRequired == true 
			    && itemProvider.ListProvider.SelectedItemsCount > 0) 
				throw new InvalidOperationException ();	
			else if (multipleSelection == true && selectionRequired == true 
			         && itemProvider.ListProvider.SelectedItemsCount == 1)
				throw new InvalidOperationException ();	
			else {
				itemProvider.ListProvider.UnselectItem (itemProvider);

//				//TODO: Would be great if this code is refactored to use an Event
//				if (AutomationInteropProvider.ClientsAreListening) {
//					AutomationEvent automationEvent;
//
//					if (itemProvider.ListProvider.SelectedItemsCount == 1)
//						automationEvent = SelectionItemPatternIdentifiers.ElementSelectedEvent;
//					else 
//						automationEvent = SelectionItemPatternIdentifiers.ElementRemovedFromSelectionEvent;
//	
//					AutomationEventArgs args = new AutomationEventArgs (automationEvent);
//					AutomationInteropProvider.RaiseAutomationEvent (automationEvent, 
//					                                                Provider, args);
//				}

			}
		}

		public void Select ()
		{
			if (IsSelected == true)
				return;
			
			if (itemProvider.ListProvider.Control.InvokeRequired == true) {
				itemProvider.ListProvider.Control.BeginInvoke (new MethodInvoker (Select));
				return;
			}
			itemProvider.ListProvider.SelectItem (itemProvider);
			
			//TODO: Would be great if this code is refactored to use an Event
//			if (AutomationInteropProvider.ClientsAreListening) {
//				AutomationEvent automationEvent;
//
//				if (itemProvider.ListProvider.SelectedItemsCount == 1)
//					automationEvent = SelectionItemPatternIdentifiers.ElementSelectedEvent;
//				else 
//					automationEvent = SelectionItemPatternIdentifiers.ElementAddedToSelectionEvent;
//
//				AutomationEventArgs args = new AutomationEventArgs (automationEvent);
//				AutomationInteropProvider.RaiseAutomationEvent (automationEvent, 
//				                                                Provider, args);
//			}
		}

		public bool IsSelected {
			get { return itemProvider.ListProvider.IsItemSelected (itemProvider); }
		}

		public IRawElementProviderSimple SelectionContainer {
			get { return itemProvider.ListProvider; }
		}

		#endregion
		
		#region Private Fields
		
		private ListItemProvider itemProvider;
		
		#endregion
		
	}
}
