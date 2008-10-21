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
using SWF = System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms.Behaviors;
using Mono.UIAutomation.Winforms.Behaviors.ListItem;
using Mono.UIAutomation.Winforms.Behaviors.CheckedListBox;

namespace Mono.UIAutomation.Winforms
{

	internal class CheckedListBoxProvider : ListBoxProvider
	{
		
		#region Constructors

		public CheckedListBoxProvider (SWF.CheckedListBox checkedListBox)
			: base (checkedListBox)
		{
		}
		
		#endregion
		
		#region ListBoxProvider specializations
		
		public override void ToggleItem (ListItemProvider item)
		{
			if (ContainsItem (item) == true) {
				SWF.CheckedListBox checkedListBox = (SWF.CheckedListBox) Control;
				bool chcked = checkedListBox.GetItemChecked (item.Index);
				checkedListBox.SetItemChecked (item.Index, !chcked);
			}
		}
		
		public override ToggleState GetItemToggleState (ListItemProvider item)
		{
			if (ContainsItem (item) == true) {
				SWF.CheckedListBox checkedListBox = (SWF.CheckedListBox) Control;	
				
				SWF.CheckState state = checkedListBox.GetItemCheckState (item.Index);
				switch (state) {
					case SWF.CheckState.Checked:
						return ToggleState.On;
					case SWF.CheckState.Unchecked:
						return ToggleState.Off;
					default:
						return ToggleState.Indeterminate;
				}
			} else
				return ToggleState.Indeterminate;
		}
		
		#endregion
		
		#region ListProvider: Methods
		
		internal override IProviderBehavior GetListItemBehaviorRealization (AutomationPattern behavior,
		                                                                    ListItemProvider listItem)
		{
			if (behavior == TogglePatternIdentifiers.Pattern)
				return new ListItemToggleProviderBehavior (listItem);	
			else
				return base.GetListItemBehaviorRealization (behavior, listItem);
		}
		
		#endregion
		
	}
}
