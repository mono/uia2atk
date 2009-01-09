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
using SD = System.Drawing;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using SWF = System.Windows.Forms;
using System.Windows;
using System.Reflection;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;

namespace Mono.UIAutomation.Winforms.Behaviors.Generic
{

	internal abstract class ScrollProviderBehavior<T> 
		: ProviderBehavior, IScrollProvider 
			where T : FragmentControlProvider, IScrollBehaviorSubject
	{
		
		#region Constructors

		protected ScrollProviderBehavior (T subject) : base (subject)
		{
			this.subject = subject;
			hscrollbar = subject.ScrollBehaviorObserver.HorizontalScrollBar;
			vscrollbar = subject.ScrollBehaviorObserver.VerticalScrollBar;
		}

		#endregion
		
		#region IProviderBehavior Interface

		public override AutomationPattern ProviderPattern { 
			get { return ScrollPatternIdentifiers.Pattern; }
		}
		
		public override void Disconnect ()
		{		
			Provider.SetEvent (ProviderEventType.ScrollPatternHorizontallyScrollableProperty,
			                   null);
			Provider.SetEvent (ProviderEventType.ScrollPatternHorizontalScrollPercentProperty,
			                   null);
			Provider.SetEvent (ProviderEventType.ScrollPatternHorizontalViewSizeProperty,
			                   null);
			Provider.SetEvent (ProviderEventType.ScrollPatternVerticallyScrollableProperty,
			                   null);
			Provider.SetEvent (ProviderEventType.ScrollPatternVerticalScrollPercentProperty,
			                   null);
			Provider.SetEvent (ProviderEventType.ScrollPatternVerticalViewSizeProperty,
			                   null);
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
			get { return subject.ScrollBehaviorObserver.HasHorizontalScrollbar; }
		}
		
		//FIXME: This MUST BE locale-specific
		public double HorizontalScrollPercent {
			get { 
				if (HorizontallyScrollable == false || hscrollbar.Maximum == 0)
					return ScrollPatternIdentifiers.NoScroll;
				else
					return (hscrollbar.Value * 100) / hscrollbar.Maximum; 
			}
		}
		
		public double HorizontalViewSize {
			get { 
				if (HorizontallyScrollable == false)
					return 100;
				else {
					SD.Rectangle thumbArea = hscrollbar.UIAThumbPosition;
					return ((thumbArea.Width + (thumbArea.Height * 2)) * 100)
						/ Provider.Control.Width;
				}
			}
		}

		public bool VerticallyScrollable {
			get { return subject.ScrollBehaviorObserver.HasVerticalScrollbar; }
		}

		//FIXME: This MUST BE locale-specific
		public double VerticalScrollPercent {
			get { 
				if (VerticallyScrollable == false || vscrollbar.Maximum == 0)
					return ScrollPatternIdentifiers.NoScroll;
				else
					return (vscrollbar.Value * 100) / vscrollbar.Maximum; 
			}
		}

		public double VerticalViewSize {
			get { 
				if (VerticallyScrollable == false)
					return 100;
				else {
					SD.Rectangle thumbArea = hscrollbar.UIAThumbPosition;
					return ((thumbArea.Height+ (thumbArea.Width * 2)) * 100)
						/ Provider.Control.Height;
				}
			}
		}
		
		public void Scroll (ScrollAmount horizontalAmount, 
		                    ScrollAmount verticalAmount)
		{
			if (horizontalAmount != ScrollAmount.NoAmount
			    && HorizontallyScrollable)
				ScrollByAmount (hscrollbar, GetAmountString (horizontalAmount));
			if (verticalAmount != ScrollAmount.NoAmount
			    && VerticallyScrollable)
				ScrollByAmount (vscrollbar, GetAmountString (verticalAmount));
		}

		public void SetScrollPercent (double horizontalPercent, 
		                              double verticalPercent)
		{
			if (horizontalPercent != ScrollPatternIdentifiers.NoScroll) {
				if (horizontalPercent < 0 || horizontalPercent > 100)
					throw new ArgumentOutOfRangeException ();
				else
					PerformScrollByPercent (hscrollbar, (int) ((horizontalPercent * hscrollbar.Maximum) / 100));
			}
			
			if (verticalPercent != ScrollPatternIdentifiers.NoScroll) {
			    if (verticalPercent < 0 || verticalPercent > 100)
					throw new ArgumentOutOfRangeException ();
				else 
					PerformScrollByPercent (vscrollbar, (int) ((verticalPercent * vscrollbar.Maximum) / 100));
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
		
		private void ScrollByAmount (SWF.ScrollBar scrollbar, string method) 
		{
			if (method == string.Empty)
				return;
			
			MethodInfo methodInfo = typeof (SWF.ScrollBar).GetMethod (method,
			                                                          BindingFlags.InvokeMethod
			                                                          | BindingFlags.NonPublic
			                                                          | BindingFlags.Instance);
			invoke
				= (Action<SWF.ScrollBar>) Delegate.CreateDelegate (typeof (Action<SWF.ScrollBar>), 
				                                                   methodInfo);
			PerformScrollByAmount (scrollbar);
		}
		
		private void PerformScrollByAmount (SWF.ScrollBar scrollbar)
		{
			if (scrollbar.InvokeRequired == true) {
				scrollbar.BeginInvoke (new ScrollByAmountDelegate (PerformScrollByAmount),
				                       new object [] { scrollbar });
				return;
			}
			invoke (scrollbar);
			invoke = null;
		}
		
		private void PerformScrollByPercent (SWF.ScrollBar scrollbar, int value)
		{
			if (scrollbar.InvokeRequired == true) {
				scrollbar.BeginInvoke (new ScrollByPercentDelegate (PerformScrollByPercent),
				                       new object [] { scrollbar, value });
				return;
			}
			scrollbar.Value = value;
		}

		#endregion
		
		#region Private Fields
		
		private SWF.ScrollBar hscrollbar;
		private SWF.ScrollBar vscrollbar;
		private Action<SWF.ScrollBar> invoke;
		private IScrollBehaviorSubject subject;
		
		#endregion

	}
	
	delegate void ScrollByAmountDelegate (SWF.ScrollBar scrollbar);
	delegate void ScrollByPercentDelegate (SWF.ScrollBar scrollbar, int value);
				                      
}
