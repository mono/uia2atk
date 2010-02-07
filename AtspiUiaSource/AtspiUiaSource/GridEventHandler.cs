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
//	Mario Carrion <mcarrion@novell.com>
//

using Atspi;
using Mono.UIAutomation.Source;
using System.Windows.Automation;

namespace AtspiUiaSource
{
	internal class GridEventHandler : ISourceEventHandler {

		public GridEventHandler (Element element)
		{
			this.element = element;

			element.accessible.ObjectEvents.RowInserted += OnRowInserted;
			element.accessible.ObjectEvents.RowDeleted += OnRowDeleted;
			element.accessible.ObjectEvents.ColumnInserted += OnColumnInserted;
			element.accessible.ObjectEvents.ColumnDeleted += OnColumnDeleted;
		}

		public void Terminate ()
		{
			element.accessible.ObjectEvents.RowInserted -= OnRowInserted;
			element.accessible.ObjectEvents.RowDeleted -= OnRowDeleted;
			element.accessible.ObjectEvents.ColumnInserted -= OnColumnInserted;
			element.accessible.ObjectEvents.ColumnDeleted -= OnColumnDeleted;
		}

		private void OnRowInserted (Accessible sender, int row, int nInserted)
		{
			int newVal = element.accessible.QueryTable ().NRows;
			AutomationSource.RaisePropertyChangedEvent (
				element,
				GridPatternIdentifiers.RowCountProperty,
				newVal - nInserted,
				newVal);
		}

		private void OnRowDeleted (Accessible sender, int row, int nDeleted)
		{
			int newVal = element.accessible.QueryTable ().NRows;
			AutomationSource.RaisePropertyChangedEvent (
				element,
				GridPatternIdentifiers.RowCountProperty,
				newVal + nDeleted,
				newVal);
		}

		private void OnColumnInserted (Accessible sender, int row, int nInserted)
		{
			int newVal = element.accessible.QueryTable ().NColumns;
			AutomationSource.RaisePropertyChangedEvent (
				element,
				GridPatternIdentifiers.ColumnCountProperty,
				newVal - nInserted,
				newVal);
		}

		private void OnColumnDeleted (Accessible sender, int row, int nDeleted)
		{
			int newVal = element.accessible.QueryTable ().NColumns;
			AutomationSource.RaisePropertyChangedEvent (
				element,
				GridPatternIdentifiers.ColumnCountProperty,
				newVal + nDeleted,
				newVal);
		}

		private Element element;
	}
}

