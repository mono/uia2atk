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
//	Neville Gao <nevillegao@gmail.com>
// 

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms.Behaviors;
using Mono.UIAutomation.Winforms.Behaviors.ToolBar;

namespace Mono.UIAutomation.Winforms
{
	internal class ToolBarProvider : FragmentRootControlProvider
	{
		#region Constructor

		public ToolBarProvider (ToolBar toolBar) : base (toolBar)
		{
			this.toolBar = toolBar;
			buttons = new List<ToolBarButtonProvider> ();
		}

		#endregion
		
		#region SimpleControlProvider: Specializations
		
		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.ToolBar.Id;
			else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
				return "tool bar";
			else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
				return toolBar.Text;
			else
				return base.GetProviderPropertyValue (propertyId);
		}
		
		#endregion
		
		#region FragmentRootControlProvider: Specializations
		
		public override void InitializeChildControlStructure ()
		{	
			try {
				Helper.AddPrivateEvent (typeof (ToolBar.ToolBarButtonCollection),
				                        toolBar.Buttons,
				                        "UIACollectionChanged",
				                        this,
				                        "OnCollectionChanged");
			} catch (NotSupportedException) { }
			
			for (int i = 0; i < toolBar.Buttons.Count; ++i) {
				ToolBarButtonProvider button = GetButtonProvider (i);
				OnNavigationChildAdded (false, button);
			}
		}
		
		public override void FinalizeChildControlStructure ()
		{
			try {
				Helper.RemovePrivateEvent (typeof (ToolBar.ToolBarButtonCollection),
				                           toolBar.Buttons,
				                           "UIACollectionChanged",
				                           this,
				                           "OnCollectionChanged");
			} catch (NotSupportedException) { }
			
			foreach (ToolBarButtonProvider button in buttons)
				OnNavigationChildRemoved (false, button);
			OnNavigationChildrenCleared (false);
		}
		
		#endregion
		
		#region Public Methods
		
		public ToolBarButtonProvider GetButtonProvider (int index)
		{
			ToolBarButtonProvider button = null;
			
			if (index < 0 || index >= toolBar.Buttons.Count)
				return null;
			else if (index >= buttons.Count) {
				for (int loop = buttons.Count - 1; loop < index; ++loop) {
					button = new ToolBarButtonProvider (toolBar.Buttons [index]);
					buttons.Add (button);
					button.Initialize ();
				}
			}
			
			return buttons [index];
		}
		
		public ToolBarButtonProvider RemoveButtonAt (int index)
		{
			ToolBarButtonProvider button = null;
			
			if (index < buttons.Count) {
				button = buttons [index];
				buttons.RemoveAt (index);
				button.Terminate ();
			}
			
			return button;
		}
		
		public void ClearButtonsCollection ()
		{
			while (buttons.Count > 0) {
				RemoveButtonAt (buttons.Count - 1);
			}
		}
		
		#endregion
		
		#region Private Methods

		#pragma warning disable 169
		
		private void OnCollectionChanged (object sender, CollectionChangeEventArgs e)
		{
			if (e.Action == CollectionChangeAction.Add) {
				ToolBarButtonProvider button = GetButtonProvider ((int) e.Element);
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
		private List<ToolBarButtonProvider> buttons;
		
		#endregion
		
		#region Internal Class: ToolBarButton Provider
		
		internal class ToolBarButtonProvider : FragmentControlProvider
		{
			#region Constructor

			public ToolBarButtonProvider (ToolBarButton toolBarButton) : base (toolBarButton)
			{
				this.toolBarButton = toolBarButton;
			}
		
			#endregion
			
			#region SimpleControlProvider: Specializations
		
			public override void Initialize ()
			{
				base.Initialize ();
				
				SetBehavior (InvokePatternIdentifiers.Pattern,
				             new ToolBarButtonInvokeProviderBehavior (this));
			}
			
			#endregion
			
			#region Public Methods
		
			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
					return ControlType.MenuItem.Id;
				else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
					return "menu item";
				else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
					return toolBarButton.Text;
				else
					return base.GetProviderPropertyValue (propertyId);
			}
		
			#endregion
			
			#region Private Fields
		
			private ToolBarButton toolBarButton;
		
			#endregion
		}
		
		#endregion
	}
}
