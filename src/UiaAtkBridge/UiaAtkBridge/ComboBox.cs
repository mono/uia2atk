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

using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{
	public class ComboBox : ComponentAdapter, Atk.ActionImplementor, Atk.SelectionImplementor
	{
		
		private Menu innerChild = new Menu (new string[] { "First Element", "Second Element", "Third Element" });
		
		public ComboBox (IRawElementProviderSimple provider)
		{
			this.provider = provider;
		}

		public override IRawElementProviderSimple Provider {
			get { return provider; }
		}
		
		private IRawElementProviderSimple provider;
		
		protected override Atk.Object OnRefChild (int i)
		{
			if (i != 0)
				return null;
			return innerChild;
		}

		protected override int OnGetNChildren ()
		{
			return 1;
		}
		
		int Atk.ActionImplementor.NActions {
			get {
				throw new NotImplementedException();
			}
		}

		int Atk.SelectionImplementor.SelectionCount {
			get {
				throw new NotImplementedException();
			}
		}
		
		bool Atk.ActionImplementor.DoAction (int i)
		{
			throw new NotImplementedException();
		}

		string Atk.ActionImplementor.GetDescription (int i)
		{
			throw new NotImplementedException();
		}

		string Atk.ActionImplementor.GetName (int i)
		{
			throw new NotImplementedException();
		}

		string Atk.ActionImplementor.GetKeybinding (int i)
		{
			throw new NotImplementedException();
		}

		bool Atk.ActionImplementor.SetDescription (int i, string desc)
		{
			throw new NotImplementedException();
		}

		string Atk.ActionImplementor.GetLocalizedName (int i)
		{
			throw new NotImplementedException();
		}

		
		bool Atk.SelectionImplementor.AddSelection (int i)
		{
			throw new NotImplementedException();
		}

		bool Atk.SelectionImplementor.ClearSelection ()
		{
			throw new NotImplementedException();
		}

		Atk.Object Atk.SelectionImplementor.RefSelection (int i)
		{
			throw new NotImplementedException();
		}

		bool Atk.SelectionImplementor.IsChildSelected (int i)
		{
			throw new NotImplementedException();
		}

		bool Atk.SelectionImplementor.RemoveSelection (int i)
		{
			throw new NotImplementedException();
		}

		bool Atk.SelectionImplementor.SelectAllSelection ()
		{
			throw new NotImplementedException();
		}
		
		public override void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs e)
		{
			throw new NotImplementedException ();
		}

		public override void RaiseAutomationPropertyChangedEvent (AutomationPropertyChangedEventArgs e)
		{
			throw new NotImplementedException ();
		}
	}
}
