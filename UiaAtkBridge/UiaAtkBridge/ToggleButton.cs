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
//  Neville Gao <nevillegao@gmail.com>
// 

using System;
using System.Windows.Automation;
using Mono.UIAutomation.Services;
using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{
	
	public class ToggleButton : CheckBoxButton
	{
		private IToggleProvider toggleProvider;
		
		public ToggleButton (IRawElementProviderSimple provider) : base (provider)
		{
			toggleProvider = (IToggleProvider) provider.GetPatternProvider (TogglePatternIdentifiers.Pattern.Id);
			imageImplementor = new ImageImplementorHelper (this);
			if (toggleProvider == null)
				throw new ArgumentException ("The provider for ToggleButton should implement the Toggle pattern");

			Role = Atk.Role.ToggleButton;
		}
		
		public override string GetName (int action)
		{
			if (action != 0)
				return null;

			return "click";
		}
		
		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();
			
			if (states.ContainsState (Atk.StateType.Defunct))
				return states;

			if (toggleProvider != null) {
				ToggleState state = toggleProvider.ToggleState;
				
				if (IsToggled (state)) {
					states.AddState (Atk.StateType.Armed);
					states.AddState (Atk.StateType.Checked);
				} else {
					states.RemoveState (Atk.StateType.Armed);
					states.RemoveState (Atk.StateType.Checked);
				}
			}
			
			return states;
		}

		public override bool DoAction (int action)
		{
			if (toggleProvider != null) {
				try {
					if (action != 0)
						return false;
					toggleProvider.Toggle ();
					return true;
				} catch (ElementNotEnabledException e) {
					Log.Debug (e);
				}
			}
			return false;
		}
		
		public override void RaiseAutomationPropertyChangedEvent (AutomationPropertyChangedEventArgs e)
		{
			if (e.Property == TogglePatternIdentifiers.ToggleStateProperty)
				NotifyStateChange (Atk.StateType.Checked, IsToggled ((ToggleState)e.NewValue));
			else
				base.RaiseAutomationPropertyChangedEvent (e);
		}
		
		private bool IsToggled (ToggleState state)
		{
			switch (state) {
				case ToggleState.On:
					return true;
				case ToggleState.Indeterminate:
				case ToggleState.Off:
					return false;
				default:
					throw new NotSupportedException ("Unknown toggleState " + state.ToString ());
			}
		}
	}
}
