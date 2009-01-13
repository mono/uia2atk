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
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.Unix;
using Mono.UIAutomation.Winforms.Behaviors;
using Mono.UIAutomation.Winforms.Behaviors.SplitContainer;
using Mono.UIAutomation.Winforms.Navigation;

namespace Mono.UIAutomation.Winforms
{
	internal class SplitContainerProvider : FragmentRootControlProvider
	{
		#region Constructor

		public SplitContainerProvider (SplitContainer splitContainer) : base (splitContainer)
		{
		}
		
		#endregion
		
		#region FragmentRootControlProvider: Specializations
		
		public override void InitializeChildControlStructure ()
		{
			SplitContainer splitContainer = (SplitContainer) Control;
			
			if (!splitContainer.Panel1Collapsed) {
				panel1 = new SplitterPanelProvider (splitContainer.Panel1);
				panel1.Initialize ();
				OnNavigationChildAdded (true, panel1);
			}
			if (!splitContainer.Panel2Collapsed) {
				panel2 = new SplitterPanelProvider (splitContainer.Panel2);
				panel2.Initialize ();
				OnNavigationChildAdded (true, panel2);
			}
		}
		
		public override void FinalizeChildControlStructure ()
		{
			if (panel1 != null) {
				panel1.Terminate ();
				panel1 = null;
			}
			if (panel2 != null) {
				panel2.Terminate ();
				panel2 = null;
			}
		}
		
		#endregion
		
		#region SimpleControlProvider: Specializations
		
		public override void Initialize ()
		{
			base.Initialize ();
			
			SetBehavior (DockPatternIdentifiers.Pattern,
			             new DockProviderBehavior (this));
		}
		
		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.Pane.Id;
			else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
				return Catalog.GetString ("pane");
			else
				return base.GetProviderPropertyValue (propertyId);
		}
		
		#endregion
		
		#region Private Fields
		
		private FragmentControlProvider panel1;
		private FragmentControlProvider panel2;
		
		#endregion
		
		#region Internal Class: SplitterPanel Provider
		
		internal class SplitterPanelProvider : FragmentControlProvider
		{
			#region Constructor

			public SplitterPanelProvider (SplitterPanel splitterPanel)
				: base (splitterPanel)
			{
			}
		
			#endregion
			
			#region SimpleControlProvider: Specializations

			public override void Initialize()
			{
				base.Initialize ();
				
				SetBehavior (TransformPatternIdentifiers.Pattern,
				             new SplitterPanelTransformProviderBehavior (this));
			}
			
			#endregion
		
			#region Public Methods
			
			public override IRawElementProviderFragmentRoot FragmentRoot {
				get { 
					return (IRawElementProviderFragmentRoot) ProviderFactory.FindProvider (Control);
				}
			}
			
			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
					return ControlType.Pane.Id;
				else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
					return "pane";
				else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
					return false;
				else
					return base.GetProviderPropertyValue (propertyId);
			}
		
			#endregion
		}
		
		#endregion
	}
}