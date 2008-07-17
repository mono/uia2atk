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
using Mono.UIAutomation.Winforms.Navigation;

namespace Mono.UIAutomation.Winforms
{

	internal class ComboBoxProvider : ListProvider
	{

		#region Constructor
		
		public ComboBoxProvider (ComboBox combobox) : base (combobox)
		{
			combobox_control = combobox;
			
			combobox_control.DropDownStyleChanged += new EventHandler (OnDropDownStyleChanged);
			
			listbox_provider = new ComboBoxProvider.ComboBoxListBoxProvider (combobox_control,
			                                                                 this);
			listbox_provider.InitializeChildControlStructure ();
		}
		
		#endregion
		
		#region Public Properties

		public override INavigation Navigation {
			get { 
				if (navigation == null)
					navigation = new ComboBoxNavigation (this);

				return navigation;
			}
		}
		
		#endregion
		
		#region Public Methods
		
		public override void Terminate ()
		{
			base.Terminate ();
			
			combobox_control.DropDownStyleChanged -= new EventHandler (OnDropDownStyleChanged);
		}
		
		public FragmentControlProvider GetChildTextBoxProvider ()
		{
			return textbox_provider;
		}
		
		public FragmentRootControlProvider GetChildListBoxProvider ()
		{
			return listbox_provider;
		}
		
		public FragmentControlProvider GetChildButtonProvider ()
		{
			return button_provider;
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

		#endregion		

		#region FragmentRootControlProvider: Specializations
		
		public override IRawElementProviderFragment ElementProviderFromPoint (double x, double y)
		{
			throw new NotImplementedException ();
		}

		#endregion
		
		#region ListProvider: Specializations
		
		public override bool SupportsMultipleSelection { 
			get { return false; } 
		}
		
		public override int SelectedItemsCount { 
			get { return ListControl.SelectedIndex == -1 ? 0 : 1; }
		}
		
		public override int ItemsCount {
			get { return combobox_control.Items.Count; }
		}
		
		public override ListItemProvider[] GetSelectedItemsProviders ()
		{
			if (combobox_control.SelectedIndex == -1)
				return null;
			else
				return new ListItemProvider [] { (ListItemProvider) GetFocus () };
		}

		public override string GetItemName (ListItemProvider item)
		{
			if (ContainsItem (item) == true)
				return (combobox_control).Items [item.Index].ToString ();
			else
				return string.Empty;
		}
		
		public override void SelectItem (ListItemProvider item)
		{
			if (ContainsItem (item) == true)
				ListControl.SelectedIndex = item.Index;
		}

		public override void UnselectItem (ListItemProvider item)
		{
		}
		
		public override bool IsItemSelected (ListItemProvider item)
		{
			return ContainsItem (item) == false 
				? false : item.Index == ListControl.SelectedIndex;
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
		
		public override void FinalizeChildControlStructure ()
		{
			if (button_provider != null) {
				button_provider.Terminate ();
				button_provider = null;
			}
			if (textbox_provider != null) {
				textbox_provider.Terminate ();
				textbox_provider = null;
			}
			if (listbox_provider != null) {
				listbox_provider.Terminate ();
				listbox_provider = null;
			}
		}

		public override void InitializeChildControlStructure ()
		{
			UpdateBehaviors ();
		}

		#endregion

		#region Private Methods
		
		private void OnDropDownStyleChanged (object sender, EventArgs args)
		{
			UpdateBehaviors ();
		}
		
		private void UpdateBehaviors () 
		{
			if (combobox_control.DropDownStyle == ComboBoxStyle.Simple) {
				SetBehavior (ExpandCollapsePatternIdentifiers.Pattern, 
				             null);
				SetBehavior (ValuePatternIdentifiers.Pattern,
				             new ComboBoxValueProviderBehavior (this));
				
				TerminateButtonProvider ();
				InitializeEditProvider ();
			} else if (combobox_control.DropDownStyle == ComboBoxStyle.DropDown) {
				SetBehavior (ExpandCollapsePatternIdentifiers.Pattern,
				             new ComboBoxExpandCollapseProviderBehavior (this));
				SetBehavior (ValuePatternIdentifiers.Pattern,
				             new ComboBoxValueProviderBehavior (this));
				
				InitializeButtonProvider ();
				InitializeEditProvider ();
			} else if (combobox_control.DropDownStyle == ComboBoxStyle.DropDownList) {
				SetBehavior (ExpandCollapsePatternIdentifiers.Pattern,
				             new ComboBoxExpandCollapseProviderBehavior (this));
				SetBehavior (ValuePatternIdentifiers.Pattern, 
				             null);
				
				InitializeButtonProvider ();
				TerminateEditProvider ();
			} 
		}
		
		private void InitializeEditProvider ()
		{
			if (textbox_provider == null) {
				TextBox textbox = (TextBox) Helper.GetPrivateField (combobox_control.GetType (), 
				                                                    combobox_control, 
				                                                    "textbox_ctrl");
				textbox_provider = new TextBoxProvider (textbox);
				Helper.RaiseStructureChangedEvent (StructureChangeType.ChildAdded,
				                                   textbox_provider);
				Helper.RaiseStructureChangedEvent (StructureChangeType.ChildrenInvalidated,
				                                   this);
			}
		}
		
		private void TerminateEditProvider ()
		{
			if (textbox_provider != null) {
				Helper.RaiseStructureChangedEvent (StructureChangeType.ChildRemoved,
				                                   textbox_provider);
				Helper.RaiseStructureChangedEvent (StructureChangeType.ChildrenInvalidated,
				                                   this);
				textbox_provider.Terminate ();
				textbox_provider = null;
			}
		}
		
		private void InitializeButtonProvider ()
		{
			if (button_provider == null) {
				button_provider = new ComboBoxProvider.ComboBoxButtonProvider (combobox_control,
				                                                               this);
				Helper.RaiseStructureChangedEvent (StructureChangeType.ChildAdded,
				                                   button_provider);
				Helper.RaiseStructureChangedEvent (StructureChangeType.ChildrenInvalidated,
				                                   this);
			}
		}
		
		private void TerminateButtonProvider ()
		{
			//TODO: UISpy doesn't report this structure change, according to MSDN we should do so
			if (button_provider != null) {
				Helper.RaiseStructureChangedEvent (StructureChangeType.ChildRemoved,
				                                   button_provider);
				Helper.RaiseStructureChangedEvent (StructureChangeType.ChildrenInvalidated,
				                                   this);
				
				button_provider.Terminate ();
				button_provider = null;
			}
		}
		
		#endregion
			
		#region Private Fields
		
		private ComboBox combobox_control;
		private ComboBoxProvider.ComboBoxButtonProvider button_provider;
		private ComboBoxProvider.ComboBoxListBoxProvider listbox_provider;
		private TextBoxProvider textbox_provider;
		
		#endregion
		
		#region Internal Class: ListBox provider
		
		//TODO: This class missing ScrollBar Navigation.
		internal class ComboBoxListBoxProvider : ListProvider
		{
	
			public ComboBoxListBoxProvider (ComboBox control, 
			                                ComboBoxProvider provider)
				: base (control)
			{
				combobox_control = control;
				combobox_provider = provider;
			}
	
			public override object GetPropertyValue (int propertyId)
			{
				//TODO: Include: HelpTextProperty, LabeledByProperty, NameProperty
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
			
			public new ListItemProvider GetItemProvider (int index)
			{
				return combobox_provider.GetItemProvider (index);
			}
			
			public new int IndexOfItem (ListItemProvider item)
			{
				return combobox_provider.IndexOfItem (item);
			}
			
			public override IRawElementProviderFragment ElementProviderFromPoint (double x, double y)
			{
				return combobox_provider.ElementProviderFromPoint (x, y);
			}
			
			public override int SelectedItemsCount { 
				get { return combobox_provider.SelectedItemsCount; }
			}
			
			public override bool SupportsMultipleSelection { 
				get { return false; }
			}
			
			public override int ItemsCount {
				get { return combobox_provider.ItemsCount; }
			}
			
			public override ListItemProvider[] GetSelectedItemsProviders ()
			{
				return combobox_provider.GetSelectedItemsProviders ();
			}
			
			public override string GetItemName (ListItemProvider item)
			{
				return combobox_provider.GetItemName (item);
			}
			
			public override void SelectItem (ListItemProvider item)
			{
				combobox_provider.SelectItem (item);
			}
	
			public override void UnselectItem (ListItemProvider item)
			{
				combobox_provider.UnselectItem (item);
			}
			
			public override bool IsItemSelected (ListItemProvider item)
			{
				return combobox_provider.IsItemSelected (item);
			}
	
			protected override Type GetTypeOfObjectCollection ()
			{
				return typeof (ComboBox.ObjectCollection);
			}
			
			protected override object GetInstanceOfObjectCollection ()
			{
				return combobox_control.Items;
			}
	
			public override void InitializeChildControlStructure ()
			{
				base.InitializeChildControlStructure ();
	
				for (int index = 0; index < combobox_control.Items.Count; index++)
					GenerateChildAddedEvent (index);
			}

			private ComboBox combobox_control;
			private ComboBoxProvider combobox_provider;

		}
		
		#endregion

		#region Internal Class: Button provider

		internal class ComboBoxButtonProvider : FragmentControlProvider
		{

			public ComboBoxButtonProvider (Control control,
			                               ComboBoxProvider provider)
				: base (control)
			{
				SetBehavior (InvokePatternIdentifiers.Pattern, 
				             new ComboBoxButtonInvokeBehavior (provider));
			}
	
			public override object GetPropertyValue (int propertyId)
			{
				//TODO: i18n?
				if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
					return "Drop Down Button";
				else
					return base.GetPropertyValue (propertyId);
			}
		}
		
		#endregion
		
	}
}
