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
	//NOTE: 
	//     This class is meant to be used by providers supporting Scroll Pattern.
	//     Updates Navigation and generates event to indicate whether should
	//     support ScrollPattern or not.
	internal class ScrollBehaviorObserver : IScrollBehaviorObserver
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
					hscrollbar.EnabledChanged -= UpdateHScrollBehavior;
					hscrollbar.VisibleChanged -= UpdateHScrollBehavior;
				}
				
				hscrollbar = value;
				if (hscrollbar != null) {
					hscrollbar.EnabledChanged += UpdateHScrollBehavior;
					hscrollbar.VisibleChanged += UpdateHScrollBehavior;
				}
			}
		}
		
		public SWF.ScrollBar VerticalScrollBar {
			get { return vscrollbar; }
			set { 
				if (vscrollbar == value)
					return;
				
				if (vscrollbar != null) {
					vscrollbar.EnabledChanged -= UpdateVScrollBehavior;
					vscrollbar.VisibleChanged -= UpdateVScrollBehavior;
				}
				
				vscrollbar = value;
				if (vscrollbar != null) {
					vscrollbar.EnabledChanged += UpdateVScrollBehavior;
					vscrollbar.VisibleChanged += UpdateVScrollBehavior;
				}
			}
		}
		
		public bool SupportsScrollPattern {
			get { return HasHorizontalScrollbar || HasVerticalScrollbar; }
		}		
		
		#endregion
		
		#region Public Events
		
		public EventHandler ScrollPatternSupportChanged;
		
		#endregion
		
		#region Public Methods
		
		public void InitializeScrollBarProviders ()
		{
			if (HasHorizontalScrollbar == true)
				RaiseNavigationEvent (StructureChangeType.ChildAdded,
				                      ref hscrollbarProvider,
				                      HorizontalScrollBar,
				                      false);
			if (HasVerticalScrollbar == true)
				RaiseNavigationEvent (StructureChangeType.ChildAdded,
				                      ref vscrollbarProvider,
				                      VerticalScrollBar,
				                      false);
		}

		public void FinalizeScrollBarProviders ()
		{
			if (hscrollbarProvider != null) {
				subject.RemoveChildProvider (false, hscrollbarProvider);
				hscrollbarProvider.Terminate ();
				hscrollbarProvider = null;
			}

			if (vscrollbarProvider != null) {
				subject.RemoveChildProvider (false, vscrollbarProvider);
				vscrollbarProvider.Terminate ();
				vscrollbarProvider = null;
			}
		}
		
		#endregion
		
		#region Protected Methods
	
		protected void OnScrollPatternSupportChanged ()
		{
			if (ScrollPatternSupportChanged != null)
				ScrollPatternSupportChanged (this, EventArgs.Empty);
			
			scrollpatternSet = !scrollpatternSet;
		}

		#endregion
		
		#region Private Methods
		
		private void UpdateHScrollBehavior (object sender, EventArgs args)
		{
			//Updating Navigation
			if (HasHorizontalScrollbar == true)
				UpdateScrollbarNavigation (HorizontalScrollBar, true);
			else
				UpdateScrollbarNavigation (HorizontalScrollBar, false);
			
			//Updating Behavior
			if (scrollpatternSet == false) {
				if (HasHorizontalScrollbar == true) {
					OnScrollPatternSupportChanged ();
				}
			} else {
				if (HasHorizontalScrollbar == false && HasVerticalScrollbar == false)
					OnScrollPatternSupportChanged ();
			}
		}

		private void UpdateVScrollBehavior (object sender, EventArgs args)
		{
			//Updating Navigation			
			if (HasVerticalScrollbar == true)
				UpdateScrollbarNavigation (VerticalScrollBar, true);
			else
				UpdateScrollbarNavigation (VerticalScrollBar, false);
			
			//Updating Behavior
			if (scrollpatternSet == false) {
				if (HasVerticalScrollbar == true)
					OnScrollPatternSupportChanged ();
			} else  {
				if (HasVerticalScrollbar == false && HasHorizontalScrollbar == false)
					OnScrollPatternSupportChanged ();
			}
		}
		
		#endregion
		
		#region Private Methods: Navigation
		
		private void UpdateScrollbarNavigation (SWF.ScrollBar scrollbar,
		                                        bool navigable)
		{
			if (scrollbar == vscrollbar) {
	           if (navigable == false && vscrollbarProvider != null)
					RaiseNavigationEvent (StructureChangeType.ChildRemoved,
					                      ref vscrollbarProvider,
					                      vscrollbar,
					                      true);
	           else if (navigable == true && vscrollbarProvider == null)
					RaiseNavigationEvent (StructureChangeType.ChildAdded,
					                      ref vscrollbarProvider,
					                      vscrollbar,
					                      true);
			} else if (scrollbar == hscrollbar) {
	           if (navigable == false && hscrollbarProvider != null)
					RaiseNavigationEvent (StructureChangeType.ChildRemoved,
					                      ref hscrollbarProvider,
					                      hscrollbar,
					                      true);
	           else if (navigable == true && hscrollbarProvider == null)
					RaiseNavigationEvent (StructureChangeType.ChildAdded,
					                      ref hscrollbarProvider,
					                      hscrollbar,
					                      true);
			}
		}
		
		private void RaiseNavigationEvent (StructureChangeType type,
		                                   ref FragmentControlProvider provider,
		                                   SWF.ScrollBar scrollbar,
		                                   bool generateEvent)
		{
			if (type == StructureChangeType.ChildAdded) {
				provider = subject.GetScrollbarProvider (scrollbar);
				provider.InitializeEvents ();
				subject.AddChildProvider (generateEvent, provider);
			} else {
				subject.RemoveChildProvider (generateEvent, provider);
				provider.Terminate ();
				provider = null;
			}
		}
		
		#endregion

		#region Private Fields
		
		private SWF.ScrollBar hscrollbar;
		private SWF.ScrollBar vscrollbar;
		private FragmentControlProvider hscrollbarProvider;
		private FragmentControlProvider vscrollbarProvider;
		private bool scrollpatternSet;
		private IScrollBehaviorSubject subject;
		
		#endregion
	}

}
