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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation;
using Mono.Unix;
using Mono.UIAutomation.Source;
using Atspi;

namespace AtspiUiaSource
{
	public class SelectionSource : ISelectionPattern
	{
		private Accessible accessible;
		private Selection AtspiSelection {
			get {
				Selection val = accessible.QuerySelection ();
				if (val == null)
					throw new NotSupportedException ();
				return val;
			}
		}

		public SelectionSource (Element element)
		{
			accessible = element.Accessible;
		}

		public IElement [] GetSelection ()
		{
			int nSelectedChildren = AtspiSelection.NSelectedChildren;
			if (nSelectedChildren < 0)
				nSelectedChildren = 0;
			IElement [] currentSelection = new IElement [nSelectedChildren];
			Selection selection = AtspiSelection;
			for (int i = 0; i < nSelectedChildren; i++)
				currentSelection [i] = Element.GetElement (selection.GetSelectedChild (i));
			return currentSelection;
		}

		public bool CanSelectMultiple {
			get {
				// TODO: have at-spi support this
				return false;
			}
		}

		public bool IsSelectionRequired {
			get {
				// TODO: have at-spi support this
				return false;
			}
		}
	}
}
