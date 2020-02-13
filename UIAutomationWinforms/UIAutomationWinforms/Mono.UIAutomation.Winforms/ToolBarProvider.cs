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
// Copyright (c) 2008,2009 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//	Neville Gao <nevillegao@gmail.com>
//	Andrés G. Aragoneses <aaragoneses@novell.com>
// 

using System;
using Mono.Unix;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms.Behaviors;
using Mono.UIAutomation.Winforms.Behaviors.ToolBar;
using Mono.UIAutomation.Winforms.Events;
using ETB = Mono.UIAutomation.Winforms.Events.ToolBarButton;

namespace Mono.UIAutomation.Winforms
{
	[MapsComponent (typeof (ToolBar))]
	internal class ToolBarProvider : FragmentRootControlProvider
	{
		#region Constructor

		public ToolBarProvider (ToolBar toolBar) : base (toolBar)
		{
			this.toolBar = toolBar;
		}

		#endregion
		
		#region SimpleControlProvider: Specializations
		
		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.ToolBar.Id;
			else if (propertyId == AutomationElementIdentifiers.LabeledByProperty.Id)
				return null;
			else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
				return false;
			else if (propertyId == AutomationElementIdentifiers.IsContentElementProperty.Id)
				return false;
			else
				return base.GetProviderPropertyValue (propertyId);
		}
		
		#endregion
		
		#region FragmentRootControlProvider: Specializations
		
		protected override void InitializeChildControlStructure ()
		{	
			base.InitializeChildControlStructure ();

			toolBar.Buttons.UIACollectionChanged +=
				new CollectionChangeEventHandler (OnCollectionChanged);
			
			for (int i = 0; i < toolBar.Buttons.Count; ++i) {
				var button = ProviderFactory.GetProvider (toolBar.Buttons [i]);
				AddChildProvider ((FragmentControlProvider)button);
			}
		}
		
		protected override void FinalizeChildControlStructure()
		{
			toolBar.Buttons.UIACollectionChanged -=
				new CollectionChangeEventHandler (OnCollectionChanged);
			
			for (int index = 0; index < toolBar.Buttons.Count; index++) {
				ToolBarButtonProvider buttonProvider 
					= (ToolBarButtonProvider) ProviderFactory.FindProvider (toolBar.Buttons [index]);
				RemoveChildProvider (buttonProvider);
			}
			OnNavigationChildrenCleared ();

			base.FinalizeChildControlStructure ();
		}
		
		#endregion
		
		#region Public Methods
		
		public ToolBarButtonProvider RemoveButtonAt (int index)
		{
			ToolBarButtonProvider button = null;
			
			if (index < toolBar.Buttons.Count) {
				button = (ToolBarButtonProvider)
					ProviderFactory.GetProvider (toolBar.Buttons [index]);
				ProviderFactory.ReleaseProvider (toolBar.Buttons [index]);
			}
			
			return button;
		}
		
		public void ClearButtonsCollection ()
		{
			while (toolBar.Buttons.Count > 0) {
				RemoveButtonAt (toolBar.Buttons.Count - 1);
			}
		}
		
		#endregion
		
		#region Private Methods

		#pragma warning disable 169
		
		private void OnCollectionChanged (object sender, CollectionChangeEventArgs e)
		{
			if (e.Action == CollectionChangeAction.Add) {
				ToolBarButtonProvider button = (ToolBarButtonProvider)
					ProviderFactory.GetProvider (toolBar.Buttons [(int) e.Element]);
				AddChildProvider (button);
			} else if (e.Action == CollectionChangeAction.Remove) {
				ToolBarButtonProvider button = RemoveButtonAt ((int) e.Element);
				RemoveChildProvider (button);
			} else if (e.Action == CollectionChangeAction.Refresh) {
				ClearButtonsCollection ();
				OnNavigationChildrenCleared ();
			}
		}
		
		#pragma warning restore 169
		
		#endregion
		
		#region Private Fields
		
		private ToolBar toolBar;
		
		#endregion
		
		#region Internal Class: ToolBarButton Provider

		[MapsComponent (typeof (ToolBarButton))]
		internal class ToolBarButtonProvider : FragmentControlProvider
		{
			#region Constructor

			public ToolBarButtonProvider (ToolBarButton toolBarButton) : base (toolBarButton)
			{
				this.toolBarButton = toolBarButton;
				this.style = toolBarButton.Style;
			}
		
			#endregion

			public override IRawElementProviderFragmentRoot FragmentRoot {
				get {
					return (IRawElementProviderFragmentRoot) ProviderFactory.FindProvider (toolBarButton.Parent);
				}
			}
			
			#region SimpleControlProvider: Specializations
		
			public override void Initialize ()
			{
				base.Initialize ();
				
				SetBehavior (EmbeddedImagePatternIdentifiers.Pattern, 
				             new ToolBarButtonEmbeddedImageProviderBehavior (this));
				SetEvent (ProviderEventType.AutomationElementHasKeyboardFocusProperty,
				          new ETB.AutomationHasKeyboardFocusPropertyEvent (this));
				SetEvent (ProviderEventType.AutomationElementNameProperty,
				          new ETB.AutomationNamePropertyEvent (this));
				SetEvent (ProviderEventType.AutomationElementIsEnabledProperty, 
				          new ETB.AutomationIsEnabledPropertyEvent (this));

				try {
					//TODO: remove reflection when we don't depend on Mono 2.4 (and the pragma below):
					
					//toolBarButton.UIAStyleChanged += new EventHandler (OnDropDownMenuChanged);
					Helper.AddPrivateEvent (typeof (ToolBarButton), toolBarButton, "UIAStyleChanged",
					                        this, "OnStyleChanged");
				} catch (NotSupportedException) { }
				SetStyleBehaviors ();
			}

			private void SetStyleBehaviors ()
			{
				if (style == ToolBarButtonStyle.DropDownButton) {
					SetBehavior (InvokePatternIdentifiers.Pattern,
					             new ToolBarButtonInvokeProviderBehavior (this));
					SetBehavior (ExpandCollapsePatternIdentifiers.Pattern,
					             new ToolBarButtonExpandCollapseProviderBehavior (this));
					SetBehavior (TogglePatternIdentifiers.Pattern, null);
				} else if (style == ToolBarButtonStyle.PushButton) {
					SetBehavior (InvokePatternIdentifiers.Pattern,
					             new ToolBarButtonInvokeProviderBehavior (this));
					SetBehavior (ExpandCollapsePatternIdentifiers.Pattern, null);
					SetBehavior (TogglePatternIdentifiers.Pattern, null);
				} else if (style == ToolBarButtonStyle.ToggleButton) {
					SetBehavior (InvokePatternIdentifiers.Pattern, null);
					SetBehavior (ExpandCollapsePatternIdentifiers.Pattern, null);
					SetBehavior (TogglePatternIdentifiers.Pattern,
					             new ToolBarButtonToggleProviderBehavior (this));
				} else {
					SetBehavior (InvokePatternIdentifiers.Pattern, null);
					SetBehavior (ExpandCollapsePatternIdentifiers.Pattern, null);
					SetBehavior (TogglePatternIdentifiers.Pattern, null);
				}
			}
			
			#endregion
			
			#region Public Methods

			public override void Terminate ()
			{
				base.Terminate ();
				try {
					//TODO: remove reflection when we don't depend on Mono 2.4 (and the pragma below):
					
					//toolBarButton.UIADropDownMenuChanged -= new EventHandler (OnDropDownMenuChanged);
					Helper.RemovePrivateEvent (typeof (ToolBarButton), toolBarButton, "UIAStyleChanged",
					                           this, "OnStyleChanged");
				} catch (NotSupportedException) { }
			}
			
			protected override void InitializeChildControlStructure ()
			{
				try {
					//TODO: remove reflection when we don't depend on Mono 2.4 (and the pragma below):
					//toolBarButton.UIADropDownMenuChanged += new EventHandler (OnDropDownMenuChanged);
					Helper.AddPrivateEvent (typeof (ToolBarButton), toolBarButton, "UIADropDownMenuChanged",
					                        this, "OnDropDownMenuChanged");
				} catch (NotSupportedException) { }

				if (style == ToolBarButtonStyle.DropDownButton &&
				    toolBarButton.DropDownMenu != null) {
					ContextMenu menu = toolBarButton.DropDownMenu as ContextMenu;
					if (menu != null) {
						dropDownMenu = menu;
						menu.Popup += OnMenuPopup;
						menu.Collapse += OnMenuCollapse;
					}
				}

			}

			protected override void FinalizeChildControlStructure()
			{
				if (style == ToolBarButtonStyle.DropDownButton &&
				    toolBarButton.DropDownMenu != null) {
					ContextMenu menu = toolBarButton.DropDownMenu as ContextMenu;
					if (menu != null) {
						menu.Popup -= OnMenuPopup;
						menu.Collapse -= OnMenuCollapse;
					}
				}

				try {
					//TODO: remove reflection when we don't depend on Mono 2.4 (and the pragma below):
					
					//toolBarButton.UIADropDownMenuChanged -= new EventHandler (OnDropDownMenuChanged);
					Helper.RemovePrivateEvent (typeof (ToolBarButton), toolBarButton, "UIADropDownMenuChanged",
					                           this, "OnDropDownMenuChanged");
				} catch (NotSupportedException) { }

			}
			
#pragma warning disable 169
			private void OnDropDownMenuChanged (object sender, EventArgs args)
			{
				if (dropDownMenu != null) {
					dropDownMenu.Popup -= OnMenuPopup;
					dropDownMenu.Collapse -= OnMenuCollapse;
				}
				if (toolBarButton.DropDownMenu != null) {
					dropDownMenu = toolBarButton.DropDownMenu as ContextMenu;
					if (dropDownMenu != null) {
						dropDownMenu.Popup += OnMenuPopup;
						dropDownMenu.Collapse += OnMenuCollapse;
					}
				}
			}
			
			private void OnStyleChanged (object sender, EventArgs args)
			{
				SetStyleBehaviors ();
			}
#pragma warning restore 169
			
			private void OnMenuPopup (object sender, EventArgs args)
			{
				var menuProvider =
					ProviderFactory.GetProvider (toolBarButton.DropDownMenu) as FragmentControlProvider;
				if (menuProvider != null)
					AddChildProvider (menuProvider);
			}

			private void OnMenuCollapse (object sender, EventArgs args)
			{
				var menuProvider =
					ProviderFactory.GetProvider (toolBarButton.DropDownMenu) as FragmentControlProvider;
				if (menuProvider != null) {
					menuProvider.Terminate ();
					RemoveChildProvider (menuProvider);
					OnNavigationChildrenCleared ();
				}
			}

			#endregion

			#region Protected Methods
			
			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id) {
					if (style == ToolBarButtonStyle.DropDownButton)
						return ControlType.SplitButton.Id;
					else if (style == ToolBarButtonStyle.PushButton ||
					         style == ToolBarButtonStyle.ToggleButton)
						return ControlType.Button.Id;
					else
						return ControlType.Separator.Id;
				}
				else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
					return false;
				else if (propertyId == AutomationElementIdentifiers.LabeledByProperty.Id)
					return null;
				else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
					return toolBarButton.Text;
				else if (propertyId == AutomationElementIdentifiers.HasKeyboardFocusProperty.Id) {
					try {
						return Helper.GetPrivateProperty<ToolBarButton, bool> (toolBarButton,
						                                                       "UIAHasFocus");
					} catch (NotSupportedException) { }
					return base.GetProviderPropertyValue (propertyId);
				} else if (propertyId == AutomationElementIdentifiers.IsEnabledProperty.Id) {
					return toolBarButton.Enabled;
				} else
					return base.GetProviderPropertyValue (propertyId);
			}

			protected override System.Drawing.Rectangle ScreenBounds {
				get {
					System.Drawing.Rectangle area = toolBarButton.Rectangle;
	
					if (toolBarButton.Parent == null)
						return area;
	
					return toolBarButton.Parent.RectangleToScreen (area);
				}
			}

			#endregion

			#region Private Fields 	 
			
			private ToolBarButton toolBarButton;
			private ToolBarButtonStyle style;
			private ContextMenu dropDownMenu = null;
			
			#endregion
		}
		
		#endregion
	}
}
