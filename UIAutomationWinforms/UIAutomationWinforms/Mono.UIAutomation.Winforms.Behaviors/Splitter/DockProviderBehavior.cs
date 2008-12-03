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
using System.Drawing;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.Splitter;

namespace Mono.UIAutomation.Winforms.Behaviors.Splitter
{
	internal class DockProviderBehavior : ProviderBehavior, IDockProvider
	{
		#region Constructor

		public DockProviderBehavior (SplitterProvider provider) : base (provider)
		{
			this.splitter = (SWF.Splitter) Provider.Control;
		}
		
		#endregion

		#region IProviderBehavior Interface
		
		public override AutomationPattern ProviderPattern {
			get { return DockPatternIdentifiers.Pattern; }
		}
		
		public override void Connect ()
		{
			Provider.SetEvent (ProviderEventType.DockPatternDockPositionProperty,
			                   new DockPatternDockPositionEvent (Provider));
		}
		
		public override void Disconnect ()
		{
			Provider.SetEvent (ProviderEventType.DockPatternDockPositionProperty,
			                   null);
		}
		
		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == DockPatternIdentifiers.DockPositionProperty.Id)
				return DockPosition;
			else
				return base.GetPropertyValue (propertyId);
		}
		
		#endregion
		
		#region IDockProvider Members
		
		public DockPosition DockPosition {
			get {
				if (splitter.Dock == SWF.DockStyle.Top)
					return (DockPosition) SWF.DockStyle.Top;
				else if (splitter.Dock == SWF.DockStyle.Bottom)
					return (DockPosition) SWF.DockStyle.Bottom;
				else if (splitter.Dock == SWF.DockStyle.Left)
					return (DockPosition) SWF.DockStyle.Left;
				else
					return (DockPosition) SWF.DockStyle.Right;
			}
		}
		
		public void SetDockPosition (DockPosition dockPosition)
		{
			if (dockPosition == (DockPosition) SWF.DockStyle.Fill ||
			    dockPosition == (DockPosition) SWF.DockStyle.None)
				throw new InvalidOperationException ();
			
			if (splitter.InvokeRequired == true) {
				splitter.BeginInvoke (new PerformSetDockPositionDelegate (SetDockPosition),
				                      new object [] { dockPosition });
				return;
			}
			
			splitter.Dock = (SWF.DockStyle) DockPosition;
		}
		
		#endregion
				
		#region Private Fields
		
		SWF.Splitter splitter;
		
		#endregion
	}
	
	delegate void PerformSetDockPositionDelegate (DockPosition dockPosition);
}
