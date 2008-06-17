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
using System.Windows;
using Mono.UIAutomation.Winforms.Behaviors;

namespace Mono.UIAutomation.Winforms
{

	public class ComboBoxProvider : FragmentRootControlProvider
	{
		
#region Private Fields
		
		private ComboBox combobox;
		
#endregion

#region Constructor
		
		public ComboBoxProvider (ComboBox combobox) : base (combobox)
		{
			this.combobox = combobox;

			combobox.DropDownStyleChanged += delegate (object obj, 
			                                           EventArgs args) {
				SetBehaviors ();
			};
			
			SetBehaviors ();
		}
		
#endregion
		
#region Private Methods
		
		private void SetBehaviors () {
			if (combobox.DropDownStyle == ComboBoxStyle.Simple) {
				SetBehavior (ExpandCollapsePatternIdentifiers.Pattern, 
				             null);
				SetBehavior (SelectionPatternIdentifiers.Pattern,
				             new ComboBoxSelectionProviderBehavior (this));
				SetBehavior (ValuePatternIdentifiers.Pattern,
				             new ComboBoxValueProviderBehavior (this));
			} else if (combobox.DropDownStyle == ComboBoxStyle.DropDown) {
				SetBehavior (ExpandCollapsePatternIdentifiers.Pattern,
				             new ComboBoxExpandCollapseProviderBehavior (this));
				SetBehavior (SelectionPatternIdentifiers.Pattern,
				             new ComboBoxSelectionProviderBehavior (this));
				SetBehavior (ValuePatternIdentifiers.Pattern,
				             new ComboBoxValueProviderBehavior (this));
			} else if (combobox.DropDownStyle == ComboBoxStyle.DropDownList) {
				SetBehavior (ExpandCollapsePatternIdentifiers.Pattern,
				             new ComboBoxExpandCollapseProviderBehavior (this));
				SetBehavior (SelectionPatternIdentifiers.Pattern,
				             new ComboBoxSelectionProviderBehavior (this));
				SetBehavior (ValuePatternIdentifiers.Pattern, 
				             null);
			} 
		}
		
#endregion
		
#region FragmentRootControlProvider Overrides
		
		public override IRawElementProviderFragment ElementProviderFromPoint (double x, double y)
		{
			//TODO: Evaluate all cases.
			return base.ElementProviderFromPoint (x, y);
		}
		
		public override IRawElementProviderFragment GetFocus ()
		{
			//TODO: Evaluate when combobox.DropDownStyle is List or there's
			//no selected element
			return combobox.SelectedIndex == -1 ? null :
				new ComboBoxItemProvider (this, combobox);
		}
		
#endregion

	}
}
