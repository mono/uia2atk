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
//      Andres G. Aragoneses <aaragoneses@novell.com>
//      Calvin Gaisford <cgaisford@novell.com>
// 

using System;
using System.Collections.Generic;
using System.Windows.Automation;

using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{
	internal class ComponentImplementorHelper : Atk.ComponentImplementor
	{
		public ComponentImplementorHelper (Adapter resource)
		{
			this.resource = resource;
			
			lastFocusHandlerId = 0;
			focusHandlers = new Dictionary<uint, Atk.FocusHandler> ();
		}

		private Adapter					resource;
		private Dictionary<uint, Atk.FocusHandler>	focusHandlers;
		private uint					lastFocusHandlerId;
		private ITransformProvider			transformProvider = null;
		
		public virtual uint AddFocusHandler (Atk.FocusHandler handler)
		{
			if (focusHandlers.ContainsValue(handler))
				return 0;
			
			lastFocusHandlerId++;
			focusHandlers[lastFocusHandlerId] = handler;
			return lastFocusHandlerId;
		}

		public bool Contains (int x, int y, Atk.CoordType coordType)
		{
			if (coordType == Atk.CoordType.Window)
				resource.ConvertCoords (ref x, ref y, true);
			return resource.BoundingRectangle.Contains (new System.Windows.Point (x, y));
		}

		public virtual Atk.Object RefAccessibleAtPoint (int x, int y, Atk.CoordType coordType)
		{
			//TODO: check for children at this point?
			return this.resource;
		}

		public void GetExtents (out int x, out int y, out int width, out int height, Atk.CoordType coordType)
		{
			GetPosition (out x, out y, coordType);
			width = (int)resource.BoundingRectangle.Width;
			height = (int)resource.BoundingRectangle.Height;
		}

		public void GetPosition (out int x, out int y, Atk.CoordType coordType)
		{
			x = (int)resource.BoundingRectangle.X;
			y = (int)resource.BoundingRectangle.Y;
			if (coordType == Atk.CoordType.Window)
				resource.ConvertCoords (ref x, ref y, false);
		}

		public virtual void GetSize (out int width, out int height)
		{
			width = (int)resource.BoundingRectangle.Width;
			height = (int)resource.BoundingRectangle.Height;
		}

		public virtual bool GrabFocus ()
		{
			IRawElementProviderFragment fragment = null;
			if ((fragment = resource.Provider as IRawElementProviderFragment) != null) {
				fragment.SetFocus ();
				return true;
			}

			return false;
		}

		public void RemoveFocusHandler (uint handler_id)
		{
			if (focusHandlers.ContainsKey (handler_id))
				focusHandlers.Remove (handler_id);
		}

		public bool SetExtents (int x, int y, int width, int height, Atk.CoordType coordType)
		{
			if (transformProvider == null)
				transformProvider = (ITransformProvider) resource.Provider.GetPatternProvider (TransformPatternIdentifiers.Pattern.Id);
			
			if ((transformProvider != null) && 
			    (transformProvider.CanResize) && 
			    (transformProvider.CanMove)) {
				transformProvider.Move (x, y);
				transformProvider.Resize (width, height);
				return true;
			}
			return false;
		}

		public bool SetPosition (int x, int y, Atk.CoordType coordType)
		{
			if (transformProvider == null)
				transformProvider = (ITransformProvider) resource.Provider.GetPatternProvider (TransformPatternIdentifiers.Pattern.Id);
			
			if ((transformProvider != null) && (transformProvider.CanMove)) {
				transformProvider.Move (x, y);
				return true;
			}
			return false;
		}

		public bool SetSize (int width, int height)
		{
			if (transformProvider == null)
				transformProvider = (ITransformProvider) resource.Provider.GetPatternProvider (TransformPatternIdentifiers.Pattern.Id);
			
			if ((transformProvider != null) && (transformProvider.CanResize)) {
				transformProvider.Resize (width, height);
				return true;
			}
			return false;
		}
		
		public IntPtr Handle {
			get { return resource.Handle; }
		}

		public int MdiZorder {
			get { return 0; }
		}
		
		public Atk.Layer Layer {
			get { return Atk.Layer.Widget; }
		}

		public virtual double Alpha {
			get { return 1; }
		}

		public bool CanResize {
			get {
				if (transformProvider == null && resource.Provider != null)
					transformProvider = (ITransformProvider) resource.Provider.GetPatternProvider (TransformPatternIdentifiers.Pattern.Id);
			
				if (transformProvider != null)
					return transformProvider.CanResize;
				return false;
			}
		}
	}
}
