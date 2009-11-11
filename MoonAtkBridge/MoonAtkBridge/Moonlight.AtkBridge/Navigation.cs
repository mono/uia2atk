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
//

using System;
using System.Collections.Generic;
using System.Windows.Automation.Peers;

namespace Moonlight.AtkBridge
{
	public static class Navigation
	{
		public static AutomationPeer Navigate (this AutomationPeer peer, NavigateDirection direction)
		{
			List<AutomationPeer> children = null;
			if (direction == NavigateDirection.FirstChild || direction == NavigateDirection.LastChild) {
				children = peer.GetChildren ();
				if (children == null || children.Count == 0)
					return null;

				if (direction == NavigateDirection.FirstChild)
					return children [0];

				return children [children.Count - 1];
			}

			var parent = peer.GetParent ();

			if (direction == NavigateDirection.Parent || parent == null)
				return parent;

			children = parent.GetChildren ();
			if (children == null)
				return null;

			AutomationPeer previous = null;
			AutomationPeer current = null;
			foreach (AutomationPeer child in children) {
				previous = current;
				current = child;

				if (child == peer && direction == NavigateDirection.PreviousSibling)
					return previous;

				if (previous == peer && direction == NavigateDirection.NextSibling)
					return current;
			}
			return null;
		}

	}

	public enum NavigateDirection
	{
		Parent = 0,
		NextSibling = 1,
		PreviousSibling = 2,
		FirstChild = 3,
		LastChild = 4,
	}
}
