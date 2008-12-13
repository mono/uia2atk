// MiscHierarchyTest.cs created with MonoDevelop
// User: knocte at 4:53 PM 12/12/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using SWF = System.Windows.Forms;
using System.Windows.Automation;

using Mono.UIAutomation.Winforms;

using NUnit;
using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms
{
	
	[TestFixture]
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
