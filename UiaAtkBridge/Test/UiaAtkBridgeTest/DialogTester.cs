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
	public class DialogTester {
		SWF.CommonDialog dialog;

		internal DialogTester (SWF.CommonDialog dialog)
		{
			this.dialog = dialog;
		}

		[Test]
		public void OpenFileDialog ()
		{
			OpenFileDialogStatic ();
		}

		internal static void OpenFileDialogStatic ()
		{
			new DialogTester (new SWF.OpenFileDialog ()).Test ();
		}

		internal void Test ()
		{
			var th = new Thread (new ThreadStart (Show));
			th.SetApartmentState (ApartmentState.STA);
			try {
				th.Start ();
				Atk.Object dialogAdapter = BridgeTester.GetAdapterForWidget (dialog);
				Assert.AreEqual (dialogAdapter.Role, Atk.Role.Dialog, "dialog should have dialog role");
				Assert.IsTrue (dialogAdapter.NAccessibleChildren > 0, "dialog should have children");
			}
			finally {
				th.Abort ();
			}
		}

		private void Show ()
		{
			dialog.ShowDialog ();
		}
	}
}
