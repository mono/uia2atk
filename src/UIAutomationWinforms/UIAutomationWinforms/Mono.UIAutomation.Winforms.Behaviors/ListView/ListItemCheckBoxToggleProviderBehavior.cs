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
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.ListView;

namespace Mono.UIAutomation.Winforms.Behaviors.ListView
{
	internal class ListItemCheckBoxToggleProviderBehavior 
		: ProviderBehavior, IToggleProvider
	{
		#region Constructors
		
		public ListItemCheckBoxToggleProviderBehavior (ListViewProvider.ListViewListItemCheckBoxProvider checkboxProvider)
			: base (checkboxProvider)
		{
			this.checkboxProvider = checkboxProvider;
		}
		
		#endregion
		
		#region IProviderBehavior specializations

		public override AutomationPattern ProviderPattern { 
			get { return TogglePatternIdentifiers.Pattern; }
		}
		
		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == TogglePatternIdentifiers.ToggleStateProperty.Id)
				return ToggleState;
			else
				return null;
		}
		
		public override void Disconnect ()
		{
			Provider.SetEvent (ProviderEventType.TogglePatternToggleStateProperty,
			                   null);
		}

		
		public override void Connect ()
		{
			Provider.SetEvent (ProviderEventType.TogglePatternToggleStateProperty,
			                   new ListItemCheckBoxTogglePatternToggleStateEvent (checkboxProvider));
		}

		#endregion

		#region IToggleProvider specializations 
	
		public ToggleState ToggleState {
			get { 
				return checkboxProvider.ItemProvider.ListProvider.GetItemToggleState (checkboxProvider.ItemProvider); 
			}
		}
		
		public void Toggle ()
		{
			ListItemProvider provider = checkboxProvider.ItemProvider;
			
			if (provider.ListProvider.Control.InvokeRequired == true) {
				provider.ListProvider.Control.BeginInvoke (new SWF.MethodInvoker (Toggle));
				return;
			}
			
			provider.ListProvider.ToggleItem (provider);
		}

		#endregion


		#region Private Fields
		
		private ListViewProvider.ListViewListItemCheckBoxProvider checkboxProvider;

		#endregion
	}
}
