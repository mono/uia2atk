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

using System;
using System.ComponentModel;
using System.Windows.Automation;
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;

namespace Mono.UIAutomation.Winforms.Events.PropertyGrid
{	
	internal class SelectionPatternSelectionEvent
		: BaseAutomationPropertyEvent
	{
#region Constructor
		public SelectionPatternSelectionEvent (PropertyGridViewProvider provider) 
			: base (provider,
			        SelectionPatternIdentifiers.SelectionProperty)
		{
			this.provider = provider;
		}
#endregion
		
#region ProviderEvent Methods
		public override void Connect ()
		{
			provider.PropertyGrid.SelectedGridItemChanged
				+= OnSelectedGridItemChanged;
		}

		public override void Disconnect ()
		{
			provider.PropertyGrid.SelectedGridItemChanged
				-= OnSelectedGridItemChanged;
		}
#endregion 
		
#region Private Methods
		private void OnSelectedGridItemChanged (object o,
		                                        SWF.SelectedGridItemChangedEventArgs args)
		{
			RaiseAutomationPropertyChangedEvent ();
		}
#endregion

#region Private Fields
		private PropertyGridViewProvider provider;
#endregion
	}
}
