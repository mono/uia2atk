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
//      Sandy Armstrong <sanfordarmstrong@gmail.com>
// 

using System;
using SD = System.Drawing;
using SWF = System.Windows.Forms;

using System.Windows.Automation;
using System.Windows.Automation.Provider;

using Mono.UIAutomation.Bridge;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.ToolStripItem;

namespace Mono.UIAutomation.Winforms.Behaviors.ToolStripItem
{
	internal class InvokeProviderBehavior 
		: ProviderBehavior, IInvokeProvider, IEmbeddedImage
	{
		#region Private Members

		ToolStripItemProvider itemProvider;

		#endregion
		
		#region Constructor
		
		public InvokeProviderBehavior (ToolStripItemProvider provider)
			: base (provider)
		{
			this.itemProvider = provider;
		}
		
		#endregion
		
		#region IEmbeddedImage Interface
		
		public System.Windows.Rect Bounds {
			get {
				SWF.ToolStripItem item
					= (SWF.ToolStripItem)itemProvider.Component;

				SD.Rectangle r = item.ContentRectangle;
				if (item is SWF.ToolStripSplitButton) {
					r.Width -= ((SWF.ToolStripSplitButton)item).DropDownButtonWidth + 1;
				}

				SD.Rectangle text_rect, image_rect;
				item.CalculateTextAndImageRectangles (
					r, out text_rect, out image_rect);

				System.Windows.Rect bounds
					= (System.Windows.Rect) itemProvider.GetPropertyValue (
						AutomationElementIdentifiers.BoundingRectangleProperty.Id);
				image_rect.X += (int) bounds.X;
				image_rect.Y += (int) bounds.Y;

				return Helper.RectangleToRect (image_rect);
			}
		}

		public string Description {
			get { return String.Empty; }
		}

		#endregion

		#region IProviderBehavior Interface

		public override void Connect ()
		{
			Provider.SetEvent (ProviderEventType.InvokePatternInvokedEvent, 
			                   new InvokePatternInvokedEvent (itemProvider));
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
		
		public virtual void Invoke ()
		{
			if (((SWF.ToolStripItem)Provider.Component).Enabled == false)
				throw new ElementNotEnabledException ();

			PerformClick ();
		}
		
		#endregion	
		
		#region Private Methods
		
		private void PerformClick ()
		{
			SWF.ToolStripItem item = (SWF.ToolStripItem) Provider.Component;
			if (item.Owner != null && item.Owner.InvokeRequired) {
				item.Owner.BeginInvoke (new SWF.MethodInvoker (PerformClick));
				return;
			}

			// Make sure selection changes, or else another item's
			// dropdown menu might still appear.
			if (item.Owner != null && item.GetCurrentParent () != null)
				item.Select ();
			
			// PerformClick does _not_ show/hide the DropDown, so
			// we must do this manually.  On Vista, clicking the
			// button appears to both Show the drop down and
			// Perform a click, so we emulate that behavior.
			if (item is SWF.ToolStripDropDownButton) {
				SWF.ToolStripDropDownButton dropDown = (SWF.ToolStripDropDownButton) item;
				if (!dropDown.Pressed) {
					dropDown.ShowDropDown ();
				} else {
					dropDown.HideDropDown ();
				}
			} else if (item is SWF.ToolStripMenuItem) {
				SWF.ToolStripMenuItem menuItem = (SWF.ToolStripMenuItem) item;
				if (!menuItem.Pressed) {
					menuItem.ShowDropDown ();
				} else {
					menuItem.HideDropDown ();
				}
			}

			item.PerformClick ();
		}
		
		#endregion
	}
}
