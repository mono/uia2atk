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
using System.ComponentModel;
using System.Windows.Automation;
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;

namespace Mono.UIAutomation.Winforms.Events.DataGrid
{	
	internal class SelectionPatternInvalidatedEvent
		: BaseAutomationEvent
	{

		#region Constructor

		public SelectionPatternInvalidatedEvent (DataGridProvider provider) 
			: base (provider,
			        SelectionPatternIdentifiers.InvalidatedEvent)
		{
		}
		
		#endregion
		
		#region ProviderEvent Methods

		public override void Connect ()
		{
			((SWF.DataGrid) Provider.Control).Navigate += OnInvalidatedEvent;
			((SWF.DataGrid) Provider.Control).DataSourceChanged += OnInvalidatedEvent;
			((SWF.DataGrid) Provider.Control).UIACollectionChanged += OnUIACollectionChanged;
		}
		
		public override void Disconnect ()
		{
			((SWF.DataGrid) Provider.Control).Navigate -= OnInvalidatedEvent;
			((SWF.DataGrid) Provider.Control).DataSourceChanged -= OnInvalidatedEvent;
			((SWF.DataGrid) Provider.Control).UIACollectionChanged -= OnUIACollectionChanged;
		}
		
		#endregion 
		
		#region Protected methods
		
		private void OnInvalidatedEvent (object sender, EventArgs args)
		{
			RaiseAutomationEvent ();
		}

		private void OnUIACollectionChanged (object sender, CollectionChangeEventArgs args)
		{
			RaiseAutomationEvent ();
		}

		#endregion
	}
}
