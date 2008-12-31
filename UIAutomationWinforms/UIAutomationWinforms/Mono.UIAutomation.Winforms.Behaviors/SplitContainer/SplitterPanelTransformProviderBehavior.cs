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
using Mono.UIAutomation.Winforms.Events.SplitContainer;

namespace Mono.UIAutomation.Winforms.Behaviors.SplitContainer
{
	internal class SplitterPanelTransformProviderBehavior : ProviderBehavior, ITransformProvider
	{
		#region Constructor

		public SplitterPanelTransformProviderBehavior (SplitContainerProvider.SplitterPanelProvider provider)
			: base (provider)
		{
			this.splitContainer = (SWF.SplitContainer) Provider.Container;
		}
		
		#endregion

		#region IProviderBehavior Interface
		
		public override AutomationPattern ProviderPattern {
			get { return TransformPatternIdentifiers.Pattern; }
		}
		
		public override void Connect ()
		{
			// NOTE: CanMove Property NEVER changes
			// NOTE: CanRotate Property NEVER changes
			Provider.SetEvent (ProviderEventType.TransformPatternCanResizeProperty,
			                   new SplitterPanelTransformPatternCanResizeEvent (Provider));
		}
		
		public override void Disconnect ()
		{
			Provider.SetEvent (ProviderEventType.TransformPatternCanResizeProperty,
			                   null);
		}
		
		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == TransformPatternIdentifiers.CanMoveProperty.Id)
				return CanMove;
			else if (propertyId == TransformPatternIdentifiers.CanResizeProperty.Id)
				return CanResize;
			else if (propertyId == TransformPatternIdentifiers.CanRotateProperty.Id)
				return CanRotate;
			else
				return base.GetPropertyValue (propertyId);
		}
		
		#endregion
		
		#region ITransformProvider Members
		
		public bool CanMove {
			get { return false; }
		}
		
		public bool CanResize {
			get {
				if (splitContainer.Panel1Collapsed || splitContainer.Panel2Collapsed)
					return false;
				else
					return true;
			}
		}
		
		public bool CanRotate {
			get { return false; }
		}
		
		public void Move (double x, double y)
		{
			throw new InvalidOperationException ();
		}
		
		public void Resize (double width, double height)
		{
			if (!CanResize)
				throw new InvalidOperationException ();
			
			if (splitContainer.InvokeRequired == true) {
				splitContainer.BeginInvoke (new PerformResizeDelegate (Resize),
				                            new object [] { width, height });
				return;
			}
			
			if (splitContainer.Orientation == SWF.Orientation.Horizontal)
				splitContainer.Panel1.Width = (int) width;
			else
				splitContainer.Panel1.Width = (int) height;
		}
		
		public void Rotate (double degrees)
		{
			throw new InvalidOperationException ();
		}
		
		#endregion
		
		#region Private Fields
		
		SWF.SplitContainer splitContainer;
		
		#endregion
	}
	
	delegate void PerformResizeDelegate (double width, double height);
}
