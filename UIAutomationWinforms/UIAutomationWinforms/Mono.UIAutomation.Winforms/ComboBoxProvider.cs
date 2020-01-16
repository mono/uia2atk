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
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Automation;
using System.Collections.Generic;
using SWF = System.Windows.Forms;
using System.Windows.Automation.Provider;
using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

using Mono.Unix;
using Mono.UIAutomation.Services;
using Mono.UIAutomation.Winforms.Behaviors;
using Mono.UIAutomation.Winforms.Behaviors.ListItem;
using Mono.UIAutomation.Winforms.Behaviors.ComboBox;
using Mono.UIAutomation.Winforms.Navigation;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.ComboBox;

namespace Mono.UIAutomation.Winforms
{

	[MapsComponent (typeof (SWF.ComboBox))]
	internal class ComboBoxProvider : FragmentRootControlProvider
	{

		#region Constructor
		
		public ComboBoxProvider (SWF.ComboBox combobox) : base (combobox)
		{
			comboboxControl = combobox;
			comboboxControl.DropDownStyleChanged += OnDropDownStyleChanged;

			listboxProvider = new ComboBoxProvider.ComboBoxListBoxProvider (comboboxControl, this);
		}
		
		#endregion

		#region Public Properties

		public ListProvider ListProvider {
			get { return listboxProvider; }
		}

		#endregion
		
		#region SimpleControlProvider: Specializations

		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AEIds.ControlTypeProperty.Id)
				return ControlType.ComboBox.Id;
			else if (propertyId == AEIds.IsKeyboardFocusableProperty.Id)
				return true;
			else if (propertyId == AEIds.HasKeyboardFocusProperty.Id
				 && comboboxControl.DropDownStyle == SWF.ComboBoxStyle.DropDown)
				return false;
			else
				return base.GetProviderPropertyValue (propertyId);
		}

		public override void Initialize ()
		{
			base.Initialize ();

			SetBehavior (SelectionPatternIdentifiers.Pattern,
			             new SelectionProviderBehavior (this));
			UpdateBehaviors (false);
		}
		
		public override void Terminate ()
		{
			base.Terminate ();
			
			comboboxControl.DropDownStyleChanged -= OnDropDownStyleChanged;
		}	

		#endregion

		#region FragmentRootControlProvider: Specializations
		
		public override IRawElementProviderFragment GetFocus ()
		{
			return listboxProvider.GetItemProviderFrom (listboxProvider, comboboxControl.SelectedItem);
		}
		
		public override IRawElementProviderFragment ElementProviderFromPoint (double x, double y)
		{
			//TODO
			Log.Warn ("ListBoxProvider:ElementProviderFromPoint not implemented");
			return null;
		}

		#endregion
		
		#region ListProvider: Specializations
		
		public override void InitializeChildControlStructure ()
		{
			listboxProvider.Initialize ();
			AddChildProvider (listboxProvider);
			UpdateBehaviors (true);
		}
		
		public override void FinalizeChildControlStructure ()
		{
			if (buttonProvider != null) {
				RemoveChildProvider (buttonProvider);
				buttonProvider.Terminate ();
				buttonProvider = null;
			}
			if (listboxProvider != null) {
				RemoveChildProvider (listboxProvider);
				listboxProvider.Terminate ();
				listboxProvider = null;
			}
			if (textboxProvider != null) {
				RemoveChildProvider (textboxProvider);
				textboxProvider.Terminate ();
				textboxProvider = null;
			}
		}

		#endregion

		#region Private Methods
		
		private void OnDropDownStyleChanged (object sender, EventArgs args)
		{
			UpdateBehaviors (true);
		}
		
		private void UpdateBehaviors (bool updateAssociatedChildren) 
		{
			if (comboboxControl.DropDownStyle == SWF.ComboBoxStyle.Simple) {
				SetBehavior (ExpandCollapsePatternIdentifiers.Pattern, 
				             null);
				if (!IsBehaviorEnabled (ValuePatternIdentifiers.Pattern))
					SetBehavior (ValuePatternIdentifiers.Pattern,
					             new ValueProviderBehavior (this));

				SetEvent (ProviderEventType.AutomationElementHasKeyboardFocusProperty,
					  new AutomationHasKeyboardFocusPropertyEvent (this));

				if (updateAssociatedChildren) {
					TerminateButtonProvider ();
					InitializeEditProvider ();
				}
			} else if (comboboxControl.DropDownStyle == SWF.ComboBoxStyle.DropDown) {
				if (!IsBehaviorEnabled (ExpandCollapsePatternIdentifiers.Pattern))
					SetBehavior (ExpandCollapsePatternIdentifiers.Pattern,
					             new ExpandCollapseProviderBehavior (this));
				if (!IsBehaviorEnabled (ValuePatternIdentifiers.Pattern))
					SetBehavior (ValuePatternIdentifiers.Pattern,
					             new ValueProviderBehavior (this));

				SetEvent (ProviderEventType.AutomationElementHasKeyboardFocusProperty,
					  null);

				if (updateAssociatedChildren) {
					InitializeButtonProvider ();
					InitializeEditProvider ();
				}
			} else if (comboboxControl.DropDownStyle == SWF.ComboBoxStyle.DropDownList) {
				if (!IsBehaviorEnabled (ExpandCollapsePatternIdentifiers.Pattern))
					SetBehavior (ExpandCollapsePatternIdentifiers.Pattern,
					             new ExpandCollapseProviderBehavior (this));
				SetBehavior (ValuePatternIdentifiers.Pattern, 
				             null);

				SetEvent (ProviderEventType.AutomationElementHasKeyboardFocusProperty,
					  new AutomationHasKeyboardFocusPropertyEvent (this));

				if (updateAssociatedChildren) {
					InitializeButtonProvider ();
					TerminateEditProvider ();
				}
			} 
		}
		
		private void InitializeEditProvider ()
		{
			if (textboxProvider == null) {
				textboxProvider = new ComboBoxTextBoxProvider (comboboxControl.UIATextBox, this);
				textboxProvider.Initialize ();

				AddChildProvider (textboxProvider);
			}
		}

		private void TerminateEditProvider ()
		{
			if (textboxProvider != null) {
				RemoveChildProvider (textboxProvider);
				textboxProvider.Terminate ();
				textboxProvider = null;
			}
		}
		
		private void InitializeButtonProvider ()
		{
			if (buttonProvider == null) {
				buttonProvider = new ComboBoxProvider.ComboBoxButtonProvider (comboboxControl,
				                                                              this);
				buttonProvider.Initialize ();
				AddChildProvider (buttonProvider);
			}
		}
		
		private void TerminateButtonProvider ()
		{
			if (buttonProvider != null) {
				RemoveChildProvider (buttonProvider);
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
		
		internal class ComboBoxListBoxProvider : ListProvider
		{
			
			public ComboBoxListBoxProvider (SWF.ComboBox control, 
			                                ComboBoxProvider provider)
				: base (control)
			{
				comboboxControl = control;
				comboboxProvider = provider;

				// To keep track of the internal Control that represents the ListBox
				comboboxControl.DropDownStyleChanged += OnDropDownStyleChanged;
				comboboxControl.DropDown += OnDropDown;
				comboboxControl.DropDownClosed += OnDropDownClosed;
			}
			
			public override IRawElementProviderFragmentRoot FragmentRoot {
				get { return comboboxProvider; }
			}
	
			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
					return ControlType.List.Id;
				else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
					return true;
				else if (propertyId == AutomationElementIdentifiers.HasKeyboardFocusProperty.Id)
					return true;
				else if (propertyId == AutomationElementIdentifiers.IsScrollPatternAvailableProperty.Id)
					return IsBehaviorEnabled (ScrollPatternIdentifiers.Pattern);
				else if (propertyId == AutomationElementIdentifiers.IsTablePatternAvailableProperty.Id)
					return false;
				else if (propertyId == AutomationElementIdentifiers.IsOffscreenProperty.Id) {
					if (comboboxControl.DropDownStyle == SWF.ComboBoxStyle.Simple)
						return false;
					
					IExpandCollapseProvider pattern 
						= comboboxProvider.GetPatternProvider (ExpandCollapsePatternIdentifiers.Pattern.Id) as IExpandCollapseProvider;
					return pattern != null && pattern.ExpandCollapseState == ExpandCollapseState.Collapsed;
				} else if (propertyId == AutomationElementIdentifiers.NativeWindowHandleProperty.Id)
					//Not like Windows, ComboBox.ListBox is a "real control" and has its own handle.
					//On mono ComboxBox.ListBox dosen't has native window handle.
					return null;
				else
					return base.GetProviderPropertyValue (propertyId);
			}

			protected override Rect BoundingRectangleProperty {
				get {
					//We try to get internal UIAComboListBox in SWF.ComboBox if returns null we 
					//use the SWF.ComboBox bounds
					SWF.Control listboxControl = ListBoxControl;
					if (listboxControl == null)
						return (Rect) comboboxProvider.GetProviderPropertyValue (AutomationElementIdentifiers.BoundingRectangleProperty.Id);
					else
						return Helper.GetControlScreenBounds (listboxControl.Bounds, listboxControl);
				}
			}

			public override int SelectedItemsCount {
				get { return comboboxControl.SelectedIndex == -1 ? 0 : 1; }
			}
			
			public override int ItemsCount {
				get { return comboboxControl.Items.Count; }
			}

			public override int IndexOfObjectItem (object objectItem)
			{
				return comboboxControl.Items.IndexOf (objectItem);
			}	

			public override IRawElementProviderSimple[] GetSelectedItems ()
			{
				if (comboboxControl == null || comboboxControl.SelectedIndex == -1)
					return new IRawElementProviderSimple [0];
				else
					return new IRawElementProviderSimple [] { GetItemProviderFrom (this, comboboxControl.SelectedItem) };
			}

			public override void SelectItem (ListItemProvider item)
			{
				if (ContainsItem (item) == true) {
					comboboxControl.SelectedIndex = item.Index;

					// Raise SelectionChangeCommitted event
					// on the ComboBox. Since we are simulating
					// a user interacting with the control,
					// this event is expected.
					//
					// NOTE: This is a protected method that
					//       is a part of the documented
					//       API for SWF.ComboBox.
					MethodInfo onSelectionChangeCommitted =
						comboboxControl.GetType ().GetMethod ("OnSelectionChangeCommitted",
						                                      BindingFlags.NonPublic | BindingFlags.Instance);
					if (onSelectionChangeCommitted != null) {
						onSelectionChangeCommitted.Invoke (comboboxControl,
							                                   new object [] {EventArgs.Empty});
					}
				}
			}
	
			public override void UnselectItem (ListItemProvider item)
			{
				if (ContainsItem (item) == true)
					comboboxControl.SelectedIndex = -1;
			}
			
			public override bool IsItemSelected (ListItemProvider item)
			{
				return ContainsItem (item) == false 
					? false : item.Index == comboboxControl.SelectedIndex;
			}

			public override void InitializeChildControlStructure ()
			{
				base.InitializeChildControlStructure ();
			}

			public override void FinalizeChildControlStructure ()
			{
				base.FinalizeChildControlStructure ();
			}

			public override void ScrollItemIntoView (ListItemProvider item) 
			{ 
				//FIXME: Implement
				Log.Warn ("ComboBoxListBoxProvider not implemented");
			}

			public override object GetItemPropertyValue (ListItemProvider item,
			                                             int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
					return comboboxControl.GetItemText (item.ObjectItem);

				int topItem = -1;
				if (ListBoxControl != null)
					topItem = ListBoxControl.UIATopItem;

				if (ContainsItem (item) == false)
					return null;
				else if (propertyId == AutomationElementIdentifiers.HasKeyboardFocusProperty.Id)
					return comboboxControl.Focused && item.Index == comboboxControl.SelectedIndex;
				else if (propertyId == AutomationElementIdentifiers.BoundingRectangleProperty.Id) {
					//FIXME: We need to improve this
					int index = item.Index;
					System.Drawing.Rectangle rectangle = System.Drawing.Rectangle.Empty;
					System.Drawing.Rectangle bounds = System.Drawing.Rectangle.Empty;

					if (ListBoxControl == null)
						bounds = comboboxControl.Bounds;
					else
						bounds = ListBoxControl.Bounds;

					int itemHeight = comboboxControl.GetItemHeight (0);// TODO: always true?
					rectangle.Height = comboboxControl.GetItemHeight (index);
					rectangle.Width = bounds.Width;
					rectangle.X = bounds.X;
					rectangle.Y = bounds.Y + (index * itemHeight) - (topItem * itemHeight);// decreaseY;

					if (ListBoxControl == null)
						return Helper.GetControlScreenBounds (rectangle, comboboxControl);
					else
						return Helper.GetControlScreenBounds (rectangle, ListBoxControl);
				} else if (propertyId == AutomationElementIdentifiers.IsOffscreenProperty.Id) {
					if (comboboxControl.SelectedIndex == item.Index)
						return false;
					
					if (topItem == -1 || !ListBoxControl.Visible)
						return !(comboboxControl.SelectedIndex == item.Index);

					int lastItem = ListBoxControl.UIALastItem;				
					if ((item.Index >= topItem && item.Index < lastItem) 
					    || (item.Index == lastItem && comboboxControl.Items.Count == lastItem + 1))
						return false;
					else
						return true;
				} else
					return null;
			}
			
			internal override IProviderBehavior GetBehaviorRealization (AutomationPattern behavior)
			{
				if (behavior == SelectionPatternIdentifiers.Pattern)
					return new ListBoxSelectionProviderBehavior (this, 
					                                             comboboxProvider);
				//FIXME: Implement ScrollPattern
				else 
					return null;
			}		
			
			public override IProviderBehavior GetListItemBehaviorRealization (AutomationPattern behavior,
			                                                                  ListItemProvider listItem)
			{
				if (behavior == SelectionItemPatternIdentifiers.Pattern)
					return new ListItemSelectionItemProviderBehavior (listItem);
				else
					return base.GetListItemBehaviorRealization (behavior, listItem);
			}

			public override IConnectable GetListItemEventRealization (ProviderEventType eventType, 
			                                                          ListItemProvider prov)
			{
				if (eventType == ProviderEventType.AutomationElementIsOffscreenProperty)
				    return new ListItemAutomationIsOffscreenPropertyEvent (prov);
				else
					return base.GetListItemEventRealization (eventType, prov);
				//FIXME: Return AutomationIsKeyboardFocusablePropertyEvent
			}

			public SWF.ComboBox.ComboListBox ListBoxControl {
				get { return comboboxControl.UIAComboListBox; }
			}
	
			protected override SWF.ScrollBar HorizontalScrollBar {
				// No Horizontal ScrollBar support
				get { return null; }
			}
	
			protected override SWF.ScrollBar VerticalScrollBar { 
				get {
					if (ListBoxControl == null)
						return null;
					return ListBoxControl.UIAVScrollBar; 
				}
			}

			protected override void InitializeScrollBehaviorObserver ()
			{
				// We implement Scroll logic this way because the internal 
				// control in charge of handling scrolling is disposed when
				// DropDownStyle is changed and is Expanded/Collapsed
				if (observer != null 
				    && (ListBoxControl == null || ListBoxControl != listControl)) {
					observer.ScrollPatternSupportChanged -= OnScrollPatternSupportChanged;
					observer.Terminate ();
					SetBehavior (ScrollPatternIdentifiers.Pattern, null);
				}
				
				if (ListBoxControl != null && ListBoxControl != listControl) {					
					if (ListBoxControl != null) {
						observer = new ScrollBehaviorObserver (this, null, VerticalScrollBar);
						observer.ScrollPatternSupportChanged += OnScrollPatternSupportChanged;
						observer.Initialize ();
						UpdateScrollBehavior (observer);
						ListBoxControl.Disposed += delegate (object obj, EventArgs args) {
							observer.Terminate ();
							SetBehavior (ScrollPatternIdentifiers.Pattern, null);
							listControl = null;
						};
					}
					listControl = ListBoxControl;
				}
			}

			private void OnDropDownStyleChanged (object sender, EventArgs args)
			{
				InitializeScrollBehaviorObserver ();
			}

			private void OnDropDown (object sender, EventArgs args)
			{
				foreach (object objectItem in comboboxControl.Items) {
					ListItemProvider item = GetItemProviderFrom (this, objectItem);
					AddChildProvider (item);
				}
				InitializeScrollBehaviorObserver ();
			}

			private void OnDropDownClosed (object sender, EventArgs args)
			{
				foreach (var childListItemProvider in this.OfType<ListItemProvider> ()) {
					RemoveChildProvider (childListItemProvider); 
				}
				InitializeScrollBehaviorObserver ();
			}

			private SWF.Control listControl;
			private SWF.ComboBox comboboxControl;
			private ComboBoxProvider comboboxProvider;
			private ScrollBehaviorObserver observer;
		}
		
		#endregion

		#region Internal Class: Button provider

		internal class ComboBoxButtonProvider : FragmentControlProvider
		{

			public ComboBoxButtonProvider (SWF.Control control,
			                               ComboBoxProvider provider)
				: base (control)
			{
			}

			public override void Initialize ()
			{
				base.Initialize ();

				SetBehavior (InvokePatternIdentifiers.Pattern,
				             new ButtonInvokeBehavior (this));
			}
	
			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
					return ControlType.Button.Id;
				else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
					return Catalog.GetString ("Drop Down Button");
				else if (propertyId == AutomationElementIdentifiers.LabeledByProperty.Id)
					return null;
				else if (propertyId == AutomationElementIdentifiers.NativeWindowHandleProperty.Id)
					return null;
				else
					return base.GetProviderPropertyValue (propertyId);
			}
		}
		
		#endregion
		
		#region Internal Class: TextBox provider
	
		internal class ComboBoxTextBoxProvider : TextBoxProvider
		{
			public ComboBoxTextBoxProvider (SWF.TextBox textbox,
			                                ComboBoxProvider comboProvider)
				: base (textbox)
			{
				this.comboProvider = comboProvider;
			}

			public override void Initialize ()
			{
				base.Initialize ();

				SetEvent (ProviderEventType.AutomationElementHasKeyboardFocusProperty,
				          new TextBoxAutomationHasKeyboardFocusPropertyEvent (this, comboProvider));
			}

			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id) {
					// This control cannot be selected, but
					// it can still be keyboard focused, so
					// we override it manually.
					return Control.CanFocus;
				}

				return base.GetProviderPropertyValue (propertyId);
			}

			private ComboBoxProvider comboProvider;
		}

		#endregion

	}
}
