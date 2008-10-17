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
// 	Neville Gao <nevillegao@gmail.com>
// 

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms.Behaviors;
using Mono.UIAutomation.Winforms.Behaviors.StatusBar;
using Mono.UIAutomation.Winforms.Navigation;

namespace Mono.UIAutomation.Winforms
{
	internal class StatusBarProvider : FragmentRootControlProvider
	{
		#region Constructor

		public StatusBarProvider (StatusBar statusBar) : base (statusBar)
		{
			this.statusBar = statusBar;
			panels = new List<StatusBarPanelProvider> ();
			
			SetBehavior (GridPatternIdentifiers.Pattern,
			             new GridProviderBehavior (this));
		}

		#endregion
		
		#region FragmentRootControlProvider: Specializations
		
		public override void InitializeChildControlStructure ()
		{	
			try {
				Helper.AddPrivateEvent (typeof (StatusBar.StatusBarPanelCollection),
				                        statusBar.Panels,
				                        "UIACollectionChanged",
				                        this,
				                        "OnCollectionChanged");
			} catch (NotSupportedException) { }
		
			for (int i = 0; i < statusBar.Panels.Count; ++i) {
				StatusBarPanelProvider panel = GetPanelProvider (i);
				OnNavigationChildAdded (false, panel);
			}
		}
		
		public override void FinalizeChildControlStructure ()
		{
			try {
				Helper.RemovePrivateEvent (typeof (StatusBar.StatusBarPanelCollection),
				                           statusBar.Panels,
				                           "UIACollectionChanged",
				                           this,
				                           "OnCollectionChanged");
			} catch (NotSupportedException) { }
			
			foreach (StatusBarPanelProvider panel in panels)
				OnNavigationChildRemoved (false, panel);
			OnNavigationChildrenCleared (false);
		}

		#endregion
		
		#region Public Methods
		
		public StatusBarPanelProvider GetPanelProvider (int index)
		{
			StatusBarPanelProvider panel = null;
			
			if (index < 0 || index >= statusBar.Panels.Count)
				return null;
			else if (index >= panels.Count) {
				for (int loop = panels.Count - 1; loop < index; ++loop) {
					panel = new StatusBarPanelProvider (statusBar.Panels [index]);
					panels.Add (panel);
					panel.InitializeEvents ();
				}
			}
			
			return panels [index];
		}
		
		public StatusBarPanelProvider RemovePanelAt (int index)
		{
			StatusBarPanelProvider panel = null;
			
			if (index < panels.Count) {
				panel = panels [index];
				panels.RemoveAt (index);
				panel.Terminate ();
			}
			
			return panel;
		}
		
		public void ClearPanelsCollection ()
		{
			while (panels.Count > 0) {
				RemovePanelAt (panels.Count - 1);
			}
		}
		
		#endregion
		
		#region Private Methods

		#pragma warning disable 169
		
		private void OnCollectionChanged (object sender, CollectionChangeEventArgs e)
		{
			if (e.Action == CollectionChangeAction.Add) {
				StatusBarPanelProvider panel = GetPanelProvider ((int) e.Element);
				OnNavigationChildAdded (true, panel);
			} else if (e.Action == CollectionChangeAction.Remove) {
				StatusBarPanelProvider panel = RemovePanelAt ((int) e.Element);
				OnNavigationChildRemoved (true, panel);
			} else if (e.Action == CollectionChangeAction.Refresh) {
				ClearPanelsCollection ();
				OnNavigationChildrenCleared (true);
			}
		}
		
		#pragma warning restore 169
		
		#endregion
		
		#region SimpleControlProvider: Specializations
		
		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.StatusBar.Id;
			else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
				return "status bar";
			else if (propertyId == AutomationElementIdentifiers.LabeledByProperty.Id)
				return null;
			else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
				return statusBar.Text;
			else
				return base.GetPropertyValue (propertyId);
		}
		
		#endregion
		
		#region Private Fields
		
		private StatusBar statusBar;
		private List<StatusBarPanelProvider> panels;
		
		#endregion
		
		#region Internal Class: StatusBarPanel Provider
		
		internal class StatusBarPanelProvider : FragmentControlProvider
		{
			#region Constructor

			public StatusBarPanelProvider (StatusBarPanel statusBarPanel) : base (statusBarPanel)
			{
				this.statusBarPanel = statusBarPanel;
				
				SetBehavior (ValuePatternIdentifiers.Pattern,
				             new StatusBarPanelValueProviderBehavior (this));
				SetBehavior (GridItemPatternIdentifiers.Pattern,
				             new StatusBarPanelGridItemProviderBehavior (this));
			}
		
			#endregion
		
			#region Public Methods
		
			public override object GetPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
					return ControlType.Edit.Id;
				else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
					return "edit";
				else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
					return statusBarPanel.Text;
				else
					return base.GetPropertyValue (propertyId);
			}
		
			#endregion
			
			#region Private Fields
		
			private StatusBarPanel statusBarPanel;
		
			#endregion
		}
		
		#endregion
	}
}
