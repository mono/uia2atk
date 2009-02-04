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
			else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
				return Catalog.GetString ("tool bar");
			else
				return base.GetProviderPropertyValue (propertyId);
		}
		
		#endregion
		
		#region FragmentRootControlProvider: Specializations
		
		public override void InitializeChildControlStructure ()
		{	
			toolBar.Buttons.UIACollectionChanged +=
				new CollectionChangeEventHandler (OnCollectionChanged);
			
			for (int i = 0; i < toolBar.Buttons.Count; ++i) {
				var button = ProviderFactory.GetProvider (toolBar.Buttons [i]);
				OnNavigationChildAdded (false, (FragmentControlProvider)button);
			}
		}
		
		public override void FinalizeChildControlStructure ()
		{
			toolBar.Buttons.UIACollectionChanged -=
				new CollectionChangeEventHandler (OnCollectionChanged);
			
			foreach (ToolBarButtonProvider button in toolBar.Buttons)
				OnNavigationChildRemoved (false, button);
			OnNavigationChildrenCleared (false);
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
				OnNavigationChildAdded (true, button);
			} else if (e.Action == CollectionChangeAction.Remove) {
				ToolBarButtonProvider button = RemoveButtonAt ((int) e.Element);
				OnNavigationChildRemoved (true, button);
			} else if (e.Action == CollectionChangeAction.Refresh) {
				ClearButtonsCollection ();
				OnNavigationChildrenCleared (true);
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
				
				SetBehavior (InvokePatternIdentifiers.Pattern,
				             new ToolBarButtonInvokeProviderBehavior (this));
				SetEvent (ProviderEventType.AutomationElementHasKeyboardFocusProperty,
				          new ETB.AutomationHasKeyboardFocusPropertyEvent (this));
			}
			
			#endregion
			
			#region Public Methods
			
			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
					return ControlType.MenuItem.Id;
				else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
					return true;
				else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
					return Catalog.GetString ("menu item");
				else if (propertyId == AutomationElementIdentifiers.LabeledByProperty.Id)
					return null;
				else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
					return toolBarButton.Text;
				else if (propertyId == AutomationElementIdentifiers.HasKeyboardFocusProperty.Id) {
					try {
						return Helper.GetPrivateProperty<ToolBarButton, bool> (typeof(ToolBarButton),
						                                                       toolBarButton,
						                                                       "UIAHasFocus");
					} catch (NotSupportedException) { }
					return base.GetProviderPropertyValue (propertyId);
				}
				else
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
			
			#endregion
		}
		
		#endregion
	}
}
