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
//	Mario Carrion <mcarrion@novell.com>
//

using Atspi;
using Mono.UIAutomation.Source;
using System.Windows.Automation;

namespace AtspiUiaSource
{
	internal class SelectionEventHandler : ISourceEventHandler {

		public SelectionEventHandler (Element element, SelectionSource source)
		{
			this.source = source;
			this.element = element;

			oldSelection = source.Selection;
			element.accessible.ObjectEvents.SelectionChanged += SelectionChanged;
		}

		public void Terminate ()
		{
			element.accessible.ObjectEvents.SelectionChanged -= SelectionChanged;
		}

		private void SelectionChanged (string detail, int v1, int v2, object any)
		{
			// To always expose public API
			AutomationElement []oldElements = null;
			if (oldSelection != null)
				oldElements = SourceManager.GetOrCreateAutomationElements (oldSelection);

			AutomationElement []newElements = null;
			IElement []newSelection = source.Selection;
			if (newSelection != null)
				newElements = SourceManager.GetOrCreateAutomationElements (newSelection);

			AutomationSource.RaisePropertyChangedEvent (element,
			                                            SelectionPattern.SelectionProperty,
								    oldElements ?? new AutomationElement [0],
								    newElements ?? new AutomationElement [0]);
			oldSelection = newSelection;
		}

		private Element element;
		private IElement[] oldSelection;
		private SelectionSource source;
	}
}

