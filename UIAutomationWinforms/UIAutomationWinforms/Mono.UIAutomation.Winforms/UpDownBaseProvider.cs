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
using Mono.UIAutomation.Winforms.Behaviors;
using Mono.UIAutomation.Winforms.Behaviors.UpDownBase;
using Mono.UIAutomation.Winforms.Navigation;

namespace Mono.UIAutomation.Winforms
{
	internal abstract class UpDownBaseProvider : FragmentRootControlProvider
	{
		#region Constructor

		public UpDownBaseProvider (UpDownBase upDownBase) : base (upDownBase)
		{
		}
		
		#endregion
		
		#region FragmentRootControlProvider: Specializations
		
		public override void InitializeChildControlStructure ()
		{
			UpDownBase upDownBase = (UpDownBase) Control;
			
			SetBehavior (ValuePatternIdentifiers.Pattern,
			             new ValueProviderBehavior (this));

			if (forwardButton == null) {
				forwardButton = new UpDownBaseButtonProvider (upDownBase,
				                                              UpDownBaseButtonOrientation.Forward);
				forwardButton.Initialize ();
				OnNavigationChildAdded (true, forwardButton);
			}
			if (backwardButton == null) {
				backwardButton = new UpDownBaseButtonProvider (upDownBase,
				                                               UpDownBaseButtonOrientation.Backward);
				backwardButton.Initialize ();
				OnNavigationChildAdded (true, backwardButton);
			}
		}
		
		public override void FinalizeChildControlStructure ()
		{
			if (forwardButton != null) {
				forwardButton.Terminate ();
				forwardButton = null;
			}
			if (backwardButton != null) {
				backwardButton.Terminate ();
				backwardButton = null;
			}
		}
		
		#endregion
		
		#region SimpleControlProvider: Specializations
		
		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.Spinner.Id;
			else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
				return "spinner";
			else
				return base.GetProviderPropertyValue (propertyId);
		}
		
		#endregion
		
		#region Private Fields
		
		private FragmentControlProvider forwardButton;
		private FragmentControlProvider backwardButton;
		
		#endregion
		
		#region Internal Enumeration: Button Orientation
		
		internal enum UpDownBaseButtonOrientation
		{
			Forward,
			Backward
		}
		
		#endregion
		
		#region Internal Class: Button Provider
		
		internal class UpDownBaseButtonProvider : FragmentControlProvider
		{
			#region Constructor

			public UpDownBaseButtonProvider (UpDownBase upDownBase,
			                                 UpDownBaseButtonOrientation orientation)
				: base (upDownBase)
			{
				this.orientation = orientation;
			}
		
			#endregion
		
			#region Public Methods
			
			public override IRawElementProviderFragmentRoot FragmentRoot {
				get { 
					return (IRawElementProviderFragmentRoot) ProviderFactory.FindProvider (Control);
				}
			}
			
			public override void Initialize ()
			{
				base.Initialize ();
				
				SetBehavior (InvokePatternIdentifiers.Pattern,
				             new ButtonInvokeProviderBehavior (this));
			}
			
			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
					return ControlType.Button.Id;
				else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
					return "button";
				else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
					return GetNameProperty ();
				else if (propertyId == AutomationElementIdentifiers.IsContentElementProperty.Id)
					return false;
				else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
					return false;
				else
					return base.GetProviderPropertyValue (propertyId);
			}
			
			public UpDownBaseButtonOrientation Orientation {
				get { return orientation; }
			}
		
			#endregion
			
			#region Private Methods
			
			private string GetNameProperty ()
			{
				if (orientation == UpDownBaseButtonOrientation.Forward)
					return "Forward";
				else if (orientation == UpDownBaseButtonOrientation.Backward)
					return "Backward";
				else
					return string.Empty;
			}
			
			#endregion
			
			#region Private Fields
			
			private UpDownBaseButtonOrientation orientation;
			
			#endregion
		}
		
		#endregion
	}
}
