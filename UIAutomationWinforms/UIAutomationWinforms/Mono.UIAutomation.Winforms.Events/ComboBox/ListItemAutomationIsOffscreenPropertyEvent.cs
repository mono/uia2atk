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
using SWF = System.Windows.Forms;
using System.Windows.Automation;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Navigation;

namespace Mono.UIAutomation.Winforms.Events.ComboBox
{
	
	internal class ListItemAutomationIsOffscreenPropertyEvent 
		: AutomationIsOffscreenPropertyEvent
	{
		
		#region Constructors
		
		public ListItemAutomationIsOffscreenPropertyEvent (ListItemProvider provider)
			: base (provider)
		{
			listboxProvider 
				= (ComboBoxProvider.ComboBoxListBoxProvider) provider.ListProvider;
			combobox = (SWF.ComboBox) listboxProvider.Control;
		}
		
		#endregion
		
		#region IConnectable Overrides
		
		public override void Connect ()
		{
			combobox.DropDown += OnDropDown;
			combobox.DropDownClosed += OnDropDown;
			listboxProvider.NavigationUpdated += OnNavigationUpdated;
		}

		public override void Disconnect ()
		{
			combobox.DropDown -= OnDropDown;
			combobox.DropDownClosed -= OnDropDown;
			listboxProvider.NavigationUpdated -= OnNavigationUpdated;
			if (scrollbar != null) {
				scrollbar.ValueChanged -= OnScrollValueChanged;
				scrollbar = null;
			}
		}
		
		#endregion
		
		#region Private Methods		
		
		private void OnDropDown (object sender, EventArgs e)
		{
			RaiseAutomationPropertyChangedEvent ();
		}

		private void OnNavigationUpdated (object sender, NavigationEventArgs e)
		{
			ScrollBarProvider provider;
			if ((provider = e.ChildProvider as ScrollBarProvider) != null) {
				if (provider != null) {
					SWF.ScrollBar scrollbarProvider = (SWF.ScrollBar) provider.Control;
					if (e.ChangeType == StructureChangeType.ChildAdded) {
						if (scrollbar == scrollbarProvider)
							return;

						if (scrollbar != null)
							scrollbar.ValueChanged -= OnScrollValueChanged;
						scrollbar = scrollbarProvider;
						scrollbar.ValueChanged += OnScrollValueChanged;
					} else if (e.ChangeType == StructureChangeType.ChildRemoved
					           || e.ChangeType == StructureChangeType.ChildrenBulkRemoved) {
						if (scrollbar != null)
							scrollbar.ValueChanged -= OnScrollValueChanged;
						scrollbar = null;
					}
				}
			}
			RaiseAutomationPropertyChangedEvent ();
		}

		private void OnScrollValueChanged (object sender, EventArgs args)
		{
			RaiseAutomationPropertyChangedEvent ();
		}
		
		#endregion

		#region Private fields

		private SWF.ScrollBar scrollbar;
		private SWF.ComboBox combobox;
		private ComboBoxProvider.ComboBoxListBoxProvider listboxProvider;

		#endregion
	}
}
