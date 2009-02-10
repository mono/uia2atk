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
// Copyright (c) 2009 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//	Brad Taylor <brad@getcoded.net>
// 

using System.Windows.Automation;
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;

namespace Mono.UIAutomation.Winforms.Events.PropertyGrid
{
	internal class ListItemSelectionItemPatternIsSelectedEvent
		: BaseAutomationPropertyEvent
	{
#region Constructors
		public ListItemSelectionItemPatternIsSelectedEvent (PropertyGridListItemProvider provider)
			: base (provider, 
			        SelectionItemPatternIdentifiers.IsSelectedProperty)
		{
			this.provider = provider;
			this.isSelected = provider.PropertyGridViewProvider.IsItemSelected (provider);
		}
#endregion
		
#region Overridden Methods
		public override void Connect ()
		{
			provider.PropertyGridViewProvider.PropertyGrid.SelectedGridItemChanged
				+= OnSelectedGridItemChanged;
		}

		public override void Disconnect ()
		{
			provider.PropertyGridViewProvider.PropertyGrid.SelectedGridItemChanged
				-= OnSelectedGridItemChanged;
		}
#endregion 
		
#region Private Methods
		private void OnSelectedGridItemChanged (object o,
		                                        SWF.SelectedGridItemChangedEventArgs args)
		{
			bool selected = provider.PropertyGridViewProvider.IsItemSelected (provider);
			if (selected != isSelected) 
				RaiseAutomationPropertyChangedEvent ();
		}
#endregion

#region Private Fields
		private PropertyGridListItemProvider provider;
		private bool isSelected = false;
#endregion
	}
}
