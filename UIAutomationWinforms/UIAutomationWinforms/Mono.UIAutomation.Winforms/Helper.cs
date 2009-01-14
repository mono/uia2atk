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
using System.Reflection;
using System.Windows;
using SWF = System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace Mono.UIAutomation.Winforms
{

	internal static class Helper
	{

		#region Internal Static Methods

		internal static object GetDefaultAutomationPropertyValue (int propertyId)
		{
			// The idea of this method is to avoid crashing when the bridge
			// expects non-null values.
			
			// TODO: Some properties define default values, what about those whose dont'?
			if (propertyId == AutomationElementIdentifiers.IsOffscreenProperty.Id)
				return true;
			else if (propertyId == AutomationElementIdentifiers.HasKeyboardFocusProperty.Id)
				return false;
			else if (propertyId == AutomationElementIdentifiers.BoundingRectangleProperty.Id)
				return Rect.Empty;
			else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
				return false;
			else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
				return string.Empty;
			else if (propertyId == AutomationElementIdentifiers.IsEnabledProperty.Id)
				return true;
			else
				return null;
		}

		internal static Rect GetControlScreenBounds (Rectangle bounds, SWF.Control control)
		{
			return GetControlScreenBounds (bounds, control, false);
		}

		internal static Rect GetControlScreenBounds (Rectangle bounds, SWF.Control control, bool controlIsParent)
		{
			if (controlIsParent)
				return Helper.RectangleToRect (control.RectangleToScreen (bounds));
			if (control.Parent == null || control.TopLevelControl == null)
				return Helper.RectangleToRect (bounds);
			else {
				if (control.FindForm () == control.Parent)
					return Helper.RectangleToRect (control.TopLevelControl.RectangleToScreen (bounds));
				else
					return Helper.RectangleToRect (control.Parent.RectangleToScreen (bounds));
			}
		}

		internal static Rect GetToolStripItemScreenBounds (SWF.ToolStripItem item)
		{
			return RectangleToRect (GetToolStripItemScreenBoundsAsRectangle (item));
		}

		internal static System.Drawing.Rectangle GetToolStripItemScreenBoundsAsRectangle (SWF.ToolStripItem item)
		{
			if (item.Owner == null)
				return item.Bounds;
			else
				return item.Owner.RectangleToScreen (item.Bounds);
		}

		internal static object GetClickablePoint (SimpleControlProvider provider)
		{
			bool offScreen
				= (bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsOffscreenProperty.Id);
			if (offScreen == true)
				return null;
			else {
				// TODO: Verify with MS behavior.
				Rect rectangle
					= (Rect) provider.GetPropertyValue (AutomationElementIdentifiers.BoundingRectangleProperty.Id);
				return new System.Windows.Point (rectangle.X, rectangle.Y);
			}
		}		

		internal static bool IsOffScreen (Rectangle bounds, SWF.Control referenceControl)
		{
			return IsOffScreen (Helper.RectangleToRect (bounds), referenceControl);
		}

		internal static bool IsOffScreen (Rect bounds, SWF.Control referenceControl, bool scrollable)
		{
			Rect screen;

			if (scrollable)
				screen = Helper.GetControlScreenBounds (referenceControl.Bounds, referenceControl);
			else
				screen = Helper.RectangleToRect (SWF.Screen.GetWorkingArea (referenceControl));

			return !bounds.IntersectsWith (screen);
		}

		internal static bool IsListItemOffScreen (Rect itemBounds,
		                                          SWF.Control containerControl,
		                                          bool visibleHeader,
		                                          Rectangle headerBounds,
		                                          IScrollBehaviorObserver observer)
		{
			Rectangle listViewRectangle = containerControl.Bounds;
			if (visibleHeader) {
				listViewRectangle.Y += headerBounds.Height;
				listViewRectangle.Height -= headerBounds.Height;
			}
			if (observer.HorizontalScrollBar.Visible)
				listViewRectangle.Height -= observer.HorizontalScrollBar.Height;
			if (observer.VerticalScrollBar.Visible)
				listViewRectangle.Width -= observer.VerticalScrollBar.Width;

			Rect screen 
				= Helper.RectangleToRect (containerControl.Parent.RectangleToScreen (listViewRectangle));
			return !itemBounds.IntersectsWith (screen);
		}

		internal static bool ToolStripItemIsOffScreen (SWF.ToolStripItem item)
		{
			System.Drawing.Rectangle bounds =
				GetToolStripItemScreenBoundsAsRectangle (item);
			System.Drawing.Rectangle screen =
				SWF.Screen.GetWorkingArea (bounds);
			return !bounds.IntersectsWith (screen);
		}

		internal static bool IsOffScreen (Rect bounds, SWF.Control referenceControl)
		{
			return Helper.IsOffScreen (bounds, referenceControl, false);
		}
		
		internal static Rect GetButtonBaseImageBounds (FragmentControlProvider provider,
		                                               SWF.ButtonBase buttonBase)
		{
			//Implementation highly based in ThemeWin32Classic.ButtonBase_DrawImage method
			
			Image image;
			int	imageX;
			int	imageY;
			int	imageWidth;
			int	imageHeight;
			int width = buttonBase.Width;
			int height = buttonBase.Height;			

			if (buttonBase.ImageIndex != -1)
				image = buttonBase.ImageList.Images [buttonBase.ImageIndex];
			else
				image = buttonBase.Image;
			
			if (image == null)
				return Rect.Empty;

			imageWidth = image.Width;
			imageHeight = image.Height;
		
			switch (buttonBase.ImageAlign) {
				case ContentAlignment.TopLeft: {
					imageX = 5;
					imageY = 5;
					break;
				}
				
				case ContentAlignment.TopCenter: {
					imageX = (width - imageWidth) / 2;
					imageY = 5;
					break;
				}
				
				case ContentAlignment.TopRight: {
					imageX = width - imageWidth - 5;
					imageY = 5;
					break;
				}
				
				case ContentAlignment.MiddleLeft: {
					imageX = 5;
					imageY = (height - imageHeight) / 2;
					break;
				}
				
				case ContentAlignment.MiddleCenter: {
					imageX = (width - imageWidth) / 2;
					imageY = (height - imageHeight) / 2;
					break;
				}
				
				case ContentAlignment.MiddleRight: {
					imageX = width - imageWidth - 4;
					imageY = (height - imageHeight) / 2;
					break;
				}
				
				case ContentAlignment.BottomLeft: {
					imageX = 5;
					imageY = height - imageHeight - 4;
					break;
				}
				
				case ContentAlignment.BottomCenter: {
					imageX = (width - imageWidth) / 2;
					imageY = height - imageHeight - 4;
					break;
				}
				
				case ContentAlignment.BottomRight: {
					imageX = width - imageWidth - 4;
					imageY = height - imageHeight - 4;
					break;
				}
				
				default: {
					imageX = 5;
					imageY = 5;
					break;
				}
			}
			
			Rect buttonRect 
				= (Rect) provider.GetPropertyValue (AutomationElementIdentifiers.BoundingRectangleProperty.Id);
			
			imageX += (int) buttonRect.X;
			imageY += (int) buttonRect.Y;

			Rect imageRect = new Rect (imageX, imageY, imageWidth, imageHeight);
			buttonRect.Intersect (imageRect);

			return buttonRect;
		}

		internal static int GetUniqueRuntimeId ()
		{
			return ++id;
		}

		internal static TResult GetPrivateField<TResult> (Type referenceType, 
		                                                  object reference,
		                                                  string fieldName)
		{
			FieldInfo fieldInfo = referenceType.GetField (fieldName,
			                                              BindingFlags.NonPublic
			                                              | BindingFlags.Instance
			                                              | BindingFlags.GetField);
			if (fieldInfo == null)
				throw new NotSupportedException ("Field not found: " + fieldName);

			return (TResult) fieldInfo.GetValue (reference);
		}
		
		internal static TResult GetPrivateProperty<T, TResult> (Type referenceType, 
		                                                        T reference,
		                                                        string propertyName)
		{
			PropertyInfo propertyInfo = referenceType.GetProperty (propertyName,
			                                                       BindingFlags.NonPublic
			                                                       | BindingFlags.Instance
			                                                       | BindingFlags.GetProperty);
			if (propertyInfo == null)
				throw new NotSupportedException ("Property not found: " + propertyName);

			Func<T, TResult> invoke = 
				(Func<T, TResult>) Delegate.CreateDelegate (typeof (Func<T, TResult>),
				                                             propertyInfo.GetGetMethod (true));
			return invoke (reference);
		}

		internal static void AddPrivateEvent (Type referenceType, object reference, 
		                                      string eventName, object referenceDelegate,
		                                      string delegateName)
		{
			AddRemovePrivateEvent (referenceType, reference, eventName, 
			                       referenceDelegate, delegateName, true);
		}
		
		internal static void RemovePrivateEvent (Type referenceType, object reference, 
		                                         string eventName, object referenceDelegate,
		                                         string delegateName)
		{
			AddRemovePrivateEvent (referenceType, reference, eventName, 
			                       referenceDelegate, delegateName, false);
		}
		
		internal static void RaiseStructureChangedEvent (StructureChangeType type,
		                                                 IRawElementProviderFragment provider)
		{	
			if (AutomationInteropProvider.ClientsAreListening) {
				int []runtimeId = null;
				if (type == StructureChangeType.ChildRemoved) {
					if (provider.FragmentRoot == null) //FIXME: How to fix it?
						runtimeId = provider.GetRuntimeId (); 
					else
						runtimeId = provider.FragmentRoot.GetRuntimeId ();
				} else if (type == StructureChangeType.ChildrenBulkAdded ||
				           type == StructureChangeType.ChildrenBulkRemoved)
					runtimeId = new int[] {0};
				else
					runtimeId = provider.GetRuntimeId ();
				
				StructureChangedEventArgs invalidatedArgs 
					= new StructureChangedEventArgs (type, runtimeId);
				AutomationInteropProvider.RaiseStructureChangedEvent (provider, 
				                                                      invalidatedArgs);
			}
		}
		
		internal static Rect RectangleToRect (Rectangle rectangle) 
		{
			if (rectangle == Rectangle.Empty)
				return Rect.Empty;
			return new Rect (rectangle.X, rectangle.Y, 
			                 rectangle.Width, rectangle.Height);
		}

		internal static Rectangle RectToRectangle (Rect rectangle) 
		{
			return new Rectangle ((int) rectangle.X, (int) rectangle.Y, 
			                      (int) rectangle.Width, (int) rectangle.Height);
		}
		
		#endregion
		
		#region Private Static Methods
		
		private static void AddRemovePrivateEvent (Type referenceType, object reference, 
		                                           string eventName, object referenceDelegate,
		                                           string delegateName, bool addEvent)
		{
			EventInfo eventInfo;
			if (reference != null)
				eventInfo = referenceType.GetEvent (eventName,
				                                    BindingFlags.Instance 
				                                    | BindingFlags.NonPublic);
			else
				eventInfo = referenceType.GetEvent (eventName,
				                                    BindingFlags.Static 
				                                    | BindingFlags.NonPublic);
				
			if (eventInfo == null)
				throw new NotSupportedException ("Event not found: " + eventName);
			
			Type delegateType = eventInfo.EventHandlerType;
			MethodInfo eventMethod = addEvent 
				? eventInfo.GetAddMethod (true) :eventInfo.GetRemoveMethod (true);
			Delegate delegateValue;
			if (reference != null)
				delegateValue = Delegate.CreateDelegate (delegateType, 
				                                         referenceDelegate,
				                                         delegateName, 
				                                         false);
			else
				delegateValue = Delegate.CreateDelegate (delegateType, 
				                                         (Type) referenceDelegate,
				                                         delegateName, 
				                                         false);
			eventMethod.Invoke (reference, new object[] { delegateValue });
		}
		
		#endregion

		#region Static Fields
		
		static int id = 0;

		#endregion
	}
}
