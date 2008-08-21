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
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;
using System.Windows;
using System.Reflection;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;

namespace Mono.UIAutomation.Winforms.Behaviors
{

	internal class ListBoxScrollProviderBehavior 
		: ProviderBehavior, IScrollProvider
	{
		
		#region Constructors

		public ListBoxScrollProviderBehavior (ListBoxProvider provider,
		                                      ScrollBar hscrollbar,
		                                      ScrollBar vscrollbar)
			: base (provider)
		{
			this.hscrollbar = hscrollbar;
			this.vscrollbar = vscrollbar;
		}

		#endregion
		
		#region IProviderBehavior Interface

		public override AutomationPattern ProviderPattern { 
			get { return ScrollPatternIdentifiers.Pattern; }
		}
		
		public override void Connect (Control control)
		{
			//TODO: Add events
			//HorizontallyScrollable
			//HorizontalScrollPercent
			//HorizontalViewSize
			//VerticallyScrollable
			//VerticalScrollPercent
			//VerticalViewSize			
		}
		
		public override void Disconnect (Control control)
		{
			//TODO: Delete events
			//HorizontallyScrollable
			//HorizontalScrollPercent
			//HorizontalViewSize
			//VerticallyScrollable
			//VerticalScrollPercent
			//VerticalViewSize
		}

		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == ScrollPatternIdentifiers.HorizontallyScrollableProperty.Id)
				return HorizontallyScrollable;
			else if (propertyId == ScrollPatternIdentifiers.HorizontalScrollPercentProperty.Id)
				return HorizontalScrollPercent;
			else if (propertyId == ScrollPatternIdentifiers.HorizontalViewSizeProperty.Id)
				return HorizontalViewSize;
			else if (propertyId == ScrollPatternIdentifiers.VerticallyScrollableProperty.Id)
				return VerticallyScrollable;
			else if (propertyId == ScrollPatternIdentifiers.VerticalScrollPercentProperty.Id)
				return VerticalScrollPercent;
			else if (propertyId == ScrollPatternIdentifiers.VerticalViewSizeProperty.Id)
				return VerticalViewSize;
			else
				return base.GetPropertyValue (propertyId);
		}
		
		#endregion
		
		#region IScrollProvider Interface
			
		public bool HorizontallyScrollable {
			get {
				if (((ListBox) Provider.Control).ScrollAlwaysVisible)
					return hscrollbar.Enabled;
				else
					return hscrollbar.Visible;
			}
		}
		
		//The horizontal scroll position as a percentage of the total content 
		//area within the control.
		public double HorizontalScrollPercent {
			get { return (hscrollbar.Value * 100) / hscrollbar.Maximum; }
		}
		
		//The horizontal size of the viewable region as a percentage of the 
		//total content area within the control.
		public double HorizontalViewSize {
			//TODO: Is this OK?
			get { return HorizontalScrollPercent; }
		}

		public bool VerticallyScrollable {
			get {
				if (((ListBox) Provider.Control).ScrollAlwaysVisible)
					return vscrollbar.Enabled;
				else
					return vscrollbar.Visible;
			}
		}

		public double VerticalScrollPercent {
			get { return (vscrollbar.Value * 100) / vscrollbar.Maximum; }
		}

		public double VerticalViewSize {
			//TODO: Is this OK?
			get { return VerticalScrollPercent; }
		}
		
		public void Scroll (ScrollAmount horizontalAmount, 
		                    ScrollAmount verticalAmount)
		{
			if (horizontalAmount != ScrollAmount.NoAmount)
				ScrollByAmount (hscrollbar, GetAmountString (horizontalAmount));
			if (verticalAmount != ScrollAmount.NoAmount)
				ScrollByAmount (vscrollbar, GetAmountString (verticalAmount));
		}

		public void SetScrollPercent (double horizontalPercent, 
		                              double verticalPercent)
		{
			if (horizontalPercent != ScrollPatternIdentifiers.NoScroll) {
				if (horizontalPercent < 0 || horizontalPercent > 100)
					throw new ArgumentOutOfRangeException ();
				else
					hscrollbar.Value 
						= (int) ((horizontalPercent * hscrollbar.Maximum) / 100);
			}
			
			if (verticalPercent != ScrollPatternIdentifiers.NoScroll) {
			    if (verticalPercent < 0 || verticalPercent > 100)
					throw new ArgumentOutOfRangeException ();
				else 
					vscrollbar.Value 
						= (int) ((verticalPercent * vscrollbar.Maximum) / 100);
			}
		}

		#endregion
		
		#region Private Methods

		private string GetAmountString (ScrollAmount amount) 
		{
			string str = string.Empty;

			switch (amount) {
			case ScrollAmount.LargeDecrement:
				str = "UIALargeDecrement";
				break;
			case ScrollAmount.LargeIncrement:
				str = "UIALargeIncrement";
				break;
			case ScrollAmount.SmallDecrement:
				str = "UIASmallDecrement";
				break;
			case ScrollAmount.SmallIncrement:
				str = "UIASmallIncrement";
				break;
			}

			return str;
		}
		
		private void ScrollByAmount (ScrollBar scrollbar, string method) 
		{
			if (method == string.Empty)
				return;
			
			MethodInfo methodInfo = typeof (ScrollBar).GetMethod (method,
			                                                      BindingFlags.InvokeMethod
			                                                      | BindingFlags.NonPublic
			                                                      | BindingFlags.Instance);
			Action<ScrollBar> invoke 
				= (Action<ScrollBar>) Delegate.CreateDelegate (typeof (Action<ScrollBar>), 
				                                               methodInfo);
			invoke (scrollbar);
		}
		
		#endregion
		
		#region Private Fields
		
		private ScrollBar hscrollbar;
		private ScrollBar vscrollbar;
		
		#endregion

	}
}
