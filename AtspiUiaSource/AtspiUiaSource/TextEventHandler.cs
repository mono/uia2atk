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
//	Mike Gorse <mgorse@novell.com>
//

using Atspi;
using System.Windows.Automation;

namespace AtspiUiaSource
{
	public class TextEventHandler : ISourceEventHandler {

		public TextEventHandler (Element element)
		{
			this.element = element;

			element.accessible.ObjectEvents.TextChanged += OnTextChanged;
			element.accessible.ObjectEvents.TextSelectionChanged += OnTextSelectionChanged;
		}

		public void Terminate ()
		{
			element.accessible.ObjectEvents.TextSelectionChanged -= OnTextSelectionChanged;
			element.accessible.ObjectEvents.TextChanged -= OnTextChanged;
		}

		private void OnTextChanged (Accessible sender, string detail, int v1, int v2, object any)
		{
			AutomationSource.RaiseAutomationEvent (element,
			                                            TextPattern.TextChangedEvent);
		}

		private void OnTextSelectionChanged (Accessible sender)
		{
			AutomationSource.RaiseAutomationEvent (element,
			                                            TextPattern.TextSelectionChangedEvent);
		}

		private Element element;
	}
}
