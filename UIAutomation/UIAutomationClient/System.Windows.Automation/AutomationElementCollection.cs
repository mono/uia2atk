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
//      Sandy Armstrong <sanfordarmstrong@gmail.com>
// 

using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Windows.Automation
{
	public class AutomationElementCollection : ICollection, IEnumerable
	{
#region Private Fields
		private List<AutomationElement> elements;
#endregion

#region Public Properties
		public int Count {
			get { return elements.Count; }
		}

		public virtual bool IsSynchronized {
			get { return ((ICollection) elements).IsSynchronized; }
		}

		public AutomationElement this [int index] {
			get { return elements [index]; }
		}

		public virtual object SyncRoot {
			get { return ((ICollection) elements).SyncRoot; }
		}
#endregion

#region Constructor
		internal AutomationElementCollection (IEnumerable<AutomationElement> elements)
		{
			this.elements = new List<AutomationElement> (elements);
		}
#endregion

#region Public Methods
		public virtual void CopyTo (Array array, int index)
		{
			((ICollection) elements).CopyTo (array, index);
		}

		public void CopyTo (AutomationElement[] array, int index)
		{
			elements.CopyTo (array, index);
		}

		public IEnumerator GetEnumerator ()
		{
			return elements.GetEnumerator ();
		}
#endregion
	}
}
