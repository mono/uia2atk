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
//  Sandy Armstrong <sanfordarmstrong@gmail.com>
// 

using System;

namespace System.Windows.Automation
{
	public sealed class TreeWalker
	{
		private Condition condition;
		
		public TreeWalker (Condition condition)
		{
			this.condition = condition;
		}

		public AutomationElement GetParent (AutomationElement element)
		{
			throw new NotImplementedException ();
		}

		public AutomationElement GetFirstChild (AutomationElement element)
		{
			throw new NotImplementedException ();
		}

		public AutomationElement GetLastChild (AutomationElement element)
		{
			throw new NotImplementedException ();
		}

		public AutomationElement GetNextSibling (AutomationElement element)
		{
			throw new NotImplementedException ();
		}

		public AutomationElement GetPreviousSibling (AutomationElement element)
		{
			throw new NotImplementedException ();
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

		public Condition Condition {
			get {
				return condition;
			}
		}

		public static readonly TreeWalker RawViewWalker;

		public static readonly TreeWalker ControlViewWalker;

		public static readonly TreeWalker ContentViewWalker;
	}
}
