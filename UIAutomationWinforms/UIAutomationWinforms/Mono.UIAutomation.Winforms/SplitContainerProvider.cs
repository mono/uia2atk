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
	[MapsComponent (typeof (SplitContainer))]
	internal class SplitContainerProvider : ContainerControlProvider
	{
		#region Constructor

		public SplitContainerProvider (SplitContainer splitContainer) : base (splitContainer)
		{
		}
		
		#endregion
		
		#region FragmentRootControlProvider: Specializations
		
		protected override void InitializeChildControlStructure ()
		{
			SplitContainer splitContainer = (SplitContainer) Control;
			
			if (!splitContainer.Panel1Collapsed) {
				panel1 = new SplitterPanelProvider (splitContainer.Panel1);
				panel1.Initialize ();
				AddChildProvider (panel1);
			}
			if (!splitContainer.Panel2Collapsed) {
				panel2 = new SplitterPanelProvider (splitContainer.Panel2);
				panel2.Initialize ();
				AddChildProvider (panel2);
			}
		}
		
		protected override void FinalizeChildControlStructure ()
		{
			DestroyLocalChild (panel1);
			DestroyLocalChild (panel2);
			panel1 = null;
			panel2 = null;
		}

		private void DestroyLocalChild (FragmentControlProvider child)
		{
			if (child == null)
				return;
			RemoveChildProvider (child);
			child.Terminate ();
		}
		
		#endregion
		
		#region SimpleControlProvider: Specializations
		
		public override void Initialize ()
		{
			base.Initialize ();
			
			SetBehavior (DockPatternIdentifiers.Pattern,
			             new DockProviderBehavior (this));
			SetBehavior (RangeValuePatternIdentifiers.Pattern,
			             new RangeValueProviderBehavior (this));
		}
		
		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.Pane.Id;
			else if (propertyId == AutomationElementIdentifiers.OrientationProperty.Id)
				return ((SplitContainer)Control).Orientation == Orientation.Vertical? OrientationType.Vertical: OrientationType.Horizontal;
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
