// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//
// Copyright (c) 2019 AxxonSoft (http://axxonsoft.com)
//
// Authors:
//   Nikita Voronchev <nikita.voronchev@ru.axxonsoft.com>
//

using System;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.UiaDbus.Interfaces;

namespace Mono.UIAutomation.UiaDbusBridge.Wrappers
{
	public class LegacyIAccessiblePatternWrapper : ILegacyIAccessiblePattern
	{
#region Private Fields

		private ILegacyIAccessibleProvider provider;

#endregion

#region Constructor

		public LegacyIAccessiblePatternWrapper (ILegacyIAccessibleProvider provider)
		{
			this.provider = provider;
		}

#endregion

#region ILegacyIAccessiblePattern Members

		public int ChildId {
			get {
				return provider.ChildId;
			}
		}

		public string DefaultAction {
			get {
				return provider.DefaultAction;
			}
		}

		public string Description {
			get {
				return provider.Description;
			}
		}

		public string Help {
			get {
				return provider.Help;
			}
		}

		public string KeyboardShortcut {
			get {
				return provider.KeyboardShortcut;
			}
		}

		public string Name {
			get {
				return provider.Name;
			}
		}

		public int Role {
			get {
				return provider.Role;
			}
		}

		public int State {
			get {
				return provider.State;
			}
		}

		public string Value {
			get {
				return provider.Value;
			}
		}

		public void DoDefaultAction ()
		{
			provider.DoDefaultAction ();
		}

#endregion
	}
}
