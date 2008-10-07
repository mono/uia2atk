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
using System.Drawing;
using System.Windows.Automation;
using System.Windows.Forms;
using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms.Client
{
	//According to http://msdn.microsoft.com/en-us/library/ms742462.aspx
	[TestFixture]
	[Description ("Tests SWF.ListView as ControlType.List & ControlType.DataGrid")]
	public class ListViewTest : ListBoxTest
	{

		#region Properties

		[Test]
		[LameSpec]
		[Description ("Value: True | Notes: The list control is always included in the control view of the UI Automation tree.")]
		public override void MsdnIsControlElementPropertyTest ()
		{
			AutomationElement child = GetAutomationElement ();
			Assert.AreEqual (true,
				child.GetCurrentPropertyValue (AutomationElementIdentifiers.IsControlElementProperty, true),
				"IsControlElementProperty");
		}

		#endregion

		#region Patterns

		[Test]
		[Description ("Support/Value: Depends. | Notes: Implement this control pattern if the control can "
			+ "support multiple views of the items in the container.")]
		public override void MsdnMultipleViewPatternTest ()
		{
			AutomationElement element = GetAutomationElement ();
			Assert.IsTrue (SupportsPattern (element, MultipleViewPatternIdentifiers.Pattern),
				string.Format ("MultipleViewPattern SHOULD BE supported: {0} -> {1}",
				GetControl ().GetType (), GetControlTypeFromElement (element).ProgrammaticName));
		}

		[Test]
		[UISpyMissing]
		[Description ("Support/Value: Depends. | Notes: Implement this pattern when grid navigation needs "
			+"to be available on an item by item basis.")]
		public override void MsdnGridPatternTest ()
		{
			AutomationElement element = GetAutomationElement ();
			Assert.IsTrue (SupportsPattern (element, GridPatternIdentifiers.Pattern),
				string.Format ("GridPattern SHOULD BE supported: {0} -> {1}",
				GetControl ().GetType (), GetControlTypeFromElement (element).ProgrammaticName));
		}

		#endregion

		#region MutipleView Pattern Tests

		[Test]
		[UISpyMissing]
		public void MultipleView_GetSupportedViewsTest ()
		{
			ListView view = GetControl () as ListView;
			int []supportedViews = null;
			AutomationElement element;
			MultipleViewPattern pattern;

			foreach (View viewVal in Enum.GetValues (typeof (View))) {
				view.View = viewVal;
				element = GetAutomationElementFromControl (view);
				pattern = element.GetCurrentPattern (MultipleViewPatternIdentifiers.Pattern) as MultipleViewPattern;

				supportedViews = pattern.Current.GetSupportedViews ();
				Assert.AreEqual (1, supportedViews.Length, string.Format ("GetSupportedViews Length: {0}", viewVal));
				Assert.AreEqual (0, supportedViews [0], string.Format ("GetSupportedViews Value ", viewVal));
			}
		}

		[Test]
		[UISpyMissing]
		public void MultipleView_CurrentViewTest ()
		{
			ListView view = GetControl () as ListView;
			AutomationElement element;
			MultipleViewPattern pattern;

			foreach (View viewVal in Enum.GetValues (typeof (View))) {
				view.View = viewVal;
				element = GetAutomationElementFromControl (view);
				pattern = element.GetCurrentPattern (MultipleViewPatternIdentifiers.Pattern) as MultipleViewPattern;

				Assert.AreEqual (0, pattern.Current.CurrentView, string.Format ("CurrentView Value = 0 -> {0}", view.View));
			}
		}

		[Test]
		public void MultipleView_ViewNameTest ()
		{
		    ListView view = GetControl () as ListView;
		    AutomationElement element;
		    MultipleViewPattern pattern;
			int []supportedViews = null;

			foreach (View viewVal in Enum.GetValues (typeof (View))) {
				view.View = viewVal;
				element = GetAutomationElementFromControl (view);
				pattern = element.GetCurrentPattern (MultipleViewPatternIdentifiers.Pattern) as MultipleViewPattern;

				supportedViews = pattern.Current.GetSupportedViews ();
				int lastViewId = 0;
				foreach (int viewId in supportedViews) {
					pattern.SetCurrentView (viewId);
					Assert.AreEqual ("Icons", pattern.GetViewName (viewId), 
						string.Format ("GetViewName -> {0}", view.View));
				}
				//We should throw an exception
				try {
					lastViewId += 12345;
					pattern.SetCurrentView (lastViewId);
					Assert.Fail ("Should throw ArgumentException");
				} catch (ArgumentException) { }
			}
		}


		[Test]
		public void MultipleView_SetCurrentViewTest ()
		{
			ListView view = GetControl () as ListView;
			AutomationElement element;
			MultipleViewPattern pattern;
			int [] supportedViews = null;

			foreach (View viewVal in Enum.GetValues (typeof (View))) {
				view.View = viewVal;
				element = GetAutomationElementFromControl (view);
				pattern = element.GetCurrentPattern (MultipleViewPatternIdentifiers.Pattern) as MultipleViewPattern;

				supportedViews = pattern.Current.GetSupportedViews ();
				int lastViewId = 0;
				foreach (int viewId in supportedViews) {
					pattern.SetCurrentView (viewId);
					Assert.AreEqual ("Icons", pattern.GetViewName (viewId),
						string.Format ("GetViewName -> {0}", view.View));
					lastViewId = viewId;
				}
				//We should throw an exception
				try {
					lastViewId += 12345;
					pattern.SetCurrentView (lastViewId);
					Assert.Fail ("Should throw ArgumentException");
				} catch (ArgumentException) { }
			}
		}

		#endregion

		#region Control Type Tests

		[Test]
		public void ControlTypeTest ()
		{
			ListView view = GetControl () as ListView;
			AutomationElement element;
			ControlType defaultControlType = ControlType.List;

			foreach (View viewVal in Enum.GetValues (typeof (View))) {
				view.View = viewVal;
				element = GetAutomationElementFromControl (view);

				ControlType controlType
					= element.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty) as ControlType;

				if (view.View == View.Details)
					defaultControlType = ControlType.DataGrid;
				else
					defaultControlType = ControlType.List;

				Assert.AreEqual (defaultControlType, controlType,
					string.Format ("Different Control Type: {0} {1}", controlType.ProgrammaticName, view.View));
			}
		}

		#endregion

		#region Protected Methods

		protected override Control GetControl ()
		{
			ListView listview = new ListView ();
			listview.View = View.LargeIcon;
			listview.Items.Add (new ListViewItem (new string [] {"1", "2", "3", "4", "5", "6"}));
			listview.Size = new Size (100, 100);
			listview.Location = new Point (3, 3);
			return listview;
		}

		#endregion
	}
}
