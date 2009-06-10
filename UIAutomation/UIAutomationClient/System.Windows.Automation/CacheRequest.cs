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

namespace System.Windows.Automation
{
	public sealed class CacheRequest
	{
#region Public Properties
		public AutomationElementMode AutomationElementMode {
			get; set;
		}

		public Condition TreeFilter {
			get; set;
		}

		public TreeScope TreeScope {
			get; set;
		}
#endregion

#region Public Static Properties
		public static CacheRequest Current {
			get { throw new NotImplementedException (); }
		}
#endregion

#region Constructor
		public CacheRequest ()
		{
		}
#endregion
		
#region Public Methods
		public IDisposable Activate ()
		{
			throw new NotImplementedException ();
		}

		public void Add (AutomationPattern pattern)
		{
			throw new NotImplementedException ();
		}

		public void Add (AutomationProperty property)
		{
			throw new NotImplementedException ();
		}

		public CacheRequest Clone ()
		{
			throw new NotImplementedException ();
		}

		public void Pop ()
		{
			throw new NotImplementedException ();
		}

		public void Push ()
		{
			throw new NotImplementedException ();
		}
#endregion
	}
}
