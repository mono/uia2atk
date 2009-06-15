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
	public sealed class TreeWalker
	{
		#region Private Fields
		private static IList<IAutomationSource> automationSources;

		private Condition condition;
		private List<AutomationElement> directChildren =
			new List<AutomationElement> ();
		#endregion

		#region Static Constructor
		static TreeWalker ()
		{
			automationSources = SourceManager.GetAutomationSources ();
			RawViewWalker = new TreeWalker (null);
			foreach (IAutomationSource source in automationSources)
				foreach (IElement sourceElement in source.GetRootElements ())
					RawViewWalker.directChildren.Add (new AutomationElement (sourceElement));
		}
		#endregion

		#region Public Constructor
		public TreeWalker (Condition condition)
		{
			this.condition = condition;
		}
		#endregion

		#region Public Methods
		public AutomationElement GetParent (AutomationElement element)
		{
			if (element == AutomationElement.RootElement)
				return null;
			return SourceManager.GetOrCreateAutomationElement (element.SourceElement.Parent);
		}

		public AutomationElement GetFirstChild (AutomationElement element)
		{
			if (element == AutomationElement.RootElement)
				return RawViewWalker.directChildren.Count > 0 ?
					RawViewWalker.directChildren [0] :
					null;
			return SourceManager.GetOrCreateAutomationElement (element.SourceElement.FirstChild);
		}

		public AutomationElement GetLastChild (AutomationElement element)
		{
			if (element == AutomationElement.RootElement)
				return RawViewWalker.directChildren.Count > 0 ?
					RawViewWalker.directChildren [RawViewWalker.directChildren.Count - 1] :
					null;
			return SourceManager.GetOrCreateAutomationElement (element.SourceElement.LastChild);
		}

		public AutomationElement GetNextSibling (AutomationElement element)
		{
			if (element == AutomationElement.RootElement)
				return null;

			int elementIndex = RawViewWalker.directChildren.IndexOf (element);
			if (elementIndex >= 0 && elementIndex < (RawViewWalker.directChildren.Count - 1))
				return RawViewWalker.directChildren [elementIndex + 1];

			return SourceManager.GetOrCreateAutomationElement (element.SourceElement.NextSibling);
		}

		public AutomationElement GetPreviousSibling (AutomationElement element)
		{
			if (element == AutomationElement.RootElement)
				return null;

			int elementIndex = RawViewWalker.directChildren.IndexOf (element);
			if (elementIndex > 0)
				return RawViewWalker.directChildren [elementIndex - 1];

			return SourceManager.GetOrCreateAutomationElement (element.SourceElement.PreviousSibling);
		}

		public AutomationElement Normalize (AutomationElement element)
		{
			throw new NotImplementedException ();
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
		public Condition Condition {
			get {
				return condition;
			}
		}
		#endregion

		#region Public Static Fields
		public static readonly TreeWalker RawViewWalker;

		public static readonly TreeWalker ControlViewWalker;

		public static readonly TreeWalker ContentViewWalker;
		#endregion
	}
}
