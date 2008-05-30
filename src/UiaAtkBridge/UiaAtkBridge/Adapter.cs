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
// 

using System;
using System.Collections.Generic;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{
	public abstract class Adapter : Atk.Object, Atk.ComponentImplementor
	{

#region Private Members
		private Dictionary<uint, Atk.FocusHandler>	focusHandlers;
		private uint								lastFocusHandlerId;
	
#endregion

#region ComponentImplementor Properties

		public virtual double Alpha
		{
			get {
				return 1;
			}
		}
		
#endregion

#region Constructors
		
		public Adapter ()
		{
			lastFocusHandlerId = 0;
			focusHandlers = new Dictionary<uint, Atk.FocusHandler> ();
		}
		
#endregion

#region Adapter Methods
	
		public abstract IRawElementProviderSimple Provider { get; }
		
		public abstract void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs e);
		
		public abstract void RaiseAutomationPropertyChangedEvent (AutomationPropertyChangedEventArgs e);
		
#endregion


#region ComponentImplementor Methods

		public virtual uint AddFocusHandler (Atk.FocusHandler handler)
		{
			if(focusHandlers.ContainsValue(handler))
				return 0;
			
			lastFocusHandlerId++;
			focusHandlers[lastFocusHandlerId] = handler;
			return lastFocusHandlerId;
		}

		public virtual bool Contains (int x, int y, Atk.CoordType coord_type)
		{
			//TODO: Implement Contains
			return false;
		}

		public virtual void GetExtents (out int x, out int y, out int width, out int height, Atk.CoordType coord_type)
		{
			//TODO: Implement GetExtents
			x = 0;
			y = 0;
			width = 0;
			height = 0;
		}
		
		public virtual void GetPosition (out int x, out int y, Atk.CoordType coord_type)
		{
			//TODO: Implement GetPosition
			x = 0;
			y = 0;
		}

		// we should use "override" instead of "new" when this bug is fixed and it gets
		// propragated to GTK#: http://bugzilla.gnome.org/show_bug.cgi?id=526752
		public virtual new Atk.Layer Layer {
			get { return Atk.Layer.Widget; }
		}
		
		public virtual new int MdiZorder {
			get { return 0; }
		}
		
		public virtual void GetSize (out int width, out int height)
		{
			//TODO: Implement GetSize
			width = 0;
			height = 0;
		}
		
		public virtual bool GrabFocus ()
		{
			//TODO: Implement GrabFocus
			return false;
		}
		
		public virtual Atk.Object RefAccessibleAtPoint (int x, int y, Atk.CoordType coord_type)
		{
			//TODO: Implement RefAccessibleAtPoint
			return null;
		}
		
		public virtual void RemoveFocusHandler (uint handler_id)
		{
			if(focusHandlers.ContainsKey(handler_id))
				focusHandlers.Remove(handler_id);
		}
		
		public virtual bool SetExtents (int x, int y, int width, int height, Atk.CoordType coord_type)
		{
			//TODO: Implement SetExtents
			return false;
		}
		
		public virtual bool SetPosition (int x, int y, Atk.CoordType coord_type)
		{
			//TODO: Implement SetPosition
			return false;
		}
		
		public virtual bool SetSize (int width, int height)
		{
			//TODO: Implement SetSize
			return false;
		}
		
#endregion

	}
}
