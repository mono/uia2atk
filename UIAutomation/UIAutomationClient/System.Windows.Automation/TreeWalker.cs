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
//  Sandy Armstrong <sanfordarmstrong@gmail.com>
// 

using System;
using System.Collections.Generic;

using Mono.UIAutomation.Source;
using Mono.UIAutomation.Services;

namespace System.Windows.Automation
{
	public sealed partial class TreeWalker
	{
		#region Private Fields
		private Object directChildrenLock = new Object ();
		private List<AutomationElement> directChildren;
		#endregion

		#region Static Constructor
		static TreeWalker ()
		{
			RawViewWalker = new TreeWalker (Automation.RawViewCondition);
			InitializeRootElements ();

			ControlViewWalker = new TreeWalker (Automation.ControlViewCondition);
			ContentViewWalker = new TreeWalker (Automation.ContentViewCondition);
		}
		#endregion

		#region Public Constructor
		public TreeWalker (Condition condition)
		{
			if (condition == null)
				throw new ArgumentNullException ("condition");
			Condition = condition;
		}
		#endregion

		#region Internal Methods

		internal static void InitializeRootElements ()
		{
			lock (RawViewWalker.directChildrenLock) {
				RawViewWalker.directChildren = new List<AutomationElement> ();
				foreach (IAutomationSource source in SourceManager.GetAutomationSources ()) {
					foreach (IElement sourceElement in source.GetRootElements ())
						RawViewWalker.directChildren.Add (
							SourceManager.GetOrCreateAutomationElement (sourceElement));
					source.RootElementsChanged += (s, e) =>
						OnSourceRootElementChanged (source);
				}
			}
		}

		#endregion

		#region Private Methods

		private static void OnSourceRootElementChanged (IAutomationSource source)
		{
			lock (RawViewWalker.directChildrenLock) {
				List<AutomationElement> rootElements = new List<AutomationElement> ();
				foreach (AutomationElement element in RawViewWalker.directChildren) {
					if (element.SourceElement.AutomationSource != source)
						rootElements.Add (element);
				}
				// We don't handle the cleanup of AutomationElements here, they're
				// handled by each AutomationSource.
				// "Clean up" includes removing event handlers etc.
				foreach (IElement sourceElement in source.GetRootElements ())
					rootElements.Add (SourceManager.GetOrCreateAutomationElement (sourceElement));
				RawViewWalker.directChildren = rootElements;
			}
			Log.Debug ("Root elements are refreshed, Count: {0}", RawViewWalker.directChildren.Count);
		}

		#endregion

		#region Public Methods
		public AutomationElement GetParent (AutomationElement element)
		{
			return new TreeIterator (Condition).GetParent (element);
		}

		public AutomationElement GetFirstChild (AutomationElement element)
		{
			return new TreeIterator (Condition).GetFirstChild (element);
		}

		public AutomationElement GetLastChild (AutomationElement element)
		{
			return new TreeIterator (Condition).GetLastChild (element);
		}

		public AutomationElement GetNextSibling (AutomationElement element)
		{
			return new TreeIterator (Condition).GetNextSibling (element);
		}

		public AutomationElement GetPreviousSibling (AutomationElement element)
		{
			return new TreeIterator (Condition).GetPreviousSibling (element);
		}

		public AutomationElement Normalize (AutomationElement element)
		{
			if (element == null)
				throw new ArgumentNullException ("element");
			if (Condition.AppliesTo (element))
				return element;
			if (element == AutomationElement.RootElement)
				// LAMESPEC: This is according to MSDN:
				// http://msdn.microsoft.com/en-us/library/system.windows.automation.treewalker.normalize.aspx
//				return element;
				// This is matching Microsoft's actual implementation:
				return null;
			return Normalize (RawViewWalker.GetParent (element));
		}

		public AutomationElement GetParent (AutomationElement element, CacheRequest request)
		{
			if (request == null)
				throw new ArgumentNullException ("request");
			var result = GetParent (element);
			if (result != null)
				return result.GetUpdatedCache (request);
			return result;
		}

		public AutomationElement GetFirstChild (AutomationElement element, CacheRequest request)
		{
			if (request == null)
				throw new ArgumentNullException ("request");
			var result = GetFirstChild (element);
			if (result != null)
				return result.GetUpdatedCache (request);
			return result;
		}

		public AutomationElement GetLastChild (AutomationElement element, CacheRequest request)
		{
			if (request == null)
				throw new ArgumentNullException ("request");
			var result = GetLastChild (element);
			if (result != null)
				return result.GetUpdatedCache (request);
			return result;
		}

		public AutomationElement GetNextSibling (AutomationElement element, CacheRequest request)
		{
			if (request == null)
				throw new ArgumentNullException ("request");
			var result = GetNextSibling (element);
			if (result != null)
				return result.GetUpdatedCache (request);
			return result;
		}

		public AutomationElement GetPreviousSibling (AutomationElement element, CacheRequest request)
		{
			if (request == null)
				throw new ArgumentNullException ("request");
			var result = GetPreviousSibling (element);
			if (result != null)
				return result.GetUpdatedCache (request);
			return result;
		}

		public AutomationElement Normalize (AutomationElement element, CacheRequest request)
		{
			if (request == null)
				throw new ArgumentNullException ("request");
			var result = Normalize (element);
			if (result != null)
				return result.GetUpdatedCache (request);
			return result;
		}
		#endregion

		#region Public Properties
		public Condition Condition { get; private set; }
		#endregion

		#region Public Static Fields
		public static readonly TreeWalker RawViewWalker;

		public static readonly TreeWalker ControlViewWalker;

		public static readonly TreeWalker ContentViewWalker;
		#endregion
	}
}
