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
// Copyright (c) 2008 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//      Brad Taylor <brad@getcoded.net>
// 

using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Windows.Automation
{
	public sealed class CacheRequest
	{
#region Private Fields
		private List<RequestToken> disposables =
			new List<RequestToken> ();
		private List<AutomationPattern> cachedPatterns =
			new List<AutomationPattern> ();
		private List<AutomationProperty> cachedProperties =
			new List<AutomationProperty> ();
		private AutomationElementMode mode;
		private Condition treeFilter;
		private TreeScope scope;
#endregion

#region Public Properties
		public AutomationElementMode AutomationElementMode {
			get { return mode; }
			set {
				if (IsContainedInAnyThreadStack ())
					throw new InvalidOperationException ("Cannot modify a CacheRequest that is already on the stack");
				mode = value;
			}
		}

		public Condition TreeFilter {
			get { return treeFilter; }
			set {
				if (IsContainedInAnyThreadStack ())
					throw new InvalidOperationException ("Cannot modify a CacheRequest that is already on the stack");
				treeFilter = value;
			}
		}

		public TreeScope TreeScope {
			get { return scope; }
			set {
				if ((value & TreeScope.Ancestors) == TreeScope.Ancestors ||
				    (value & TreeScope.Parent) == TreeScope.Parent)
					throw new ArgumentException ("TreeScope.Ancestors and TreeScope.Parent are invalid for CacheRequest.TreeScope");
				if (IsContainedInAnyThreadStack ())
					throw new InvalidOperationException ("Cannot modify a CacheRequest that is already on the stack");
				scope = value;
			}
		}
#endregion

#region Public Static Properties
		public static CacheRequest Current {
			get {
				var requestStack = GetCurrentStack ();
				return requestStack.Peek (); // NOTE: We guarantee non-empty stack
			}
		}
#endregion

#region Internal Properties
		internal IList<AutomationProperty> CachedProperties {
			get { return cachedProperties; }
		}

		internal IList<AutomationPattern> CachedPatterns {
			get { return cachedPatterns; }
		}
#endregion

#region Constructor
		public CacheRequest ()
		{
			mode = AutomationElementMode.Full;
			treeFilter = Automation.ControlViewCondition;
			scope = TreeScope.Element;
		}
#endregion

#region Public Methods
		public IDisposable Activate ()
		{
			var requestStack = GetCurrentStack ();
			RequestToken disposable = new RequestToken (this);
			disposables.Add (disposable);
			requestStack.Push (this);
			return disposable;
		}

		public void Add (AutomationPattern pattern)
		{
			if (pattern == null)
				throw new ArgumentNullException ("pattern");
			if (IsContainedInAnyThreadStack ())
				throw new InvalidOperationException ("Cannot modify an active CacheRequest");
			cachedPatterns.Add (pattern);
		}

		public void Add (AutomationProperty property)
		{
			if (property == null)
				throw new ArgumentNullException ("property");
			if (IsContainedInAnyThreadStack ())
				throw new InvalidOperationException ("Cannot modify an active CacheRequest");
			cachedProperties.Add (property);
		}

		public CacheRequest Clone ()
		{
			var clone = new CacheRequest ();
			clone.TreeFilter = TreeFilter;
			clone.TreeScope = TreeScope;
			clone.AutomationElementMode = AutomationElementMode;

			clone.cachedPatterns.AddRange (cachedPatterns);
			clone.cachedProperties.AddRange (cachedProperties);

			return clone;
		}

		public void Pop ()
		{
			var requestStack = GetCurrentStack ();
			if (requestStack.Peek () != this ||
			    this == DefaultRequest)
				throw new InvalidOperationException ("Can only pop CacheRequest.Current");
			requestStack.Pop ();
		}

		public void Push ()
		{
			var requestStack = GetCurrentStack ();
			requestStack.Push (this);
		}
#endregion

#region Private Static Members
		private static Dictionary<Thread, Stack<CacheRequest>> requestStacks =
			new Dictionary<Thread, Stack<CacheRequest>> ();
		internal static readonly CacheRequest DefaultRequest =
			new CacheRequest ();
		static Object stackLock = new Object ();

		static Stack<CacheRequest> GetCurrentStack ()
		{
			Stack<CacheRequest> requestStack;
			if (!requestStacks.TryGetValue (Thread.CurrentThread, out requestStack)) {
				lock (stackLock) {
					if (!requestStacks.TryGetValue (Thread.CurrentThread, out requestStack)) {
						requestStack = new Stack<CacheRequest> ();
						requestStack.Push (DefaultRequest);
						requestStacks [Thread.CurrentThread] = requestStack;
					}
				}
			}
			return requestStack;
		}
#endregion

#region Private Methods
		private bool IsContainedInAnyThreadStack ()
		{
			lock (stackLock) {
				foreach (var stack in requestStacks.Values)
					if (stack.Contains (this))
						return true;
			}
			return false;
		}
#endregion

#region Private Disposable RequestToken Class
		private class RequestToken : IDisposable
		{
			private CacheRequest request;

			public RequestToken (CacheRequest request)
			{
				this.request = request;
			}

			public void Dispose ()
			{
				if (request.disposables.Count != 0) {
				    //request.disposables.Peek () == this) {
					request.disposables.Remove (this);
					request.Pop ();
				}
			}
		}
#endregion
	}
}
