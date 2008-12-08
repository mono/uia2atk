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
//	Brad Taylor <brad@getcoded.net>
// 

using System;
using System.Windows.Automation;
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms.Events.MonthCalendar;

namespace Mono.UIAutomation.Winforms.Behaviors.MonthCalendar
{
	internal class ListItemSelectionItemProviderBehavior 
		: ProviderBehavior, ISelectionItemProvider
	{
#region Public Methods
		public ListItemSelectionItemProviderBehavior (MonthCalendarListItemProvider provider)
			: base (provider)
		{
			this.itemProvider = provider;
		}
#endregion
		
#region IProviderBehavior Interface
		public override void Connect ()
		{
			//NOTE: SelectionItem.SelectionContainer never changes.
			Provider.SetEvent (ProviderEventType.SelectionItemPatternElementSelectedEvent,
			                   new ListItemSelectionItemPatternElementSelectedEvent (itemProvider));
			Provider.SetEvent (ProviderEventType.SelectionItemPatternElementAddedEvent, 
			                   new ListItemSelectionItemPatternElementAddedEvent (itemProvider));
			Provider.SetEvent (ProviderEventType.SelectionItemPatternElementRemovedEvent, 
			                   new SelectionItemPatternElementRemovedEvent (itemProvider));
			Provider.SetEvent (ProviderEventType.SelectionItemPatternIsSelectedProperty, 
			                   new ListItemSelectionItemPatternIsSelectedEvent (itemProvider));
		}	

		public override void Disconnect ()
		{
			Provider.SetEvent (ProviderEventType.SelectionItemPatternElementSelectedEvent, null);
			Provider.SetEvent (ProviderEventType.SelectionItemPatternElementAddedEvent, null);
			Provider.SetEvent (ProviderEventType.SelectionItemPatternElementRemovedEvent, null);
			Provider.SetEvent (ProviderEventType.SelectionItemPatternIsSelectedProperty, null);
		}

		public override AutomationPattern ProviderPattern { 
			get { return SelectionItemPatternIdentifiers.Pattern; }
		}		

		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == SelectionItemPatternIdentifiers.IsSelectedProperty.Id)
				return IsSelected;
			else if (propertyId == SelectionItemPatternIdentifiers.SelectionContainerProperty.Id)
				return SelectionContainer;
			return null;
		}
#endregion

#region ISelectionItemProvider Interface
		public void AddToSelection ()
		{
			if (IsSelected) {
				return;
			}

			itemProvider.DataGridProvider
				.AddToSelection (itemProvider);
		}

		public void RemoveFromSelection ()
		{
			if (!IsSelected) {
				return;
			}

			itemProvider.DataGridProvider
				.RemoveFromSelection (itemProvider);
		}

		public void Select ()
		{
			if (IsSelected) {
				return;
			}

			if (itemProvider.DataGridProvider.Control.InvokeRequired) {
				itemProvider.DataGridProvider.Control.BeginInvoke (new SWF.MethodInvoker (Select));
				return;
			}

			itemProvider.DataGridProvider.SelectItem (itemProvider);
		}

		public bool IsSelected {
			get { return itemProvider.DataGridProvider.IsItemSelected (itemProvider); }
		}

		public IRawElementProviderSimple SelectionContainer {
			get { return itemProvider.DataGridProvider; }
		}
#endregion
		
		private MonthCalendarListItemProvider itemProvider;
	}
}
