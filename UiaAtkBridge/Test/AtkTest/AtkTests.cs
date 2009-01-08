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
			Label (BasicWidgetType.Label);
		}
		
		protected void Label (BasicWidgetType type)
		{
			Atk.Object accessible = InterfaceText (type,
				(type == BasicWidgetType.ToolStripLabel));

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
		}
		
		[Test]
		public void Button ()
		{
			BasicWidgetType type = BasicWidgetType.NormalButton;
			Atk.Object accessible;

			string name = "test";
			accessible = GetAccessible (type, name);

			States (accessible,
			  Atk.StateType.Enabled,
			  Atk.StateType.Focusable,
			  Atk.StateType.Selectable,
			  Atk.StateType.Sensitive,
			  Atk.StateType.Showing,
			  Atk.StateType.Visible);

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
			accessible = GetAccessible (type, name, true);
			atkWithOutImage = CastToAtkInterface <Atk.Image> (accessible);
			accessible = GetAccessibleThatEmbedsAnImage (type, name, true);
			atkWithImage = CastToAtkInterface <Atk.Image> (accessible);
			atkComponent = CastToAtkInterface<Atk.Component> (accessible);
			InterfaceImage (type, atkWithImage, atkComponent, atkWithOutImage);

			//Key Binding tests
			name = "t_est";
			accessible = GetAccessible (type, name, true);
			Assert.IsNull (atkAction.GetKeybinding (-1), "GetKeybinding (-1))");
			Assert.IsNull (atkAction.GetKeybinding (1), "GetKeybinding (1))");
			string keyBinding = atkAction.GetKeybinding (0);
			Assert.IsNotNull (keyBinding, "GetKeybinding (0))");
			Assert.AreEqual (keyBinding, "<Alt>e", "<Alt>e");
		}

		[Test]
		public void Checkbox ()
		{
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

			//test with an image
			Atk.Image atkWithOutImage, atkWithImage;
			atkWithOutImage = CastToAtkInterface <Atk.Image> (accessible);
			accessible = GetAccessibleThatEmbedsAnImage (type, name, true);
			atkWithImage = CastToAtkInterface <Atk.Image> (accessible);
			atkComponent = CastToAtkInterface<Atk.Component> (accessible);
			InterfaceImage (type, atkWithImage, atkComponent, atkWithOutImage);

			//Key Binding tests
			name = "te_st";
			accessible = GetAccessible (type, name, true);
			Assert.IsNull (atkAction.GetKeybinding (-1), "GetKeybinding (-1))");
			Assert.IsNull (atkAction.GetKeybinding (1), "GetKeybinding (1))");
			string keyBinding = atkAction.GetKeybinding (0);
			Assert.IsNotNull (keyBinding, "GetKeybinding (0))");
			Assert.AreEqual (keyBinding, "<Alt>s", "<Alt>s");
		}
		
		[Test]
		public void RadioButtons ()
		{
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

			//Key Binding tests
			name = "_test 01";
			accessible = GetAccessible (type, name, true);
			Assert.IsNull (atkAction.GetKeybinding (-1), "GetKeybinding (-1))");
			Assert.IsNull (atkAction.GetKeybinding (1), "GetKeybinding (1))");
			string keyBinding = atkAction.GetKeybinding (0);
			Assert.IsNotNull (keyBinding, "GetKeybinding (0))");
			Assert.AreEqual (keyBinding, "<Alt>t", "<Alt>t");
		}
		
		[Test]
		public void StatusBar () { RunInGuiThread (RealStatusBar); }
		public void RealStatusBar()
		{
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
		}
		
		[Test]
		public void HScrollBar ()
		{
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

			//Simple Set/Get
			GLib.Value glibValue = GLib.Value.Empty;
			atkValue.GetMaximumValue (ref glibValue);
			double maxValue = (double) glibValue.Val;

			glibValue = new GLib.Value (maxValue - 1);
			if (atkValue.SetCurrentValue (glibValue) == true) {
				atkValue.GetCurrentValue (ref glibValue);				
				Assert.AreEqual (maxValue - 1,
				                 (double) glibValue.Val, "Set/Get values failed.");
			}

			//SHOULD NOT THROW ANY EXCEPTION when Maximum + 1
			atkValue.GetCurrentValue (ref glibValue);
			double currentValue = (double) glibValue.Val;
			glibValue = new GLib.Value (maxValue + 10);
			if (atkValue.SetCurrentValue (glibValue) == true) {
				atkValue.GetCurrentValue (ref glibValue);
				Assert.AreEqual (currentValue,
				                 (double) glibValue.Val, "Set/Get values failed. (Maximum + 1)");
			}

			//SHOULD NOT THROW ANY EXCEPTION when Minimum - 1
			atkValue.GetCurrentValue (ref glibValue);
			currentValue = (double) glibValue.Val;
			atkValue.GetMinimumValue (ref glibValue);
			double minimumValue = (double) glibValue.Val;
			glibValue = new GLib.Value (minimumValue - 10);
			if (atkValue.SetCurrentValue (glibValue) == true) {
				atkValue.GetCurrentValue (ref glibValue);
				Assert.AreEqual (minimumValue,
				                 (double) glibValue.Val, "Set/Get values failed. (Maximum + 1)");
			}
		}
		
		[Test]
		public void VScrollBar ()
		{
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

			//Simple Set/Get
			GLib.Value glibValue = GLib.Value.Empty;
			atkValue.GetMaximumValue (ref glibValue);
			double maxValue = (double) glibValue.Val;

			glibValue = new GLib.Value (maxValue - 1);
			if (atkValue.SetCurrentValue (glibValue) == true) {
				atkValue.GetCurrentValue (ref glibValue);				
				Assert.AreEqual (maxValue - 1,
				                 (double) glibValue.Val, "Set/Get values failed.");
			}

			//SHOULD NOT THROW ANY EXCEPTION when Maximum + 1
			atkValue.GetCurrentValue (ref glibValue);
			double currentValue = (double) glibValue.Val;
			glibValue = new GLib.Value (maxValue + 10);
			if (atkValue.SetCurrentValue (glibValue) == true) {
				atkValue.GetCurrentValue (ref glibValue);
				Assert.AreEqual (currentValue,
				                 (double) glibValue.Val, "Set/Get values failed. (Maximum + 1)");
			}

			//SHOULD NOT THROW ANY EXCEPTION when Minimum - 1
			atkValue.GetCurrentValue (ref glibValue);
			currentValue = (double) glibValue.Val;
			atkValue.GetMinimumValue (ref glibValue);
			double minimumValue = (double) glibValue.Val;
			glibValue = new GLib.Value (minimumValue - 10);
			if (atkValue.SetCurrentValue (glibValue) == true) {
				atkValue.GetCurrentValue (ref glibValue);
				Assert.AreEqual (minimumValue,
				                 (double) glibValue.Val, "Set/Get values failed. (Maximum + 1)");
			}
		}
		
 		[Test]
		public void ProgressBar ()
		{
			ProgressBar (BasicWidgetType.ProgressBar);
		}

		protected void ProgressBar (BasicWidgetType type)
		{
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
		}
		
 		[Test]
		public void Spinner ()
		{
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

			InterfaceEditableText (type, accessible);

			Atk.Action atkAction = CastToAtkInterface<Atk.Action> (accessible);
			InterfaceAction (type, atkAction, accessible);

			SetReadOnly (accessible, true);
			Assert.IsFalse (accessible.RefStateSet().ContainsState (Atk.StateType.Editable), "ReadOnly spinner should not be editable");
			EventMonitor.Start ();
			SetReadOnly (accessible, false);
			EventCollection events = EventMonitor.Pause ();
			string evType = "object:state-changed:editable";
			EventCollection evs = events.FindByRole (Atk.Role.SpinButton).FindByType (evType);
			string eventsInXml = String.Format (" events in XML: {0}", Environment.NewLine + events.OriginalGrossXml);
			Assert.AreEqual (1, evs.Count, "bad number of events expected!" + eventsInXml);
			Assert.IsTrue (accessible.RefStateSet().ContainsState (Atk.StateType.Editable), "Non-ReadOnly spinner should be editable");
		}
		
		[Test]
		public void TextBoxEntry ()
		{
			TextBoxEntry (null, true);
		}

		public void TextBoxEntry (object widget, bool expectFocusable)
		{
			BasicWidgetType type = BasicWidgetType.TextBoxEntry;

			Atk.Object accessible = null;
			if (widget == null) {
				accessible = InterfaceText (type, true);
			} else {
				accessible = InterfaceText (type, true, widget);
			}

			States (accessible,
			  Atk.StateType.Editable, 
			  Atk.StateType.Enabled, 
			  (expectFocusable? Atk.StateType.Focusable: Atk.StateType.Enabled),
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
		}

		[Test]
		public void TextBoxView ()
		{
			TextBoxView (null, true);
		}
		
		public void TextBoxView (object widget, bool expectFocusable)
		{
			BasicWidgetType type = BasicWidgetType.TextBoxView;
			
			Atk.Object accessible = null;
			if (widget == null) {
				accessible = InterfaceText (type, false);
			} else {
				accessible = InterfaceText (type, false, widget);
			}

			States (accessible,
			  Atk.StateType.Editable, 
			  Atk.StateType.Enabled, 
			  (expectFocusable? Atk.StateType.Focusable: Atk.StateType.Enabled),
			  Atk.StateType.MultiLine,
			  Atk.StateType.Sensitive,
			  Atk.StateType.Showing,
			  Atk.StateType.Visible);
		}

		[Test]
		public virtual void MaskedTextBoxEntry ()
		{
			BasicWidgetType type = BasicWidgetType.MaskedTextBoxEntry;
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
		}
		
		[Test]
		public void MainMenuBar ()
		{
			BasicWidgetType type = BasicWidgetType.MainMenuBar;
			Atk.Object accessible = null;
			
			List <MenuLayout> menu = new List <MenuLayout> ();
			menu.Add (new MenuLayout ("XFile", new MenuLayout ("New...", new MenuLayout ("Project"), new MenuLayout ("Text")), new MenuLayout ("Quit!")));
			menu.Add (new MenuLayout ("GimmeHelp", new MenuLayout ("About?")));
			
			accessible = GetAccessible (type, menu);

			Assert.IsNull (accessible.Name, "name of the menubar should be null, now it's:" + accessible.Name);
			
			States (accessible,
			  Atk.StateType.Enabled, 
			  Atk.StateType.Sensitive,
			  Atk.StateType.Showing,
			  Atk.StateType.Visible);

			PropertyRole (type, accessible);

			Assert.AreEqual (menu.Count, accessible.NAccessibleChildren, "number of children; children roles:" + childrenRoles (accessible));

			for (int i = 0; i < accessible.NAccessibleChildren; i++) {
				Atk.Object parentMenuChild = accessible.RefAccessibleChild (i);
				Assert.IsNotNull (parentMenuChild, "menubar child#" + i + " should not be null");

				Assert.AreEqual ( //FIXME: check if it's possible to have a MenuItem alone (like a push button)
				  parentMenuChild.Role, Atk.Role.Menu, "menubar children should have Menu role");
				Assert.AreEqual (menu [i].Label, parentMenuChild.Name, "name of the parentmenu is the same as its label");
			}

			Atk.Component atkComponent = CastToAtkInterface <Atk.Component> (accessible);
			InterfaceComponent (type, atkComponent);

			//FIXME:
			//Atk.Selection atkSelection = CastToAtkInterface <Atk.Selection> (accessible);
			//InterfaceSelection (atkSelection, names, accessible, type);
		}
		
		[Test]
		public virtual void ContextMenu () 
		{
			BasicWidgetType type = BasicWidgetType.ContextMenu;
			Atk.Object accessible = null;
			
			MenuLayout [] firstSubmenus = new MenuLayout [] { new MenuLayout ("Schema", new MenuLayout ("XSD"), new MenuLayout ("DTD")), new MenuLayout ("Source") };
			List <MenuLayout> menu = new List <MenuLayout> ();
			menu.Add (new MenuLayout ("Edit", firstSubmenus));
			menu.Add (new MenuLayout ("Close"));

			Assert.AreEqual (1, GetTopLevelRootItem ().NAccessibleChildren, "Windows in my app should be 1, and I got:" + childrenRoles (GetTopLevelRootItem ()));
			accessible = GetAccessible (type, menu);
			Assert.AreEqual (2, GetTopLevelRootItem ().NAccessibleChildren, "Windows in my app should be 1, and I got:" + childrenRoles (GetTopLevelRootItem ()));

			Assert.IsNull (accessible.Name, "name of the menubar should be null, now it's:" + accessible.Name);

			PropertyRole (type, accessible);

			Assert.AreEqual (accessible.Parent.Role, Atk.Role.Window);

			Assert.AreEqual (accessible.Parent.Parent, GetTopLevelRootItem ());
			
			States (accessible,
			  Atk.StateType.Focused,
			  Atk.StateType.Enabled,
			  Atk.StateType.Sensitive,
			  Atk.StateType.Visible,
			  Atk.StateType.Showing);

			Assert.AreEqual (menu.Count, accessible.NAccessibleChildren, "number of children; children roles:" + childrenRoles (accessible));

			for (int i = 0; i < accessible.NAccessibleChildren; i++) {
				Atk.Object menuChild = accessible.RefAccessibleChild (i);
				Assert.IsNotNull (menuChild, "menu child#0 should not be null");
				Assert.IsTrue (
				  ((menuChild.Role == Atk.Role.Menu) ||
				   (menuChild.Role == Atk.Role.MenuItem) ||
				   (menuChild.Role == Atk.Role.TearOffMenuItem) ||
				   (menuChild.Role == Atk.Role.Separator)), "valid roles for a child of a parentMenu");
				
				Assert.IsTrue (menuChild.NAccessibleChildren > 0 || (menuChild.Role != Atk.Role.Menu),
				   "only grandchildren allowed if parent is menu");

				Assert.AreEqual (menuChild.Name, menu [i].Label, "name of the menu is the same as its label");
			}
			
			Atk.Component atkComponent = CastToAtkInterface <Atk.Component> (accessible);
			InterfaceComponent (type, atkComponent);
		}
		
		[Test]
		public void ParentMenu () 
		{
			BasicWidgetType type = BasicWidgetType.ParentMenu;
			Atk.Object accessible = null;
			
			string menuName = "File";
			MenuLayout [] firstSubmenus = new MenuLayout [] { new MenuLayout ("New...", new MenuLayout ("Project"), new MenuLayout ("Text")), new MenuLayout ("Quit!") };

			List <MenuLayout> menu = new List <MenuLayout> ();
			menu.Add (new MenuLayout ("File", firstSubmenus));
			menu.Add (new MenuLayout ("Help", new MenuLayout ("About?")));

			accessible = GetAccessible (type, menu);
			
			Assert.AreEqual (menuName, accessible.Name, "name of the menu is the same as its label");

			PropertyRole (type, accessible);

			States (accessible,
			  Atk.StateType.Enabled,
			  Atk.StateType.Selectable, 
			  Atk.StateType.Sensitive,
			  Atk.StateType.Visible,
			  Atk.StateType.Showing);
			
			Assert.AreEqual (firstSubmenus.Length, accessible.NAccessibleChildren, "number of children; children roles:" + childrenRoles (accessible));
			
			for (int i = 0; i < accessible.NAccessibleChildren; i++) {
				Atk.Object menuChild = accessible.RefAccessibleChild (i);
				Assert.IsNotNull (menuChild, "menu child#0 should not be null");
				Assert.IsTrue (
				  ((menuChild.Role == Atk.Role.Menu) ||
				   (menuChild.Role == Atk.Role.MenuItem) ||
				   (menuChild.Role == Atk.Role.TearOffMenuItem) ||
				   (menuChild.Role == Atk.Role.Separator)), "valid roles for a child of a parentMenu");
				
				Assert.IsTrue (menuChild.NAccessibleChildren > 0 || (menuChild.Role != Atk.Role.Menu),
				   "only grandchildren allowed if parent is menu");

				Assert.AreEqual (menuChild.Name, firstSubmenus [i].Label, "name of the menu is the same as its label");
			}
			
			Atk.Component atkComponent = CastToAtkInterface <Atk.Component> (accessible);
			InterfaceComponent (type, atkComponent);

			List <string> names = new List <string> ();
			names.Add (menuName);
			foreach (MenuLayout submenu in firstSubmenus)
				names.Add (submenu.Label);
			Atk.Selection atkSelection = CastToAtkInterface <Atk.Selection> (accessible);
			InterfaceSelection (atkSelection, names.ToArray (), accessible, type);
			
			Atk.Action atkAction = CastToAtkInterface <Atk.Action> (accessible);
			InterfaceAction (type, atkAction, accessible);

			menu = new List <MenuLayout> (new MenuLayout [] { new MenuLayout (simpleTestText, firstSubmenus), new MenuLayout ("Help") });
			accessible = GetAccessible (type, menu);
			InterfaceText (type, true, accessible);
		}

		[Test]
		public void ComboBoxDropDownEntry ()
		{
			ComboBoxDropDownEntry (null);
		}
		
		protected void ComboBoxDropDownEntry (object widget)
		{
			BasicWidgetType type = BasicWidgetType.ComboBoxDropDownEntry;
			Atk.Object accessible;
			
			string [] names = new string[] { "First item", "Second Item", "Last Item" };
			accessible = GetAccessible (type, names, widget);
			
			StatesComboBox (accessible);

			Assert.AreEqual (2, accessible.NAccessibleChildren, "numChildren; children roles:" + childrenRoles (accessible));
			
			Atk.Object menuChild = accessible.RefAccessibleChild (0);
			CheckComboBoxMenuChild (menuChild, names, false);

			Atk.Object entryChild = accessible.RefAccessibleChild (1);
			Assert.IsNotNull (entryChild, "ComboBox child#1 should not be null");
			Assert.AreEqual (entryChild.Role, Atk.Role.Text, "Role of 2nd child");
			Assert.IsNull (entryChild.Name, "textbox .Name should be null");

			Atk.Component atkComponent = CastToAtkInterface <Atk.Component> (accessible);
			InterfaceComponent (type, atkComponent);

			PropertyRole (type, accessible);

			//we get the accessible again because the MWF toolkit doesn't support clearing the selection
			names = new string [] { "1st item", "Second Item", "Third Item", "Last Item" };
			accessible = GetAccessible (type, names, widget);
			Assert.AreEqual (names.Length, accessible.RefAccessibleChild (0).NAccessibleChildren, "number of children changed");

			Atk.Object secondComboBoxItem = accessible.RefAccessibleChild (0).RefAccessibleChild (1);

			Atk.Selection atkSelection = CastToAtkInterface <Atk.Selection> (accessible);
			InterfaceSelection (atkSelection, names, accessible, type);
			
			Atk.Action atkAction = CastToAtkInterface <Atk.Action> (accessible);
			InterfaceAction (type, atkAction, accessible, names);
			
			//check the Action impl of a comboboxitem (menuitem role)
			atkAction = CastToAtkInterface <Atk.Action> (secondComboBoxItem);
			InterfaceAction (BasicWidgetType.ComboBoxItem, atkAction, secondComboBoxItem);
		}
		
		//it's safer to put this test the last, apparently Atk makes it unresponsive after dealing with
		//the widget, so we kill all with the method marked as [TestFixtureTearDown]
		[Test]
		public void ComboBoxDropDownList ()
		{
			ComboBoxDropDownList (null);
		}

		public void ComboBoxDropDownList (object widget)
		{
			BasicWidgetType type = BasicWidgetType.ComboBoxDropDownList;
			Atk.Object accessible;
			
			string[] names = new string [] { "First item", "Second Item", "Last Item" };
			accessible = GetAccessible (type, names, widget);
			
			StatesComboBox (accessible);

			Assert.AreEqual (1, accessible.NAccessibleChildren, "numChildren; children roles:" + childrenRoles (accessible));
			
			Atk.Component atkComponent = CastToAtkInterface <Atk.Component> (accessible);
			InterfaceComponent (type, atkComponent);
			
			PropertyRole (type, accessible);

			Atk.Object menuChild = accessible.RefAccessibleChild (0);
			CheckComboBoxMenuChild (menuChild, names, false);

			//we get the accessible again because the MWF toolkit doesn't support clearing the selection
			names = new string [] { "1st item", "Second Item", "Third Item", "Last Item" };
			accessible = GetAccessible (type, names, widget);
			Assert.AreEqual (names.Length, accessible.RefAccessibleChild (0).NAccessibleChildren, "number of children changed");
			
			Atk.Object secondComboBoxItem = accessible.RefAccessibleChild (0).RefAccessibleChild (1);
			
			Atk.Selection atkSelection = CastToAtkInterface <Atk.Selection> (accessible);
			InterfaceSelection (atkSelection, names, accessible, type);

			Atk.Action atkAction = CastToAtkInterface <Atk.Action> (accessible);
			InterfaceAction (type, atkAction, accessible, names);

			//check the Action impl of a comboboxitem (menuitem role)
			atkAction = CastToAtkInterface <Atk.Action> (secondComboBoxItem);
			InterfaceAction (BasicWidgetType.ComboBoxItem, atkAction, secondComboBoxItem);
		}
		
		[Test]
		public void TabControl () { RunInGuiThread (RealTabControl); }
		public void RealTabControl ()
		{
			BasicWidgetType type = BasicWidgetType.TabControl;
			Atk.Object accessible = null;
			string [] names = new string [] { "page1", "page2" };

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
		}

		//[Test]
		public void ListView ()
		{
			BasicWidgetType type = BasicWidgetType.ListView;
			Atk.Object accessible;

			string name = "<table><th><td>Title</td><td>Author</td><td>year</td></th>"+
				"<tr><td>Non-C#</td>"+
				"<tr><td>Programming Windows</td><td>Petzold, Charles</td><td>1998</td></tr>"+
				"<tr><td>Code: The Hidden Language of Computer Hardware and Software</td><td>Petzold, Charles</td><td>2000</td></tr>"+
				"<tr><td>Coding Techniques for Microsoft Visual Basic .NET</td><td>Connell, John</td><td>2001</td></tr>"+
				"</tr><tr><td>C#</td>"+
				"<tr><td>Programming Windows with C#</td><td>Petzold, Charles</td><td>2001</td></tr>"+
				"<tr><td>C# for Java Developers</td><td>Jones, Allen &amp; Freeman, Adam</td><td>2002</td></tr>"+
				"</tr></table>";
			accessible = GetAccessible (type, name, true);
			
			Atk.Component atkComponent = CastToAtkInterface <Atk.Component> (accessible);
			InterfaceComponent (type, atkComponent);
			
			States (accessible,
			  Atk.StateType.Enabled,
			  Atk.StateType.Focusable,
			  Atk.StateType.ManagesDescendants,
			  Atk.StateType.Sensitive,
			  Atk.StateType.Showing,
			  Atk.StateType.Visible);

			PropertyRole (type, accessible);

			Atk.Table atkTable = CastToAtkInterface<Atk.Table> (accessible);
			Assert.AreEqual (ValidNChildrenForAListView, accessible.NAccessibleChildren, "ListView numChildren");
			Atk.Object header = FindObjectByRole (accessible, Atk.Role.TableColumnHeader);
			Assert.IsNotNull (header, "Header not null");
			States (header,
				Atk.StateType.Enabled,
				Atk.StateType.Sensitive,
				Atk.StateType.Showing,
				Atk.StateType.Visible);

			Atk.Object child1 = FindObjectByName (accessible, "Programming Windows with C#");
			int child1Index = child1.IndexInParent;
			Assert.IsTrue (child1Index >= 0, "Child 1 index > 0");
			InterfaceText (child1, "Programming Windows with C#");
			Assert.IsNotNull (child1, "FindObjectByName #1");
			Assert.AreEqual (Atk.Role.TableCell, child1.Role, "Child 1 role");
			Atk.Object group = FindObjectByName (accessible, "C#");
			int groupIndex = group.IndexInParent;
			Assert.IsTrue (groupIndex >= 0, "Group index > 0");
			Assert.IsFalse (child1Index == groupIndex, "Child should have a different index from its group");

			InterfaceText (group, "C#");

			// For some reason, the next line would cause crashes
			// in later tests.
			//Relation (Atk.RelationType.NodeChildOf, child1, group);

			Assert.AreEqual (3, atkTable.NColumns, "Table NumColumns");
			Assert.AreEqual (1, atkTable.GetRowAtIndex (groupIndex), "GetRowAtIndex");
			Assert.AreEqual (0, atkTable.GetColumnAtIndex (groupIndex), "GetColumnAtIndex");
			Assert.AreEqual (group, atkTable.RefAt (1, 0), "ListView RefAt");
		}

		[Test]
		public void TreeView ()
		{
			BasicWidgetType type = BasicWidgetType.TreeView;
			Atk.Object accessible;

			string name = "<table>"+
				"<tr><td>Group1</td>"+
				"<tr><td>Item1</td></tr>"+
				"<tr><td>Item2</td></tr>"+
				"<tr><td>Item3</td></tr>"+
				"</tr><tr><td>Group2</td>"+
				"<tr><td>Item4</td></tr>"+
				"<tr><td>Item5</td></tr>"+
				"</tr></table>";

			accessible = GetAccessible (type, name, true);
			
			Atk.Component atkComponent = CastToAtkInterface <Atk.Component> (accessible);
			InterfaceComponent (type, atkComponent);
			
			States (accessible,
			  Atk.StateType.Enabled,
			  Atk.StateType.Focusable,
			  Atk.StateType.ManagesDescendants,
			  Atk.StateType.Sensitive,
			  Atk.StateType.Showing,
			  Atk.StateType.Visible);

			PropertyRole (type, accessible);

			Atk.Table atkTable = CastToAtkInterface<Atk.Table> (accessible);
			if (TreeViewHasHeader) {
				Atk.Object header = accessible.RefAccessibleChild (0);
				Assert.AreEqual (Atk.Role.TableColumnHeader, header.Role, "Child 0 role");
			}
			Atk.Object group2 = FindObjectByName (accessible, "Group2");
			Assert.IsNotNull (group2, "FindObjectByName (group2");
			int group2Index = group2.IndexInParent;
			Assert.IsTrue (group2Index >= 0, "Group2 index > 0");
			InterfaceText (group2, "Group2");

			Assert.AreEqual (2, atkTable.NRows, "TreeView NRows before expand");
			Assert.AreEqual (1, atkTable.NColumns, "Table NumColumns");
			Assert.AreEqual (1, atkTable.GetRowAtIndex (group2Index), "GetRowAtIndex");
			Assert.AreEqual (0, atkTable.GetColumnAtIndex (group2Index), "GetColumnAtIndex");
			Assert.AreEqual (group2, atkTable.RefAt (1, 0), "TreeView RefAt");

			States (group2,
				Atk.StateType.Enabled,
				Atk.StateType.Expandable,
				Atk.StateType.Focusable,
				Atk.StateType.Selectable,
				Atk.StateType.Sensitive,
				Atk.StateType.Showing,
				Atk.StateType.SingleLine,
				Atk.StateType.Transient,
				Atk.StateType.Visible);

			EventMonitor.Start ();
			ExpandTreeView (type);

			EventCollection events = EventMonitor.Pause ();
			string evType = "object:state-changed:expanded";
			EventCollection evs = events.FindByRole (Atk.Role.TableCell).FindByType (evType);
			string eventsInXml = String.Format (" events in XML: {0}", Environment.NewLine + events.OriginalGrossXml);
			// I'd expect 2 events, but gail only gives us 1. A bug?
			//Assert.AreEqual (2, evs.Count, "bad number of events expected!" + eventsInXml);
			Assert.IsTrue (evs.Count > 0 && evs.Count < 3, "bad number of events expected!" + eventsInXml);
			Assert.IsTrue (group2.RefStateSet().ContainsState (Atk.StateType.Expanded));

			// Gail has a header, and MWF has a ScrollBar
			Assert.AreEqual (8, accessible.NAccessibleChildren, "TreeView numChildren");

			Assert.AreEqual (7, atkTable.NRows, "TreeView NRows after expand");

			Atk.Object item4 = FindObjectByName (accessible, "Item4");
			Assert.IsNotNull (item4, "FindObjectByName");
			int item4Index = item4.IndexInParent;
			Assert.IsTrue (item4Index >= 0, "Child 1 index > 0");
			InterfaceText (item4, "Item4");
			Assert.AreEqual (Atk.Role.TableCell, item4.Role, "Child 1 role");
			Assert.IsFalse (item4Index == group2Index, "Child should have a different index from its group");
			Assert.AreEqual (5, atkTable.GetRowAtIndex (item4Index), "child row");

			// For some reason, the next line would cause crashes
			// in later tests.
			//Relation (Atk.RelationType.NodeChildOf, item4, group2);

			item4 = null;
			CollapseTreeView (type);
			Assert.AreEqual (2, atkTable.NRows, "TreeView NRows after collapse");
			item4 = FindObjectByName (accessible, "Item4");
			if (item4 != null) {
				item4Index = item4.IndexInParent;
				// Strangely, gail now returns 2 for GetRowAtIndex
				int rowIndex = atkTable.GetRowAtIndex (item4Index);
				Assert.IsTrue (rowIndex == -1 || rowIndex >= atkTable.NRows, "Child row after collapse");
			}
		}

		[Test]
		public void Window () { RunInGuiThread (RealWindow); }
		public void RealWindow ()
		{
			BasicWidgetType type = BasicWidgetType.Window;
			Atk.Object accessible;
			
			string name = "test";
			accessible = GetAccessible (type, name, true);
			
			Atk.Component atkComponent = CastToAtkInterface <Atk.Component> (accessible);

			InterfaceComponent (type, atkComponent);
			
			PropertyRole (type, accessible);
			Parent (type, accessible);

			States (accessible,
			  Atk.StateType.Active, //FIXME: figure out why the ACTIVE state sometimes doesn't appear
			  Atk.StateType.Enabled,
			  Atk.StateType.Sensitive,
			  Atk.StateType.Resizable,
			  Atk.StateType.Showing,
			  Atk.StateType.Visible); 
		}

		[Test]
		public void Pane ()
		{
			BasicWidgetType type = BasicWidgetType.ContainerPanel;
			Atk.Object accessible;
			
			string name = "test";
			accessible = GetAccessible (type, name, true);
			
			Atk.Component atkComponent = CastToAtkInterface <Atk.Component> (accessible);

			InterfaceComponent (type, atkComponent);
			
			PropertyRole (type, accessible);
			Parent (type, accessible);

			States (accessible,
			  Atk.StateType.Enabled,
			(ContainerPanelIsResizable? Atk.StateType.Resizable: Atk.StateType.Enabled),
			  Atk.StateType.Sensitive,
			  Atk.StateType.Showing,
			  Atk.StateType.Visible);
		}
		
	}
}
