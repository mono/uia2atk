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
//	Mario Carrion <mcarrion@novell.com>
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
using Mono.UIAutomation.Winforms.Events.DataGrid;

namespace Mono.UIAutomation.Winforms.Behaviors.DataGrid
{
	
	internal class HeaderItemInvokeProviderBehavior 
		: ProviderBehavior, IInvokeProvider
	{
		
		#region Constructor
		
		public HeaderItemInvokeProviderBehavior (DataGridProvider.DataGridHeaderItemProvider itemProvider)
			: base (itemProvider)
		{
			this.itemProvider = itemProvider;
			dataGrid = itemProvider.HeaderProvider.DataGridProvider.DataGrid;
		}
		
		#endregion
		
		#region IProviderBehavior Interface

		public override void Connect ()
		{
			Provider.SetEvent (ProviderEventType.InvokePatternInvokedEvent, 
			                   new HeaderItemInvokePatternInvokedEvent (itemProvider));
		}
		
		public override void Disconnect ()
		{
			Provider.SetEvent (ProviderEventType.InvokePatternInvokedEvent, 
			                   null);
		}

		public override AutomationPattern ProviderPattern { 
			get { return InvokePatternIdentifiers.Pattern; }
		}
		
		#endregion
		
		#region IInvokeProvider Members
		
		public void Invoke ()
		{
			if (!dataGrid.Enabled || !dataGrid.AllowSorting || !dataGrid.ColumnHeadersVisible)
				throw new ElementNotEnabledException ();

			PerformClick ();
		}
		
		#endregion	
		
		#region Private Members
		
		private void PerformClick ()
		{
			if (dataGrid.InvokeRequired) {
				dataGrid.BeginInvoke (new SWF.MethodInvoker (PerformClick));
				return;
			}

			// FIXME: We need to replace reflection with an internal method in SWF.DataGrid
			MethodInfo methodInfo = typeof (SWF.DataGrid).GetMethod ("OnMouseDown",
			                                                          BindingFlags.InvokeMethod
			                                                          | BindingFlags.NonPublic
			                                                          | BindingFlags.Instance);
			Rect rect = itemProvider.BoundingRectangle;

			SD.Rectangle sdRect = new SD.Rectangle ((int) rect.X, 
			                                        (int) rect.Y,
			                                        (int) rect.Width, 
			                                        (int) rect.Height);
			sdRect = dataGrid.RectangleToClient (sdRect);

			Action<SWF.DataGrid, SWF.MouseEventArgs> mouseDownMethod
				= (Action<SWF.DataGrid, SWF.MouseEventArgs>) Delegate.CreateDelegate 
					(typeof (Action<SWF.DataGrid, SWF.MouseEventArgs>), methodInfo);

			SWF.MouseEventArgs args 
				= new SWF.MouseEventArgs (SWF.MouseButtons.Left,
				                          1,
				                          sdRect.X + 2,
				                          sdRect.Y + 2,
				                          0);
			mouseDownMethod (dataGrid, args);
		}

		private SWF.DataGrid dataGrid;		
		private DataGridProvider.DataGridHeaderItemProvider itemProvider;

		#endregion
		
	}

}
