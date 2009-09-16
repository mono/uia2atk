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
//      Brad Taylor <brad@getcoded.net>
//

using Atk;

using System;
using System.Windows;
using System.Windows.Automation.Peers;

namespace Moonlight.AtkBridge
{
	public class RootVisualAdapter : Adapter
	{
#region Public Methods
		public RootVisualAdapter ()
			: base (new WindowAutomationPeer (
				(FrameworkElement) Application.Current.RootVisual))
		{
		}
#endregion

#region Protected Methods
		protected override string OnGetName ()
		{
			// TODO: I18N
			return "Silverlight Control";
		}

		protected override int OnGetIndexInParent ()
		{
			// Our object will always be the only child of the
			// plugin host's parent.
			return 0;
		}

		protected override Atk.Object OnGetParent ()
		{
			return parent;
		}

		protected override void OnSetParent (Atk.Object parent)
		{
			// Allow the web browser's plugin host to set our
			// parent
			this.parent = parent;
		}
#endregion

#region Private Fields
		private Atk.Object parent = null;
#endregion
	}
}
