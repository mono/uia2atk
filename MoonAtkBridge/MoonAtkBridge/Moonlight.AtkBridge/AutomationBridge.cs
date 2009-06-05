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
//      Andres Aragoneses <aaragoneses@novell.com>
//      Brad Taylor <brad@getcoded.net>
// 

using Atk;

using System;
using System.Windows;

namespace Moonlight.AtkBridge
{
	public class AutomationBridge
	{
#region Public Methods
		public AutomationBridge ()
		{
			adapterFactory = new DynamicAdapterFactory ();
		}

		public static bool IsAccessibilityEnabled ()
		{
			// Detect whether it is turned on at a platform level
			// TODO: Copy this code from UiaAtkBridge
			return true;
		}

		public IntPtr GetAccessibleHandle ()
		{
			Atk.Object adapter
				= adapterFactory.GetAdapter (
					Application.Current.RootVisual);
			return (adapter == null) ? IntPtr.Zero
			                         : adapter.Handle;
		}
#endregion
		
#region Private Fields
		private DynamicAdapterFactory adapterFactory;
#endregion
	}
}
