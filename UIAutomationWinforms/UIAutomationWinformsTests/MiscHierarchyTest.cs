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
//      Andrés G. Aragoneses <aaragoneses@novell.com>
// 

using System;
using SWF = System.Windows.Forms;
using System.Windows.Automation;

using Mono.UIAutomation.Winforms;

using NUnit;
using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms
{

	// FIXME: Enable when complete.
	//[TestFixture]
	public class MiscHierarchyTest : BaseProviderTest
	{
		[Test]
		public void TextBoxWithButtonTest ()
		{
			SWF.Button but1 = new SWF.Button ();
			SWF.TextBox tbx1 = new SWF.TextBox ();
			SWF.Form form = new SWF.Form ();
			ProviderFactory.GetProvider (form);

			bridge.ResetEventLists ();
			
			form.Controls.Add (but1);
			form.Controls.Add (tbx1);
			
			form.Show ();
//			Assert.AreEqual (bridge.StructureChangedEvents[0].e.StructureChangeType, StructureChangeType.ChildAdded);
//			Assert.AreEqual ("", bridge.StructureChangedEvents[0].provider.GetType().Name);
			Assert.AreEqual (2, bridge.StructureChangedEvents.Count, "2 control additions");
		}

		protected override SWF.Control GetControlInstance () {
			return null;
		}
	}
}
