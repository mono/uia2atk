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

		[Test]
		public void OpenFileDialog ()
		{
			using (var runner = new DialogRunner (new SWF.OpenFileDialog ())) {
				VerifyBasicProperties (runner.Dialog);

				UiaAtkBridge.Window dialogAdapter =
					BridgeTester.GetAdapterForWidget (runner.Dialog) as UiaAtkBridge.Window;

				Atk.Object popupButtonPanelAdapter = dialogAdapter.RefAccessibleChild (10);
				Assert.AreEqual (5, popupButtonPanelAdapter.NAccessibleChildren, "PopupButtonPanel (toolbar) should have 5 children");

				Atk.Object popupButtonAdapter1 = popupButtonPanelAdapter.RefAccessibleChild (0).RefAccessibleChild (0);
				AtkTester.States (popupButtonAdapter1,
				                  Atk.StateType.Enabled,
				                  Atk.StateType.Selectable,
				                  Atk.StateType.Focusable,
				                  Atk.StateType.Sensitive,
				                  Atk.StateType.Showing,
				                  Atk.StateType.Visible);
				// TODO: Simulate selecting so we can teset Selected/Focused
			}
		}

		[Test]
		public void SaveFileDialog ()
		{
			using (var runner = new DialogRunner (new SWF.SaveFileDialog ())) {
				VerifyBasicProperties (runner.Dialog);
			}
		}

		[Test]
		public void ColorDialog ()
		{
			using (var runner = new DialogRunner (new SWF.ColorDialog ())) {
				VerifyBasicProperties (runner.Dialog);
			}
		}

		[Test]
		public void FontDialog ()
		{
			using (var runner = new DialogRunner (new SWF.FontDialog ())) {
				VerifyBasicProperties (runner.Dialog);
			}
		}

		[Test]
		public void ThreadExceptionDialog ()
		{
			System.Exception exception = new System.Exception ();
			try {
			int zero = 0;
				int x = 1 / zero;
				zero = x;
			} catch (System.Exception e) {
				exception = e;
			}

			using (var runner = new DialogRunner (new SWF.ThreadExceptionDialog (exception))) {
				VerifyBasicProperties (runner.Dialog);
			}
		}

		//TODO: test Atk.Role.Dialog when using ShowDialog() [currently threading problems]
		private void VerifyBasicProperties (System.ComponentModel.Component dialog)
		{
			UiaAtkBridge.Window dialogAdapter = BridgeTester.GetAdapterForWidget (dialog) as UiaAtkBridge.Window;
			Assert.IsNotNull (dialogAdapter, "dialogAdapter has a different type than Window");
			Assert.IsTrue (dialogAdapter.NAccessibleChildren > 0, "dialog should have children");
		}
	}

	public class DialogRunner : IDisposable
	{
		private SWF.CommonDialog commonDialog;
		private SWF.ThreadExceptionDialog threadExceptionDialog;
		private SWF.Form f;

		public DialogRunner (System.ComponentModel.Component dialog)
		{
			commonDialog = dialog as SWF.CommonDialog;
			if (commonDialog == null)
				threadExceptionDialog = dialog as SWF.ThreadExceptionDialog;
			if (commonDialog == null && threadExceptionDialog == null)
				throw new ArgumentException ("Unsupported dialog type: " + dialog);

			Show ();
		}

		public void Dispose ()
		{
			if (commonDialog != null)
				commonDialog.Dispose ();
			else if (threadExceptionDialog != null) {
				if (threadExceptionDialog.InvokeRequired) {
					threadExceptionDialog.BeginInvoke (new SWF.MethodInvoker (Dispose));
					return;
				}
				threadExceptionDialog.Close ();
				threadExceptionDialog.Dispose ();
			}
		}

		private void Show ()
		{
			if (commonDialog != null) {
				var fi = typeof (SWF.CommonDialog).GetField ("form",
				           System.Reflection.BindingFlags.Instance |
				           System.Reflection.BindingFlags.NonPublic);
				f = (SWF.Form)fi.GetValue (commonDialog);
			}
			else if (threadExceptionDialog != null)
				f = threadExceptionDialog;
			f.Show ();
		}

		public SWF.Form Dialog {
			get { return f; }
		}
	}
}
