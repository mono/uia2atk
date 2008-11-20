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
//      Sandy Armstrong <sanfordarmstrong@gmail.com>
// 

using System;
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

using Mono.UIAutomation.Winforms;

using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms
{
	[TestFixture]
	public class ToolStripTextBoxProviderTest : TextBoxProviderTest
	{
		[Test]
		public void NavigationTest ()
		{
			ToolStrip strip = new ToolStrip ();
			ToolStripTextBox stripTextBox1 = new ToolStripTextBox ();
			ToolStripMenuItem item = new ToolStripMenuItem ();
			ToolStripTextBox stripTextBox2 = new ToolStripTextBox ();

			item.DropDownItems.Add (stripTextBox2);
			strip.Items.Add (item);
			strip.Items.Add (stripTextBox1);

			IRawElementProviderFragment stripProvider =
				GetProviderFromControl (strip);
			
			IRawElementProviderFragment itemProvider = (IRawElementProviderFragment)
				ProviderFactory.GetProvider (item);

			IRawElementProviderFragment box1Provider = (IRawElementProviderFragment)
				ProviderFactory.GetProvider (stripTextBox1);
			IRawElementProviderFragment box2Provider = (IRawElementProviderFragment)
				ProviderFactory.GetProvider (stripTextBox2);

			Assert.AreEqual (stripProvider,
			                 box1Provider.Navigate (NavigateDirection.Parent));
			Assert.AreEqual (itemProvider,
			                 box2Provider.Navigate (NavigateDirection.Parent));
		}
		
		protected override TextBox CreateTextBox ()
		{
			ToolStripTextBox stripTextBox = new ToolStripTextBox ();
			return stripTextBox.TextBox;
		}

		protected override Control GetControlInstance()
		{
			return null;
		}


	}
}
