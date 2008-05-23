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
//      Sandy Armstrong <sanfordarmstrong@gmail.com>
//      Mario Carrion <mcarrion@novell.com>
// 

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace UiaAtkBridgeTest
{
	public abstract class TestProviderSimple : IRawElementProviderSimple
	{
#region Protected Fields
		
#endregion
		
#region Private Fields

		protected Dictionary<int, object> properties;

#endregion
		
#region Constructors
		
		public TestProviderSimple () : base()
		{
			// make sure types are initialized
			GLib.GType.Init ();

			properties = new Dictionary<int,object> ();

			properties[AutomationElementIdentifiers.AutomationIdProperty.Id] =
				Guid.NewGuid().ToString();
		}
		
#endregion
		


#region Public

		public virtual void SetPropertyValue(int propertyId, object propertyValue)
		{
			properties[propertyId] = propertyValue;
		}

#endregion
		
#region IRawElementProviderSimple Members
	
		public abstract object GetPatternProvider (int patternId);
		
		public virtual object GetPropertyValue (int propertyId)
		{
			if(properties.ContainsKey(propertyId))
				return properties[propertyId];
			else
				return null;
		}

		public virtual IRawElementProviderSimple HostRawElementProvider {
			get {
				return null; //AutomationInteropProvider.HostProviderFromHandle (control.TopLevelControl.Handle);
			}
		}
		
		public virtual ProviderOptions ProviderOptions {
			get {
				return ProviderOptions.ServerSideProvider;
			}
		}

#endregion
	}
}
