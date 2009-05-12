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
//	Mike Gorse <mgorse@novell.com>
// 
using System;
using System.Reflection;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.DataGridView;

namespace Mono.UIAutomation.Winforms.Behaviors.ListView
{
	
	internal class ListItemInvokeProviderBehavior 
		: ProviderBehavior, IInvokeProvider
	{
		
		#region Constructor
		
		public ListItemInvokeProviderBehavior (ListItemProvider itemProvider)
			: base (itemProvider)
		{
			this.itemProvider = itemProvider;
			this.mwfFileView = (SWF.MWFFileView)itemProvider.ContainerControl;
		}
		
		#endregion
		
		#region IProviderBehavior Interface

		public override void Connect ()
		{
			//Provider.SetEvent (ProviderEventType.InvokePatternInvokedEvent, 
			                   //new ListItemInvokePatternInvokedEvent (itemProvider));
		}
		
		public override void Disconnect ()
		{
			//Provider.SetEvent (ProviderEventType.InvokePatternInvokedEvent, 
			                   //null);
		}

		public override AutomationPattern ProviderPattern { 
			get { return InvokePatternIdentifiers.Pattern; }
		}
		
		#endregion
		
		#region IInvokeProvider Members
		
		public void Invoke ()
		{
			if (!mwfFileView.Enabled)
				throw new ElementNotEnabledException ();

			PerformDoubleClick ();
		}
		
		#endregion	
		
		#region Private Members
		
		private void PerformDoubleClick ()
		{
			if (mwfFileView.InvokeRequired) {
				mwfFileView.BeginInvoke (new SWF.MethodInvoker (PerformDoubleClick));
				return;
			}

			mwfFileView.SelectedItems.Clear ();
			((SWF.ListViewItem)itemProvider.ObjectItem).Selected = true;
			mwfFileView.PerformDoubleClick ();
		}

		private ListItemProvider itemProvider;
		private SWF.MWFFileView mwfFileView;

		#endregion
		
	}

}
