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
//      Sandy Armstrong <sanfordarmstrong@gmail.com>
//

using System;
using System.Collections.Generic;

using Mono.UIAutomation.Source;
using Mono.UIAutomation.Services;

namespace System.Windows.Automation
{
	public sealed partial class TreeWalker
	{
		private class TreeIterator
		{
			#region Private Fields
			private List<AutomationElement> markedElements;
			private Condition condition;
			#endregion

			#region Constructor
			public TreeIterator (Condition condition)
			{
				this.condition = condition;
				markedElements = new List<AutomationElement> ();
			}
			#endregion

			#region Private Direct Navigation Methods
			private AutomationElement GetFirstDirectChild (AutomationElement element)
			{
				AutomationElement firstChild = null;

				if (element == AutomationElement.RootElement)
					lock (TreeWalker.RawViewWalker.directChildrenLock) {
						firstChild = TreeWalker.RawViewWalker.directChildren.Count > 0
							? TreeWalker.RawViewWalker.directChildren [0]
							: null;
					}
				else
					firstChild = SourceManager.GetOrCreateAutomationElement (element.SourceElement.FirstChild);

				return firstChild;
			}

			private AutomationElement GetLastDirectChild (AutomationElement element)
			{
				AutomationElement lastChild = null;

				if (element == AutomationElement.RootElement)
					lock (TreeWalker.RawViewWalker.directChildrenLock) {
						lastChild = TreeWalker.RawViewWalker.directChildren.Count > 0 ?
							TreeWalker.RawViewWalker.directChildren [TreeWalker.RawViewWalker.directChildren.Count - 1] :
							null;
					}
				else
					lastChild = SourceManager.GetOrCreateAutomationElement (element.SourceElement.LastChild);

				return lastChild;
			}

			private AutomationElement GetNextDirectSibling (AutomationElement element)
			{
				AutomationElement parent = TreeWalker.RawViewWalker.GetParent (element);
				AutomationElement nextSibling = null;

				if (parent == AutomationElement.RootElement)
					lock (TreeWalker.RawViewWalker.directChildrenLock) {
						int nextIndex = TreeWalker.RawViewWalker.directChildren.IndexOf (element) + 1;
						if (nextIndex > -1 && nextIndex < TreeWalker.RawViewWalker.directChildren.Count)
							nextSibling = TreeWalker.RawViewWalker.directChildren [nextIndex];
						else
							nextSibling = null;
					}
				else
					nextSibling = SourceManager.GetOrCreateAutomationElement (element.SourceElement.NextSibling);

				return nextSibling;
			}

			private AutomationElement GetPreviousDirectSibling (AutomationElement element)
			{
				AutomationElement parent = TreeWalker.RawViewWalker.GetParent (element);
				AutomationElement prevSibling = null;

				if (parent == AutomationElement.RootElement)
					lock (TreeWalker.RawViewWalker.directChildrenLock) {
						int prevIndex = TreeWalker.RawViewWalker.directChildren.IndexOf (element) - 1;
						if (prevIndex > -1)
							prevSibling = TreeWalker.RawViewWalker.directChildren [prevIndex];
						else
							prevSibling = null;
					}
				else
					prevSibling = SourceManager.GetOrCreateAutomationElement (element.SourceElement.PreviousSibling);

				return prevSibling;
			}
			#endregion

			#region Private Utility Methods
			private void AddMarkedElement (AutomationElement element)
			{
				markedElements.Add (element);
			}

			/// <summary>
			/// Iterate to the first matching child of an element
			/// *after* the specified child.
			/// </summary>
			/// <param name="element">
			/// The element whose children should be iterated.
			/// </param>
			/// <param name="afterThisChild">
			/// The direct child of <paramref name="element"/>
			/// after which to iterate, or null if the absolute first
			/// matching child is desired.
			/// </param>
			/// <returns>
			/// The first matching <see cref="AutomationElement"/>
			/// after <paramref name="afterThisChild"/>.
			/// </returns>
			private AutomationElement GetFirstChild (AutomationElement element, AutomationElement afterThisChild)
			{
				if (element == null)
					throw new ArgumentNullException ("element");
				AddMarkedElement (element);

				var child = (afterThisChild == null)
					? GetFirstDirectChild(element)
					: GetNextDirectSibling(afterThisChild);
				
				while (child != null && markedElements.Contains (child))
					child = GetNextDirectSibling (child);

				if (child == null || markedElements.Contains (child))
					return null;

				var firstChild = (!markedElements.Contains(child) && condition.AppliesTo(child))
					? child
					: GetFirstChild (child);

				while (firstChild == null) {
					child = GetNextDirectSibling (child);
					if (child == null)
						return null;
					firstChild = GetFirstChild (child);
				}
				return firstChild;
			}

			/// <summary>
			/// Iterate to the last matching child of an element
			/// *before* the specified child.
			/// </summary>
			/// <param name="element">
			/// The element whose children should be iterated.
			/// </param>
			/// <param name="beforeThisChild">
			/// The direct child of <paramref name="element"/>
			/// before which to iterate, or null if the absolute last
			/// matching child is desired.
			/// </param>
			/// <returns>
			/// The last matching <see cref="AutomationElement"/>
			/// before <paramref name="beforeThisChild"/>.
			/// </returns>
			private AutomationElement GetLastChild (AutomationElement element, AutomationElement beforeThisChild)
			{
				if (element == null)
					throw new ArgumentNullException ("element");
				AddMarkedElement (element);

				AutomationElement child;
				if (beforeThisChild == null)
					child = GetLastDirectChild (element);
				else
					child = GetPreviousDirectSibling (beforeThisChild);
				while (child != null && markedElements.Contains (child))
					child = GetPreviousDirectSibling (child);
				if (child == null || markedElements.Contains (child))
					return null;

				AutomationElement lastChild = null;
				if (!markedElements.Contains (child) && condition.AppliesTo (child))
					lastChild = child;
				else {
					lastChild = GetLastChild (child);
				}

				while (lastChild == null && child != null) {
					child = GetPreviousDirectSibling (child);
					if (child == null)
						return null;
					lastChild = GetLastChild (child);
				}
				return lastChild;
			}
			#endregion

			#region Public Methods
			public AutomationElement GetParent (AutomationElement element)
			{
				if (element == null)
					throw new ArgumentNullException ("element");
				else if (element == AutomationElement.RootElement)
					return null;
				AutomationElement ancestor =
					SourceManager.GetOrCreateAutomationElement (element.SourceElement.Parent);

				lock (TreeWalker.RawViewWalker.directChildrenLock)
					if (ancestor == null && RawViewWalker.directChildren.Contains (element))
						ancestor = SourceManager.GetOrCreateAutomationElement (AutomationElement.RootElement.SourceElement);

				if (ancestor != null && !condition.AppliesTo (ancestor))
					return GetParent (ancestor);
				return ancestor;
			}

			public AutomationElement GetFirstChild (AutomationElement element)
			{
				return GetFirstChild (element, null);
			}

			public AutomationElement GetLastChild (AutomationElement element)
			{
				return GetLastChild (element, null);
			}

			public AutomationElement GetNextSibling (AutomationElement element)
			{
				if (element == null)
					throw new ArgumentNullException ("element");
				if (element == AutomationElement.RootElement)
					return null;
				AddMarkedElement (element);

				AutomationElement sibling = GetNextDirectSibling (element);

				AutomationElement previousParent = null;
				if (sibling == null && condition != Automation.RawViewCondition) {
					previousParent = TreeWalker.RawViewWalker.GetParent (element);
					markedElements.Add (previousParent);
					sibling = TreeWalker.RawViewWalker.GetParent (previousParent);
				}

				if (sibling == null)
					return null;

				AutomationElement nextSibling = null;
				if (!markedElements.Contains (sibling) && condition.AppliesTo (sibling))
					nextSibling = sibling;
				else {
					while (sibling != null && nextSibling == null && sibling != AutomationElement.RootElement) {
						nextSibling = GetFirstChild (sibling, previousParent);
						previousParent = sibling;
						sibling = TreeWalker.RawViewWalker.GetParent (sibling);
					}
				}

				return nextSibling;
			}

			public AutomationElement GetPreviousSibling (AutomationElement element)
			{
				if (element == null)
					throw new ArgumentNullException ("element");
				if (element == AutomationElement.RootElement)
					return null;
				AddMarkedElement (element);

				AutomationElement sibling = GetPreviousDirectSibling (element);

				AutomationElement previousParent = null;
				if (sibling == null && condition != Automation.RawViewCondition) {
					previousParent = TreeWalker.RawViewWalker.GetParent (element);
					markedElements.Add (previousParent);
					sibling = TreeWalker.RawViewWalker.GetParent (previousParent);
				}

				if (sibling == null)
					return null;

				AutomationElement prevSibling = null;
				if (!markedElements.Contains (sibling) && condition.AppliesTo (sibling))
					prevSibling = sibling;
				else {
					while (sibling != null && prevSibling == null && sibling != AutomationElement.RootElement) {
						prevSibling = GetLastChild (sibling, previousParent);
						previousParent = sibling;
						sibling = TreeWalker.RawViewWalker.GetParent (sibling);
					}
				}

				return prevSibling;
			}
			#endregion
		}
	}
}
