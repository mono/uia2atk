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
using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace Mono.UIAutomation.Winforms
{

	internal sealed class Helper
	{
		
		private Helper ()
		{
		}
		
		#region Internal Static Methods

		internal static int GetUniqueRuntimeId ()
		{
			return ++id;
		}
		
		internal static void RaiseStructureChangedEvent (StructureChangeType type,
		                                                 IRawElementProviderFragment provider)
		{
			Console.WriteLine ("Helper.RaiseStructureChangedEvent: {0} - {1} - {2} ",
			                   type, provider.GetType (), provider.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id));
			
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

		#region Static Fields
		
		static int id = 0;
		#endregion
	}
}
