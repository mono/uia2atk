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

using NUnit.Framework;

namespace UiaAtkBridgeTest
{

	public abstract class AtkTests : AtkTester
	{
		
		[Test]
		public void Label ()
		{
			BasicWidgetType type = BasicWidgetType.Label;
			Atk.Object accessible = InterfaceText (type);

			PropertyRole (type, accessible);
			
			//a label always contains this state, not because it's multi_line, but because it can be multi_line
			Assert.IsTrue (accessible.RefStateSet ().ContainsState (Atk.StateType.MultiLine), "RefStateSet().Contains(MultiLine)");
			
			Assert.AreEqual (0, accessible.NAccessibleChildren, "Label numChildren");
			
			//TODO: check parent (it seems it only works for real objects)
			//Assert.IsNotNull (accessible.Parent, "Label parent");
		}
		
		[Test]
		public void Button ()
		{
			BasicWidgetType type = BasicWidgetType.NormalButton;
			Atk.Object accessible;
			
			InterfaceText (type);
			
			string name = "test";
			
			Atk.Component atkComponent = (Atk.Component)
				GetAtkObjectThatImplementsInterface <Atk.Component> (type, name, out accessible, true);
			InterfaceComponent (type, atkComponent);
			
			Atk.StateSet stateSet = accessible.RefStateSet();
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Enabled), "Button Enabled");
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Focusable), "Button Focusable");
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Sensitive), "Button Sensitive");
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Showing), "Button Showing");
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Visible), "Button Visible");
			name = "test";
			Atk.Action atkAction = (Atk.Action)
				GetAtkObjectThatImplementsInterface <Atk.Action> (type, name, out accessible, true);
			InterfaceAction (type, atkAction, accessible);
			
			PropertyRole (type, accessible);
			
			Assert.AreEqual (0, accessible.NAccessibleChildren, "Button numChildren");

			Parent (type, accessible);
		}

		[Test]
		public void CheckBox ()
		{
			BasicWidgetType type = BasicWidgetType.CheckBox;
			Atk.Object accessible;
			
			InterfaceText (type);
			
			string name = "test";
			Atk.Component atkComponent = (Atk.Component)
				GetAtkObjectThatImplementsInterface <Atk.Component> (type, name, out accessible, true);
			InterfaceComponent (type, atkComponent);
			
			name = "test";
			Atk.Action atkAction = (Atk.Action)
				GetAtkObjectThatImplementsInterface <Atk.Action> (type, name, out accessible, true);
			
			InterfaceAction (type, atkAction, accessible);
			
			PropertyRole (type, accessible);
			
			Assert.AreEqual (0, accessible.NAccessibleChildren, "CheckBox numChildren");
			Parent (type, accessible);
		}
		
		[Test]
		public void RadioButtons ()
		{
			BasicWidgetType type = BasicWidgetType.RadioButton;
			Atk.Object accessible, accessible2, accessible3;
			
			string name = "test 01";
			Atk.Action atkAction = (Atk.Action)
				GetAtkObjectThatImplementsInterface <Atk.Action> (type, name, out accessible, true);
			InterfaceAction (type, atkAction, accessible);
			
			name = "test 02";
			Atk.Action atkAction2 = (Atk.Action)
				GetAtkObjectThatImplementsInterface <Atk.Action> (type, name, out accessible2, true);
			//the third radio button is disconnected from the previous ones
			name = "test 03";
			Atk.Action atkAction3 = (Atk.Action)
				GetAtkObjectThatImplementsInterface <Atk.Action> (type, name, out accessible3, true);
			
			InterfaceActionFor3RadioButtons (atkAction, accessible,
			                                 atkAction2, accessible2,
			                                 atkAction3, accessible3);
			
			Parent (type, accessible);
			Parent (type, accessible2);
			Parent (type, accessible3);

			name = "test 04";
			Atk.Component atkComponent = (Atk.Component)
				GetAtkObjectThatImplementsInterface <Atk.Component> (type, name, out accessible, true);
			InterfaceComponent (type, atkComponent);
			
			PropertyRole (type, accessible);
			
			Assert.AreEqual (0, accessible.NAccessibleChildren, "RadioButton numChildren");

			Parent (type, accessible);
		}
		
		[Test]
		public void StatusBar () { RunInGuiThread (RealStatusBar); }
		public void RealStatusBar()
		{
			BasicWidgetType type = BasicWidgetType.StatusBar;
			Atk.Object accessible = InterfaceTextSingleLine (type);

			PropertyRole (type, accessible);

			Assert.AreEqual (ValidNChildrenForASimpleStatusBar, accessible.NAccessibleChildren, "StatusBar numChildren");

			string name = "test";
			Atk.Component atkComponent = (Atk.Component)
				GetAtkObjectThatImplementsInterface <Atk.Component> (type, name, out accessible, true);
			InterfaceComponent (type, atkComponent);
			int x, y, width, height;
			atkComponent.GetExtents (out x, out y, out width, out height, Atk.CoordType.Screen);
			Assert.IsTrue (width > 0 && height > 0);

			Parent (type, accessible);
		}
		
		[Test]
		public void HScrollBar ()
		{
			BasicWidgetType type = BasicWidgetType.HScrollBar;
			Atk.Object accessible;
			string name = "test";

			Atk.Value atkValue = (Atk.Value)
				GetAtkObjectThatImplementsInterface <Atk.Value> (type, name, out accessible, true);
			
			InterfaceValue (type, atkValue);

			PropertyRole (type, accessible);

			Atk.StateSet stateSet = accessible.RefStateSet();
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Horizontal), "HScrollBar state");

			Assert.AreEqual (ValidNChildrenForAScrollBar, accessible.NAccessibleChildren, "HScrollBar numChildren");

			Atk.Component atkComponent = (Atk.Component)
				GetAtkObjectThatImplementsInterface <Atk.Component> (type, name, out accessible, true);
			InterfaceComponent (type, atkComponent);
		}
		
		[Test]
		public void VScrollBar ()
		{
			BasicWidgetType type = BasicWidgetType.VScrollBar;
			Atk.Object accessible;
			string name = "test";

			Atk.Value atkValue = (Atk.Value)
				GetAtkObjectThatImplementsInterface <Atk.Value> (type, name, out accessible, true);
			
			InterfaceValue (type, atkValue);

			PropertyRole (type, accessible);

			Atk.StateSet stateSet = accessible.RefStateSet();
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Vertical), "VScrollBar state");

			Assert.AreEqual (ValidNChildrenForAScrollBar, accessible.NAccessibleChildren, "VScrollBar numChildren");

			Atk.Component atkComponent = (Atk.Component)
				GetAtkObjectThatImplementsInterface <Atk.Component> (type, name, out accessible, true);
			InterfaceComponent (type, atkComponent);
		}
		
 		[Test]
		public void ProgressBar ()
		{
			BasicWidgetType type = BasicWidgetType.ProgressBar;
			Atk.Object accessible;
			string name = "test";

			Atk.Value atkValue = (Atk.Value)
				GetAtkObjectThatImplementsInterface <Atk.Value> (type, name, out accessible, true);
			InterfaceValue (type, atkValue);

			PropertyRole (type, accessible);

			Assert.AreEqual (0, accessible.NAccessibleChildren, "ProgressBar numChildren");

			Atk.Component atkComponent = (Atk.Component)
				GetAtkObjectThatImplementsInterface <Atk.Component> (type, name, out accessible, true);
			InterfaceComponent (type, atkComponent);

			Parent (type, accessible);
		}
		
 		[Test]
		public void  Spinner ()
		{
			BasicWidgetType type = BasicWidgetType.Spinner;
			Atk.Object accessible;
			string name = "test";

			Atk.Value atkValue = (Atk.Value)
				GetAtkObjectThatImplementsInterface <Atk.Value> (type, name, out accessible, true);
			Atk.Text atkText = (Atk.Text)
				GetAtkObjectThatImplementsInterface <Atk.Text> (type, name, out accessible, true);
			InterfaceValue (type, atkValue, atkText);

			InterfaceValue (type, atkValue);

			PropertyRole (type, accessible);

			Assert.AreEqual (0, accessible.NAccessibleChildren, "Spinner numChildren");

			Atk.Component atkComponent = (Atk.Component)
				GetAtkObjectThatImplementsInterface <Atk.Component> (type, name, out accessible, true);
			InterfaceComponent (type, atkComponent);
		}
		
		[Test]
		public void TextBoxEntry ()
		{
			BasicWidgetType type = BasicWidgetType.TextBoxEntry;
			Atk.Object accessible = InterfaceText (type);
			
			string name = "Edit test#1";
			Atk.Action atkAction = (Atk.Action)
				GetAtkObjectThatImplementsInterface <Atk.Action> (type, name, out accessible, true);
			InterfaceAction (type, atkAction, accessible);
			
			name = "Edit test#2";
			Atk.Component atkComponent = (Atk.Component)
				GetAtkObjectThatImplementsInterface <Atk.Component> (type, name, out accessible, true);
			InterfaceComponent (type, atkComponent);
			
			PropertyRole (type, accessible);
			
			Assert.AreEqual (0, accessible.NAccessibleChildren, "TextBoxEntry numChildren");
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
			BasicWidgetType type = BasicWidgetType.ParentMenu;
			Atk.Object accessible = null;
			
			string menuName = "File!";
			string[] names = new string[] { menuName, "New", "Quit" };
			Atk.Component atkComponent = (Atk.Component)
				GetAtkObjectThatImplementsInterface <Atk.Component> (type, names, out accessible, true);
			
			Assert.AreEqual (menuName, accessible.Name, "name of the menu is the same as its label");
			
			InterfaceComponent (type, atkComponent);
			
			Atk.Selection atkSelection = (Atk.Selection)
				GetAtkObjectThatImplementsInterface <Atk.Selection> (type, names, out accessible, true);

			InterfaceSelection (atkSelection, names, accessible, type);
			
			//TODO: test text interface
			
			PropertyRole (type, accessible);
			
			Atk.Action atkAction = (Atk.Action)
				GetAtkObjectThatImplementsInterface <Atk.Action> (type, names, out accessible, true);
			
			InterfaceAction (type, atkAction, accessible);
			
			Assert.IsTrue (accessible.NAccessibleChildren > 0, "number of children in menu");
			
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
			}
		}
		
		//it's safer to put this test the last, apparently Atk makes it unresponsive after dealing with
		//the widget, so we kill all with the method marked as [TestFixtureTearDown]
		[Test]
		public void ComboBox ()
		{
			BasicWidgetType type = BasicWidgetType.ComboBox;
			Atk.Object accessible;
			
			string[] names = new string[] { "First item", "Second Item", "Last Item" };
			Atk.Component atkComponent = (Atk.Component)
				GetAtkObjectThatImplementsInterface <Atk.Component> (type, names, out accessible, true);

			InterfaceComponent (type, atkComponent);
			
			PropertyRole (type, accessible);
			
			Atk.Action atkAction = (Atk.Action)
				GetAtkObjectThatImplementsInterface <Atk.Action> (type, names, out accessible, true);
			InterfaceAction (type, atkAction, accessible);
			
			Atk.Selection atkSelection = (Atk.Selection)
				GetAtkObjectThatImplementsInterface <Atk.Selection> (type, names, out accessible, true);
			InterfaceSelection (atkSelection, names, accessible, type);
			
			Assert.AreEqual (1, accessible.NAccessibleChildren, "ComboBox#RO numChildren");
			//FIXME: enable this when we test the editable combobox:
			//Assert.AreEqual (2, accessible.NAccessibleChildren, "ComboBox#editable numChildren");
			
			Atk.Object menuChild = accessible.RefAccessibleChild (0);
			Assert.IsNotNull (menuChild, "ComboBox child#0 should not be null");
			Assert.AreEqual (menuChild.Name, null, "the ComboBox menu should not have a name");
			Assert.AreEqual (menuChild.Role, Atk.Role.Menu, "ComboBox child#0 should be a menu");
			
			Assert.AreEqual (names.Length, menuChild.NAccessibleChildren, "ComboBox menu numChildren");
			Atk.Object menuItemChild = menuChild.RefAccessibleChild (0);
			Assert.IsNotNull (menuItemChild, "ComboBox child#0 child#0 should not be null");
			Assert.AreEqual (menuItemChild.Role, Atk.Role.MenuItem, "ComboBox child#0 child#0 should be a menuItem");
			Assert.AreEqual (menuItemChild.Name, names[0], "ComboBox menuitem names should be the same as the items");
			
			Assert.AreEqual (0, menuItemChild.NAccessibleChildren, "ComboBox menuItem numChildren");
		}
		
		[Test]
		public void Window () { RunInGuiThread (RealWindow); }
		public void RealWindow ()
		{
			BasicWidgetType type = BasicWidgetType.Window;
			Atk.Object accessible;
			
			string name = "test";
			Atk.Component atkComponent = (Atk.Component)
				GetAtkObjectThatImplementsInterface <Atk.Component> (type, name, out accessible, true);
			InterfaceComponent (type, atkComponent);
			
			PropertyRole (type, accessible);
			Parent (type, accessible);

			Atk.StateSet stateSet = accessible.RefStateSet();
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Enabled), "Window Enabled");
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Sensitive), "Window Sensitive");
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Resizable), "Window Resizable");
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Showing), "Window Showing");
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Visible), "Window Visible");
		}
		

	}
}
