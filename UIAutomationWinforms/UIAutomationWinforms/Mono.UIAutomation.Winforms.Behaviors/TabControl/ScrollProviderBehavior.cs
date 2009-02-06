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
// 	Neville Gao <nevillegao@gmail.com>
// 

using System;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Behaviors.Generic;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.TabControl;

namespace Mono.UIAutomation.Winforms.Behaviors.TabControl
{
	internal class ScrollProviderBehavior : ProviderBehavior, IScrollProvider
	{
		#region Constructor

		public ScrollProviderBehavior (TabControlProvider provider)
			: base (provider)
		{
			this.tabControl = (SWF.TabControl) Provider.Control;
		}
		
		#endregion

		#region IProviderBehavior Interface

		public override AutomationPattern ProviderPattern { 
			get { return ScrollPatternIdentifiers.Pattern; }
		}
		
		public override void Connect ()
		{
			// NOTE: VerticalScrollPercent Property NEVER changes.
			// NOTE: VerticalViewSize Property NEVER changes.
			// NOTE: VerticallyScrollable Property NEVER changes.
			Provider.SetEvent (ProviderEventType.ScrollPatternHorizontalScrollPercentProperty,
			                   new ScrollPatternHorizontalScrollPercentEvent ((TabControlProvider) Provider));
			Provider.SetEvent (ProviderEventType.ScrollPatternHorizontalViewSizeProperty,
			                   new ScrollPatternHorizontalViewSizeEvent ((TabControlProvider) Provider));
			Provider.SetEvent (ProviderEventType.ScrollPatternHorizontallyScrollableProperty,
			                   new ScrollPatternHorizontallyScrollableEvent ((TabControlProvider) Provider));
		}

		public override void Disconnect ()
		{
			Provider.SetEvent (ProviderEventType.ScrollPatternHorizontalScrollPercentProperty,
			                   null);
			Provider.SetEvent (ProviderEventType.ScrollPatternHorizontalViewSizeProperty,
			                   null);
			Provider.SetEvent (ProviderEventType.ScrollPatternVerticalScrollPercentProperty,
			                   null);
			Provider.SetEvent (ProviderEventType.ScrollPatternVerticalViewSizeProperty,
			                   null);
			Provider.SetEvent (ProviderEventType.ScrollPatternHorizontallyScrollableProperty,
			                   null);
			Provider.SetEvent (ProviderEventType.ScrollPatternVerticallyScrollableProperty,
			                   null);
		}

		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == ScrollPatternIdentifiers.HorizontalScrollPercentProperty.Id)
				return HorizontalScrollPercent;
			else if (propertyId == ScrollPatternIdentifiers.HorizontalViewSizeProperty.Id)
				return HorizontalViewSize;
			else if (propertyId == ScrollPatternIdentifiers.VerticalScrollPercentProperty.Id)
				return VerticalScrollPercent;
			else if (propertyId == ScrollPatternIdentifiers.VerticalViewSizeProperty.Id)
				return VerticalViewSize;
			else if (propertyId == ScrollPatternIdentifiers.HorizontallyScrollableProperty.Id)
				return HorizontallyScrollable;
			else if (propertyId == ScrollPatternIdentifiers.VerticallyScrollableProperty.Id)
				return VerticallyScrollable;
			else
				return base.GetPropertyValue (propertyId);
		}
		
		#endregion

		#region IScrollProvider Members

		public double HorizontalScrollPercent {
			get {
				if (!tabControl.ShowSlider)
					return ScrollPatternIdentifiers.NoScroll;
				else
					return 100 / tabControl.TabCount;
			}
		}

		public double HorizontalViewSize {
			get {
				if (!tabControl.ShowSlider)
					return 100;
				else {
					// FIXME: remove this when we can depend on MWF > 2.4
					try {
						return Helper.GetPrivateProperty<SWF.TabControl, double> (tabControl,
						                                                          "UIAHorizontalViewSize");
					} catch (NotSupportedException) { }
					try {
						int leftArea =
							Helper.GetPrivateProperty<SWF.TabControl, int> (tabControl,
							                                                "LeftScrollButtonArea.Left");
						return (double) leftArea * 100 / tabControl.TabPages [tabControl.TabCount - 1].TabBounds.Right;
					} catch (NotSupportedException) { }
					return Double.NaN;
				}
//					return tabControl.UIAHorizontalViewSize;
			}
		}

		public double VerticalScrollPercent {
			get { return ScrollPatternIdentifiers.NoScroll; }
		}

		public double VerticalViewSize {
			get { return 100; }
		}

		public bool HorizontallyScrollable {
			get { return tabControl.ShowSlider; }
		}

		public bool VerticallyScrollable {
			get { return false; }
		}

		public void Scroll (ScrollAmount horizontalAmount, ScrollAmount verticalAmount)
		{
			if (verticalAmount != ScrollAmount.NoAmount)
				throw new InvalidOperationException ();
			
			if (horizontalAmount == ScrollAmount.LargeIncrement ||
			    horizontalAmount == ScrollAmount.LargeDecrement)
				throw new ArgumentException ();

			if (tabControl.SelectedIndex == 0 &&
			    horizontalAmount == ScrollAmount.SmallDecrement)
				throw new ArgumentException ();

			if (tabControl.SelectedIndex == tabControl.TabCount - 1 &&
			    horizontalAmount == ScrollAmount.SmallIncrement)
				throw new ArgumentException ();

			// TODO:
//			if (horizontalAmount == ScrollAmount.SmallIncrement)
//				tabControl.SliderPos += 1;
//			else if (horizontalAmount == ScrollAmount.SmallDecrement)
//				tabControl.SliderPos -= 1;
//			else
//				return;
		}

		public void SetScrollPercent (double horizontalPercent, double verticalPercent)
		{
			if (verticalPercent != -1)
				throw new InvalidOperationException ();
			
			if (horizontalPercent > 100 || horizontalPercent < 0 && horizontalPercent != -1)
				throw new ArgumentOutOfRangeException ();

			try {
				Convert.ToDouble (horizontalPercent);
			} catch (InvalidCastException) {
				throw new ArgumentException ();
			}

			// TODO:
		}

		#endregion

		#region Private Fields

		private SWF.TabControl tabControl;

		#endregion
	}
}
