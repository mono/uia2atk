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
		private static IList<IAutomationSource> automationSources;

		private List<AutomationElement> directChildren;
		#endregion

		#region Static Constructor
		static TreeWalker ()
		{
			automationSources = SourceManager.GetAutomationSources ();
			RawViewWalker = new TreeWalker (Automation.RawViewCondition);
			RawViewWalker.directChildren = new List<AutomationElement> ();
			foreach (IAutomationSource source in automationSources)
				foreach (IElement sourceElement in source.GetRootElements ())
					RawViewWalker.directChildren.Add (new AutomationElement (sourceElement));

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
			throw new NotImplementedException ();
		}

		public AutomationElement GetFirstChild (AutomationElement element, CacheRequest request)
		{
			throw new NotImplementedException ();
		}

		public AutomationElement GetLastChild (AutomationElement element, CacheRequest request)
		{
			throw new NotImplementedException ();
		}

		public AutomationElement GetNextSibling (AutomationElement element, CacheRequest request)
		{
			throw new NotImplementedException ();
		}

		public AutomationElement GetPreviousSibling (AutomationElement element, CacheRequest request)
		{
			throw new NotImplementedException ();
		}

		public AutomationElement Normalize (AutomationElement element, CacheRequest request)
		{
			throw new NotImplementedException ();
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
