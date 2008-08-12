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
using System.Drawing;				                                              
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;
using Mono.UIAutomation.Winforms.Behaviors;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Navigation;

namespace Mono.UIAutomation.Winforms
{

	internal class ScrollBarProvider : FragmentRootControlProvider
	{
		
		#region Constructor

		public ScrollBarProvider (ScrollBar scrollbar) : base (scrollbar)
		{
			orientation = scrollbar is HScrollBar 
				? OrientationType.Horizontal : OrientationType.Vertical;
			Control.ParentChanged += new EventHandler (OnParentChanged);
			UpdateBehavior ();
		}

		#endregion

		#region Public Methods
		
		public FragmentControlProvider GetChildButtonProvider (ScrollBarButtonOrientation orientation)
		{
			if (orientation == ScrollBarButtonOrientation.LargeBack)
				return largeBackButton;
			else if (orientation == ScrollBarButtonOrientation.LargeForward)
				return largeForwardButton;
			else if (orientation == ScrollBarButtonOrientation.SmallBack)
				return smallBackButton;
			else //Is SmallForward
				return smallForwardButton;
		}
		
		public FragmentControlProvider GetChildThumbProvider ()
		{
			return thumb;
		}
		
		#endregion

		#region SimpleControlProvider: Specializations
		
		public override void Terminate ()
		{
			base.Terminate ();
			
			Control.ParentChanged -= new EventHandler (OnParentChanged);
		}
		
		public override void InitializeEvents ()
		{
			base.InitializeEvents (); 

			SetEvent (ProviderEventType.FocusChangedEvent,
			          new AutomationFocusChangedEvent (this));
		}
		
		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.ScrollBar.Id;
			else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
				return "scroll bar";
			else if (propertyId == AutomationElementIdentifiers.ClickablePointProperty.Id)
				return Single.NaN;
			else if (propertyId == AutomationElementIdentifiers.LabeledByProperty.Id)
				return null;
			else if (propertyId == AutomationElementIdentifiers.IsContentElementProperty.Id)
				return false;
			else if (propertyId == AutomationElementIdentifiers.OrientationProperty.Id)
				return orientation;
			else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
				return null;
			else
				return base.GetPropertyValue (propertyId);
		}

		#endregion
		
		#region FragmentRootControlProvider: Specializations
		
		public override void InitializeChildControlStructure ()
		{
			ScrollBar scrollbar = (ScrollBar) Control;

			if (smallBackButton == null) {
				smallBackButton = new ScrollBarButtonProvider (scrollbar,
				                                               ScrollBarButtonOrientation.SmallBack);
				OnNavigationChildAdded (false, smallBackButton);
			}
			if (smallForwardButton == null) {
				smallForwardButton = new ScrollBarButtonProvider (scrollbar,
				                                                  ScrollBarButtonOrientation.SmallForward);
				OnNavigationChildAdded (false, smallForwardButton);
			}
			if (thumb == null) {
				thumb = new ScrollBarThumbProvider (scrollbar);
				OnNavigationChildAdded (false, thumb);
			}
			if (largeBackButton == null) {
				largeBackButton = new ScrollBarButtonProvider (scrollbar,
				                                               ScrollBarButtonOrientation.LargeBack);
				OnNavigationChildAdded (false, largeBackButton);
			}
			if (largeForwardButton == null) {
				largeForwardButton = new ScrollBarButtonProvider (scrollbar,
				                                                  ScrollBarButtonOrientation.LargeForward);
				OnNavigationChildAdded (false, largeForwardButton);
			}
		}
		
		public override void FinalizeChildControlStructure ()
		{
			if (smallBackButton != null) {
				smallBackButton.Terminate ();
				smallBackButton = null;
			}
			if (smallForwardButton != null) {
				smallForwardButton.Terminate ();
				smallForwardButton = null;
			}
			if (largeBackButton != null) {
				largeBackButton.Terminate ();
				largeBackButton = null;
			}
			if (largeForwardButton != null) {
				largeForwardButton.Terminate ();
				largeForwardButton = null;
			}
			if (thumb != null) {
				thumb.Terminate ();
				thumb = null;
			}
		}
		
		#endregion

		#region Private Methods
		
		private void OnParentChanged (object sender, EventArgs args)
		{
			UpdateBehavior ();
		}
		
		private void UpdateBehavior ()
		{	
			IRawElementProviderFragment container 
				= ProviderFactory.GetProvider (Container);

			if (container != null) {
				IScrollProvider provider 
					= (IScrollProvider) container.GetPatternProvider (ScrollPatternIdentifiers.Pattern.Id);
				if (provider == null)
					SetBehavior (RangeValuePatternIdentifiers.Pattern,
					             new ScrollBarRangeValueBehavior (this));
				else
					SetBehavior (RangeValuePatternIdentifiers.Pattern, null);
			} else
				//TODO: Is this default behavior OK?
				SetBehavior (RangeValuePatternIdentifiers.Pattern,
				             new ScrollBarRangeValueBehavior (this));
		}

		#endregion
	
		#region Private Fields

		private FragmentControlProvider largeBackButton;
		private FragmentControlProvider largeForwardButton;
		private FragmentControlProvider smallBackButton;
		private FragmentControlProvider smallForwardButton;
		private FragmentControlProvider thumb;
		private OrientationType orientation;
		
		#endregion
		
		#region Internal Enumeration: Button Orientation
		
		internal enum ScrollBarButtonOrientation
		{
			SmallBack,
			SmallForward,
			LargeBack,
			LargeForward
		}
		
		#endregion

		#region Internal Class: Button Provider
		
		internal class ScrollBarButtonProvider : FragmentControlProvider
		{
		
			public ScrollBarButtonProvider (ScrollBar scrollbarContainer,
			                                ScrollBarButtonOrientation orientation)
				: base (scrollbarContainer) 
			{
				this.orientation = orientation;
				this.scrollbarContainer = scrollbarContainer;
				
				SetBehavior (InvokePatternIdentifiers.Pattern, 
				             new ScrollBarButtonInvokeProviderBehavior (this));
			}
	
			public ScrollBarButtonOrientation Orientation {
				get { return orientation; }
			}
			
			public ScrollBar ScrollBarContainer {
				get { return scrollbarContainer; }
			}
	
			public override object GetPropertyValue (int propertyId)
			{
				//TODO: We may need to get VALID information using Reflection
				if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
					return GetNameFromOrientation ();
				else if (propertyId == AutomationElementIdentifiers.IsContentElementProperty.Id)
					return false;
				else
					return base.GetPropertyValue (propertyId);
			}
			
			private string GetNameFromOrientation ()
			{
				//TODO: Should we use generalization? Should this be translatable?
				if (orientation == ScrollBarButtonOrientation.LargeBack)
					return "Back by large amount";
				else if (orientation == ScrollBarButtonOrientation.LargeForward)
					return "Forward by large amount";
				else if (orientation == ScrollBarButtonOrientation.SmallBack)
					return "Back by small amount";
				else //Should be ScrollBarButtonOrientation.SmallForward
					return "Forward by small amount";
			}		
	
			private ScrollBarButtonOrientation orientation;
			private ScrollBar scrollbarContainer;
		}

		#endregion
		
		#region Internal Class: Thumb Provider
		
		internal class ScrollBarThumbProvider : FragmentControlProvider
		{
	
			public ScrollBarThumbProvider (ScrollBar scrollbar) : base (scrollbar)
			{
				runtimeId = -1;
			}
	
			public override object GetPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.AutomationIdProperty.Id) {
					if (runtimeId == -1)
						runtimeId = Helper.GetUniqueRuntimeId ();
	
					return runtimeId;
				} else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
					return "Thumb";
				else if (propertyId == AutomationElementIdentifiers.BoundingRectangleProperty.Id)
					return GetThumbArea ();
				else if (propertyId == AutomationElementIdentifiers.ClickablePointProperty.Id)
					return null; //TODO: We may need to use Reflection to get the "real" value
				else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
					return true;
				else if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
					return ControlType.Thumb.Id;
				else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
					return "thumb";
				else if (propertyId == AutomationElementIdentifiers.IsContentElementProperty.Id)
					return false;
				else if (propertyId == AutomationElementIdentifiers.IsControlElementProperty.Id)
					return true;
				else
					return null;
			}
			
			private Rect GetThumbArea ()
			{
				Rectangle thumbArea = (Rectangle) Helper.GetPrivateField (typeof (ScrollBar), 
				                                                          Control,
				                                                          "thumb_area");
				return Helper.RectangleToRect (thumbArea);
			}
			
			
			private int runtimeId;
			
		}
		
		#endregion

	}
}
