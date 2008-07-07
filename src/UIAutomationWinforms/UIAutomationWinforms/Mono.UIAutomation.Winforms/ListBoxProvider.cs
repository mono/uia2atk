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
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;
using System.Windows;
using Mono.UIAutomation.Winforms.Behaviors;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Navigation;

namespace Mono.UIAutomation.Winforms
{

	public class ListBoxProvider : ListProvider
	{
		
#region Constructor 

		public ListBoxProvider (ListBox listbox) : base (listbox)
		{
			listbox_control = listbox;

			SetScrollBehavior (listbox);
			
			Navigation = new ListBoxNavigation (this);
		}

#endregion
		
#region IProviderBehavior Interface

		public override object GetPropertyValue (int propertyId)
		{
			//TODO: Include: HelpTextProperty, LabeledByProperty, NameProperty
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.List.Id;
			//FIXME: According the documentation this is valid, however you can
			//focus only when control.CanFocus, this doesn't make any sense.
			else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
				return true;
			else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
				return "list";
			else
				return base.GetPropertyValue (propertyId);
		}
		
#endregion
		
#region FragmentControlProvider Overrides
		
		public override IRawElementProviderFragment ElementProviderFromPoint (double x, double y)
		{
			throw new NotImplementedException ();
		}

#endregion
		
#region ListProvider Methods
		
		public override bool SupportsMulipleSelection { 
			get { 
				return listbox_control.SelectionMode == SelectionMode.MultiExtended 
					|| listbox_control.SelectionMode == SelectionMode.MultiSimple;
			} 
		}
		
		public override int SelectedItemsCount { 
			get { return listbox_control.SelectedItems.Count; }
		}
		
		public override int ItemsCount {
			get { return listbox_control.Items.Count;  }
		}
		
		public override ListItemProvider[] GetSelectedItemsProviders ()
		{
			ListItemProvider []items;

			if (listbox_control.SelectedIndices.Count == 0)
				return null;
			
			items = new ListItemProvider [listbox_control.SelectedIndices.Count];			
			for (int index = 0; index < items.Length; index++) 
				items [index] = GetItemProvider (listbox_control.SelectedIndices [index]);
			
			return items;
		}
		
		public override string GetItemName (ListItemProvider item)
		{
			return listbox_control.Items [item.Index].ToString ();
		}
		
		public override void SelectItem (ListItemProvider item)
		{
			((ListBox) ListControl).SetSelected (item.Index, true);
		}

		public override void UnselectItem (ListItemProvider item)
		{
			((ListBox) ListControl).SetSelected (item.Index, false);
		}
		
		public override bool IsItemSelected (ListItemProvider item)
		{
			return ((ListBox) ListControl).SelectedIndices.Contains (item.Index);
		}
		
#endregion

#region Private Methods

		private void SetScrollBehavior (ListBox listbox) 
		{
			FieldInfo fieldInfo;
			
			fieldInfo = listbox.GetType ().GetField ("vscrollbar",
			                                         BindingFlags.NonPublic 
			                                         | BindingFlags.Instance);
			vscrollbar = (VScrollBar) fieldInfo.GetValue (listbox);
			vscrollbar.VisibleChanged += new EventHandler (UpdateScrollBehaviorVisible);
			vscrollbar.EnabledChanged += new EventHandler (UpdateScrollBehaviorEnable);
						
			fieldInfo = listbox.GetType ().GetField ("hscrollbar",
			                                         BindingFlags.NonPublic
			                                         | BindingFlags.Instance);
			hscrollbar = (HScrollBar) fieldInfo.GetValue (listbox);
			hscrollbar.VisibleChanged += new EventHandler (UpdateScrollBehaviorVisible);
		}
		
		private void UpdateScrollBehaviorVisible (object sender, EventArgs args)
		{
			
			if (scrollpattern_set == false
			    && (hscrollbar.Visible == true || vscrollbar.Visible == true)) {
				
				if (sender == hscrollbar && listbox_control.ScrollAlwaysVisible)
					return;
				
				SetBehavior (ScrollPatternIdentifiers.Pattern,
				             new ListBoxScrollProviderBehavior (this, hscrollbar,
				                                                vscrollbar));
				scrollpattern_set = true;
			} else if (scrollpattern_set == true
			           && (hscrollbar.Visible == false || vscrollbar.Visible == false)) { 
				if (sender == vscrollbar && listbox_control.ScrollAlwaysVisible 
				    && hscrollbar.Enabled == false) {
					SetBehavior (ScrollPatternIdentifiers.Pattern, null);
					scrollpattern_set = false;
				}
			}
		}
		
		private void UpdateScrollBehaviorEnable (object sender, EventArgs args)
		{
			if (scrollpattern_set == false 
			    && listbox_control.ScrollAlwaysVisible == true) {
				if (hscrollbar.Enabled) {
					SetBehavior (ScrollPatternIdentifiers.Pattern,
					             new ListBoxScrollProviderBehavior (this, hscrollbar,
					                                                vscrollbar));
					scrollpattern_set = true;
				}
			} else if (scrollpattern_set == false 
			           && listbox_control.ScrollAlwaysVisible == true) {
				if (hscrollbar.Enabled == false && hscrollbar.Visible == true) {
					SetBehavior (ScrollPatternIdentifiers.Pattern, null);
					scrollpattern_set = false;
				}
			}
		}
		
#endregion
		
#region Private Fields
		
		private HScrollBar hscrollbar;
		private ListBox listbox_control;
		private bool scrollpattern_set;		
		private VScrollBar vscrollbar;
		
#endregion
	}
}
