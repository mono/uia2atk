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
//	Mike Gorse <mgorse@novell.com>
// 
using System;
using System.Drawing;				                                              
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;
using System.ComponentModel;
using Mono.Unix;
using Mono.UIAutomation.Winforms.Behaviors;
using Mono.UIAutomation.Winforms.Behaviors.TrackBar;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Navigation;

namespace Mono.UIAutomation.Winforms
{
	[MapsComponent (typeof (TrackBar))]
	internal class TrackBarProvider : FragmentRootControlProvider
	{
		
		#region Constructor

		public TrackBarProvider (TrackBar trackBar) : base (trackBar)
		{
			this.trackBar = trackBar;
		}
		
		#endregion

		#region Static Methods

		#endregion

		#region Public Methods
		
		public FragmentControlProvider GetChildButtonProvider (TrackBarButtonOrientation orientation)
		{
			if (orientation == TrackBarButtonOrientation.LargeBack)
				return largeBackButton;
			else	// LargeForward
				return largeForwardButton;
		}
		
		#endregion

		#region SimpleControlProvider: Specializations

		public override void Initialize ()
		{
			base.Initialize ();
			
			SetBehavior (RangeValuePatternIdentifiers.Pattern,
			             new RangeValueProviderBehavior (this));
		}
		
		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.Slider.Id;
			else if (propertyId == AutomationElementIdentifiers.IsContentElementProperty.Id)
				return true;
			else if (propertyId == AutomationElementIdentifiers.OrientationProperty.Id)
				return trackBar.Orientation == Orientation.Horizontal
					? OrientationType.Horizontal : OrientationType.Vertical;
			else
				return base.GetProviderPropertyValue (propertyId);
		}

		#endregion
		
		#region FragmentRootControlProvider: Specializations
		
		public override void InitializeChildControlStructure ()
		{
			TrackBar trackbar = (TrackBar) Control;

			if (largeBackButton == null) {
				largeBackButton = new TrackBarButtonProvider (trackbar,
				                                               TrackBarButtonOrientation.LargeBack);
				AddChildProvider (largeBackButton);
			}
			if (thumb == null) {
				thumb = new TrackBarThumbProvider (trackbar);
				AddChildProvider (thumb);
			}
			if (largeForwardButton == null) {
				largeForwardButton = new TrackBarButtonProvider (trackbar,
				                                                  TrackBarButtonOrientation.LargeForward);
				AddChildProvider (largeForwardButton);
			}
		}
		
		public override void FinalizeChildControlStructure ()
		{
			if (largeBackButton != null) {
				largeBackButton.Terminate ();
				largeBackButton = null;
			}
			if (thumb != null) {
				thumb.Terminate ();
				thumb = null;
			}
			if (largeForwardButton != null) {
				largeForwardButton.Terminate ();
				largeForwardButton = null;
			}
		}
		
		#endregion

		#region Private Fields

		private FragmentControlProvider largeBackButton;
		private FragmentControlProvider largeForwardButton;
		private FragmentControlProvider thumb;
		private TrackBar trackBar;
		
		#endregion
		
		#region Internal Enumeration: Button Orientation
		
		internal enum TrackBarButtonOrientation
		{
			LargeBack,
			LargeForward
		}
		
		#endregion

		#region Internal Class: Button Provider
		
		internal class TrackBarButtonProvider : FragmentControlProvider
		{
		
			public TrackBarButtonProvider (TrackBar trackbarContainer,
			                                TrackBarButtonOrientation orientation)
				: base (trackbarContainer) 
			{
				this.orientation = orientation;
				this.trackbarContainer = trackbarContainer;
				
				SetBehavior (InvokePatternIdentifiers.Pattern, 
				             new ButtonInvokeProviderBehavior (this));
			}
			
			public override IRawElementProviderFragmentRoot FragmentRoot {
				get { 
					return (IRawElementProviderFragmentRoot) ProviderFactory.FindProvider (Control);
				}
			}
	
			public TrackBarButtonOrientation Orientation {
				get { return orientation; }
			}
			
			public TrackBar TrackBarContainer {
				get { return trackbarContainer; }
			}
	
			protected override object GetProviderPropertyValue (int propertyId)
			{
				//TODO: We may need to get VALID information using Reflection
				if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
					return GetNameFromOrientation ();
				else if (propertyId == AutomationElementIdentifiers.IsContentElementProperty.Id)
					return false;
				else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
					return false;				
				else if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
					return ControlType.Button.Id;
				else
					return base.GetProviderPropertyValue (propertyId);
			}
			
			private string GetNameFromOrientation ()
			{
				//TODO: Should we use generalization? Should this be translatable?
				if (orientation == TrackBarButtonOrientation.LargeBack)
					return "Back by large amount";
				else	// LargeForward
					return "Forward by large amount";
			}		
	
			private TrackBarButtonOrientation orientation;
			private TrackBar trackbarContainer;
		}

		#endregion
		
		#region Internal Class: Thumb Provider
		
		internal class TrackBarThumbProvider : FragmentControlProvider
		{
	
			public TrackBarThumbProvider (TrackBar trackbar) : base (trackbar)
			{
				runtimeId = -1;
			}
			
			public override IRawElementProviderFragmentRoot FragmentRoot {
				get { 
					return (IRawElementProviderFragmentRoot) ProviderFactory.FindProvider (Control);
				}
			}
	
			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.AutomationIdProperty.Id) {
					if (runtimeId == -1)
						runtimeId = Helper.GetUniqueRuntimeId ();
					return runtimeId.ToString ();
				} else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
					return "Thumb";
				else if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
					return ControlType.Thumb.Id;
				else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
					return false;
				else if (propertyId == AutomationElementIdentifiers.HasKeyboardFocusProperty.Id)
					return false;
				else if (propertyId == AutomationElementIdentifiers.IsContentElementProperty.Id)
					return false;
				else
					return base.GetProviderPropertyValue (propertyId);
			}

			protected override System.Drawing.Rectangle ScreenBounds {
				get {
					System.Drawing.Rectangle thumbArea
						= ((TrackBar) Control).ThumbArea;
	
					if (Control.Parent == null || Control.TopLevelControl == null)
						return thumbArea;
					else {
						if (Control.FindForm () == Control.Parent)
							return Control.TopLevelControl.RectangleToScreen (thumbArea);
						else
							return Control.Parent.RectangleToScreen (thumbArea);
					}
				}
			}
			
			private int runtimeId;
			
		}
		
		#endregion

	}
}
