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
//      Calvin Gaisford <cgaisford@novell.com>
// 

using System;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{
	public class Pane : ComponentParentAdapter
	{
		private IRawElementProviderSimple 	provider;
		private ITransformProvider			transformProvider;
		private IDockProvider				dockProvider;
		

#region UI Automation Properties supported

		// AutomationElementIdentifiers.AutomationIdProperty.Id
		public string AutomationId
		{
			get {	
				return (string) provider.GetPropertyValue (AutomationElementIdentifiers.AutomationIdProperty.Id);
			}
		}
		
		// AutomationElementIdentifiers.BoundingRectangleProperty.Id
		public System.Windows.Rect BoundingRectangle
		{
			get {	
				return (System.Windows.Rect) provider.GetPropertyValue (AutomationElementIdentifiers.BoundingRectangleProperty.Id);
			}
		}
		
		// AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id
		public bool IsKeyboardFocusable
		{ 
			get {
				return (bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id);
			}
		}
		
		// AutomationElementIdentifiers.NameProperty.Id
		// already handled by the Atk object

		// AutomationElementIdentifiers.ClickablePointProperty.Id
		public System.Windows.Point ClickablePoint
		{ 
			get {
				return (System.Windows.Point) provider.GetPropertyValue (AutomationElementIdentifiers.ClickablePointProperty.Id);
			}
		}

		// AutomationElementIdentifiers.LabeledByProperty.Id
		public IRawElementProviderSimple LabeledBy
		{ 
			get {
				return (IRawElementProviderSimple) provider.GetPropertyValue (AutomationElementIdentifiers.LabeledByProperty.Id);
			}
		}

		// AutomationElementIdentifiers.ControlTypeProperty.Id
		public int ControlType
		{ 
			get {
				return (int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);
			}
		}

		// AutomationElementIdentifiers.LocalizedControlTypeProperty.Id
		public string LocalizedControlType
		{ 
			get {
				return (string) provider.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id);
			}
		}
		
		// AutomationElementIdentifiers.IsContentElementProperty.Id
		public bool IsContentElement
		{ 
			get {
				return (bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsContentElementProperty.Id);
			}
		}

		// AutomationElementIdentifiers.IsControlElementProperty.Id
		public bool IsControlElement
		{ 
			get {
				return (bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsControlElementProperty.Id);
			}
		}

		// AutomationElementIdentifiers.HelpTextProperty.Id
		public string HelpText
		{ 
			get {
				return (string) provider.GetPropertyValue (AutomationElementIdentifiers.HelpTextProperty.Id);
			}
		}
		
		// AutomationElementIdentifiers.AccessKeyProperty.Id
		public string AccessKey
		{ 
			get {
				return (string) provider.GetPropertyValue (AutomationElementIdentifiers.AccessKeyProperty.Id);
			}
		}

#endregion

		
		public Pane (IRawElementProviderSimple provider)
		{
			this.provider = provider;

			Role = Atk.Role.Panel;
			
			// The Pane doesn't have to have either of these
			if(provider is ITransformProvider) {
				transformProvider = (ITransformProvider)provider;
			} else if(provider is IDockProvider)  {
				dockProvider = (IDockProvider)provider;
			}
			
			Name = (string) provider.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id);
			
			if (IsKeyboardFocusable)
				RefStateSet ().AddState (Atk.StateType.Selectable);
			else
				RefStateSet ().RemoveState (Atk.StateType.Selectable);
		}
		
		public override IRawElementProviderSimple Provider {
			get { return provider; }
		}

		public override void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs e)
		{
			if (eventId == AutomationElementIdentifiers.AsyncContentLoadedEvent) {
				// TODO: Handle AsyncContentLoadedEvent
			} else if (eventId == AutomationElementIdentifiers.AutomationFocusChangedEvent) {
				// TODO: Handle AutomationFocusChangedEvent
			} else if (eventId == AutomationElementIdentifiers.StructureChangedEvent) {
				// TODO: Handle StructureChangedEvent
			}
		}

		public override void RaiseAutomationPropertyChangedEvent (AutomationPropertyChangedEventArgs e)
		{
		    if(e.Property == AutomationElementIdentifiers.BoundingRectangleProperty) {
		    	// TODO: Handle BoundingRectangleProperty change
		    } else if(e.Property == AutomationElementIdentifiers.IsOffscreenProperty) { 
				if((bool)e.NewValue)
					RefStateSet ().AddState (Atk.StateType.Visible);
				else
					RefStateSet ().RemoveState (Atk.StateType.Visible);
		    } else if(e.Property == AutomationElementIdentifiers.IsEnabledProperty) {
				if((bool)e.NewValue)
					RefStateSet ().AddState (Atk.StateType.Sensitive);
				else
					RefStateSet ().RemoveState (Atk.StateType.Sensitive);
		    } else if(e.Property == ScrollPatternIdentifiers.HorizontallyScrollableProperty) {
		        // TODO: Handle HorizontallyScrollableProperty change		    
		    } else if(e.Property == ScrollPatternIdentifiers.HorizontalScrollPercentProperty) {
		        // TODO: Handle HorizontalScrollPercentProperty	 change		    
		    } else if(e.Property == ScrollPatternIdentifiers.HorizontalViewSizeProperty) {
		        // TODO: Handle HorizontalViewSizeProperty	 change		    
		    } else if(e.Property == ScrollPatternIdentifiers.VerticalScrollPercentProperty) {
		        // TODO: Handle VerticalScrollPercentProperty	 change		    
		    } else if(e.Property == ScrollPatternIdentifiers.VerticallyScrollableProperty) {
		        // TODO: Handle VerticallyScrollableProperty	 change		    
		    } else if(e.Property == ScrollPatternIdentifiers.VerticalViewSizeProperty) {
		        // TODO: Handle VerticalViewSizeProperty	 change		    
		    } else if(e.Property == WindowPatternIdentifiers.WindowVisualStateProperty) {
		    	switch((WindowVisualState)e.NewValue)
		    	{
		    		case WindowVisualState.Normal:
		    			break;
		    		case WindowVisualState.Maximized:
		    			break;
		    		case WindowVisualState.Minimized:
		    			break;
		    	}
		        // TODO: Handle WindowVisualStateProperty	 change		    
		    }
		}
		
		public override void RaiseStructureChangedEvent (object childProvider, StructureChangedEventArgs e)
		{
			/*IRawElementProviderSimple simpleChildProvider =
				(IRawElementProviderSimple) childProvider;
			//TODO: remove elements
			if (e.StructureChangeType == StructureChangeType.ChildrenBulkAdded) {
				int controlTypeId = (int) simpleChildProvider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);
				if (controlTypeId == ControlType.Button.Id) {
					// TODO: Consider generalizing...
					Button button = new Button ((IInvokeProvider) childProvider);
					AddOneChild (button);
					AddRelationship (Atk.RelationType.Embeds, button);
					//TODO: add to mappings
				}
			}*/
		}
		

#region ComponentImplementor Methods

		public override bool Contains (int x, int y, Atk.CoordType coord_type)
		{
			// TODO: handle the coord_type?  Screen or Window
			return BoundingRectangle.Contains(new System.Windows.Point(x, y));
		}

		public override void GetExtents (out int x, out int y, out int width, out int height, Atk.CoordType coord_type)
		{
			// TODO: handle the coord_type?  Screen or Window
			x = (int)BoundingRectangle.Left;
			y = (int)BoundingRectangle.Right;
			width = (int)BoundingRectangle.Width;
			height = (int)BoundingRectangle.Height;
		}
		
		public override void GetPosition (out int x, out int y, Atk.CoordType coord_type)
		{
			// TODO: handle the coord_type?  Screen or Window
			x = (int)BoundingRectangle.Left;
			y = (int)BoundingRectangle.Right;
		}

		public override void GetSize (out int width, out int height)
		{
			width = (int)BoundingRectangle.Width;
			height = (int)BoundingRectangle.Height;
		}
		
		public override bool GrabFocus ()
		{
			//TODO: Implement GrabFocus
			return false;
		}
		
		public override Atk.Object RefAccessibleAtPoint (int x, int y, Atk.CoordType coord_type)
		{
			//TODO: check for children at this point?
			return this;
		}
		
		public override bool SetExtents (int x, int y, int width, int height, Atk.CoordType coord_type)
		{
			if( (transformProvider != null) && (transformProvider.CanResize) && (transformProvider.CanMove) ) {
				transformProvider.Move(x, y);
				transformProvider.Resize(width, height);
				return true;
			}
			return false;
		}
		
		public override bool SetPosition (int x, int y, Atk.CoordType coord_type)
		{
			if( (transformProvider != null) && (transformProvider.CanMove) ) {
				transformProvider.Move(x, y);
				return true;
			}
			return false;
		}
		
		public override bool SetSize (int width, int height)
		{
			if( (transformProvider != null) && (transformProvider.CanResize) ) {
				transformProvider.Resize(width, height);
				return true;
			}
			return false;
		}
		
#endregion
		
		
		
	}	
}