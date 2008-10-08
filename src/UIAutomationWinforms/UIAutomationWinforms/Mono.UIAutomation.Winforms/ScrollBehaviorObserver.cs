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
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Winforms.Navigation;

namespace Mono.UIAutomation.Winforms
{
	internal class ScrollBehaviorObserver
	{
		
		#region Constructors
		
		public ScrollBehaviorObserver (IScrollBehaviorSubject subject,
		                               SWF.ScrollBar horizontal,
		                               SWF.ScrollBar vertical)
		{
			this.subject = subject;
			HorizontalScrollBar = horizontal;
			VerticalScrollBar = vertical;
			scrollpatternSet = false;
			
			if (SupportsScrollPattern == true)
				OnScrollPatternSupportChanged ();
		}
		
		#endregion
		
		#region Public Properties

		
		public bool HasHorizontalScrollbar {
			get {
				return HorizontalScrollBar != null 
					&& HorizontalScrollBar.Visible && HorizontalScrollBar.Enabled; 
			}
		}
		
		public bool HasVerticalScrollbar {
			get { 
				return VerticalScrollBar != null 
					&& VerticalScrollBar.Visible && VerticalScrollBar.Enabled; 
			}
		}
	
		public SWF.ScrollBar HorizontalScrollBar {
			get { return hscrollbar; }
			set {
				if (hscrollbar == value)
					return;
				
				if (hscrollbar != null) {
					hscrollbar.VisibleChanged -= UpdateHScrollBehaviorVisible;
					hscrollbar.EnabledChanged -= UpdateHScrollBehaviorEnable;
				}
				
				hscrollbar = value;
				if (hscrollbar != null) {
					hscrollbar.VisibleChanged += UpdateHScrollBehaviorVisible;
					hscrollbar.EnabledChanged += UpdateHScrollBehaviorEnable;
				}
			}
		}
		
		public SWF.ScrollBar VerticalScrollBar {
			get { return vscrollbar; }
			set { 
				if (vscrollbar == value)
					return;
				
				if (vscrollbar != null) {
					vscrollbar.VisibleChanged -= UpdateVScrollBehaviorVisible;
					vscrollbar.EnabledChanged -= UpdateVScrollBehaviorEnable;
				}
				
				vscrollbar = value;
				if (vscrollbar != null) {
					vscrollbar.VisibleChanged += UpdateVScrollBehaviorVisible;
					vscrollbar.EnabledChanged += UpdateVScrollBehaviorEnable;
				}
			}
		}
		
		public bool SupportsScrollPattern {
			get { return HasHorizontalScrollbar || HasVerticalScrollbar; }
		}		
		
		#endregion
		
		#region Public Events
		
		public NavigationEventHandler HorizontalNavigationUpdated;
		
		public NavigationEventHandler VerticalNavigationUpdated;
		
		public EventHandler ScrollPatternSupportChanged;
		
		#endregion
		
		#region Protected Methods

		protected void OnHorizontalNavigationUpdated (NavigationEventArgs args)
		{
			if (HorizontalNavigationUpdated != null)
				HorizontalNavigationUpdated (this, args);
		}
	
		protected void OnScrollPatternSupportChanged ()
		{
			if (ScrollPatternSupportChanged != null)
				ScrollPatternSupportChanged (this, EventArgs.Empty);
			scrollpatternSet = !scrollpatternSet;
		}
		
		protected void OnVerticalNavigationUpdated (NavigationEventArgs args)
		{
			if (VerticalNavigationUpdated != null)
				VerticalNavigationUpdated (this, args);
		}
		
		#endregion
		
		#region Private Methods
		
		private void UpdateHScrollBehaviorEnable (object sender, EventArgs args)
		{
			//Event used to update Navigation chain
			if (subject.SupportsHorizontalScrollbar == true 
			    && HorizontalScrollBar.Visible == true && HorizontalScrollBar.Enabled == true)
				OnHorizontalNavigationUpdated (new NavigationEventArgs (StructureChangeType.ChildAdded,
				                                                        HorizontalScrollBar));
			else
				OnHorizontalNavigationUpdated (new NavigationEventArgs (StructureChangeType.ChildRemoved,
				                                                        HorizontalScrollBar));
			
			//Updating Behavior
			if (HorizontalScrollBar.Enabled == true) {
				if (scrollpatternSet == false) {
					if (subject.SupportsHorizontalScrollbar == true)
						OnScrollPatternSupportChanged ();
				}
			} else {
				if (scrollpatternSet == true) {
					if (vscrollbar.Visible == true && vscrollbar.Enabled == true)
						return;

					OnScrollPatternSupportChanged ();
				}
			}
		}
		
		private void UpdateHScrollBehaviorVisible (object sender, EventArgs args)
		{
			//Event used to update Navigation chain
			if (subject.SupportsHorizontalScrollbar == true 
			    && HorizontalScrollBar.Visible == true && HorizontalScrollBar.Enabled == true)
				OnHorizontalNavigationUpdated (new NavigationEventArgs (StructureChangeType.ChildAdded,
				                                                        HorizontalScrollBar));
			else
				OnHorizontalNavigationUpdated (new NavigationEventArgs (StructureChangeType.ChildRemoved,
				                                                        HorizontalScrollBar));
			
			//Updating Behavior
			if (scrollpatternSet == false) {
				if (HorizontalScrollBar.Visible == true) {
					if (HorizontalScrollBar.Enabled == false 
					    || subject.SupportsHorizontalScrollbar == false)
						return;

					OnScrollPatternSupportChanged ();
				}
			} else {
				if (HorizontalScrollBar.Visible == false) {
					if (vscrollbar.Visible == true && vscrollbar.Enabled)
						return;

					OnScrollPatternSupportChanged ();
				}
			}
		}
		
		private void UpdateVScrollBehaviorEnable (object sender, EventArgs args)
		{
			//Event used to update Navigation chain
			if (subject.SupportsVerticalScrollbar == true
			    && VerticalScrollBar.Visible == true && VerticalScrollBar.Enabled == true)
				OnVerticalNavigationUpdated (new NavigationEventArgs (StructureChangeType.ChildAdded,
				                                                      VerticalScrollBar));
			else
				OnVerticalNavigationUpdated  (new NavigationEventArgs (StructureChangeType.ChildRemoved,
				                                                       VerticalScrollBar));
			
			//Updating Behavior
			if (VerticalScrollBar.Visible == true && scrollpatternSet == false)
				OnScrollPatternSupportChanged ();
			else if (VerticalScrollBar.Visible == true && scrollpatternSet == true) {
				if (HorizontalScrollBar.Visible == true && HorizontalScrollBar.Enabled == true)
					return;
				OnScrollPatternSupportChanged ();
			}
		}		
		
		private void UpdateVScrollBehaviorVisible (object sender, EventArgs args)
		{
			//Event used to update Navigation chain
			if (subject.SupportsVerticalScrollbar == false
			    && VerticalScrollBar.Enabled == true && VerticalScrollBar.Visible == true)
				OnVerticalNavigationUpdated (new NavigationEventArgs (StructureChangeType.ChildAdded,
				                                                      VerticalScrollBar));
			else
				OnVerticalNavigationUpdated (new NavigationEventArgs (StructureChangeType.ChildAdded,
				                                                      VerticalScrollBar));
			
			//Updating Behavior
			if (scrollpatternSet == false) {
				if (VerticalScrollBar.Visible == true) {
					if (VerticalScrollBar.Enabled == false)
						return;
					OnScrollPatternSupportChanged ();
				}
			} else {
				if (VerticalScrollBar.Visible == false) {
					if (HorizontalScrollBar.Visible == true && HorizontalScrollBar.Enabled == true)
						return;
					OnScrollPatternSupportChanged ();
				}
			}
		}
		
		#endregion

		#region Private Fields
		
		private SWF.ScrollBar hscrollbar;
		private SWF.ScrollBar vscrollbar;
		private bool scrollpatternSet;
		private IScrollBehaviorSubject subject;
		
		#endregion
	}

}
