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
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using SWF = System.Windows.Forms;
using System.Windows;
using Mono.UIAutomation.Winforms.Behaviors;
using Mono.UIAutomation.Winforms.Behaviors.ListItem;
using Mono.UIAutomation.Winforms.Behaviors.ComboBox;
using Mono.UIAutomation.Winforms.Navigation;

namespace Mono.UIAutomation.Winforms
{

	internal class ComboBoxProvider : ListProvider
	{

		#region Constructor
		
		public ComboBoxProvider (SWF.ComboBox combobox) : base (combobox)
		{
			comboboxControl = combobox;
			comboboxControl.DropDownStyleChanged += new EventHandler (OnDropDownStyleChanged);
		}
		
		#endregion
		
		#region SimpleControlProvider: Specializations

		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.ComboBox.Id;
			else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
				return true;
			else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
				return "combo box";
			else
				return base.GetPropertyValue (propertyId);
		}
		
		public override void Terminate ()
		{
			base.Terminate ();
			
			comboboxControl.DropDownStyleChanged -= new EventHandler (OnDropDownStyleChanged);
		}	

		#endregion		

		#region FragmentRootControlProvider: Specializations
		
		public override IRawElementProviderFragment GetFocus ()
		{
			return GetItemProviderAt (comboboxControl.SelectedIndex);
		}		
		
		public override IRawElementProviderFragment ElementProviderFromPoint (double x, double y)
		{
			throw new NotImplementedException ();
		}

		#endregion
		
		#region ListItem: Properties Methods
		
		public override object GetItemPropertyValue (ListItemProvider item,
		                                             int propertyId)
		{
			if (ContainsItem (item) == false)
				return null;
			
			if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
				return comboboxControl.Items [item.Index].ToString ();
			else if (propertyId == AutomationElementIdentifiers.HasKeyboardFocusProperty.Id)
				return comboboxControl.Focused && item.Index == comboboxControl.SelectedIndex;
			else if (propertyId == AutomationElementIdentifiers.BoundingRectangleProperty.Id) {
				//FIXME: We need to improve this
				int loop = 1; //We start in 1 to add the Edit height or the control height depending on the style
				int index = item.Index;
				int totalHeight = 0;
				System.Drawing.Rectangle rectangle = System.Drawing.Rectangle.Empty;
				for (; loop < index; loop++)
					rectangle.Y += comboboxControl.GetItemHeight (loop);
				
				rectangle.Height = comboboxControl.GetItemHeight (index);
				rectangle.Width = comboboxControl.Bounds.Width;
				rectangle.X = comboboxControl.Bounds.X;		
				rectangle.Y += comboboxControl.Bounds.Y;
	
				if (comboboxControl.FindForm () == comboboxControl.Parent)
					rectangle = comboboxControl.TopLevelControl.RectangleToScreen (rectangle);
				else
					rectangle = comboboxControl.Parent.RectangleToScreen (rectangle);
	
				return Helper.RectangleToRect (rectangle);
			} else
				return null;
		}
		
		#endregion
		
		#region ListProvider: Specializations
		
		public override int SelectedItemsCount { 
			get { return comboboxControl.SelectedIndex == -1 ? 0 : 1; }
		}
		
		public override int ItemsCount {
			get { return comboboxControl.Items.Count; }
		}
		
		public override ListItemProvider[] GetSelectedItems ()
		{
			if (comboboxControl == null || comboboxControl.SelectedIndex == -1)
				return null;
			else
				return new ListItemProvider [] { GetItemProviderAt (comboboxControl.SelectedIndex) };
		}
		
		public override void SelectItem (ListItemProvider item)
		{
			if (ContainsItem (item) == true)
				comboboxControl.SelectedIndex = item.Index;
		}

		public override void UnselectItem (ListItemProvider item)
		{
		}
		
		public override bool IsItemSelected (ListItemProvider item)
		{
			return ContainsItem (item) == false 
				? false : item.Index == comboboxControl.SelectedIndex;
		}
		
		protected override Type GetTypeOfObjectCollection ()
		{
			//Doesn't have any list-like children: only Edit, List and Button.
			return null;
		}
		
		protected override object GetInstanceOfObjectCollection ()
		{
			//Doesn't have any list-like children: only Edit, List and Button.
			return null;
		}

		public override void InitializeChildControlStructure ()
		{
			listboxProvider = new ComboBoxProvider.ComboBoxListBoxProvider (comboboxControl,
			                                                                this);			
			OnNavigationChildAdded (false, listboxProvider);
			UpdateBehaviors (false);
		}
		
		public override void FinalizeChildControlStructure ()
		{
			if (buttonProvider != null) {
				OnNavigationChildRemoved (false, buttonProvider);
				buttonProvider.Terminate ();
				buttonProvider = null;
			}
			if (listboxProvider != null) {
				OnNavigationChildRemoved (false, listboxProvider);
				listboxProvider.Terminate ();
				listboxProvider = null;
			}
			if (textboxProvider != null) {
				OnNavigationChildRemoved (false, textboxProvider);
				textboxProvider.Terminate ();
				textboxProvider = null;
			}
		}
		
		protected override ListProvider GetItemsListProvider ()
		{
			return listboxProvider;
		}
		
		public override IConnectable GetListItemHasKeyboardFocusEvent (ListItemProvider provider)
		{
			//return new AutomationIsKeyboardFocusablePropertyEvent (provider);
			//FIXME
			return null;
		}
		
		public override void ScrollItemIntoView (ListItemProvider item)
		{
			if (ContainsItem (item) == true)
				throw new NotImplementedException ();
		}

		#endregion
		
		#region Internal Methods: Get Behaviors
		
		internal override IProviderBehavior GetBehaviorRealization (AutomationPattern behavior)
		{
			if (behavior == SelectionPatternIdentifiers.Pattern)
				return new SelectionProviderBehavior (this);
			else 
				return null;
		}

		internal override IProviderBehavior GetListItemBehaviorRealization (AutomationPattern behavior,
		                                                                    ListItemProvider listItem)
		{
			if (behavior == SelectionItemPatternIdentifiers.Pattern)
				return new ListItemSelectionItemProviderBehavior (listItem);
			else
				return base.GetListItemBehaviorRealization (behavior, listItem);
		}
		
		#endregion

		#region Private Methods
		
		private void OnDropDownStyleChanged (object sender, EventArgs args)
		{
			UpdateBehaviors (true);
		}
		
		private void UpdateBehaviors (bool generateEvent) 
		{
			if (comboboxControl.DropDownStyle == SWF.ComboBoxStyle.Simple) {
				SetBehavior (ExpandCollapsePatternIdentifiers.Pattern, 
				             null);
				SetBehavior (ValuePatternIdentifiers.Pattern,
				             new ValueProviderBehavior (this));
				
				TerminateButtonProvider (generateEvent);
				InitializeEditProvider (generateEvent);
			} else if (comboboxControl.DropDownStyle == SWF.ComboBoxStyle.DropDown) {
				SetBehavior (ExpandCollapsePatternIdentifiers.Pattern,
				             new ExpandCollapseProviderBehavior (this));
				SetBehavior (ValuePatternIdentifiers.Pattern,
				             new ValueProviderBehavior (this));
				
				InitializeButtonProvider (generateEvent);
				InitializeEditProvider (generateEvent);
			} else if (comboboxControl.DropDownStyle == SWF.ComboBoxStyle.DropDownList) {
				SetBehavior (ExpandCollapsePatternIdentifiers.Pattern,
				             new ExpandCollapseProviderBehavior (this));
				SetBehavior (ValuePatternIdentifiers.Pattern, 
				             null);
				
				InitializeButtonProvider (generateEvent);
				TerminateEditProvider (generateEvent);
			} 
		}
		
		private void InitializeEditProvider (bool generateEvent)
		{
			if (textboxProvider == null) {
				SWF.TextBox textbox 
					= Helper.GetPrivateProperty<SWF.ComboBox, SWF.TextBox> (typeof (SWF.ComboBox), 
					                                                        comboboxControl, 
					                                                        "UIATextBox");
				textboxProvider = (TextBoxProvider) ProviderFactory.GetProvider (textbox);
				OnNavigationChildAdded (generateEvent, textboxProvider);
			}
		}
		
		private void TerminateEditProvider (bool generateEvent)
		{
			if (textboxProvider != null) {
				OnNavigationChildRemoved (generateEvent, textboxProvider);
				textboxProvider.Terminate ();
				textboxProvider = null;
			}
		}
		
		private void InitializeButtonProvider (bool generateEvent)
		{
			if (buttonProvider == null) {
				buttonProvider = new ComboBoxProvider.ComboBoxButtonProvider (comboboxControl,
				                                                               this);
				OnNavigationChildAdded (generateEvent, buttonProvider);
			}
		}
		
		private void TerminateButtonProvider (bool generateEvent)
		{
			if (buttonProvider != null) {
				OnNavigationChildRemoved (generateEvent, buttonProvider);
				buttonProvider.Terminate ();
				buttonProvider = null;
			}
		}
		
		#endregion
			
		#region Private Fields
		
		private SWF.ComboBox comboboxControl;
		private ComboBoxProvider.ComboBoxButtonProvider buttonProvider;
		private ComboBoxProvider.ComboBoxListBoxProvider listboxProvider;
		private TextBoxProvider textboxProvider;
		
		#endregion
		
		#region Internal Class: ListBox provider
		
		//TODO: This class missing ScrollBar Navigation.
		internal class ComboBoxListBoxProvider : ListProvider
		{
			
			public ComboBoxListBoxProvider (SWF.ComboBox control, 
			                                ComboBoxProvider provider)
				: base (control)
			{
				comboboxControl = control;
				comboboxProvider = provider;
			}
			
			public override IRawElementProviderFragmentRoot FragmentRoot {
				get { return comboboxProvider; }
			}
	
			public override object GetPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
					return ControlType.List.Id;
				else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
					return true;
				else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
					return "list";
				else if (propertyId == AutomationElementIdentifiers.IsScrollPatternAvailableProperty.Id)
					return IsBehaviorEnabled (ScrollPatternIdentifiers.Pattern);
				else if (propertyId == AutomationElementIdentifiers.IsTablePatternAvailableProperty.Id)
					return false;
				else
					return base.GetPropertyValue (propertyId);
			}
			
			public override IRawElementProviderFragment GetFocus ()
			{
				return comboboxProvider.GetItemProviderAt (comboboxControl.SelectedIndex);
			}			
			
			public override ListItemProvider GetItemProviderAt (int index)
			{
				return comboboxProvider.GetItemProviderAt (index);
			}
					
			public override int IndexOfItem (ListItemProvider item)
			{
				return comboboxProvider.IndexOfItem (item);
			}
			
			public override ListItemProvider RemoveItemAt (int index)
			{
				return comboboxProvider.RemoveItemAt (index);
			}
			
			public override IRawElementProviderFragment ElementProviderFromPoint (double x, double y)
			{
				return comboboxProvider.ElementProviderFromPoint (x, y);
			}
			
			public override int SelectedItemsCount { 
				get { return comboboxProvider.SelectedItemsCount; }
			}
			
			public override int ItemsCount {
				get { return comboboxProvider.ItemsCount; }
			}
			
			public override ListItemProvider[] GetSelectedItems ()
			{
				return comboboxProvider == null ? null : comboboxProvider.GetSelectedItems ();
			}

			public override void SelectItem (ListItemProvider item)
			{
				comboboxProvider.SelectItem (item);
			}
	
			public override void UnselectItem (ListItemProvider item)
			{
				comboboxProvider.UnselectItem (item);
			}
			
			public override bool IsItemSelected (ListItemProvider item)
			{
				return comboboxProvider.IsItemSelected (item);
			}
			
			protected override Type GetTypeOfObjectCollection ()
			{
				return typeof (SWF.ComboBox.ObjectCollection);
			}
			
			protected override object GetInstanceOfObjectCollection ()
			{
				return comboboxControl.Items;
			}
			

			public override IConnectable GetListItemHasKeyboardFocusEvent (ListItemProvider provider)
			{
				//return new AutomationIsKeyboardFocusablePropertyEvent (provider);
				//FIXME
				return null;
			}			
	
			public override void InitializeChildControlStructure ()
			{
				base.InitializeChildControlStructure ();
				
				for (int index = 0; index < comboboxControl.Items.Count; index++) {
					ListItemProvider item = GetItemProviderAt (index);
					OnNavigationChildAdded (false, item);
				}
			}
			
			public override void FinalizeChildControlStructure ()
			{
				base.FinalizeChildControlStructure ();
				
				for (int index = 0; index < comboboxControl.Items.Count; index++) {
					ListItemProvider item = GetItemProviderAt (index);
					OnNavigationChildAdded (false, item);
				}
			}

			public override void ScrollItemIntoView (ListItemProvider item) 
			{ 
				//FIXME: Implement
				throw new NotImplementedException ();
			}

			public override object GetItemPropertyValue (ListItemProvider item,
			                                             int propertyId)
			{
				return comboboxProvider.GetItemPropertyValue (item, propertyId);
			}

			internal override IProviderBehavior GetBehaviorRealization (AutomationPattern behavior)
			{
				if (behavior == SelectionPatternIdentifiers.Pattern)
					return new SelectionProviderBehavior (this);
				else 
					return null;
			}		
			
			internal override IProviderBehavior GetListItemBehaviorRealization (AutomationPattern behavior,
			                                                                    ListItemProvider listItem)
			{
				if (behavior == SelectionItemPatternIdentifiers.Pattern)
					return new ListItemSelectionItemProviderBehavior (listItem);
				else
					return base.GetListItemBehaviorRealization (behavior, listItem);
			}

			private SWF.ComboBox comboboxControl;
			private ComboBoxProvider comboboxProvider;

		}
		
		#endregion

		#region Internal Class: Button provider

		internal class ComboBoxButtonProvider : FragmentControlProvider
		{

			public ComboBoxButtonProvider (SWF.Control control,
			                               ComboBoxProvider provider)
				: base (control)
			{
				SetBehavior (InvokePatternIdentifiers.Pattern, 
				             new ButtonInvokeBehavior (provider));
			}
	
			public override object GetPropertyValue (int propertyId)
			{
				//TODO: i18n?
				if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
					return ControlType.Button.Id;
				else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
					return "Drop Down Button";
				else
					return base.GetPropertyValue (propertyId);
			}
		}
		
		#endregion
		
	}
}
