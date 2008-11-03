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
//      Andres G. Aragoneses <aaragoneses@novell.com>
// 

using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace UiaAtkBridgeTest
{

	public abstract class AtkTests : AtkTester
	{
		
		[Test]
		public void Label ()
		{
			Console.WriteLine ("<Test id=\"Label\">");
			
			BasicWidgetType type = BasicWidgetType.Label;
			Atk.Object accessible = InterfaceText (type);

			PropertyRole (type, accessible);
			
			//a label always contains this state, not because it's multi_line, but because it can be multi_line
			States (accessible,
			  Atk.StateType.MultiLine,
			  Atk.StateType.Enabled,
			  Atk.StateType.Sensitive,
			  Atk.StateType.Showing,
			  Atk.StateType.Visible);
			
			Assert.AreEqual (0, accessible.NAccessibleChildren, "Label numChildren");
			
			//TODO: check parent (it seems it only works for real objects)
			//Assert.IsNotNull (accessible.Parent, "Label parent");

			Console.WriteLine ("</Test>");
		}
		
		[Test]
		public void Button ()
		{
			Console.WriteLine ("<Test id=\"Button\">");
			
			BasicWidgetType type = BasicWidgetType.NormalButton;
			Atk.Object accessible;

			string name = "test";
			accessible = GetAccessible (type, name, true);
			
			Atk.StateSet stateSet = accessible.RefStateSet();
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Enabled), "Button Enabled");
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Focusable), "Button Focusable");
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Sensitive), "Button Sensitive");
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Showing), "Button Showing");
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Visible), "Button Visible");
			
			InterfaceText (type);

			Atk.Component atkComponent = CastToAtkInterface <Atk.Component> (accessible);
			InterfaceComponent (type, atkComponent);

			Atk.Action atkAction = CastToAtkInterface <Atk.Action> (accessible);
			InterfaceAction (type, atkAction, accessible);
			
			PropertyRole (type, accessible);
			
			Assert.AreEqual (0, accessible.NAccessibleChildren, "Button numChildren");

			Parent (type, accessible);

			//test with an image
			Atk.Image atkWithOutImage, atkWithImage;
			atkWithOutImage = CastToAtkInterface <Atk.Image> (accessible);
			accessible = GetAccessibleThatEmbedsAnImage (type, name, true);
			atkWithImage = CastToAtkInterface <Atk.Image> (accessible);
			atkComponent = CastToAtkInterface<Atk.Component> (accessible);
			InterfaceImage (type, atkWithImage, atkComponent, atkWithOutImage);

			Console.WriteLine ("</Test>");
		}

		[Test]
		public void Checkbox ()
		{
			Console.WriteLine ("<Test id=\"Checkbox\">");
			
			EventMonitor.Start ();

			BasicWidgetType type = BasicWidgetType.CheckBox;
			Atk.Object accessible;
			
			InterfaceText (type);
			
			string name = "test";
			accessible = GetAccessible (type, name, true);
			Atk.Component atkComponent = CastToAtkInterface <Atk.Component> (accessible);
			InterfaceComponent (type, atkComponent);

			Atk.StateSet stateSet = accessible.RefStateSet ();
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Enabled), "Checkbox Enabled");
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Sensitive), "Checkbox Sensitive");
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Focusable), "Checkbox Focusable");
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Showing), "Checkbox Showing");
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Visible), "Checkbox Visible");
			
			Atk.Action atkAction = CastToAtkInterface <Atk.Action> (accessible);
			InterfaceAction (type, atkAction, accessible);
			
			PropertyRole (type, accessible);
			
			Assert.AreEqual (0, accessible.NAccessibleChildren, "CheckBox numChildren");
			Parent (type, accessible);

			EventCollection events = EventMonitor.Pause ();
			string eventsInXml = String.Format (" events in XML: {0}", Environment.NewLine + events.OriginalGrossXml);
			string evType = "object:state-changed:checked";
			EventCollection checkboxEvs = events.FindByRole (Atk.Role.CheckBox).FindWithDetail1 ("1");
			EventCollection typeEvs = checkboxEvs.FindByType (evType);
			
			Assert.AreEqual (1, typeEvs.Count, "bad number of checked events!" + eventsInXml);

			//test with an image
			Atk.Image atkWithOutImage, atkWithImage;
			atkWithOutImage = CastToAtkInterface <Atk.Image> (accessible);
			accessible = GetAccessibleThatEmbedsAnImage (type, name, true);
			atkWithImage = CastToAtkInterface <Atk.Image> (accessible);
			atkComponent = CastToAtkInterface<Atk.Component> (accessible);
			InterfaceImage (type, atkWithImage, atkComponent, atkWithOutImage);

			Console.WriteLine ("</Test>");
		}
		
		[Test]
		public void RadioButtons ()
		{
			Console.WriteLine ("<Test id=\"RadioButtons\">");
			
			BasicWidgetType type = BasicWidgetType.RadioButton;
			Atk.Object accessible = null, accessible2 = null, accessible3 = null;
			string name = "test 01";

			accessible = GetAccessible (type, name, true);
			Atk.Action atkAction = CastToAtkInterface <Atk.Action> (accessible);
			InterfaceAction (type, atkAction, accessible);

			Atk.Component atkComponent = CastToAtkInterface <Atk.Component> (accessible);
			InterfaceComponent (type, atkComponent);
			
			PropertyRole (type, accessible);
			
			Assert.AreEqual (0, accessible.NAccessibleChildren, "RadioButton numChildren");

			Parent (type, accessible);
			
			//more than one radiobutton
			name = "test 02";
			accessible2 = GetAccessible (type, name, true);
			Atk.Action atkAction2 = CastToAtkInterface <Atk.Action> (accessible2);

			//the third radio button is disconnected from the previous ones
			name = "test 03";
			accessible3 = GetAccessible (type, name, true);
			Atk.Action atkAction3 = CastToAtkInterface <Atk.Action> (accessible3);

			InterfaceActionFor3RadioButtons (atkAction, accessible,
			                                 atkAction2, accessible2,
			                                 atkAction3, accessible3);

			Parent (type, accessible);
			Parent (type, accessible2);
			Parent (type, accessible3);

			RunInGuiThread (delegate () {
				accessible = InterfaceText (type, true);
			});
			
			//test with an image
			Atk.Image atkWithOutImage, atkWithImage;
			atkWithOutImage = CastToAtkInterface <Atk.Image> (accessible);
			accessible = GetAccessibleThatEmbedsAnImage (type, name, true);
			atkWithImage = CastToAtkInterface <Atk.Image> (accessible);
			atkComponent = CastToAtkInterface <Atk.Component> (accessible);
			InterfaceImage (type, atkWithImage, atkComponent, atkWithOutImage);

			Console.WriteLine ("</Test>");
		}
		
		[Test]
		public void StatusBar () { RunInGuiThread (RealStatusBar); }
		public void RealStatusBar()
		{
			Console.WriteLine ("<Test id=\"StatusBar\">");
			
			BasicWidgetType type = BasicWidgetType.StatusBar;

			Atk.Object accessible = InterfaceText (type, true);

			Assert.AreEqual (ValidNChildrenForASimpleStatusBar, accessible.NAccessibleChildren, "StatusBar numChildren");

			string name = "test";
			accessible = GetAccessible (type, name, true);

			PropertyRole (type, accessible);
			
			Atk.Component atkComponent = CastToAtkInterface <Atk.Component> (accessible);
			InterfaceComponent (type, atkComponent);
			int x, y, width, height;
			atkComponent.GetExtents (out x, out y, out width, out height, Atk.CoordType.Screen);
			Assert.IsTrue (width > 0 && height > 0, "width and height must be > 0");

			Parent (type, accessible);

			Console.WriteLine ("</Test>");
		}
		
		[Test]
		public void HScrollBar ()
		{
			Console.WriteLine ("<Test id=\"HScrollBar\">");
			
			BasicWidgetType type = BasicWidgetType.HScrollBar;
			Atk.Object accessible;
			string name = "test";

			accessible = GetAccessible (type, name, true);
			Atk.Value atkValue = CastToAtkInterface <Atk.Value> (accessible);
			
			InterfaceValue (type, atkValue);

			PropertyRole (type, accessible);

			States (accessible,
				Atk.StateType.Horizontal,
				Atk.StateType.Enabled,
				Atk.StateType.Sensitive,
				Atk.StateType.Showing,
				Atk.StateType.Visible);
			

			Assert.AreEqual (ValidNChildrenForAScrollBar, accessible.NAccessibleChildren, "HScrollBar numChildren");

			Atk.Component atkComponent = CastToAtkInterface <Atk.Component> (accessible);

			InterfaceComponent (type, atkComponent);

			Console.WriteLine ("</Test>");
		}
		
		[Test]
		public void VScrollBar ()
		{
			Console.WriteLine ("<Test id=\"VScrollBar\">");
			
			BasicWidgetType type = BasicWidgetType.VScrollBar;
			Atk.Object accessible;
			string name = "test";

			accessible = GetAccessible (type, name, true);
			
			Atk.Value atkValue = CastToAtkInterface <Atk.Value> (accessible);
			
			InterfaceValue (type, atkValue);

			PropertyRole (type, accessible);

			States (accessible,
				Atk.StateType.Vertical,
				Atk.StateType.Enabled,
				Atk.StateType.Sensitive,
				Atk.StateType.Showing,
				Atk.StateType.Visible);
			
			Assert.AreEqual (ValidNChildrenForAScrollBar, accessible.NAccessibleChildren, "VScrollBar numChildren");

			Atk.Component atkComponent = CastToAtkInterface <Atk.Component> (accessible);

			InterfaceComponent (type, atkComponent);

			Console.WriteLine ("</Test>");
		}
		
 		[Test]
		public void ProgressBar ()
		{
			Console.WriteLine ("<Test id=\"ProgressBar\">");
			
			BasicWidgetType type = BasicWidgetType.ProgressBar;
			Atk.Object accessible;
			string name = "test";

			accessible = GetAccessible (type, name, true);
			Atk.Value atkValue = CastToAtkInterface <Atk.Value> (accessible);
			
			InterfaceValue (type, atkValue);

			PropertyRole (type, accessible);

			Assert.AreEqual (0, accessible.NAccessibleChildren, "ProgressBar numChildren");

			Atk.Component atkComponent = CastToAtkInterface <Atk.Component> (accessible);
			InterfaceComponent (type, atkComponent);

			Parent (type, accessible);

			Console.WriteLine ("</Test>");
		}
		
 		[Test]
		public void Spinner ()
		{
			Console.WriteLine ("<Test id=\"Spinner\">");
			
			BasicWidgetType type = BasicWidgetType.Spinner;
			Atk.Object accessible;
			string name = "test";

			accessible = GetAccessible (type, name, true);
			Atk.Value atkValue = CastToAtkInterface <Atk.Value> (accessible);
			Atk.Text atkText = CastToAtkInterface <Atk.Text> (accessible);

			InterfaceValue (type, atkValue, atkText);

			InterfaceValue (type, atkValue);

			PropertyRole (type, accessible);

			Assert.AreEqual (0, accessible.NAccessibleChildren, "Spinner numChildren");

			Atk.Component atkComponent = CastToAtkInterface <Atk.Component> (accessible);
			
			InterfaceComponent (type, atkComponent);

			Console.WriteLine ("</Test>");
		}
		
		[Test]
		public void TextBoxEntry ()
		{
			Console.WriteLine ("<Test id=\"TextBoxEntry\">");
			
			BasicWidgetType type = BasicWidgetType.TextBoxEntry;
			Atk.Object accessible = InterfaceText (type, true);
			
			string name = "Edit test#1";
			accessible = GetAccessible (type, name, true);

			States (accessible,
			  Atk.StateType.Editable, 
			  Atk.StateType.Enabled, 
			  Atk.StateType.Focusable,
			  Atk.StateType.SingleLine,
			  Atk.StateType.Sensitive,
			  Atk.StateType.Showing,
			  Atk.StateType.Visible);
			
//			Atk.Action atkAction = CastToAtkInterface <Atk.Action> (accessible);
//			InterfaceAction (type, atkAction, accessible);
//			
//			name = "Edit test#2";
//			accessible = GetAccessible (type, name, true);
//			Atk.Component atkComponent = CastToAtkInterface<Atk.Component> (accessible);
//
//			InterfaceComponent (type, atkComponent);
//			
//			PropertyRole (type, accessible);
//			
//			Assert.AreEqual (0, accessible.NAccessibleChildren, "TextBoxEntry numChildren");

			Console.WriteLine ("</Test>");
		}

		[Test]
		public void TextBoxView ()
		{
			Console.WriteLine ("<Test id=\"TextBoxView\">");
			
			BasicWidgetType type = BasicWidgetType.TextBoxView;
			Atk.Object accessible = InterfaceText (type, false);
			
			string name = "Edit test#1";
			accessible = GetAccessible (type, name, true);

			States (accessible,
			  Atk.StateType.Editable, 
			  Atk.StateType.Enabled, 
			  Atk.StateType.Focusable,
			  Atk.StateType.MultiLine,
			  Atk.StateType.Sensitive,
			  Atk.StateType.Showing,
			  Atk.StateType.Visible);

			Console.WriteLine ("</Test>");
		}

		[Test]
		public void MaskedTextBoxEntry ()
		{
			Console.WriteLine ("<Test id=\"MaskedTextBoxEntry\">");
			
			BasicWidgetType type = BasicWidgetType.TextBoxEntry;
			Atk.Object accessible = null;
			
			accessible = InterfaceText (type, true);
			
			string name = "Edit test#1";
			accessible = GetAccessible (type, name, true);
			
			States (accessible,
			  Atk.StateType.Editable, 
			  Atk.StateType.Enabled, 
			  Atk.StateType.Focusable,
			  Atk.StateType.Sensitive,
			  Atk.StateType.Showing,
			  Atk.StateType.Visible,
			  Atk.StateType.SingleLine);

			Console.WriteLine ("</Test>");
		}
		
//		[Test]
//		public void MenuBar ()
//		{
//			List <MenuLayout> menu = new List <MenuLayout> ();
//			menu.Add (new MenuLayout ("XFile", new MenuLayout ("Quit")));
//			menu.Add (new MenuLayout ("GimmeHelp", new MenuLayout ("About")));
//		}
		
		[Test]
		public void ParentMenu () 
		{
			Console.WriteLine ("<Test id=\"ParentMenu\">");
			
			BasicWidgetType type = BasicWidgetType.ParentMenu;
			Atk.Object accessible = null;
			
			string menuName = "File!";
			string[] names = new string[] { menuName, "New", "Quit" };
			accessible = GetAccessible (type, names, true);
			
			Assert.AreEqual (menuName, accessible.Name, "name of the menu is the same as its label");

			PropertyRole (type, accessible);

			States (accessible,
			  Atk.StateType.Enabled,
			  Atk.StateType.Selectable, 
			  Atk.StateType.Sensitive,
			  Atk.StateType.Showing, 
			  Atk.StateType.Visible);
			
			Assert.AreEqual (names.Length - 1, accessible.NAccessibleChildren, "number of children");
			
			for (int i = 0; i < accessible.NAccessibleChildren; i++){
				Atk.Object menuChild = accessible.RefAccessibleChild (i);
				Assert.IsNotNull (menuChild, "menu child#0 should not be null");
				Assert.IsTrue (
				  ((menuChild.Role == Atk.Role.Menu) ||
				   (menuChild.Role == Atk.Role.MenuItem) ||
				   (menuChild.Role == Atk.Role.TearOffMenuItem) ||
				   (menuChild.Role == Atk.Role.Separator)), "valid roles for a child of a parentMenu");
				
				Assert.IsTrue (menuChild.NAccessibleChildren > 0 || (menuChild.Role != Atk.Role.Menu),
				   "only grandchildren allowed if parent is menu");

				Assert.AreEqual (menuChild.Name, names [i + 1], "name of the menu is the same as its label");
			}
			
			Atk.Component atkComponent = CastToAtkInterface <Atk.Component> (accessible);
			InterfaceComponent (type, atkComponent);

			Atk.Action atkAction = CastToAtkInterface <Atk.Action> (accessible);
			InterfaceAction (type, atkAction, accessible);

			names [0] = simpleTestText;
			accessible = GetAccessible (type, names, true);
			InterfaceText (type, true, accessible);
			
			//FIXME:
			//Atk.Selection atkSelection = CastToAtkInterface <Atk.Selection> (accessible);
			//InterfaceSelection (atkSelection, names, accessible, type);
			
			Console.WriteLine ("</Test>");
		}

		[Test]
		public void ComboBoxDropDownEntry ()
		{
			Console.WriteLine ("<Test id=\"ComboBoxDropDownEntry\">");
			
			BasicWidgetType type = BasicWidgetType.ComboBoxDropDownEntry;
			Atk.Object accessible;
			
			string[] names = new string[] { "First item", "Second Item", "Last Item" };
			accessible = GetAccessible (type, names, true);
			
			StatesComboBox (accessible);

			Assert.AreEqual (2, accessible.NAccessibleChildren, "numChildren; children roles:" + childrenRoles (accessible));
			Atk.Object menuChild = accessible.RefAccessibleChild (0);
			CheckComboBoxMenuChild (menuChild, names);

			Atk.Object entryChild = accessible.RefAccessibleChild (1);
			Assert.IsNotNull (entryChild, "ComboBox child#1 should not be null");
			Assert.AreEqual (entryChild.Role, Atk.Role.Text, "Role of 2nd child");
			Assert.IsNull (entryChild.Name, "textbox .Name should be null");

			Console.WriteLine ("</Test>");
		}
		
		//it's safer to put this test the last, apparently Atk makes it unresponsive after dealing with
		//the widget, so we kill all with the method marked as [TestFixtureTearDown]
		[Test]
		public void ComboBoxDropDownList ()
		{
			Console.WriteLine ("<Test id=\"ComboBoxDropDownList\">");
			
			BasicWidgetType type = BasicWidgetType.ComboBoxDropDownList;
			Atk.Object accessible;
			
			string[] names = new string [] { "First item", "Second Item", "Last Item" };
			accessible = GetAccessible (type, names, true);
			
			StatesComboBox (accessible);

			Assert.AreEqual (1, accessible.NAccessibleChildren, "numChildren; children roles:" + childrenRoles (accessible));
			
			Atk.Component atkComponent = CastToAtkInterface <Atk.Component> (accessible);
			InterfaceComponent (type, atkComponent);
			
			PropertyRole (type, accessible);
			
			Atk.Action atkAction = CastToAtkInterface <Atk.Action> (accessible);
			InterfaceAction (type, atkAction, accessible);
			
			Atk.Object menuChild = accessible.RefAccessibleChild (0);
			CheckComboBoxMenuChild (menuChild, names);

			//FIXME:
			//Atk.Selection atkSelection = CastToAtkInterface <Atk.Selection> (accessible);
			//InterfaceSelection (atkSelection, names, accessible, type);

			Console.WriteLine ("</Test>");
		}
		
		[Test]
		public void TabControl () { RunInGuiThread (RealTabControl); }
		public void RealTabControl ()
		{
			Console.WriteLine ("<Test id=\"TabControl\">");
			
			BasicWidgetType type = BasicWidgetType.TabControl;
			Atk.Object accessible = null;
			string [] names = new string [] { "Page1", "Page2" };

			RunInGuiThread (delegate () {
				accessible = GetAccessible (type, names, true);
			});
			
			Atk.Component atkComponent = CastToAtkInterface <Atk.Component> (accessible);
			InterfaceComponent (type, atkComponent);
			
			PropertyRole (type, accessible);

			Atk.Selection atkSelection = CastToAtkInterface <Atk.Selection> (accessible);
			InterfaceSelection (atkSelection, names, accessible, type);
			
			Assert.AreEqual (names.Length, accessible.NAccessibleChildren, "TabControl numChildren");
			BasicWidgetType childType = BasicWidgetType.TabPage;
			Atk.Object child1 = accessible.RefAccessibleChild (0);
			PropertyRole (childType, child1);
			Atk.Text atkText = null;
			RunInGuiThread (delegate () {
				atkText = CastToAtkInterface<Atk.Text> (child1);
			});
			Assert.AreEqual (5, atkText.CharacterCount, "CharacterCount");
			Assert.AreEqual ("page1", atkText.GetText (0, 5), "GetText #1");
			Assert.AreEqual ("page1", atkText.GetText (0, -1), "GetText #2");
		}

		[Test]
		public void PictureBox ()
		{
			Console.WriteLine ("<Test id=\"PictureBox\">");
			
			BasicWidgetType type = BasicWidgetType.PictureBox;
			Atk.Object accessible;

			string name = "test";
			accessible = GetAccessibleThatEmbedsAnImage (type, name, true);
			
			States (accessible,
			  Atk.StateType.Enabled,
			  Atk.StateType.Sensitive,
			  Atk.StateType.Showing,
			  Atk.StateType.Visible);
			
			Atk.Component atkComponent = CastToAtkInterface <Atk.Component> (accessible);
			InterfaceComponent (type, atkComponent);

			PropertyRole (type, accessible);
			
			Assert.AreEqual (0, accessible.NAccessibleChildren, "Button numChildren");

			Parent (type, accessible);

			Atk.Image atkWithoutImage, atkWithImage;
			atkWithImage = CastToAtkInterface <Atk.Image> (accessible);
			atkComponent = CastToAtkInterface<Atk.Component> (accessible);
			accessible = GetAccessibleThatEmbedsAnImage (type, name, false);
			atkWithoutImage = CastToAtkInterface <Atk.Image> (accessible);
			InterfaceImage (type, atkWithImage, atkComponent, atkWithoutImage);

			Console.WriteLine ("</Test>");
		}

		[Test]
		public void Window () { RunInGuiThread (RealWindow); }
		public void RealWindow ()
		{
			Console.WriteLine ("<Test id=\"Window\">");
			
			BasicWidgetType type = BasicWidgetType.Window;
			Atk.Object accessible;
			
			string name = "test";
			accessible = GetAccessible (type, name, true);
			
			Atk.Component atkComponent = CastToAtkInterface <Atk.Component> (accessible);

			InterfaceComponent (type, atkComponent);
			
			PropertyRole (type, accessible);
			Parent (type, accessible);

			Atk.StateSet stateSet = accessible.RefStateSet ();
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Enabled), "Window Enabled");
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Sensitive), "Window Sensitive");
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Resizable), "Window Resizable");
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Showing), "Window Showing");
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Visible), "Window Visible");

			Console.WriteLine ("</Test>");
		}
		

	}
}
