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
//      Mike Gorse <mgorse@novell.com>
// 

using System;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{

	public class Splitter : SplitContainer
	{
		
		public Splitter (IRawElementProviderSimple provider) : base (provider)
		{
		}

		internal override void PostInit ()
		{
			componentExpert = new ComponentImplementorHelper (Parent as Adapter);
			base.PostInit ();
		}

		protected override Atk.StateSet OnRefStateSet ()
		{
			Adapter parent = Parent as Adapter;
			if (parent == null)
				return null;
			// We pretend to be a sub-window, not a splitter
			Atk.StateSet states = Parent.RefStateSet ();
			states.RemoveState (Atk.StateType.Active);
			states.RemoveState (Atk.StateType.Resizable);
			if (base.OnRefStateSet().ContainsState (Atk.StateType.Horizontal))
				states.AddState (Atk.StateType.Horizontal);
			else
				states.AddState (Atk.StateType.Vertical);
			if ((bool)parent.Provider.GetPropertyValue (AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id))
				states.AddState (Atk.StateType.Focusable);
			return states;
		}
	}
}
