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

		internal static bool IsOffScreen (Rect bounds, SWF.Control referenceControl)
		{
			Rect screen = Helper.RectangleToRect (SWF.Screen.GetWorkingArea (referenceControl));

			// True if the *entire* control is off-screen
			return !screen.Contains (bounds.Left, bounds.Bottom) 
				&& !screen.Contains (bounds.Left, bounds.Top) 
				&& !screen.Contains (bounds.Right, bounds.Bottom) 
				&& !screen.Contains (bounds.Right, bounds.Top);
		}
		
		internal static Rect GetBoundingRectangleFromButtonBase (FragmentControlProvider provider,
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
			return new Rect (rectangle.X, rectangle.Y, 
			                 rectangle.Width, rectangle.Height);
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
