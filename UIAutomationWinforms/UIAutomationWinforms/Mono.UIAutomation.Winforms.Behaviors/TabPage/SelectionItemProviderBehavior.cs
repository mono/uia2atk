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
using System.Windows.Forms;
using System.Windows.Automation;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms.Events.TabPage;

namespace Mono.UIAutomation.Winforms.Behaviors.TabPage
{
	internal class SelectionItemProviderBehavior 
		: ProviderBehavior, ISelectionItemProvider
	{
		
#region Constructors

		public SelectionItemProviderBehavior (TabPageProvider provider)
			: base (provider)
		{
			pageProvider = provider;
		}
		
#endregion
		
#region IProviderBehavior Interface

		public override AutomationPattern ProviderPattern { 
			get { return SelectionItemPatternIdentifiers.Pattern; }
		}		

		public override void Connect ()
		{
			Provider.SetEvent (ProviderEventType.SelectionItemPatternElementSelectedEvent, 
			                   new SelectionItemPatternElementSelectedEvent ((TabPageProvider) Provider));
			Provider.SetEvent (ProviderEventType.SelectionItemPatternElementRemovedEvent, 
			                   new SelectionItemPatternElementRemovedEvent ((TabPageProvider) Provider));
		}

		public override void Disconnect ()
		{
			Provider.SetEvent (ProviderEventType.SelectionItemPatternElementSelectedEvent, 
			                   null);
			Provider.SetEvent (ProviderEventType.SelectionItemPatternElementRemovedEvent, 
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
			if (IsSelected) {
				return;
			}
			
			if (pageProvider.TabControlProvider.HasSelection) {
				throw new InvalidOperationException ();
			}

			Select ();
		}

		public void RemoveFromSelection ()
		{
			if (!IsSelected) {
				return;
			}
			
			// This control doesn't support having no selection.
			throw new InvalidOperationException ();	
		}

		public void Select ()
		{
			if (IsSelected) {
				return;
			}
			
			if (pageProvider.TabControlProvider.Control.InvokeRequired) {
				pageProvider.TabControlProvider.Control.BeginInvoke (new MethodInvoker (Select));
				return;
			}

			pageProvider.TabControlProvider.SelectItem (pageProvider);
		}

		public bool IsSelected {
			get { return pageProvider.IsSelected; }
		}

		public IRawElementProviderSimple SelectionContainer {
			get { return pageProvider.TabControlProvider; }
		}

#endregion
		
#region Private Fields
		
		private TabPageProvider pageProvider;
		
#endregion
	}
}
