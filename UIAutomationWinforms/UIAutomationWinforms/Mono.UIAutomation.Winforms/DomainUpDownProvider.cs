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
//      Brad Taylor <brad@getcoded.net>
// 

using System;
using System.Windows;
using System.Windows.Forms;

using System.Windows.Automation;
using System.Collections.Generic;
using AEIds = System.Windows.Automation.AutomationElementIdentifiers;
using System.Windows.Automation.Provider;

using Mono.UIAutomation.Winforms.Behaviors;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Behaviors.ListItem;
using Mono.UIAutomation.Winforms.Behaviors.DomainUpDown;

namespace Mono.UIAutomation.Winforms
{
	internal class DomainUpDownProvider
		: UpDownBaseProvider, IListProvider
	{
		private DomainUpDown control;
		private Dictionary<object, ListItemProvider> children
			= new Dictionary<object, ListItemProvider> ();

#region Public Methods
		public DomainUpDownProvider (DomainUpDown upDown) :
			base (upDown)
		{
			this.control = upDown;
		}
		
		public override void Initialize ()
		{
			base.Initialize ();

			SetBehavior (SelectionPatternIdentifiers.Pattern,
			             new SelectionProviderBehavior (this));
			
			control.Items.CollectionChanged += OnCollectionChanged;
		}
#endregion

#region FragmentRootControlProvider Implementation
		public override void InitializeChildControlStructure ()
		{
			base.InitializeChildControlStructure ();
			
			AddCollectionItems ();
		}

		public override void FinalizeChildControlStructure ()
		{
			base.FinalizeChildControlStructure ();
			
			RemoveCollectionItems ();
		}
#endregion

#region IListProvider Implementation
		public int IndexOfObjectItem (object obj)
		{
			return control.Items.IndexOf (obj);
		}

		public IProviderBehavior GetListItemBehaviorRealization (AutomationPattern pattern,
		                                                         ListItemProvider prov)
		{
			if (pattern == SelectionItemPatternIdentifiers.Pattern) {
				return new ListItemSelectionItemProviderBehavior (prov);
			} else if (pattern == ValuePatternIdentifiers.Pattern) {
				return new ListItemValueProviderBehavior (prov);
			}
			return null;
		}

		public IConnectable GetListItemHasKeyboardFocusEvent (ListItemProvider prov)
		{
			// TODO:
			// return new ListItemAutomationHasKeyboardFocusPropertyEvent (prov);
			return null;
		}

		public object GetItemPropertyValue (ListItemProvider prov, int propertyId)
		{
			if (propertyId == AEIds.NameProperty.Id) {
				return (string)prov.ObjectItem;
			} else if (propertyId == AEIds.HasKeyboardFocusProperty.Id) {
				// TODO: What to do here?
				return false;
			} else if (propertyId == AEIds.BoundingRectangleProperty.Id) {
				return Rect.Empty;
			} else if (propertyId == AEIds.IsOffscreenProperty.Id) {
				// XXX: is this the right thing to do?
				return true;
			}
			return null;
		}

		public ToggleState GetItemToggleState (ListItemProvider prov)
		{
			throw new NotSupportedException ();
		}

		public void ToggleItem (ListItemProvider item)
		{
			throw new NotSupportedException ();
		}

		public bool IsItemSelected (ListItemProvider prov)
		{
			int index = control.Items.IndexOf (prov.ObjectItem);
			if (index < 0) {
				return false;
			}

			return (index == control.SelectedIndex);
		}

		public int SelectedItemsCount {
			get { return String.IsNullOrEmpty (control.Text) ? 0 : 1; }
		} 

		public void SelectItem (ListItemProvider prov)
		{
			control.SelectedItem = prov.ObjectItem;
		}

		public void UnselectItem (ListItemProvider prov)
		{
			control.SelectedItem = null;
		}

		public void ScrollItemIntoView (ListItemProvider item)
		{
			throw new NotSupportedException ();
		}
#endregion

		internal IRawElementProviderSimple GetSelectedItem ()
		{
			if (control.SelectedIndex < 0
			    || control.SelectedIndex >= control.Items.Count) {
				return null;
			}

			object val = control.SelectedItem;
			if (!children.ContainsKey (val)) {
				throw new Exception ("We haven't seen the selected item yet!");
			}

			return (IRawElementProviderSimple)children[val];
		}
		
		internal void AddCollectionItems ()
		{
			ListItemProvider prov;
			foreach (object val in control.Items) {
				prov = new ListItemProvider (
					(FragmentRootControlProvider)FragmentRoot,
					(IListProvider)this, null, val
				);
				prov.Initialize ();
				AddChildProvider (true, prov);
				children.Add (val, prov);
			}
		}

		internal void RemoveCollectionItems ()
		{
			foreach (ListItemProvider prov in children.Values) {
				RemoveChildProvider (true, prov);
				prov.Terminate ();
			}
		}

		private void OnCollectionChanged (int index, int size_delta)
		{
			RemoveCollectionItems ();
			AddCollectionItems ();
		}
	}
}
