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
//	Mario Carrion <mcarrion@novell.com>
// 

using System;
using System.Windows.Automation.Provider;
using NUnit.Framework;
using Mono.UIAutomation.Winforms.Navigation;

namespace MonoTests.Mono.UIAutomation.Winforms
{
	class DummyNavigation : INavigation
	{
		
#region INavigation implementation 
		
		public void FinalizeProvider ()
		{
		}
		
		public IRawElementProviderFragment Navigate (NavigateDirection direction)
		{
			return null;
		}
		
		public System.Windows.Automation.Provider.IRawElementProviderSimple Provider {
			get { return null; }
		}
		
		public INavigation NextSibling {
			get { return next; }
			set { next = value; }
		}
		
		public INavigation NextNavigableSibling {
			get {
				throw new NotImplementedException();
			}
		}
		
		public INavigation PreviousSibling {
			get { return previous; }
			set { previous = value; }
		}
		
		public INavigation PreviousNavigableSibling {
			get { return previous; }
		}
		
		public bool SupportsNavigation {
			get { return true; }
		}
		
#endregion	
		
		#region Private Fields
		
		private INavigation next;
		private INavigation previous;
		
		#endregion

	}
	
	[TestFixture]
	public class NavigationChainTest
	{
		
		[Test]
		public void NavigationTest ()
		{
			DummyNavigation first = new DummyNavigation ();
			DummyNavigation second = new DummyNavigation ();
			DummyNavigation third = new DummyNavigation ();
			
			//Chain should be:
			//first -> second -> third
			
			NavigationChain chain = new NavigationChain ();
			chain.AddLink (first);
			chain.AddLink (second);
			chain.AddLink (third);
			
			Assert.AreEqual (second, first.NextSibling, "first.NextSibling = second");
			Assert.AreEqual (third, second.NextSibling, "second.NextSibling = third");
			Assert.AreEqual (null, third.NextSibling, "third.NextSibling = null");
			
			Assert.AreEqual (null, first.PreviousSibling, "first.PreviousSibling = null");
			Assert.AreEqual (first, second.PreviousSibling, "second.PreviousSibling = first");
			Assert.AreEqual (second, third.PreviousSibling, "third.PreviousSibling = second");
			
			//Chain should be:
			//first -> third
			chain.RemoveLink (second);
			
			Assert.AreEqual (null, second.NextSibling, "second.NextSibling = null");
			Assert.AreEqual (null, second.PreviousSibling, "second.PreviousSibling = first");
			
			Assert.AreEqual (third, first.NextSibling, "first.NextSibling = third");
			Assert.AreEqual (null, first.PreviousSibling, "first.PreviousSibling = null");
			Assert.AreEqual (first, third.PreviousSibling, "third.PreviousSibling = first");
			Assert.AreEqual (null, third.NextSibling, "third.NextSibling = null");

			chain.Clear ();
			chain.AddLink (first);
			chain.AddLink (second);
			chain.AddLink (third);
			
			//Chain should be:
			//second -> third
			chain.RemoveLink (first);
			
			Assert.AreEqual (null, first.NextSibling, "first.NextSibling = null");
			Assert.AreEqual (null, first.PreviousSibling, "first.PreviousSibling = first");
			
			Assert.AreEqual (third, second.NextSibling, "second.NextSibling = third");
			Assert.AreEqual (null, second.PreviousSibling, "second.PreviousSibling = null");
			Assert.AreEqual (second, third.PreviousSibling, "third.PreviousSibling = second");
			Assert.AreEqual (null, third.NextSibling, "third.NextSibling = null");

			chain.Clear ();
			chain.AddLink (first);
			chain.AddLink (second);
			chain.AddLink (third);
			
			//Chain should be:
			//first -> second
			chain.RemoveLink (third);
			
			Assert.AreEqual (null, third.NextSibling, "third.NextSibling = null");
			Assert.AreEqual (null, third.PreviousSibling, "third.PreviousSibling = first");
			
			Assert.AreEqual (second, first.NextSibling, "first.NextSibling = second");
			Assert.AreEqual (null, first.PreviousSibling, "first.PreviousSibling = second");
			
			Assert.AreEqual (null, second.NextSibling, "second.NextSibling = null");
			Assert.AreEqual (first, second.PreviousSibling, "second.PreviousSibling = first");
			
		}
	}
}
