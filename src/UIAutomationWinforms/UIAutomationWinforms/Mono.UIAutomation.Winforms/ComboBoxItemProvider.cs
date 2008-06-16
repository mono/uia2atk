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
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;
using Mono.UIAutomation.Winforms.Behaviors;

namespace Mono.UIAutomation.Winforms
{	
	
	internal class ComboBoxItemProvider : FragmentControlProvider
	{
		
#region Constructors
		
		public ComboBoxItemProvider (ComboBoxProvider provider, 
		                             ComboBox control) 
			: this (provider, control, control.SelectedIndex)
		{
		}

		public ComboBoxItemProvider (ComboBoxProvider provider,
		                             ComboBox control, int index) : base (null)
		{
			combobox_provider = provider;
			combobox_control = control;
			this.index = index;
			
			SetBehavior (SelectionItemPatternIdentifiers.Pattern, 
			             new ComboBoxItemProviderBehavior (this));
			//TODO: ADD SelectionItem behavior
		}

#endregion
		
#region Public properties
		
		public ComboBox ComboBoxControl {
			get { return combobox_control; }
		}
		
		public int Index {
			get { return index; }
		}

		public ComboBoxProvider ComboBoxProvider {
			get { return combobox_provider; }
		}
		
#endregion
		
#region FragmentControlProvider Overrides
		
		public override IRawElementProviderFragmentRoot FragmentRoot {
			get { return ComboBoxProvider; }
		}
		
		public override IRawElementProviderSimple[] GetEmbeddedFragmentRoots () 
		{
			return null;
		}
		
		public override IRawElementProviderFragment Navigate (NavigateDirection direction)
		{
			if (direction == NavigateDirection.Parent)
				return ComboBoxProvider;
			else if (direction == NavigateDirection.NextSibling) {
				if (index + 1 > ComboBoxControl.Items.Count)
					return null;
				else
					return new ComboBoxItemProvider (ComboBoxProvider, 
					                                 ComboBoxControl, index + 1);
			} else if (direction == NavigateDirection.PreviousSibling) {
				if (index - 1 < 0)
					return null;
				else
					return new ComboBoxItemProvider (ComboBoxProvider, 
					                                 ComboBoxControl, index - 1);
			} else
				return null;
		}
		
		public override void SetFocus () 
		{
			combobox_control.Focus (); 
		}
		
#endregion
		
#region Private fields
		
		private ComboBox combobox_control;
		private ComboBoxProvider combobox_provider;
		private int index;		
		
#endregion
		
	}
}
