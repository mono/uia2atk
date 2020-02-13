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
// Copyright (c) 2008-09 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//      Mike Gorse <mgorse@novell.com>
// 

using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;

namespace Mono.UIAutomation.Winforms
{
	internal class KeyFilter: IKeyFilter
	{
		public bool PreFilterKey (KeyFilterData key)
		{
			bool alt = (key.ModifierKeys & Keys.Alt) != 0;
			bool control = (key.ModifierKeys & Keys.Control) != 0;
			bool shift = (key.ModifierKeys & Keys.Shift) != 0;
			System.Windows.Automation.KeyEventArgs eventArgs = new System.Windows.Automation.KeyEventArgs (key.Down, key.keycode, key.keysym, key.str, alt, control, shift);
			AutomationInteropProvider.RaiseAutomationEvent (null, eventArgs);
			return eventArgs.SuppressKeyPress;
		}
	}
}
