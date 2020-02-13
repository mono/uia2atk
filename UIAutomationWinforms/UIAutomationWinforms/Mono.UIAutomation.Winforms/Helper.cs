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
using Mono.Unix;
using Mono.UIAutomation.Services;

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
			else if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.Pane.Id;
			else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
				return Catalog.GetString ("pane");
			else
				return null;
		}

		internal static Rect GetControlScreenBounds (Rectangle bounds, SWF.Control control)
		{
			return GetControlScreenBounds (bounds, control, false);
		}

		internal static Rect GetControlScreenBounds (Rectangle bounds, SWF.Control control, bool controlIsParent)
		{
			if (control == null || !control.Visible)
				return Rect.Empty;
			else if (controlIsParent)
				return Helper.RectangleToRect (control.RectangleToScreen (bounds));
			else if (control.Parent == null || control.TopLevelControl == null)
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

		internal static bool IsOffScreen (Rectangle bounds)
		{
			System.Drawing.Rectangle screen =
				SWF.Screen.GetWorkingArea (bounds);
			return !bounds.IntersectsWith (screen);
		}

		internal static bool IsOffScreen (Rect bounds, SWF.Control referenceControl, bool scrollable)
		{
			Rect screen;

			if (scrollable)
				screen = Helper.GetControlScreenBounds (referenceControl.Bounds, referenceControl);
			else
				screen = Helper.RectangleToRect (SWF.Screen.GetWorkingArea (referenceControl));

			IRawElementProviderFragment formProvider = null;
			if ((formProvider = ProviderFactory.FindProvider (referenceControl.FindForm ())) != null)
				return !formProvider.BoundingRectangle.IntersectsWith (bounds) || !bounds.IntersectsWith (screen);

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
			return IsOffScreen (GetToolStripItemScreenBoundsAsRectangle (item));
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
			int imageX;
			int imageY;
			int imageWidth;
			int imageHeight;
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
					GetXYFromWidthInTopCenterAlignment (width, imageWidth, out imageX, out imageY);
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
		
		internal static Rect GetToolBarButtonImageBounds (FragmentControlProvider provider,
		                                                  SWF.ToolBarButton buttonBase)
		{
			//Implementation highly based in ThemeWin32Classic.ButtonBase_DrawImage method
			
			Image image;
			int imageX;
			int imageY;
			int imageWidth;
			int imageHeight;
			
			Rect buttonRect 
				= (Rect) provider.GetPropertyValue (AutomationElementIdentifiers.BoundingRectangleProperty.Id);
			
			int width = (int)buttonRect.Width;
			image = buttonBase.Image;
			
			if (image == null)
				return Rect.Empty;

			imageWidth = image.Width;
			imageHeight = image.Height;

			GetXYFromWidthInTopCenterAlignment (width, imageWidth, out imageX, out imageY);

			imageX += (int) buttonRect.X;
			imageY += (int) buttonRect.Y;

			Rect imageRect = new Rect (imageX, imageY, imageWidth, imageHeight);
			buttonRect.Intersect (imageRect);

			return buttonRect;
		}

		private static void GetXYFromWidthInTopCenterAlignment (int width, int imageWidth, out int x, out int y)
		{
			x = (width - imageWidth) / 2;
			y = 5;
		}
		
		internal static int GetUniqueRuntimeId ()
		{
			return System.Threading.Interlocked.Increment (ref id);
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

		internal static TResult GetPrivateProperty<T, TResult> (T reference,
		                                                        string propertyName)
		{
			return GetPrivateProperty<T, TResult> (typeof (T), reference, propertyName);
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
				
				var invalidatedArgs = new StructureChangedEventArgs (type, runtimeId);
				AutomationInteropProvider.RaiseStructureChangedEvent (provider, invalidatedArgs);
			}
		}
		
		internal static Rect RectangleToRect (Rectangle rectangle) 
		{
			if (rectangle == Rectangle.Empty)
				return Rect.Empty;
			try {
				return new Rect (rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
			} catch (ArgumentException ex) {
				Log.Error ($"RectangleToRect: {ex.Message}");
				return Rect.Empty;
			}
		}

		internal static Rectangle RectToRectangle (Rect rectangle) 
		{
			return new Rectangle ((int) rectangle.X, (int) rectangle.Y, 
			                      (int) rectangle.Width, (int) rectangle.Height);
		}
		
		internal static DockPosition GetDockPosition (SWF.DockStyle dockStyle)
		{
				if (dockStyle == SWF.DockStyle.Top)
					return DockPosition.Top;
				else if (dockStyle == SWF.DockStyle.Bottom)
					return DockPosition.Bottom;
				else if (dockStyle == SWF.DockStyle.Left)
					return DockPosition.Left;
				else if (dockStyle == SWF.DockStyle.Right)
					return DockPosition.Right;
				else if (dockStyle == SWF.DockStyle.Fill)
					return DockPosition.Fill;
				else
					return DockPosition.None;
		}

		internal static SWF.DockStyle GetDockStyle (DockPosition dockPosition)
		{
				if (dockPosition == DockPosition.Top)
					return SWF.DockStyle.Top;
				else if (dockPosition == DockPosition.Bottom)
					return SWF.DockStyle.Bottom;
				else if (dockPosition == DockPosition.Left)
					return SWF.DockStyle.Left;
				else if (dockPosition == DockPosition.Right)
					return SWF.DockStyle.Right;
				else if (dockPosition == DockPosition.Fill)
					return SWF.DockStyle.Fill;
				else
					return SWF.DockStyle.None;
		}

		internal static string StripAmpersands (string s)
		{
			if (s == null)
				return null;
			// Remove &, except the second in a pair
			// Will not remove an & at the end of a string, but
			// having one there wouldn't make much sense anyhow.
			for (int i = 0; i < s.Length - 1; i++)
				if (s [i] == '&')
					s = s.Remove (i, 1);
			return s;
		}

		internal static bool IsFormMinimized (SimpleControlProvider provider)
		{
			if (provider == null || provider is DesktopProvider)
				return false;

			if (provider.AssociatedControl == null) {
				Log.Warn (string.Format ("IsFormMinized: {0} returns null", provider.GetType ()));
				return false;
			}

			SWF.Form form = provider.AssociatedControl.FindForm ();
			if (form == null)
				return false;

			return form.WindowState == SWF.FormWindowState.Minimized;
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
