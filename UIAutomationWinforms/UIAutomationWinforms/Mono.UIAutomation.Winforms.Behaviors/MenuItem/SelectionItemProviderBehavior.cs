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
//	Sandy Armstrong <sanfordarmstrong@gmail.com>
// 

using System;
using SWF = System.Windows.Forms;

using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace Mono.UIAutomation.Winforms.Behaviors.MenuItem
{
	internal class SelectionItemProviderBehavior : ProviderBehavior, ISelectionItemProvider
	{
		
		public SelectionItemProviderBehavior (MenuItemProvider itemProvider) :
			base (itemProvider)
		{
		}

		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == SelectionItemPatternIdentifiers.IsSelectedProperty.Id)
				return IsSelected;
			else if (propertyId == SelectionItemPatternIdentifiers.SelectionContainerProperty.Id)
				return SelectionContainer;
			return base.GetPropertyValue (propertyId);
		}

		public override AutomationPattern ProviderPattern {
			get {
				return SelectionItemPatternIdentifiers.Pattern;
			}
		}

		public override void Connect ()
		{
		}

		public override void Disconnect ()
		{
		}

		#region ISelectionItemProvider implementation 
		
		public void AddToSelection ()
		{
		}
		
		public void RemoveFromSelection ()
		{
			throw new InvalidOperationException ("Not supported on MenuItems");
		}
		
		public void Select ()
		{
		}
		
		public bool IsSelected {
			get {
				return true;
			}
		}
		
		public IRawElementProviderSimple SelectionContainer {
			get {
				return Provider.Navigate (NavigateDirection.Parent);
			}
		}
		
		#endregion 
		
	}
}
