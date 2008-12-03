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
	internal class TransformProviderBehavior : ProviderBehavior, ITransformProvider
	{
		#region Constructor

		public TransformProviderBehavior (SplitterProvider provider) : base (provider)
		{
			this.splitter = (SWF.Splitter) Provider.Control;
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
			                   new TransformPatternCanResizeEvent (Provider));
		}
		
		public override void Disconnect ()
		{
			Provider.SetEvent (ProviderEventType.TransformPatternCanMoveProperty,
			                   null);
			Provider.SetEvent (ProviderEventType.TransformPatternCanResizeProperty,
			                   null);
			Provider.SetEvent (ProviderEventType.TransformPatternCanRotateProperty,
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
			get { return true; }
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
			if (splitter.InvokeRequired == true) {
				splitter.BeginInvoke (new PerformResizeDelegate (Resize),
				                      new object [] { width, height });
				return;
			}
			
			splitter.Size = new Size ((int) width, (int) height);
		}
		
		public void Rotate (double degrees)
		{
			throw new InvalidOperationException ();
		}
		
		#endregion
		
		#region Private Fields
		
		SWF.Splitter splitter;
		
		#endregion
	}
	
	delegate void PerformResizeDelegate (double width, double height);
}
