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
// Copyright (c) 2008,2009 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//      Andres G. Aragoneses <aaragoneses@novell.com>
// 
using System;
using SWF = System.Windows.Forms;

using NUnit.Framework;
using System.Threading;

namespace UiaAtkBridgeTest
{

	[TestFixture]
	[Ignore ("Run this TestFixture independently from the BridgeTester")]
	public class DialogTester {
		
		[Test]
		public void OpenFileDialog ()
		{
			TestDialog (new SWF.OpenFileDialog ());
		}

		[Test]
		public void SaveFileDialog ()
		{
			TestDialog (new SWF.SaveFileDialog ());
		}

		[Test]
		public void ColorDialog ()
		{
			TestDialog (new SWF.ColorDialog ());
		}

		[Test]
		public void FontDialog ()
		{
			TestDialog (new SWF.FontDialog ());
		}

		
		static void TestDialog (SWF.CommonDialog dialog)
		{
			new DialogTesterInner (dialog).Test ();
		}

		private class DialogTesterInner {
			SWF.CommonDialog dialog;
			Thread th;
	
			internal DialogTesterInner (SWF.CommonDialog dialog)
			{
				this.dialog = dialog;
			}
			
			internal void Test ()
			{
				th = new Thread (new ThreadStart (Show));
				th.SetApartmentState (ApartmentState.STA);
				try {
					th.Start ();
					Thread.Sleep (10000);
					UiaAtkBridge.Window dialogAdapter = BridgeTester.GetAdapterForWidget (dialog) as UiaAtkBridge.Window;
					Assert.IsNotNull (dialogAdapter, "dialogAdapter has a different type than Window");
					Assert.AreEqual (dialogAdapter.Role, Atk.Role.Dialog, "dialog should have dialog role");
					Assert.IsTrue (dialogAdapter.NAccessibleChildren > 0, "dialog should have children");
				}
				finally {
					dialog.Dispose ();
					th.Abort ();
				}
			}
	
			private void Show ()
			{
				dialog.ShowDialog ();
			}
		}
	}
	
	

}
