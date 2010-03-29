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
using System.Xml;

using System.Collections.Generic;

using NUnit.Framework;

namespace UiaAtkBridgeTest
{
	
	public abstract class AtkTester : TestBase {

		public abstract Atk.Object GetAccessible (BasicWidgetType type);
		
		public abstract Atk.Object GetAccessible (BasicWidgetType type, string name);

		public abstract Atk.Object GetAccessible (
		  BasicWidgetType type, string [] name, object widget);
		
		public abstract Atk.Object GetAccessible (
		  BasicWidgetType type, string [] name, int selected, object widget);

		public abstract Atk.Object GetAccessible (
		  BasicWidgetType type, string name, object widget);
		
		public abstract Atk.Object GetAccessible (
		  BasicWidgetType type, string name, bool real);

		public abstract Atk.Object GetAccessible (BasicWidgetType type, string [] name);
		
		public abstract Atk.Object GetAccessible (
		  BasicWidgetType type, string [] name, bool real);

		public abstract Atk.Object GetAccessible (
		  BasicWidgetType type, List <MenuLayout> menu);
		
		public abstract Atk.Object GetAccessibleThatEmbedsAnImage (
		  BasicWidgetType type, string name, bool real);

		public abstract I CastToAtkInterface <I> (Atk.Object accessible) where I : class;

		public abstract object CastToAtkInterface (Type t, Atk.Object accessible);

		public abstract void DisableWidget (Atk.Object accessible);

		public abstract void EnableWidget (Atk.Object accessible);

		public abstract void SetReadOnly (BasicWidgetType type, Atk.Object accessible, bool readOnly);

		public abstract Atk.Object ActivateAdditionalForm (string name);
		public abstract void RemoveAdditionalForm (Atk.Object obj);

		public abstract Atk.Object GetTopLevelRootItem ();

		public abstract bool IsBGO561414Addressed ();

		public abstract bool IsBGO567991Addressed ();

		public abstract bool IsBGO574674Addressed ();

		public abstract bool IsBGO580460Addressed ();

		public abstract bool IsBGO580452Addressed ();

		public abstract void CloseContextMenu (Atk.Object accessible);

		public abstract bool HasComboBoxSimpleLayout { get; }

		protected void InterfaceComponent (BasicWidgetType type, Atk.Component implementor)
		{
			InterfaceComponent (type, implementor, true);
		}
		
		protected void InterfaceComponent (BasicWidgetType type, Atk.Component implementor, bool showing)
		{
			Assert.AreEqual (1, implementor.Alpha, "Component.Alpha");

			if (type == BasicWidgetType.Window) {
				Assert.AreEqual (Atk.Layer.Window, implementor.Layer, "Component.Layer");
				Assert.AreEqual (-1, implementor.MdiZorder, "Component.MdiZorder");
			} else if (type == BasicWidgetType.ParentMenu ||
			           type == BasicWidgetType.ContextMenu ||
			           type == BasicWidgetType.ChildMenu) {
				Assert.AreEqual (Atk.Layer.Popup, implementor.Layer, "Component.Layer");
			} else if (type == BasicWidgetType.ToolBar) {
				Assert.AreEqual (Atk.Layer.Widget, implementor.Layer, "Component.Layer");
				Assert.AreEqual (int.MinValue, implementor.MdiZorder, "Component.MdiZorder");
				Assert.AreEqual (1, implementor.Alpha, "Component.Alpha");
			} else if (type == BasicWidgetType.ChildMenuSeparator) {
				Assert.AreEqual (Atk.Layer.Popup, implementor.Layer, "Component.Layer");
				Assert.AreEqual (int.MinValue, implementor.MdiZorder, "Component.MdiZorder");
				Assert.AreEqual (1, implementor.Alpha, "Component.Alpha");
			} else {
				Assert.AreEqual (Atk.Layer.Widget, implementor.Layer, "Component.Layer:" + type.ToString ());
				//FIXME: still don't know why this is failing in the GailTester, accerciser is lying me?
				//Assert.AreEqual (0, implementor.MdiZorder, "Component.MdiZorder(notWindow)");
			}

			RunInGuiThread (delegate () {
				int x, y, w, h, x2, y2, w2, h2;
				implementor.GetExtents (out x, out y, out w, out h, Atk.CoordType.Screen);
				
				if (showing) {
					Assert.IsTrue (x > 0, "x > 0");
					Assert.IsTrue (y > 0, "y > 0");
				} else {
					Assert.AreEqual (x, int.MinValue, "x==minValue b/c !showing");
					Assert.AreEqual (y, int.MinValue, "y==minValue b/c !showing");
				}
				Assert.IsTrue (w > 0, "w > 0");
				Assert.IsTrue (h > 0, "h > 0");

				implementor.GetExtents (out x2, out y2, out w2, out h2, Atk.CoordType.Window);
				if (type == BasicWidgetType.Window) {
					// TODO: Why is gail returning x and y < 0?
					Assert.IsTrue (x2 <= 0, "x2 <= 0");
					Assert.IsTrue (y2 <= 0, "y2 <= 0");
				} else {
					if (showing) {
						Assert.IsTrue (x2 >= 0, "x2 > 0");
						Assert.IsTrue (y2 >= 0, "y2 > 0");
					} else {
						Assert.AreEqual (x2, int.MinValue, "x==minValue b/c !showing");
						Assert.AreEqual (y2, int.MinValue, "y==minValue b/c !showing");
					}
				}
				Assert.IsTrue (w2 >= 0, "w2 > 0");
				Assert.IsTrue (h2 >= 0, "h2 > 0");

				Assert.IsTrue (x >= x2, "x >= x2");
				Assert.IsTrue (y >= y2, "y >= y2");
				Assert.IsTrue (w >= w2, "w >= w2");
				Assert.IsTrue (h >= h2, "h >= h2");
			});
		}
		
		protected abstract int ValidNumberOfActionsForAButton { get; }
		protected abstract int ValidNChildrenForAListView { get; }
		protected abstract bool TreeViewHasHeader { get; }
		protected abstract int ValidNChildrenForASimpleStatusBar { get; }
		protected abstract int ValidNChildrenForAScrollBar { get; }
		protected abstract bool AllowsEmptyingSelectionOnComboBoxes { get; }
		protected abstract bool TextBoxCaretInitiallyAtEnd { get; }
		protected abstract bool TextBoxHasScrollBar { get; }
		protected abstract bool SupportsLabeledBy (out string labelName);

		protected void InterfaceActionFor3RadioButtons (Atk.Action actionable1, Atk.Object accessible1,
		                                                Atk.Action actionable2, Atk.Object accessible2,
		                                                Atk.Action actionable3, Atk.Object accessible3)
		{
			RunInGuiThread ( delegate () {
				Assert.IsTrue (actionable1.DoAction (0), "IAF3RB::DoAction#1");
			});
			System.Threading.Thread.Sleep (200);
			RunInGuiThread ( delegate () {
				Assert.IsTrue (actionable3.DoAction (0), "IAF3RB::DoAction#1");
			});
			System.Threading.Thread.Sleep (200);
			RunInGuiThread ( delegate () {
				Assert.IsTrue (accessible1.RefStateSet ().ContainsState (Atk.StateType.Checked), "IAF3RB::Checked #1");
				Assert.IsFalse (accessible2.RefStateSet ().ContainsState (Atk.StateType.Checked), "IAF3RB::Checked #2");
				Assert.IsTrue (accessible3.RefStateSet ().ContainsState (Atk.StateType.Checked), "IAF3RB::Checked #3");
			});
			
			EventMonitor.Start ();

			RunInGuiThread ( delegate () {
				Assert.IsTrue (actionable2.DoAction (0), "IAF3RB::DoAction#1");
			});
			System.Threading.Thread.Sleep (200);
				
			// FIXME: should we detect "object:state-changed:focused" events here?? it seems gail doesn't fire them if we use this atk API
			EventCollection events = EventMonitor.Pause ();
			string evType = "object:state-changed:checked";
			EventCollection radioEvs = events.FindByRole (Atk.Role.RadioButton).FindByType (evType);
			string eventsInXml = String.Format (" events in XML: {0}", Environment.NewLine + events.OriginalGrossXml);
			Assert.AreEqual (2, radioEvs.Count, "bad number of events expected!" + eventsInXml);
			Assert.IsFalse (radioEvs [0].SourceName == radioEvs [1].SourceName, "events should come from different widgets;" + eventsInXml);
			Assert.AreEqual (1, radioEvs.FindWithDetail1 ("1").Count, "one should be Checked;" + eventsInXml);
			Assert.AreEqual (1, radioEvs.FindWithDetail1 ("0").Count, "one should be Checked;" + eventsInXml);

			RunInGuiThread ( delegate () {
				Assert.IsFalse (accessible1.RefStateSet ().ContainsState (Atk.StateType.Checked), "IAF3RB::Checked #4");
				Assert.IsTrue (accessible2.RefStateSet ().ContainsState (Atk.StateType.Checked), "IAF3RB::Checked #5");
				Assert.IsTrue (accessible3.RefStateSet ().ContainsState (Atk.StateType.Checked), "IAF3RB::Checked #6");

				Assert.IsTrue (actionable1.DoAction (0), "IAF3RB::DoAction#2");
			});
			
			System.Threading.Thread.Sleep (200);
			
			RunInGuiThread ( delegate () {
				Assert.IsTrue (accessible1.RefStateSet ().ContainsState (Atk.StateType.Checked), "IAF3RB::Checked #7");
				Assert.IsFalse (accessible2.RefStateSet ().ContainsState (Atk.StateType.Checked), "IAF3RB::Checked #8");
				Assert.IsTrue (accessible3.RefStateSet ().ContainsState (Atk.StateType.Checked), "IAF3RB::Checked #9");
	
				Assert.IsTrue (actionable1.DoAction (0), "IAF3RB::DoAction#3");
			});

			System.Threading.Thread.Sleep (200);
			
			RunInGuiThread ( delegate () {
				Assert.IsTrue (accessible1.RefStateSet ().ContainsState (Atk.StateType.Checked), "IAF3RB::Checked #10");
				Assert.IsFalse (accessible2.RefStateSet ().ContainsState (Atk.StateType.Checked), "IAF3RB::Checked #11");
				Assert.IsTrue (accessible3.RefStateSet ().ContainsState (Atk.StateType.Checked), "IAF3RB::Checked #12");
				
				Assert.IsTrue (actionable3.DoAction (0), "IAF3RB::DoAction#4");
			});
			
			System.Threading.Thread.Sleep (200);
			
			RunInGuiThread ( delegate () {
				Assert.IsTrue (accessible1.RefStateSet ().ContainsState (Atk.StateType.Checked), "IAF3RB::Checked #13");
				Assert.IsFalse (accessible2.RefStateSet ().ContainsState (Atk.StateType.Checked), "IAF3RB::Checked #14");
				Assert.IsTrue (accessible3.RefStateSet ().ContainsState (Atk.StateType.Checked), "IAF3RB::Checked #15");
			});
		}

		protected void InterfaceAction (BasicWidgetType type, Atk.Action implementor, Atk.Object accessible)
		{
			InterfaceAction (type, implementor, accessible, null);
		}
		
		protected void InterfaceAction (BasicWidgetType type, Atk.Action implementor, Atk.Object accessible, string [] names)
		{
			int validNumberOfActions = ValidNumberOfActionsForAButton;
			if (type == BasicWidgetType.TextBoxEntry ||
			    type == BasicWidgetType.ComboBoxDropDownList ||
			    type == BasicWidgetType.ComboBoxDropDownEntry ||
			    type == BasicWidgetType.ComboBoxItem || 
			    type == BasicWidgetType.ListItem || 
			    type == BasicWidgetType.ParentMenu ||
			    type == BasicWidgetType.ChildMenu ||
			    type == BasicWidgetType.HeaderItem ||
			    type == BasicWidgetType.Spinner)
				validNumberOfActions = 1;
			else if (type == BasicWidgetType.CheckedListItem)
				validNumberOfActions = 2;
			
			Assert.AreEqual (validNumberOfActions, implementor.NActions, "NActions");
			
			if (type == BasicWidgetType.ComboBoxDropDownList || 
			    type == BasicWidgetType.ComboBoxDropDownEntry) {
				Assert.AreEqual ("press", implementor.GetName (0), "GetName press");
			} else if (type == BasicWidgetType.TextBoxEntry ||
			           type == BasicWidgetType.Spinner) {
				Assert.AreEqual ("activate", implementor.GetName (0), "GetName activate");
			} else {
				Assert.AreEqual ("click", implementor.GetName (0), "GetName click");
				if (ValidNumberOfActionsForAButton > 1 &&
				    type != BasicWidgetType.ParentMenu &&
				    type != BasicWidgetType.ChildMenu &&
				    type != BasicWidgetType.ComboBoxItem) {
					Assert.AreEqual ("press", implementor.GetName (1), "GetName press");
					Assert.AreEqual ("release", implementor.GetName (2), "GetName release");
				}
				else if (type == BasicWidgetType.CheckedListItem)
					Assert.AreEqual ("toggle", implementor.GetName (1), "GetName toggle");
			}
			
			Atk.StateSet state = accessible.RefStateSet();
			Assert.IsFalse (state.IsEmpty, "RefStateSet.IsEmpty");
			Assert.IsTrue (state.ContainsState (Atk.StateType.Enabled), "RefStateSet.Enabled #1");
			//a radio button is checked by default
			if (type != BasicWidgetType.RadioButton)
				Assert.IsFalse (state.ContainsState (Atk.StateType.Checked), "RefStateSet.!Checked #1");
			
			if (type == BasicWidgetType.ListItem || type == BasicWidgetType.CheckedListItem)
				Assert.IsFalse (state.ContainsState (Atk.StateType.Selected), "RefStateSet.Selected");

			StartEventMonitor ();

			int expectedNumOfWindows = GetTopLevelRootItem ().NAccessibleChildren;

			if (type == BasicWidgetType.ComboBoxItem) {
				Assert.IsTrue (accessible.Name != accessible.Parent.Parent.Name, "combobox item is not the one currently selected" +
				               "(" + accessible.Name + "!=" + accessible.Parent.Parent.Name + ")");
			} else if (type == BasicWidgetType.ComboBoxDropDownEntry || type == BasicWidgetType.ComboBoxDropDownList) {
				Atk.Object menu = accessible.RefAccessibleChild (0);
				Assert.AreEqual (menu.Role, Atk.Role.Menu, "testing the menu states of a combobox");
				Assert.IsTrue (!menu.RefStateSet ().ContainsState (Atk.StateType.Visible) &&
				               !menu.RefStateSet ().ContainsState (Atk.StateType.Showing),
				               "menu child of combobox should not be visible or showing yet");
			}
			
			if (type == BasicWidgetType.ParentMenu)
				Assert.IsFalse (accessible.RefStateSet ().ContainsState (Atk.StateType.Selected),
				                "shouldn't contain Selected before DoAction");

			// only valid actions should work
			for (int i = 0; i < validNumberOfActions; i++) {
				RunInGuiThread (delegate {
					Assert.IsTrue (implementor.DoAction (i), "DoAction(" + i + ")");
					Assert.AreEqual (validNumberOfActions, implementor.NActions, "NActions doesn't change");
				});
			}

			if (type == BasicWidgetType.ParentMenu && !IsBGO574674Addressed ())
				Assert.IsTrue (accessible.RefStateSet ().ContainsState (Atk.StateType.Selected),
				               "should contain Selected after DoAction");

			if (names != null) {
				//because the dropdown represents a new window!
				Assert.AreEqual (++expectedNumOfWindows, GetTopLevelRootItem ().NAccessibleChildren,
				  "Windows in my app should be" + expectedNumOfWindows + 
				  " now that I opened the pandora's box; but I got:" + DescribeChildren (GetTopLevelRootItem ()));
				ExpectEvents (1, Atk.Role.Window, "window:activate");
				Atk.Object newWindow = GetTopLevelRootItem ().RefAccessibleChild (1);
				Assert.AreEqual (Atk.Role.Window, newWindow.Role, "new window role should be Atk.Role.Window");
				Assert.AreEqual (1, newWindow.NAccessibleChildren, "the window should contain a child");

				States (newWindow,
					Atk.StateType.Active,
					Atk.StateType.Enabled,
					Atk.StateType.Sensitive,
					Atk.StateType.Showing,
					Atk.StateType.Visible);

				CheckComboBoxMenuChild (newWindow.RefAccessibleChild (0), names, type, false);

				Atk.Object menu = accessible.RefAccessibleChild (0);
				Assert.AreEqual (menu.Role, Atk.Role.Menu, "testing the menu states of a combobox");
				Assert.IsTrue (menu.RefStateSet ().ContainsState (Atk.StateType.Visible) &&
				               menu.RefStateSet ().ContainsState (Atk.StateType.Showing),
				               "menu child of combobox should be visible and showing NOW!");
			}

			GlibSync ();
			if (type == BasicWidgetType.ComboBoxItem)
				Assert.AreEqual (accessible.Name, accessible.Parent.Parent.Name, "action on combobox item should yield selection");
			
			if (type == BasicWidgetType.CheckBox || type == BasicWidgetType.CheckedListItem) {
				ExpectEvents (1, Atk.Role.CheckBox, "object:state-changed:checked", 1);

				if (type == BasicWidgetType.CheckBox &&
				    validNumberOfActions > 1) {// does not apply in UIA because 1 doaction==1click==checked
				                               // (in GAIL click+press+release==2clicks==unchecked)
					//one more, to leave it checked
					RunInGuiThread (delegate {
						Assert.IsTrue (implementor.DoAction (0), "DoAction_Corrective");
					});
				}

			}
			else {
				//test again
				for (int i = 0; i < validNumberOfActions; i++) {
					RunInGuiThread (delegate {
						Assert.IsTrue (implementor.DoAction (i), "DoAction(" + i + ")");
						Assert.AreEqual (validNumberOfActions, implementor.NActions, "NActions doesn't change");
					});
				}
			}

			if (names != null)
				Assert.AreEqual (--expectedNumOfWindows, GetTopLevelRootItem ().NAccessibleChildren, 
				  "Windows in my app should be " + expectedNumOfWindows +
				  " again, but I got:" + DescribeChildren (GetTopLevelRootItem ()));
			
			state = accessible.RefStateSet ();
			Assert.IsTrue (state.ContainsState (Atk.StateType.Enabled), "RefStateSet.Enabled #2");
			if ((type == BasicWidgetType.CheckBox) || (type == BasicWidgetType.RadioButton) || (type == BasicWidgetType.CheckedListItem))
				Assert.IsTrue (state.ContainsState (Atk.StateType.Checked), "RefStateSet.Checked");
			else
				Assert.IsFalse (state.ContainsState (Atk.StateType.Checked), "RefStateSet.!Checked #2");
			if ((type == BasicWidgetType.ListItem) || (type == BasicWidgetType.CheckedListItem))
				Assert.IsTrue (state.ContainsState (Atk.StateType.Selected), "RefStateSet.Selected");
			
			
			//still need to figure out why this is null in gail
//				Assert.IsNull (implementor.GetLocalizedName (0));
//				Assert.IsNull (implementor.GetLocalizedName (1));
//				Assert.IsNull (implementor.GetLocalizedName (2));
			
			for (int i = 0; i < ValidNumberOfActionsForAButton; i++) 
				Assert.IsNull (implementor.GetDescription (i), "GetDescription null");

			//out of range
			Assert.IsFalse (implementor.DoAction (-1), "DoAction OOR#1");
			Assert.IsFalse (implementor.DoAction (validNumberOfActions), "DoAction OOR#2");
			Assert.IsNull (implementor.GetName (-1), "GetName OOR#1");
			Assert.IsNull (implementor.GetName (validNumberOfActions), "GetName OOR#2");
			Assert.IsNull (implementor.GetDescription (-1), "GetDescription OOR#1");
			Assert.IsNull (implementor.GetDescription (validNumberOfActions), "GetDescription OOR#2");
			Assert.IsNull (implementor.GetLocalizedName (-1), "GetLocalizedName OOR#1");
			Assert.IsNull (implementor.GetLocalizedName (validNumberOfActions), "GetLocalizedName OOR#2");
			
			string descrip = "Some big ugly description";
			for (int i = 0; i < validNumberOfActions; i++) {
				Assert.IsTrue (implementor.SetDescription (i, descrip), "SetDescription");
				Assert.AreEqual (descrip, implementor.GetDescription (i), "GetDescription");
				descrip += ".";
			}
			Assert.IsFalse (implementor.SetDescription(validNumberOfActions, descrip), "SetDescription OOR");
			Assert.IsNull (implementor.GetDescription (validNumberOfActions), "GetDescription OOR#3");
			
			// With no keybinding set, everything should return null
			Assert.IsNull (implementor.GetKeybinding (0), "GetKeyBinding#1");
			Assert.IsNull (implementor.GetKeybinding (1), "GetKeyBinding#2");
			Assert.IsNull (implementor.GetKeybinding (2), "GetKeyBinding#3");
			
			//out of range items too
			Assert.IsNull (implementor.GetKeybinding (-1), "GetKeyBinding OOR#1");
			Assert.IsNull (implementor.GetKeybinding (3), "GetKeyBinding OOR#2");

			//sub-items cannot be disabled, mainly because they are not widgets
			if ((type != BasicWidgetType.ListItem) &&
			    (type != BasicWidgetType.HeaderItem) &&
			    (type != BasicWidgetType.CheckedListItem) &&
			    (type != BasicWidgetType.TableCell) &&
			    (type != BasicWidgetType.ComboBoxItem)) { //disable a combobox item? let's not try weird things
				DisableWidget (accessible);
				for (int i = 0; i < validNumberOfActions; i++) 
					Assert.IsFalse (implementor.DoAction (i), "DoAction(" + i + ") after disabling");
				EnableWidget (accessible);
			}
		}
		
		private void CheckNonMultipleChildrenSelection (Atk.Selection sel, Atk.Object accessible, int theSelected, bool childrenCorrection,
		                                                BasicWidgetType type)
		{
			int initial = childrenCorrection ? 1 : 0;
			for (int i = initial; i < accessible.NAccessibleChildren; i++) {
				bool shouldBeSelected = (theSelected == i);
				
				Assert.AreEqual (shouldBeSelected, sel.IsChildSelected (i), 
				                 "IsChildSelected(" + i + ")!=" + shouldBeSelected.ToString());
				Assert.AreEqual (shouldBeSelected, 
				  accessible.RefAccessibleChild (i).RefStateSet ().ContainsState (Atk.StateType.Selected),
				  String.Format ("after AddSelection({0}), child({1}) should have correct Selected state", 
				    theSelected, i));
			}
		}
		
		protected void InterfaceSelection (Atk.Selection implementor, string [] items, Atk.Object accessible, BasicWidgetType type)
		{
			if (items == null)
				throw new ArgumentNullException ("names");
			
			if (items.Length < 2)
				throw new ArgumentException ("For testing purposes, use 2 or more items", "names");
			
			string accessibleName = null;
			if (type == BasicWidgetType.ParentMenu ||
			    type == BasicWidgetType.GroupBox)
				accessibleName = items [0];
			else if (type == BasicWidgetType.ListBox ||
			         type == BasicWidgetType.CheckedListBox ||
			         type == BasicWidgetType.DomainUpDown ||
			         type == BasicWidgetType.ListView)
				accessibleName = null;

			string labelName = null;
			if (type == BasicWidgetType.ListView && SupportsLabeledBy (out labelName))
				accessibleName = labelName;

			// Be forgiving when we can't set NULL due to
			// Gtk-CRITICALs
			if (accessibleName == null
			    && accessible.Name == String.Empty) {
				accessibleName = String.Empty;
			}

			Assert.AreEqual (accessibleName, accessible.Name,
			                 "AtkObj Name, was: " + accessible.Name);

			string [] names = items;
			if (type == BasicWidgetType.ParentMenu || type == BasicWidgetType.GroupBox) {
				names = new string [items.Length - 1];
				Array.Copy (items, 1, names, 0, names.Length);
				Assert.AreEqual (names [0], items [1], "array copy not done correctly");
				Assert.AreEqual (names [names.Length - 1], items [items.Length - 1], "array copy not done correctly");
			}
			
			int nAccessibleChildren = (Misc.IsComboBox (type)) ?
				accessible.RefAccessibleChild(0).NAccessibleChildren:
				accessible.NAccessibleChildren;

			bool clearSelection = true;
			if (type == BasicWidgetType.TabControl)
				clearSelection = false;
			if (Misc.IsComboBox (type) || type == BasicWidgetType.ComboBoxMenu)
				clearSelection = AllowsEmptyingSelectionOnComboBoxes || (implementor.SelectionCount == 0);
			
			Assert.AreEqual (clearSelection, implementor.ClearSelection (), "ClearSelection #1, expected: " + clearSelection);
			if (type != BasicWidgetType.TabControl && clearSelection) {
				for (int i = 0; i < names.Length; i++)
					Assert.IsFalse (implementor.IsChildSelected (i), "isChildSelected(" + i + ")");
				Assert.AreEqual (0, implementor.SelectionCount, "SelectionCount == 0");
			}

			bool [] indexUsed = new bool [accessible.NAccessibleChildren];
			for (int i = 0; i < names.Length; i++) {
				int val;
				if (type == BasicWidgetType.ListView || type == BasicWidgetType.TreeView) {
					Atk.Object child = FindObjectByName (accessible, names [i]);
					Assert.IsNotNull (child, "FindObjectByName: " + names [i]);
					val = child.IndexInParent;
					Assert.IsTrue (val >= 0, "IndexInParent should not be negative");
					Assert.IsFalse (indexUsed [val], "Child " + names [i] + " has already-used IndexInParent " + val);
					indexUsed [val] = true;
				} else if (type == BasicWidgetType.ComboBoxSimpleMenu && !HasComboBoxSimpleLayout) {
					val = i + 1; //column header child doesn't count to be selected
				} else {
					val = i;
				}
				bool? selected = null;
				StartEventMonitor ();
				RunInGuiThread (delegate () {
					selected = implementor.AddSelection (val);
				});
				// for ComboBox tests
				ExpectEvents (0, Atk.Role.Frame, "window:activate");

				Assert.IsTrue (selected.Value, "AddSelection(" + i + "), we got:" + selected.Value);
				
				if ((type != BasicWidgetType.ParentMenu &&
				     type != BasicWidgetType.MainMenuBar &&
				     type != BasicWidgetType.ContextMenu)
				    || !IsBGO574674Addressed ())
					CheckNonMultipleChildrenSelection (implementor, accessible, val, false, type);

				if (!Misc.IsComboBox (type))
					Assert.IsNotNull (accessible.RefAccessibleChild (val), "accessible.RefAccessibleChild (" + i + ") != null");
				Assert.IsNotNull (implementor.RefSelection (0), "implementor.RefSelection (0) != null");
				Assert.IsNull (implementor.RefSelection (-1), "implementor.RefSelection (-1) == null");
				Assert.IsNull (implementor.RefSelection (1), "implementor.RefSelection (1) == null");

				if (!Misc.IsComboBox (type))
					Assert.IsTrue (accessible.RefAccessibleChild (val) == implementor.RefSelection (0),
					               "accessible.RefAccessibleChild (" + val + ") == implementor.RefSelection (0)");
				else
					Assert.IsTrue (accessible.RefAccessibleChild (0).RefAccessibleChild (val) == implementor.RefSelection (0),
					               "accessible.RefAccessibleChild (" + i + ") == implementor.RefSelection (0)");
				
				string accName = names [i];
				if (type == BasicWidgetType.ParentMenu ||
				    type == BasicWidgetType.GroupBox)
					accName = items [0];
				else if (type == BasicWidgetType.ListView)
					accName = accessibleName;
				else if (type == BasicWidgetType.TabControl ||
				         type == BasicWidgetType.ComboBoxMenu ||
				         type == BasicWidgetType.MainMenuBar ||
				         type == BasicWidgetType.ContextMenu ||
				         type == BasicWidgetType.ComboBoxSimpleMenu ||
				         type == BasicWidgetType.ListBox ||
				         type == BasicWidgetType.CheckedListBox)
					accName = null;
				GlibSync ();
				Assert.AreEqual (accName, accessible.Name, "AtkObj Name #" + i + ", we got: " + accessible.Name);

				Atk.Object refSelObj = implementor.RefSelection (0);
				if (type != BasicWidgetType.ParentMenu && type != BasicWidgetType.ContextMenu) {
					Assert.IsNotNull (refSelObj, "refSel should not be null");
					if (type == BasicWidgetType.ComboBoxMenu ||
					    type == BasicWidgetType.MainMenuBar ||
					    type == BasicWidgetType.ContextMenu ||
					    type == BasicWidgetType.ComboBoxSimpleMenu)
						Assert.AreEqual (names [i], refSelObj.Name, 
						                 "AtkObj NameRefSel#" + i + ", should be:" + names [i]);
					else if (type != BasicWidgetType.ListBox && 
					         type != BasicWidgetType.CheckedListBox && 
					         type != BasicWidgetType.TabControl && 
					         type != BasicWidgetType.TreeView && 
					         type != BasicWidgetType.ListView && 
					         type != BasicWidgetType.GroupBox && 
					         type != BasicWidgetType.DomainUpDown)
						Assert.AreEqual (accessible.Name, refSelObj.Name, "AtkObj NameRefSel#" + i);
					Assert.AreEqual (1, implementor.SelectionCount, "SelectionCount == 1");
					Assert.IsTrue (implementor.IsChildSelected (val), "childSelected(" + val + ")");
					Assert.IsTrue (refSelObj.RefStateSet ().ContainsState (Atk.StateType.Selectable), "Selected child should have State.Selectable");
					if (((type == BasicWidgetType.ComboBoxDropDownList) ||
					     (type == BasicWidgetType.ComboBoxDropDownEntry))) {
						Assert.IsTrue (refSelObj.RefStateSet ().ContainsState (Atk.StateType.Selected) ==
						  IsBGO561414Addressed (), "Selected child(" + i + ") should have State.Selected");
					}
					for (int j = 0; j < names.Length; j++) {
						if (j == val)
							continue;
						if (type == BasicWidgetType.ComboBoxDropDownEntry ||
						    type == BasicWidgetType.ComboBoxDropDownList)
							Assert.IsFalse (
							  accessible.RefAccessibleChild (0).RefAccessibleChild (j).RefStateSet ().ContainsState (
							    Atk.StateType.Selected), "Unselected child(" + j + ") shouldn't have State.Selected");
					}
				} else {
					Assert.IsNotNull (refSelObj, "refSel should not be null");
					Assert.AreEqual (names [i], refSelObj.Name, "AtkObj NameRefSel0,#" + i);
					Assert.AreEqual (1, implementor.SelectionCount, "SelectionCount == 1");
					Assert.IsTrue (implementor.IsChildSelected (val), "childSelected(" + val + ")");
				}
				if (i == 1 && (type == BasicWidgetType.ListBox || type == BasicWidgetType.CheckedListBox || type == BasicWidgetType.DomainUpDown))
					Assert.IsFalse (accessible.RefAccessibleChild(0).RefStateSet().ContainsState (Atk.StateType.Selected), "Unselected child should not have State.Selected");
				
				int refSelPos = val;
				if (refSelPos == 0)
					refSelPos = -1;
				Assert.IsNull (implementor.RefSelection (refSelPos), "RefSelection OOR#-" + i);

				if (type != BasicWidgetType.ComboBoxMenu) { //FIXME: report this on gail, removeSelection returns true/false arbitrarily for this case
					bool removeSelectionSuccess = true;
					if (type == BasicWidgetType.ComboBoxDropDownEntry || 
					    type == BasicWidgetType.ComboBoxDropDownList)
						removeSelectionSuccess = AllowsEmptyingSelectionOnComboBoxes;
					else if (type == BasicWidgetType.DomainUpDown || 
					         type == BasicWidgetType.TabControl ||
					         type == BasicWidgetType.ComboBoxSimpleMenu ||
					         type == BasicWidgetType.TreeView)
						removeSelectionSuccess = false;
					if (i != names.Length - 1)
						Assert.AreEqual (implementor.RemoveSelection (val),
						  removeSelectionSuccess,
						  String.Format ("restoring initial situation to empty selection ({0}), expected: {1}",
						                 i, removeSelectionSuccess));
				}
			}

			Assert.IsNotNull (implementor.RefSelection (0), "RefSel!=null");

			string lastName = accessible.Name;

			if (type == BasicWidgetType.ComboBoxDropDownList ||
			    type == BasicWidgetType.ComboBoxDropDownEntry || 
			    type == BasicWidgetType.ListBox || 
			    type == BasicWidgetType.CheckedListBox ||
			    type == BasicWidgetType.DomainUpDown ||
			    type == BasicWidgetType.GroupBox ||
			    type == BasicWidgetType.TabControl) {
				//strangely, OOR selections return true (valid) -> TODO: report bug on Gail
				Assert.IsTrue (implementor.AddSelection (-1), "AddSelection OOR#1");
			} else {
				Assert.IsFalse (implementor.AddSelection (-1), "AddSelection OOR#1");
			}

			bool success = false;
			//strangely, OOR upper selection returns true for some widgets -> report to Gail
			if (type == BasicWidgetType.TabControl ||
			    Misc.IsComboBox (type))
				success = true;

			if (type != BasicWidgetType.ListBox &&
			    type != BasicWidgetType.CheckedListBox &&
			    type != BasicWidgetType.GroupBox &&
			    type != BasicWidgetType.DomainUpDown) //<- to mimic this bug in the bridge is dumb, let's file it on gail (so we put this to make bridge test pass)
				Assert.AreEqual (success, implementor.AddSelection (nAccessibleChildren), "AddSelection OOR#2");
			
			Assert.AreEqual (lastName, accessible.Name, "OOR selections shouldn't affect name");

			Assert.IsNull (implementor.RefSelection (-1), "RefSelection OOR#1"); 
			
			Assert.IsNull (implementor.RefSelection (names.Length), "RefSelection OOR#2");

			Atk.Object currentSel = null;
			
			if (type != BasicWidgetType.TabControl) {
				clearSelection = true;
				if (Misc.IsComboBox (type) ||
				    type == BasicWidgetType.ComboBoxMenu ||
				    type == BasicWidgetType.ComboBoxSimpleMenu)
					clearSelection = AllowsEmptyingSelectionOnComboBoxes;
				else if (type == BasicWidgetType.DomainUpDown)
					clearSelection = false;
				Assert.AreEqual (clearSelection, implementor.ClearSelection (),
				                 "ClearSelection #2, we got " + !clearSelection);
				currentSel = implementor.RefSelection (0);
				if (clearSelection)
					Assert.IsNull (currentSel, "RefSel after CS");
			}

			//this is a normal combobox (not multiline) (TODO: research multiline comboboxes?)
			Assert.IsFalse (implementor.SelectAllSelection (), "SelectAllSelection should return false");
			
			if (type != BasicWidgetType.ListBox && type != BasicWidgetType.CheckedListBox && type != BasicWidgetType.TabControl && type != BasicWidgetType.DomainUpDown)
				Assert.AreEqual (currentSel, implementor.RefSelection (0), "RefSel after SAS");
			
			Assert.IsTrue (names.Length > 0, "Please use a names variable that is not empty");
			int firstSelectionIndex = 0;
			if (type == BasicWidgetType.TreeView)
				firstSelectionIndex =1;
			else if (type == BasicWidgetType.ListView)
				firstSelectionIndex = FindObjectByName (accessible, names[0]).IndexInParent;
			else if (type == BasicWidgetType.ComboBoxSimpleMenu && !HasComboBoxSimpleLayout)
				firstSelectionIndex = 1; //column header child doesn't count to be selected
			RunInGuiThread (delegate() {
				Assert.IsTrue (implementor.AddSelection (firstSelectionIndex), "AddSelection->0");
			});

			Assert.IsNotNull (implementor.RefSelection (0), "RefSel!=null after AS0");

			success = true;
			if (type == BasicWidgetType.ListBox ||
			    type == BasicWidgetType.CheckedListBox ||
			    type == BasicWidgetType.TabControl ||
			    type == BasicWidgetType.TreeView ||
			    type == BasicWidgetType.ListView ||
			    type == BasicWidgetType.DomainUpDown ||
			    type == BasicWidgetType.ComboBoxMenu ||
			    type == BasicWidgetType.ParentMenu ||
			    type == BasicWidgetType.GroupBox ||
			    type == BasicWidgetType.MainMenuBar ||
			    type == BasicWidgetType.ContextMenu ||
			    type == BasicWidgetType.ComboBoxSimpleMenu)
				success = false;
			Assert.AreEqual (success, implementor.RemoveSelection (nAccessibleChildren), "RemoveSelection OOR#>n");
			Assert.AreEqual (success, implementor.RemoveSelection (-1), "RemoveSelection OOR#<0");

			if (type != BasicWidgetType.ListBox &&
			    type != BasicWidgetType.CheckedListBox &&
			    type != BasicWidgetType.TreeView &&
			    type != BasicWidgetType.ListView &&
			    type != BasicWidgetType.TabControl &&
			    type != BasicWidgetType.GroupBox &&
			    type != BasicWidgetType.DomainUpDown &&
			    type != BasicWidgetType.ComboBoxDropDownEntry &&
			    type != BasicWidgetType.ComboBoxDropDownList &&
			    type != BasicWidgetType.ComboBoxMenu &&
			    type != BasicWidgetType.ComboBoxSimpleMenu) //see FIXME above
			{
				Assert.IsTrue (implementor.RemoveSelection (firstSelectionIndex), "RemoveSelection");
				Assert.IsNull (implementor.RefSelection (0), "RefSel after RemoveSel");
			}


			if (!Misc.IsComboBox (type)) {
				implementor.ClearSelection ();

				//In List
				RunInGuiThread (delegate () {
					implementor.AddSelection (firstSelectionIndex);
				});
				currentSel = implementor.RefSelection (0);
				Assert.IsNotNull (currentSel, "Current selection should not be null");
				Atk.Component atkComponentCurrentSel = CastToAtkInterface <Atk.Component> (currentSel);
				atkComponentCurrentSel.GrabFocus ();
				Atk.StateSet stateSet = currentSel.RefStateSet ();
				if (type != BasicWidgetType.TabControl && 
				    type != BasicWidgetType.TreeView &&
				    type != BasicWidgetType.ListView &&
				    type != BasicWidgetType.GroupBox &&
				    type != BasicWidgetType.ComboBoxMenu &&
				    type != BasicWidgetType.ParentMenu &&
				    type != BasicWidgetType.MainMenuBar &&
				    type != BasicWidgetType.ContextMenu &&
				    type != BasicWidgetType.ComboBoxSimpleMenu)
					Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Focused), "Focused in selected item.");
				
				if (!(IsBGO574674Addressed ()
				      && (type == BasicWidgetType.ContextMenu
				          || type == BasicWidgetType.MainMenuBar
				          || type == BasicWidgetType.ParentMenu)))
					Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Selected), "Selected in selected item.");
			}
		}
		
		private void TextMatchesValue (BasicWidgetType type, Atk.Value atkValue, Atk.Text atkText)
		{
			if (atkText == null)
				return;
			string text;
			// For some reason, gail might preface a slider's text
			// with \x200e
			if (type == BasicWidgetType.VTrackBar)
				text = GetCurrentValue (atkValue).ToString ().Replace ("\x2005", "");
			else if (type == BasicWidgetType.Spinner)
				text = GetCurrentValue (atkValue).ToString ();
			else
				text = GetCurrentValue (atkValue).ToString ("F2");
			Assert.AreEqual (text, atkText.GetText (0, -1), "GetText");
			Assert.AreEqual (text.Length, atkText.CharacterCount, "CharacterCount");
			Assert.AreEqual (text [0], atkText.GetCharacterAtOffset (0), "GetCharacterAtOffset");
		}

		protected void InterfaceValue (BasicWidgetType type, Atk.Value atkValue, Atk.Text atkText)
		{
			Assert.IsNotNull (atkValue, "InterfaceValue value not null");
			Assert.AreEqual (  0, GetMinimumValue(atkValue), "InterfaceValue MinimumValue");
			if (type == BasicWidgetType.HScrollBar || type == BasicWidgetType.VScrollBar)
				Assert.IsTrue (GetMaximumValue(atkValue) > 0, "InterfaceValue MaximumValue > 0");
			else
				Assert.AreEqual (100, GetMaximumValue(atkValue), "InterfaceValue MaximumValue");
			if (type == BasicWidgetType.Spinner || type == BasicWidgetType.VTrackBar) {
				Assert.IsTrue (SetCurrentValue (atkValue, 25), "SetCurrentValue");
				Assert.AreEqual (25, GetCurrentValue(atkValue), "InterfaceValue CurrentValue #1");
			}
			TextMatchesValue (type, atkValue, atkText);
		}

		protected void InterfaceValue (BasicWidgetType type, Atk.Value atkValue)
		{
			InterfaceValue (type, atkValue, null);
		}

		protected void InterfaceImage (BasicWidgetType type, Atk.Image implementor, Atk.Component component, 
		                               Atk.Image withoutImageImplementor)
		{
			Assert.IsNull (implementor.ImageDescription, "ImageDescription == null initially");
			Assert.IsNull (withoutImageImplementor.ImageDescription, "wii.ImageDescription == null initially");
			string myDesc = "Hola mundo";
			Assert.IsTrue (implementor.SetImageDescription (myDesc), "Setting the img desc should return success");
			if (type != BasicWidgetType.PictureBox)
				Assert.IsFalse (withoutImageImplementor.SetImageDescription (myDesc), "Setting the img desc should return false");
			Assert.AreEqual (myDesc, implementor.ImageDescription, "The img desc should have been overriden correctly");
			if (type != BasicWidgetType.PictureBox)
				Assert.IsNull (withoutImageImplementor.ImageDescription, "wii.ImageDescription == null always");
			int ia, ib, ca, cb;
			RunInGuiThread (delegate () {
				implementor.GetImagePosition (out ia, out ib, Atk.CoordType.Screen);
				component.GetPosition (out ca, out cb, Atk.CoordType.Screen);
				Assert.IsTrue (ia > 0, "x of the image must be > 0; obtained " + ia);
				Assert.IsTrue (ib > 0, "y of the image must be > 0; obtained " + ib);
				Assert.IsTrue (ia >= ca, "x of the image must be >= x from the widget; obtained " + ia + "<" + ca);
				Assert.IsTrue (ib >= cb, "y of the image must be >= y from the widget; obtained " + ia + "<" + cb);
			});

			RunInGuiThread (delegate () {
				withoutImageImplementor.GetImagePosition (out ia, out ib, Atk.CoordType.Screen);
				Assert.AreEqual (int.MinValue, ia, "x of the image must be int.MinValue; obtained " + ia);
				Assert.AreEqual (int.MinValue, ib, "y of the image must be int.MinValue; obtained " + ib);
			});
			
			implementor.GetImageSize (out ia, out ib);
			component.GetSize (out ca, out cb);
			Assert.IsTrue (ia > 0, "width of the image must be > 0; obtained " + ia);
			Assert.IsTrue (ib > 0, "height of the image must be > 0; obtained " + ib);
			Assert.IsTrue (ia <= ca, "width of the image must be <= width from the widget; obtained " + ia + ">" + ca);
			Assert.IsTrue (ib <= cb, "height of the image must be <= height from the widget; obtained " + ib + ">" + cb);

			withoutImageImplementor.GetImageSize (out ia, out ib);
			Assert.AreEqual (-1, ia, "width of the image must be int.MinValue; obtained " + ia);
			Assert.AreEqual (-1, ib, "height of the image must be int.MinValue; obtained " + ib);
		}

		protected void CheckComboBoxMenuChild (Atk.Object menuChild, string [] items, BasicWidgetType comboType)
		{
			CheckComboBoxMenuChild (menuChild, items, comboType, true);
		}
		
		protected void CheckComboBoxMenuChild (Atk.Object menuChild, string [] items, BasicWidgetType comboType, bool defaultState)
		{
			if (comboType != BasicWidgetType.ComboBoxDropDownEntry &&
			    comboType != BasicWidgetType.ComboBoxDropDownList &&
			    comboType != BasicWidgetType.ComboBoxSimple)
				throw new ArgumentException ("comboType", "The comboType should be a comboBox");

			if (defaultState)
				States (menuChild,
				        Atk.StateType.Enabled,
				        Atk.StateType.Selectable,
				        Atk.StateType.Sensitive);
			else {
				if (comboType == BasicWidgetType.ComboBoxSimple)
					States (menuChild,
					        Atk.StateType.Enabled,
					        Atk.StateType.Sensitive,
					        Atk.StateType.Showing,
					        Atk.StateType.Visible,
					        Atk.StateType.Focusable,
					        Atk.StateType.ManagesDescendants);
				else
					States (menuChild,
					        Atk.StateType.Enabled,
					        Atk.StateType.Selectable,
					        Atk.StateType.Sensitive,
					        Atk.StateType.Selected,
					        Atk.StateType.Showing,
					        Atk.StateType.Visible,
					        comboType == BasicWidgetType.ComboBoxDropDownEntry ? 
					        Atk.StateType.Focused : Atk.StateType.Enabled);
			}
			
			Assert.IsNotNull (menuChild, "ComboBox child#0 should not be null");
			Assert.IsNull (menuChild.Name, "the ComboBox menu should not have a name");
			if (comboType == BasicWidgetType.ComboBoxSimple)
				Assert.AreEqual (menuChild.Role, Atk.Role.TreeTable, "ComboBox child#0 should be a table");
			else
				Assert.AreEqual (menuChild.Role, Atk.Role.Menu, "ComboBox child#0 should be a menu");

			Atk.Action action = CastToAtkInterface <Atk.Action> (menuChild);
			Assert.IsNull (action, "the Menu child of a combobox should not implement Atk.Action");

			//HACK: no way to hide the column header!
			int childrenCorrection = (comboType == BasicWidgetType.ComboBoxSimple && !HasComboBoxSimpleLayout)? 1 : 0;
			
			Assert.AreEqual (items.Length + childrenCorrection, 
			                 menuChild.NAccessibleChildren, 
			                 "ComboBox menu numChildren; children roles:" + 
			                 DescribeChildren (menuChild));
			
			for (int i = 0; i < items.Length; i++) {
				int nth = i + childrenCorrection;
				Atk.Object menuItemChild = menuChild.RefAccessibleChild (nth);
				Assert.IsNotNull (menuItemChild, "ComboBox child#0 child#" + nth + " should not be null");
				if (comboType == BasicWidgetType.ComboBoxSimple)
					Assert.AreEqual (menuItemChild.Role, Atk.Role.TableCell, "ComboBox child#0 child#0 should be a tableCell");
				else
					Assert.AreEqual (menuItemChild.Role, Atk.Role.MenuItem, "ComboBox child#0 child#0 should be a menuItem");

				Assert.AreEqual (menuItemChild.Name, items [i], "ComboBox menuitem names should be the same as the items");
				Assert.AreEqual (0, menuItemChild.NAccessibleChildren, "ComboBox menuItem numChildren");
				Assert.IsNull (CastToAtkInterface <Atk.Selection> (menuItemChild), "a comboboxitem should not implement Atk.Selection");

				if (defaultState)
					States (menuItemChild,
				            Atk.StateType.Enabled,
				            Atk.StateType.Selectable,
				            Atk.StateType.Sensitive,
				            Atk.StateType.Visible);
			}

			Atk.Selection atkSelection = CastToAtkInterface <Atk.Selection> (menuChild);
			Assert.IsNotNull (atkSelection, "the Menu child of a combobox should implement Atk.Selection");
			InterfaceSelection (atkSelection, items, menuChild, 
			                    comboType == BasicWidgetType.ComboBoxSimple ? 
			                      BasicWidgetType.ComboBoxSimpleMenu : BasicWidgetType.ComboBoxMenu);
		}
		
		protected void StatesComboBox (Atk.Object accessible)
		{
			States (accessible,
			  Atk.StateType.Enabled,
			  Atk.StateType.Sensitive,
			  Atk.StateType.Showing,
			  Atk.StateType.Visible);
		}

		protected string DescribeChildren (Atk.Object accessible)
		{
			string res = String.Empty;
			for (int i = 0; i < accessible.NAccessibleChildren; i++)
				res += accessible.RefAccessibleChild (i).Role.ToString () + 
				  "('" + (accessible.Name == null ? "" : accessible.Name) + "'-" +
				  accessible.NAccessibleChildren + "),";
			if (res == String.Empty)
				return "<no children>";
			return res;
		}
		
		protected void PropertyRole (BasicWidgetType type, Atk.Object accessible)
		{
			GlibSync ();
			Atk.Role role = GetRole (type);
			Assert.AreEqual (role, accessible.Role, "Atk.Role, we got " + accessible.Role);
		}

		protected Atk.Role GetRole (BasicWidgetType type)
		{
			switch (type) {
			case BasicWidgetType.Label:
			case BasicWidgetType.ToolStripLabel:
				return Atk.Role.Label;
			case BasicWidgetType.ToolBarPushButton:
			case BasicWidgetType.ToolStripButton:
			case BasicWidgetType.NormalButton:
				return Atk.Role.PushButton;
			case BasicWidgetType.Window:
				return Atk.Role.Frame;
			case BasicWidgetType.ListItem:
				return Atk.Role.ListItem;
			case BasicWidgetType.CheckedListItem:
			case BasicWidgetType.CheckBox:
				return Atk.Role.CheckBox;
			case BasicWidgetType.ComboBoxDropDownEntry:
			case BasicWidgetType.ComboBoxDropDownList:
				return Atk.Role.ComboBox;
			case BasicWidgetType.ComboBoxSimple:
				return HasComboBoxSimpleLayout ? 
				       Atk.Role.ComboBox : Atk.Role.TreeTable;
			case BasicWidgetType.RadioButton:
				return Atk.Role.RadioButton;
			case BasicWidgetType.PasswordCharTextBoxEntry:
			case BasicWidgetType.TextBoxEntry:
			case BasicWidgetType.TextBoxView:
			case BasicWidgetType.RichTextBox:
				return Atk.Role.Text;
			case BasicWidgetType.StatusBar:
				return Atk.Role.Statusbar;
			case BasicWidgetType.ChildMenu:
				return Atk.Role.MenuItem;
			case BasicWidgetType.MainMenuBar:
				return Atk.Role.MenuBar;
			case BasicWidgetType.ContextMenu:
			case BasicWidgetType.ParentMenu:
				return Atk.Role.Menu;
			case BasicWidgetType.ChildMenuSeparator:
				return Atk.Role.Separator;
			case BasicWidgetType.StatusStrip:
				return Atk.Role.Statusbar;
			case BasicWidgetType.HScrollBar:
			case BasicWidgetType.VScrollBar:
				return Atk.Role.ScrollBar;
			case BasicWidgetType.ProgressBar:
			case BasicWidgetType.ToolStripProgressBar:
				return Atk.Role.ProgressBar;
			case BasicWidgetType.ListBox:
			case BasicWidgetType.CheckedListBox:
				return Atk.Role.TreeTable;
			case BasicWidgetType.Spinner:
			case BasicWidgetType.DomainUpDown:
				return Atk.Role.SpinButton;
			case BasicWidgetType.TabControl:
				return Atk.Role.PageTabList;
			case BasicWidgetType.TabPage:
				return Atk.Role.PageTab;
			case BasicWidgetType.ListView:
				return Atk.Role.TreeTable;
			case BasicWidgetType.TreeView:
				return Atk.Role.TreeTable;
			case BasicWidgetType.PictureBox:
				return Atk.Role.Icon;
			case BasicWidgetType.ContainerPanel:
			case BasicWidgetType.ErrorProvider:
			case BasicWidgetType.DateTimePicker:
			case BasicWidgetType.GroupBox:
				return Atk.Role.Panel;
			case BasicWidgetType.ToolStripSplitButton:
			case BasicWidgetType.ToolBarDropDownButton:
				return Atk.Role.Filler;
			case BasicWidgetType.ToolStripDropDownButton:
				return Atk.Role.Menu;
			case BasicWidgetType.DataGridView:
				return Atk.Role.TreeTable;					
			case BasicWidgetType.ToolBar:
				return Atk.Role.ToolBar;
			case BasicWidgetType.StatusBarPanel:
				return Atk.Role.Label;
			case BasicWidgetType.HSplitContainer:
				return Atk.Role.SplitPane;
			case BasicWidgetType.VTrackBar:
				return Atk.Role.Slider;
			default:
				throw new NotImplementedException (String.Format (
					"Couldn't find the role for {0}.  Did you forget to add it to AtkTester::PropertyRole ()?",
					type));
			}
		}

		protected Atk.Object InterfaceText (BasicWidgetType type)
		{
			return InterfaceTextAux (type, false, null, null);
		}

		protected void InterfaceEditableText (BasicWidgetType type, Atk.Object accessible)
		{
			if (type == BasicWidgetType.Spinner) {
				InterfaceEditableTextWithValue (type, accessible);
				return;
			}
			Atk.EditableText atkEditableText = CastToAtkInterface<Atk.EditableText> (accessible);
			atkEditableText.TextContents = "abcdef";
			InterfaceText (accessible, "abcdef");
			atkEditableText.DeleteText (2, 4);
			try {
				InterfaceText (accessible, "abef");
			} catch (Misc.UntestableException) { }
			int pos = 0;
			atkEditableText.InsertText ("xx", ref pos);
			InterfaceText (accessible, "xxabef");
			pos = 5;
			atkEditableText.InsertText ("zz", ref pos);
			Assert.AreEqual (5, pos, "Position should not change after insert");
			InterfaceText (accessible, "xxabezzf");

			// Test cut/copy/paste support
			atkEditableText.TextContents = "And your head is made of clouds, but your feet are made of ground.";
			RunInGuiThread (delegate () {
				atkEditableText.CopyText (4, 13);
				atkEditableText.PasteText (0);
			});

			InterfaceText (accessible, "your headAnd your head is made of clouds, but your feet are made of ground.");

			RunInGuiThread (delegate () {
				atkEditableText.CutText (0, 8);
			});
			InterfaceText (accessible, "dAnd your head is made of clouds, but your feet are made of ground.");

			EditReadOnly (type, accessible);
			// Enabling/disabling doesn't currently work in the
			// test for ListView items
			if (type != BasicWidgetType.ListView)
				EditDisable (accessible);
		}

		void InterfaceEditableTextWithValue (BasicWidgetType type, Atk.Object accessible)
		{
			Atk.EditableText atkEditableText = CastToAtkInterface<Atk.EditableText> (accessible);
			Atk.Text atkText = CastToAtkInterface<Atk.Text> (accessible);
			Atk.Value atkValue = CastToAtkInterface<Atk.Value> (accessible);
			Atk.Action atkAction = CastToAtkInterface<Atk.Action> (accessible);
			atkEditableText.TextContents = "42";
			Assert.IsTrue (atkAction.DoAction (0), "DoAction #1");
			System.Threading.Thread.Sleep (250);
			Assert.AreEqual (42, GetCurrentValue (atkValue), "CurrentValue #1");
			TextMatchesValue (type, atkValue, atkText);
			atkEditableText.DeleteText (1, 2);
			Assert.AreEqual (42, GetCurrentValue (atkValue), "CurrentValue should not change until DoAction called");
			Assert.IsTrue (atkAction.DoAction (0), "DoAction #2");
			System.Threading.Thread.Sleep (250);
			Assert.AreEqual (4, GetCurrentValue (atkValue), "CurrentValue #3");
			TextMatchesValue (type, atkValue, atkText);
			int pos = 0;
			atkEditableText.InsertText ("6", ref pos);
			Assert.AreEqual (1, pos, "Position should increment after InsertText");
			Assert.AreEqual (4, GetCurrentValue (atkValue), "CurrentValue should not change until DoAction called");
			Assert.IsTrue (atkAction.DoAction (0), "DoAction #3");
			System.Threading.Thread.Sleep (250);
			Assert.AreEqual (64, GetCurrentValue (atkValue), "CurrentValue #5");
			TextMatchesValue (type, atkValue, atkText);
			atkEditableText.DeleteText (1, 0);
			Assert.AreEqual ("64", atkText.GetText (0, -1), "GetText #1");
			atkEditableText.DeleteText (1, -1);
			Assert.AreEqual ("6", atkText.GetText (0, -1), "GetText #2");
			atkEditableText.TextContents = "56";
			Assert.AreEqual ("56", atkText.GetText (0, -1), "GetText #3");
			atkEditableText.DeleteText (-1, 1);
			Assert.AreEqual ("6", atkText.GetText (0, -1), "GetText #4");
			atkEditableText.DeleteText (54, 99);
			Assert.AreEqual ("6", atkText.GetText (0, -1), "GetText #5");
			pos = -3;
			atkEditableText.InsertText ("5", ref pos);
			Assert.AreEqual ("65", atkText.GetText (0, -1), "GetText #6");
			Assert.AreEqual (2, pos, "InsertText pos");

			EditReadOnly (type, accessible);
			EditDisable (accessible);
		}

		protected void EditReadOnly (BasicWidgetType type, Atk.Object accessible)
		{
			Atk.EditableText atkEditableText = CastToAtkInterface<Atk.EditableText> (accessible);
			Atk.Text atkText = CastToAtkInterface<Atk.Text> (accessible);

			SetReadOnly (type, accessible, false);
			atkEditableText.TextContents = "0";
			SetReadOnly (type, accessible, true);
			Assert.IsFalse (accessible.RefStateSet().ContainsState (Atk.StateType.Editable), "ReadOnly element should not have Editable state");
			atkEditableText.TextContents = "5";
			int pos = 0;
			atkEditableText.InsertText ("6", ref pos);
			atkEditableText.DeleteText (0, 2);
			Assert.AreEqual ("0", atkText.GetText (0, -1), "AtkEditableText should not change text if ReadOnly");
			SetReadOnly (type, accessible, false);
			Assert.IsTrue (accessible.RefStateSet().ContainsState (Atk.StateType.Editable), "Non-ReadOnly element should have Editable state");
		}

		protected void EditDisable (Atk.Object accessible)
		{
			Atk.EditableText atkEditableText = CastToAtkInterface<Atk.EditableText> (accessible);
			Atk.Text atkText = CastToAtkInterface<Atk.Text> (accessible);

			EnableWidget (accessible);
			atkEditableText.TextContents = "0";
			DisableWidget (accessible);
			atkEditableText.TextContents = "5";
			bool editableExpected = (atkText.GetText (0, -1) == "5");
			Assert.AreEqual (editableExpected, accessible.RefStateSet().ContainsState (Atk.StateType.Editable), "Editable state when disabled");
			EnableWidget (accessible);
			Assert.IsTrue (accessible.RefStateSet().ContainsState (Atk.StateType.Editable), "Non-ReadOnly element should have Editable state");
		}

		protected Atk.Object InterfaceText (BasicWidgetType type, bool onlySingleLine)
		{
			return InterfaceTextAux (type, onlySingleLine, null, null);
		}

		protected void InterfaceText (BasicWidgetType type, Atk.Object accessible)
		{
			InterfaceTextAux (type, false, accessible, null);
		}

		protected Atk.Object InterfaceText (BasicWidgetType type, bool onlySingleLine, object widget)
		{
			return InterfaceTextAux (type, onlySingleLine, null, widget);
		}
		
		protected void InterfaceText (BasicWidgetType type, bool onlySingleLine, Atk.Object accessible)
		{
			InterfaceTextAux (type, onlySingleLine, accessible, null);
		}
		
		private Atk.Object InterfaceTextAux (BasicWidgetType type, bool onlySingleLine, Atk.Object accessible, object widget)
		{
			int startOffset, endOffset;
			string expected;
			string name = simpleTestText;

			Atk.Text atkText = null;

			RunInGuiThread (delegate () {
				if (accessible == null) {
					accessible = GetAccessible (type, name, widget);
				}
				atkText = CastToAtkInterface <Atk.Text> (accessible);
			});

			if (Misc.HasReadOnlyText (type))
				Assert.AreEqual (name, accessible.Name, "accessible.Name");
			else
				Assert.IsNull (accessible.Name, "accessible.Name");
			
			int caret = 0;
			if ((type == BasicWidgetType.TextBoxView || type == BasicWidgetType.RichTextBox) && TextBoxCaretInitiallyAtEnd)
				caret = name.Length;

			char passwordChar = '●';
			string passwordString = new String ('●', name.Length);
			
			Assert.AreEqual (caret, atkText.CaretOffset, "CaretOffset SL");
			Assert.AreEqual (name.Length, atkText.CharacterCount, "CharacterCount SL");
			if (type != BasicWidgetType.PasswordCharTextBoxEntry) {
				Assert.AreEqual (String.Empty + name [0], String.Empty + atkText.GetCharacterAtOffset (0), "GetCharacterAtOffset SL");
				Assert.AreEqual (name, atkText.GetText (0, name.Length), "GetText SL");
			} else {
				Assert.AreEqual (passwordChar, atkText.GetCharacterAtOffset (0), "GetCharacterAtOffset SL MTB");
				Assert.AreEqual (passwordString, atkText.GetText (0, name.Length), "GetText SL");
			}


			int highCaretOffset = 15;
			//any value (beware, this may change when this is fixed: http://bugzilla.gnome.org/show_bug.cgi?id=556453 )
			bool expectTrue = (type == BasicWidgetType.PasswordCharTextBoxEntry
			                   || type == BasicWidgetType.RichTextBox);
			Assert.AreEqual (!Misc.HasReadOnlyText (type) || expectTrue, 
			                 atkText.SetCaretOffset (-1), "SetCaretOffset#1 SL");
			Assert.AreEqual (!Misc.HasReadOnlyText (type) || expectTrue,
			                 atkText.SetCaretOffset (0), "SetCaretOffset#2 SL");
			Assert.AreEqual (!Misc.HasReadOnlyText (type) || expectTrue,
			                 atkText.SetCaretOffset (1), "SetCaretOffset#3 SL");
			Assert.AreEqual (!Misc.HasReadOnlyText (type) || expectTrue,
			                 atkText.SetCaretOffset (highCaretOffset), "SetCaretOffset#4 SL");
			
			// don't do this until bug#393565 is fixed:
			//Assert.AreEqual (typeof(Atk.TextAttribute), atkText.DefaultAttributes[0].GetType());

			int nSelections = -1;
			if ((type == BasicWidgetType.Label) || 
			    (type == BasicWidgetType.ToolStripLabel) ||
			    (type == BasicWidgetType.TextBoxEntry) ||
			    (type == BasicWidgetType.TextBoxView) ||
			    (type == BasicWidgetType.RichTextBox) ||
			    (type == BasicWidgetType.PasswordCharTextBoxEntry))
				nSelections = 0;
			
			Assert.AreEqual (nSelections, atkText.NSelections, "NSelections#1 SL");

			int caretOffset = 0;
			if (!Misc.HasReadOnlyText (type)
			    || type == BasicWidgetType.PasswordCharTextBoxEntry
			    || type == BasicWidgetType.RichTextBox)
				caretOffset = highCaretOffset;
			
			// you cannot select a label AFAIK so, all zeroes returned!
			Assert.AreEqual (null, atkText.GetSelection (0, out startOffset, out endOffset), "GetSelection#1 SL");
			Assert.AreEqual (caretOffset, startOffset, "GetSelection#2 SL");
			Assert.AreEqual (caretOffset, endOffset, "GetSelection#3 SL");
			Assert.AreEqual (null, atkText.GetSelection (1, out startOffset, out endOffset), "GetSelection#4 SL");
			Assert.AreEqual (caretOffset, startOffset, "GetSelection#5 SL");
			Assert.AreEqual (caretOffset, endOffset, "GetSelection#6 SL");
			//yes, it's wierd that we get valid offsets when calling with an OOR int, blame gail! I'm lazy to file a bug
			Assert.AreEqual (null, atkText.GetSelection (-1, out startOffset, out endOffset), "GetSelection#7 SL");
			Assert.AreEqual (caretOffset, startOffset, "GetSelection#8 SL");
			Assert.AreEqual (caretOffset, endOffset, "GetSelection#9 SL");
			
			// you cannot select a label AFAIK so, false always returned!
			Assert.AreEqual (false, atkText.SetSelection (0, 1, 2), "SetSelection#1 SL");
			// test GetSelection *after* SetSelection
			Assert.AreEqual (null, atkText.GetSelection (0, out startOffset, out endOffset), "GetSelection#10 SL");
			Assert.AreEqual (caretOffset, startOffset, "GetSelection#11 SL");
			Assert.AreEqual (caretOffset, endOffset, "GetSelection#12 SL");
			//test crazy numbers for SetSelection
			Assert.AreEqual (false, atkText.SetSelection (-3, 10, -2), "SetSelection#2 SL");
			Assert.AreEqual (null, atkText.GetSelection (0, out startOffset, out endOffset), "GetSelection#13 SL");
			Assert.AreEqual (caretOffset, startOffset, "GetSelection#14 SL");
			Assert.AreEqual (caretOffset, endOffset, "GetSelection#15 SL");
			
			//did NSelections changed?
			Assert.AreEqual (false, atkText.SetSelection (1, 2, 3), "SetSelection#3 SL");
			Assert.AreEqual (nSelections, atkText.NSelections, "NSelections#2 SL");
			Assert.AreEqual (false, atkText.RemoveSelection (0), "RemoveSelection#1 SL");
			Assert.AreEqual (nSelections, atkText.NSelections, "NSelections#3 SL");
			Assert.AreEqual (false, atkText.RemoveSelection (1), "RemoveSelection#2 SL");
			Assert.AreEqual (nSelections, atkText.NSelections, "NSelections#4 SL");
			Assert.AreEqual (false, atkText.RemoveSelection (-1), "RemoveSelection#3 SL");
			Assert.AreEqual (nSelections, atkText.NSelections, "NSelections#5 SL");



			
			//IMPORTANT NOTE about GetText*Offset methods [GetTextAtOffset(),GetTextAfterOffset(),GetTextBeforeOffset()]:
			//in Gail, they all return null if GetText() has not been called yet, however we may
			//prefer not to follow this wierd behaviour in the bridge
			
			//GetTextAtOffset
			expected = " test";

			int startCaretOffset = name.IndexOf (expected);
			int endCaretOffset = name.IndexOf (expected) + expected.Length;
			if (type == BasicWidgetType.PasswordCharTextBoxEntry) {
				expected = passwordString;
				startCaretOffset = 0;
				endCaretOffset = name.Length;
			}
			
			Assert.AreEqual (expected, 
				atkText.GetTextAtOffset (12, Atk.TextBoundary.WordEnd, out startOffset, out endOffset),
				"GetTextAtOffset,WordEnd SL");
			Assert.AreEqual (startCaretOffset, startOffset, "GetTextAtOffset,WordEnd,so SL");
			Assert.AreEqual (endCaretOffset, endOffset, "GetTextAtOffset,WordEnd,eo SL");

			if (!Misc.HasReadOnlyText (type)) {
				startCaretOffset = highCaretOffset;
				endCaretOffset = highCaretOffset;
			}
			
			//test selections after obtaining text with a different API than GetText
			Assert.AreEqual (nSelections, atkText.NSelections, "NSelections#6 SL");
			//NSelections == 0, however we have one selection, WTF?:
			Assert.AreEqual (null, atkText.GetSelection (0, out startOffset, out endOffset), "GetSelection#16 SL");
			
			Assert.AreEqual (startCaretOffset, startOffset, 
			                 "GetSelection#17 SL, we got: " + startOffset + ", should be: "  + highCaretOffset);
			Assert.AreEqual (endCaretOffset, endOffset, "GetSelection#18 SL");
			Assert.AreEqual (null, atkText.GetSelection (1, out startOffset, out endOffset), "GetSelection#19 SL");
			Assert.AreEqual (startCaretOffset, startOffset, "GetSelection#20 SL");
			Assert.AreEqual (endCaretOffset, endOffset, "GetSelection#21 SL");
			Assert.AreEqual (null, atkText.GetSelection (30, out startOffset, out endOffset), "GetSelection#22 SL");
			Assert.AreEqual (startCaretOffset, startOffset, "GetSelection#23 SL");
			Assert.AreEqual (endCaretOffset, endOffset, "GetSelection#24 SL");
			Assert.AreEqual (null, atkText.GetSelection (-1, out startOffset, out endOffset), "GetSelection#25 SL");
			Assert.AreEqual (startCaretOffset, startOffset, "GetSelection#26 SL");
			Assert.AreEqual (endCaretOffset, endOffset, "GetSelection#27 SL");
			
			Assert.AreEqual (false, atkText.SetSelection (0, 0, 0), "SetSelection#3 SL");
			Assert.AreEqual (null, atkText.GetSelection (0, out startOffset, out endOffset), "GetSelection#28 SL");
			Assert.AreEqual (startCaretOffset, startCaretOffset, "GetSelection#29 SL");
			Assert.AreEqual (endCaretOffset, endOffset, "GetSelection#30 SL");
			

			expected = "test ";

			startCaretOffset = name.IndexOf (expected);
			endCaretOffset = name.IndexOf (expected) + expected.Length;

			if (type == BasicWidgetType.PasswordCharTextBoxEntry) {
				expected = passwordString.Substring (0, 12);
				startCaretOffset = 12;
				endCaretOffset = passwordString.Length;
			}
			
			Assert.AreEqual (expected, 
				atkText.GetTextAtOffset (12, Atk.TextBoundary.WordStart, out startOffset, out endOffset),
				"GetTextAtOffset,WordStart SL");
			Assert.AreEqual (startCaretOffset, startOffset, "GetTextAtOffset,WordStart,so SL");
			Assert.AreEqual (endCaretOffset, endOffset, "GetTextAtOffset,WordStart,eo SL");

			expected = simpleTestText;
			startCaretOffset = name.IndexOf (expected);
			endCaretOffset = name.IndexOf (expected) + expected.Length;
			if (type == BasicWidgetType.PasswordCharTextBoxEntry) {
				expected = passwordString;
				startCaretOffset = 0;
			}

			Assert.AreEqual (expected,
				atkText.GetTextAtOffset (12, Atk.TextBoundary.LineEnd, out startOffset, out endOffset),
				"GetTextAtOffset,LineEnd SL");
			Assert.AreEqual (startCaretOffset, startOffset, "GetTextAtOffset,LineEnd,so SL");
			Assert.AreEqual (endCaretOffset, endOffset, "GetTextAtOffset,LineEnd,eo SL");

			Assert.AreEqual (expected,
				atkText.GetTextAtOffset (12, Atk.TextBoundary.LineStart, out startOffset, out endOffset),
				"GetTextAtOffset,LineStart SL");
			Assert.AreEqual (startCaretOffset, startOffset, "GetTextAtOffset,LineStart,so SL");
			Assert.AreEqual (endCaretOffset, endOffset, "GetTextAtOffset,LineStart,eo SL");
			
			Assert.AreEqual (expected,
				atkText.GetTextAtOffset (18, Atk.TextBoundary.SentenceEnd, out startOffset, out endOffset),
				"GetTextAtOffset,SentenceEnd SL");
			Assert.AreEqual (startCaretOffset, startOffset, "GetTextAtOffset,SentenceEnd,so SL");
			Assert.AreEqual (endCaretOffset, endOffset, "GetTextAtOffset,SentenceEnd,eo SL");

			Assert.AreEqual (expected,
				atkText.GetTextAtOffset (18, Atk.TextBoundary.SentenceStart, out startOffset, out endOffset),
				"GetTextAtOffset,SentenceStart SL");
			Assert.AreEqual (startCaretOffset, startOffset, "GetTextAtOffset,SentenceStart,so SL");
			Assert.AreEqual (endCaretOffset, endOffset, "GetTextAtOffset,SentenceStart,eo SL");

			expected = "t";
			if (type == BasicWidgetType.PasswordCharTextBoxEntry)
				expected = String.Empty + passwordChar;
			
			Assert.AreEqual (expected,
				atkText.GetTextAtOffset (18, Atk.TextBoundary.Char, out startOffset, out endOffset), "GetTextAtOffset,Char1 SL");
			Assert.AreEqual (18, startOffset, "GetTextAtOffset,Char1,so SL");
			Assert.AreEqual (19, endOffset, "GetTextAtOffset,Char1,eo SL");

			if (type != BasicWidgetType.PasswordCharTextBoxEntry)
				expected = ".";
			
			Assert.AreEqual (expected,
				atkText.GetTextAtOffset (23, Atk.TextBoundary.Char, out startOffset, out endOffset), "GetTextAtOffset,Char2 SL");
			Assert.AreEqual (23, startOffset, "GetTextAtOffset,Char2,so SL");
			Assert.AreEqual (24, endOffset, "GetTextAtOffset,Char2,eo SL");

			Assert.AreEqual (expected,
				atkText.GetTextAtOffset (name.Length - 1, Atk.TextBoundary.Char, out startOffset, out endOffset), "GetTextAtOffset,Char4 SL");
			Assert.AreEqual (name.Length - 1, startOffset, "GetTextAtOffset,Char4,so SL");
			Assert.AreEqual (name.Length, endOffset, "GetTextAtOffset,Char4,eo SL");
			
			
			if (type != BasicWidgetType.PasswordCharTextBoxEntry)
				expected = "e";
			
			Assert.AreEqual (expected,
				atkText.GetTextAtOffset (name.Length - 2, Atk.TextBoundary.Char, out startOffset, out endOffset), "GetTextAtOffset,Char3 SL");
			Assert.AreEqual (name.Length - 2, startOffset, "GetTextAtOffset,Char3,so SL");
			Assert.AreEqual (name.Length - 1, endOffset, "GetTextAtOffset,Char3,eo SL");
			

			Assert.AreEqual (String.Empty,
				atkText.GetTextAtOffset (name.Length, Atk.TextBoundary.Char, out startOffset, out endOffset), "GetTextAtOffset,Char5 SL");
			Assert.AreEqual (name.Length, startOffset, "GetTextAtOffset,Char5,so SL");
			Assert.AreEqual (name.Length, endOffset, "GetTextAtOffset,Char5,eo SL");
			Assert.AreEqual (String.Empty,
				atkText.GetTextAtOffset (-1, Atk.TextBoundary.Char, out startOffset, out endOffset), "GetTextAtOffset,Char6 SL");
			Assert.AreEqual (name.Length, startOffset, "GetTextAtOffset,Char6,so SL");
			Assert.AreEqual (name.Length, endOffset, "GetTextAtOffset,Char6,eo SL");


			//GetTextAfterOffset
			expected = " sentence";

			startCaretOffset = name.IndexOf (expected);
			endCaretOffset = name.IndexOf (expected) + expected.Length;
			
			if (type == BasicWidgetType.PasswordCharTextBoxEntry) {
				startCaretOffset = name.Length;
				endCaretOffset = startCaretOffset;
				expected = String.Empty;
			}
			
			Assert.AreEqual (expected, 
				atkText.GetTextAfterOffset (12, Atk.TextBoundary.WordEnd, out startOffset, out endOffset),
				"GetTextAfterOffset,WordEnd SL");
			Assert.AreEqual (startCaretOffset, startOffset, "GetTextAfterOffset,WordEnd,so SL");
			Assert.AreEqual (endCaretOffset, endOffset, "GetTextAfterOffset,WordEnd,eo SL");
			
			if (type != BasicWidgetType.PasswordCharTextBoxEntry) {
				expected = "sentence.";
				startCaretOffset = name.IndexOf (expected);
				endCaretOffset = name.IndexOf (expected) + expected.Length;
			}
			
			Assert.AreEqual (expected, 
				atkText.GetTextAfterOffset (12, Atk.TextBoundary.WordStart, out startOffset, out endOffset),
				"GetTextAfterOffset,WordStart SL");
			Assert.AreEqual (startCaretOffset, startOffset, "GetTextAfterOffset,WordStart,so SL");
			Assert.AreEqual (endCaretOffset, endOffset, "GetTextAfterOffset,WordStart,eo SL");

			TextSelection (type, atkText, name);

			if (onlySingleLine)
				return accessible;

			name = "This is a test sentence.\r\nSecond line. Other phrase.\nThird line?";

			RunInGuiThread (delegate () {
				accessible = GetAccessible (type, name, widget);
				atkText = CastToAtkInterface <Atk.Text> (accessible);
			});

			System.Threading.Thread.Sleep (1000);

			if (Misc.HasReadOnlyText (type) && type != BasicWidgetType.RichTextBox)
				Assert.AreEqual (name, accessible.Name, "accessible.Name");
			else
				Assert.IsNull (accessible.Name, "accessible.Name");
			
			Assert.AreEqual (name.Length, atkText.CharacterCount, "CharacterCount");
			Assert.AreEqual (String.Empty + name [0], String.Empty + atkText.GetCharacterAtOffset (0), "GetCharacterAtOffset");
			Assert.AreEqual (name, atkText.GetText (0, name.Length), "GetText");

			//IMPORTANT NOTE about GetText*Offset methods [GetTextAtOffset(),GetTextAfterOffset(),GetTextBeforeOffset()]:
			//in Gail, they all return null if GetText() has not been called yet, however we may
			//prefer not to follow this wierd behaviour in the bridge
			
			//GetTextAtOffset
			expected = " test";
			Assert.AreEqual (expected, 
				atkText.GetTextAtOffset (12, Atk.TextBoundary.WordEnd, out startOffset, out endOffset),
				"GetTextAtOffset,WordEnd");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextAtOffset,WordEnd,so");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextAtOffset,WordEnd,eo");
			
			int indexStartOffset = name.IndexOf (expected);
			int indexEndOffset = name.IndexOf (expected) + expected.Length;

			if (!Misc.HasReadOnlyText (type) || type == BasicWidgetType.RichTextBox) {
				int pos = (TextBoxCaretInitiallyAtEnd? name.Length: 0);
				indexStartOffset = pos;
				indexEndOffset = pos;
			}
			
			//test selections after obtaining text with a different API than GetText
			Assert.AreEqual (nSelections, atkText.NSelections, "NSelections#6");
			//NSelections == 0, however we have one selection, WTF?:
			Assert.AreEqual (null, atkText.GetSelection (0, out startOffset, out endOffset), "GetSelection#16");
			Assert.AreEqual (indexStartOffset, startOffset, "GetSelection#17");
			Assert.AreEqual (indexEndOffset, endOffset, "GetSelection#18");
			Assert.AreEqual (null, atkText.GetSelection (1, out startOffset, out endOffset), "GetSelection#19");
			Assert.AreEqual (indexStartOffset, startOffset, "GetSelection#20");
			Assert.AreEqual (indexEndOffset, endOffset, "GetSelection#21");
			Assert.AreEqual (null, atkText.GetSelection (30, out startOffset, out endOffset), "GetSelection#22");
			Assert.AreEqual (indexStartOffset, startOffset, "GetSelection#23");
			Assert.AreEqual (indexEndOffset, endOffset, "GetSelection#24");
			Assert.AreEqual (null, atkText.GetSelection (-1, out startOffset, out endOffset), "GetSelection#25");
			Assert.AreEqual (indexStartOffset, startOffset, "GetSelection#26");
			Assert.AreEqual (indexEndOffset, endOffset, "GetSelection#27");
			
			Assert.AreEqual (false, atkText.SetSelection (0, 0, 0), "SetSelection#3");
			Assert.AreEqual (null, atkText.GetSelection (0, out startOffset, out endOffset), "GetSelection#28");
			Assert.AreEqual (indexStartOffset, startOffset, "GetSelection#29");
			Assert.AreEqual (indexEndOffset, endOffset, "GetSelection#30");
			
			
			expected = "test ";
			Assert.AreEqual (expected, 
				atkText.GetTextAtOffset (12, Atk.TextBoundary.WordStart, out startOffset, out endOffset),
				"GetTextAtOffset,WordStart");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextAtOffset,WordStart,so");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextAtOffset,WordStart,eo");
			

			expected = "This is a test sentence.";
			Assert.AreEqual (expected,
				atkText.GetTextAtOffset (12, Atk.TextBoundary.LineEnd, out startOffset, out endOffset),
				"GetTextAtOffset,LineEnd");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextAtOffset,LineEnd,so");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextAtOffset,LineEnd,eo");

			expected = "This is a test sentence.\r\n";
			Assert.AreEqual (expected,
				atkText.GetTextAtOffset (12, Atk.TextBoundary.LineStart, out startOffset, out endOffset),
				"GetTextAtOffset,LineStart");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextAtOffset,LineStart,so");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextAtOffset,LineStart,eo");
			
			expected = "This is a test sentence.";
			Assert.AreEqual (expected,
				atkText.GetTextAtOffset (18, Atk.TextBoundary.SentenceEnd, out startOffset, out endOffset),
				"GetTextAtOffset,SentenceEnd");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextAtOffset,SentenceEnd,so");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextAtOffset,SentenceEnd,eo");
			
			expected = "This is a test sentence.\r\n";
			Assert.AreEqual (expected,
				atkText.GetTextAtOffset (18, Atk.TextBoundary.SentenceStart, out startOffset, out endOffset),
				"GetTextAtOffset,SentenceStart");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextAtOffset,SentenceStart,so");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextAtOffset,SentenceStart,eo");
			
			Assert.AreEqual ("t",
				atkText.GetTextAtOffset (18, Atk.TextBoundary.Char, out startOffset, out endOffset), "GetTextAtOffset,Char1");
			Assert.AreEqual (18, startOffset, "GetTextAtOffset,Char1,so");
			Assert.AreEqual (19, endOffset, "GetTextAtOffset,Char1,eo");
			Assert.AreEqual (".",
				atkText.GetTextAtOffset (23, Atk.TextBoundary.Char, out startOffset, out endOffset), "GetTextAtOffset,Char2");
			Assert.AreEqual (23, startOffset, "GetTextAtOffset,Char2,so");
			Assert.AreEqual (24, endOffset, "GetTextAtOffset,Char2,eo");
			Assert.AreEqual ("e",
				atkText.GetTextAtOffset (name.Length - 2, Atk.TextBoundary.Char, out startOffset, out endOffset), "GetTextAtOffset,Char3");
			Assert.AreEqual (name.Length - 2, startOffset, "GetTextAtOffset,Char3,so");
			Assert.AreEqual (name.Length - 1, endOffset, "GetTextAtOffset,Char3,eo");
			Assert.AreEqual ("?",
				atkText.GetTextAtOffset (name.Length - 1, Atk.TextBoundary.Char, out startOffset, out endOffset), "GetTextAtOffset,Char4");
			Assert.AreEqual (name.Length - 1, startOffset, "GetTextAtOffset,Char4,so");
			Assert.AreEqual (name.Length, endOffset, "GetTextAtOffset,Char4,eo");
			Assert.AreEqual (String.Empty,
				atkText.GetTextAtOffset (name.Length, Atk.TextBoundary.Char, out startOffset, out endOffset), "GetTextAtOffset,Char5");
			Assert.AreEqual (name.Length, startOffset, "GetTextAtOffset,Char5,so");
			Assert.AreEqual (name.Length, endOffset, "GetTextAtOffset,Char5,eo");
			Assert.AreEqual (String.Empty,
				atkText.GetTextAtOffset (-1, Atk.TextBoundary.Char, out startOffset, out endOffset), "GetTextAtOffset,Char6");
			Assert.AreEqual (name.Length, startOffset, "GetTextAtOffset,Char6,so");
			Assert.AreEqual (name.Length, endOffset, "GetTextAtOffset,Char6,eo");

			//GetTextAfterOffset
			expected = " sentence";
			Assert.AreEqual (expected, 
				atkText.GetTextAfterOffset (12, Atk.TextBoundary.WordEnd, out startOffset, out endOffset),
				"GetTextAfterOffset,WordEnd");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextAfterOffset,WordEnd,so");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextAfterOffset,WordEnd,eo");
			
			expected = "sentence.\r\n";
			Assert.AreEqual (expected, 
				atkText.GetTextAfterOffset (12, Atk.TextBoundary.WordStart, out startOffset, out endOffset),
				"GetTextAfterOffset,WordStart");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextAfterOffset,WordStart,so");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextAfterOffset,WordStart,eo");
			
			expected = "\r\nSecond line. Other phrase.";
			Assert.AreEqual (expected,
				atkText.GetTextAfterOffset (12, Atk.TextBoundary.LineEnd, out startOffset, out endOffset),
				"GetTextAfterOffset,LineEnd");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextAfterOffset,LineEnd,so");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextAfterOffset,LineEnd,eo");

			expected = "Second line. Other phrase.\n";
			Assert.AreEqual (expected,
				atkText.GetTextAfterOffset (12, Atk.TextBoundary.LineStart, out startOffset, out endOffset),
				"GetTextAfterOffset,LineStart");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextAfterOffset,LineStart,so");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextAfterOffset,LineStart,eo");
			
			expected = "\r\nSecond line.";
			Assert.AreEqual (expected,
				atkText.GetTextAfterOffset (18, Atk.TextBoundary.SentenceEnd, out startOffset, out endOffset),
				"GetTextAfterOffset,SentenceEnd");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextAfterOffset,SentenceEnd,so");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextAfterOffset,SentenceEnd,eo");

			expected = " Other phrase.";
			Assert.AreEqual (expected,
				atkText.GetTextAfterOffset (24, Atk.TextBoundary.SentenceEnd, out startOffset, out endOffset),
				"GetTextAfterOffset,SentenceEnd");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextAfterOffset,SentenceEnd,so");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextAfterOffset,SentenceEnd,eo");
			
			expected = "Second line. ";
			Assert.AreEqual (expected,
				atkText.GetTextAfterOffset (18, Atk.TextBoundary.SentenceStart, out startOffset, out endOffset),
				"GetTextAfterOffset,SentenceStart");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextAfterOffset,SentenceStart,so");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextAfterOffset,SentenceStart,eo");
			
			Assert.AreEqual ("e",
				atkText.GetTextAfterOffset (18, Atk.TextBoundary.Char, out startOffset, out endOffset),
				"GetTextAfterOffset,Char");
			Assert.AreEqual (19, startOffset, "GetTextAfterOffset,Char,so");
			Assert.AreEqual (20, endOffset, "GetTextAfterOffset,Char,eo");
			Assert.AreEqual ("\r",
				atkText.GetTextAfterOffset (23, Atk.TextBoundary.Char, out startOffset, out endOffset),
				"GetTextAfterOffset,Char");
			Assert.AreEqual (24, startOffset, "GetTextAfterOffset,Char,so");
			Assert.AreEqual (25, endOffset, "GetTextAfterOffset,Char,eo");
			Assert.AreEqual ("?",
				atkText.GetTextAfterOffset (name.Length - 2, Atk.TextBoundary.Char, out startOffset, out endOffset),
				"GetTextAfterOffset,Char");
			Assert.AreEqual (name.Length - 1, startOffset, "GetTextAfterOffset,Char,so");
			Assert.AreEqual (name.Length, endOffset, "GetTextAfterOffset,Char,eo");
			Assert.AreEqual (String.Empty,
				atkText.GetTextAfterOffset (name.Length - 1, Atk.TextBoundary.Char, out startOffset, out endOffset),
				"GetTextAfterOffset,Char");
			Assert.AreEqual (name.Length, startOffset, "GetTextAfterOffset,Char,so");
			Assert.AreEqual (name.Length, endOffset, "GetTextAfterOffset,Char,eo");
			Assert.AreEqual (String.Empty,
				atkText.GetTextAfterOffset (name.Length, Atk.TextBoundary.Char, out startOffset, out endOffset),
				"GetTextAfterOffset,Char");
			Assert.AreEqual (name.Length, startOffset, "GetTextAfterOffset,Char,so");
			Assert.AreEqual (name.Length, endOffset, "GetTextAfterOffset,Char,eo");
			Assert.AreEqual (null,
				atkText.GetTextAfterOffset (-1, Atk.TextBoundary.Char, out startOffset, out endOffset),
				"GetTextAfterOffset,Char");
			Assert.AreEqual (name.Length, startOffset, "GetTextAfterOffset,Char,so");
			Assert.AreEqual (name.Length, endOffset, "GetTextAfterOffset,Char,eo");
			
			
			//GetTextBeforeOffset
			expected = " a";
			Assert.AreEqual (expected, 
				atkText.GetTextBeforeOffset (12, Atk.TextBoundary.WordEnd, out startOffset, out endOffset),
				"GetTextBeforeOffset,WordEnd");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextBeforeOffset,WordEnd,so");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextBeforeOffset,WordEnd,eo");
			
			expected = "a ";
			Assert.AreEqual (expected, 
				atkText.GetTextBeforeOffset (12, Atk.TextBoundary.WordStart, out startOffset, out endOffset),
				"GetTextBeforeOffset,WordStart");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextBeforeOffset,WordStart,so");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextBeforeOffset,WordStart,eo");
			
			expected = String.Empty;
			
			Assert.AreEqual (expected,
				atkText.GetTextBeforeOffset (12, Atk.TextBoundary.LineEnd, out startOffset, out endOffset),
				"GetTextBeforeOffset,LineEnd");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextBeforeOffset,LineEnd,so");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextBeforeOffset,LineEnd,eo");

			Assert.AreEqual (expected,
				atkText.GetTextBeforeOffset (12, Atk.TextBoundary.LineStart, out startOffset, out endOffset),
				"GetTextBeforeOffset,LineStart");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextBeforeOffset,LineStart,so");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextBeforeOffset,LineStart,eo");
			
			Assert.AreEqual (expected,
				atkText.GetTextBeforeOffset (18, Atk.TextBoundary.SentenceEnd, out startOffset, out endOffset),
				"GetTextBeforeOffset,SentenceEnd");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextBeforeOffset,SentenceEnd,so");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextBeforeOffset,SentenceEnd,eo");
			
			Assert.AreEqual (expected,
				atkText.GetTextBeforeOffset (18, Atk.TextBoundary.SentenceStart, out startOffset, out endOffset),
				"GetTextBeforeOffset,SentenceStart");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextBeforeOffset,SentenceStart,so");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextBeforeOffset,SentenceStart,eo");
			
			Assert.AreEqual ("n",
				atkText.GetTextBeforeOffset (18, Atk.TextBoundary.Char, out startOffset, out endOffset),
				"GetTextBeforeOffset,Char");
			Assert.AreEqual (17, startOffset, "GetTextBeforeOffset,Char,so");
			Assert.AreEqual (18, endOffset, "GetTextBeforeOffset,Char,eo");
			Assert.AreEqual ("e",
				atkText.GetTextBeforeOffset (23, Atk.TextBoundary.Char, out startOffset, out endOffset));
			Assert.AreEqual (22, startOffset, "GetTextBeforeOffset,Char,so");
			Assert.AreEqual (23, endOffset, "GetTextBeforeOffset,Char,eo");
			Assert.AreEqual ("n",
				atkText.GetTextBeforeOffset (name.Length - 2, Atk.TextBoundary.Char, out startOffset, out endOffset));
			Assert.AreEqual (name.Length - 3, startOffset, "GetTextBeforeOffset,Char,so");
			Assert.AreEqual (name.Length - 2, endOffset, "GetTextBeforeOffset,Char,eo");
			Assert.AreEqual ("e",
				atkText.GetTextBeforeOffset (name.Length - 1, Atk.TextBoundary.Char, out startOffset, out endOffset));
			Assert.AreEqual (name.Length - 2, startOffset, "GetTextBeforeOffset,Char,so");
			Assert.AreEqual (name.Length - 1, endOffset, "GetTextBeforeOffset,Char,eo");
			Assert.AreEqual ("?",
				atkText.GetTextBeforeOffset (name.Length, Atk.TextBoundary.Char, out startOffset, out endOffset));
			Assert.AreEqual (name.Length - 1, startOffset, "GetTextBeforeOffset,Char,so");
			Assert.AreEqual (name.Length, endOffset, "GetTextBeforeOffset,Char,eo");
			Assert.AreEqual (null,
				atkText.GetTextBeforeOffset (-1, Atk.TextBoundary.Char, out startOffset, out endOffset));
			Assert.AreEqual (name.Length - 1, startOffset, "GetTextBeforeOffset,Char,so");
			Assert.AreEqual (name.Length, endOffset, "GetTextBeforeOffset,Char,eo");
			
			
			
			
			name = "Tell me; here a sentence\r\nwith EOL but without dot, and other phrase... Heh!";

			accessible = GetAccessible (type, name, widget);
			atkText = CastToAtkInterface <Atk.Text> (accessible);
			Assert.AreEqual (name, atkText.GetText(0, name.Length), "GetText#2");
			
			expected = "\r\nwith EOL but without dot, and other phrase...";
			Assert.AreEqual (expected,
				atkText.GetTextAfterOffset (3, Atk.TextBoundary.SentenceEnd, out startOffset, out endOffset),
				"GetTextAfterOffset,SentenceEnd");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextAfterOffset,SentenceEnd,so");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextAfterOffset,SentenceEnd,eo");
			
			expected = "Tell me; here a sentence\r\n";
			Assert.AreEqual (expected,
				atkText.GetTextAtOffset (4, Atk.TextBoundary.SentenceStart, out startOffset, out endOffset),
				"GetTextAtOffset,SentenceStart");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextAtOffset,SentenceStart,so");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextAtOffset,SentenceStart,eo");
			
			expected = "Tell me; here a sentence";
			Assert.AreEqual (expected,
				atkText.GetTextAtOffset (4, Atk.TextBoundary.SentenceEnd, out startOffset, out endOffset),
				"GetTextAtOffset,SentenceEnd");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextAtOffset,SentenceEnd,so");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextAtOffset,SentenceEnd,eo");
			
			
			return accessible;
		}

		protected void TextSelection (BasicWidgetType type, Atk.Object accessible, string name)
		{
			Atk.Text atkText = CastToAtkInterface<Atk.Text> (accessible);
			TextSelection (type, atkText, name);
		}

		protected void TextSelection (BasicWidgetType type, Atk.Text atkText, string name)
		{
			if (name.Length < 5)
				throw new Misc.UntestableException ("String not long enough to test.");

			bool supportsSelection = (!Misc.HasReadOnlyText (type) || type == BasicWidgetType.DomainUpDown);
			if (!supportsSelection)
				name = null;
			
			RunInGuiThread (delegate () {
				int startOffset, endOffset;
				
				Assert.AreEqual (supportsSelection, atkText.AddSelection (1, 3), "AddSelection, we should get:" + supportsSelection);
				int nSelections = supportsSelection ? 1 : 
					(type == BasicWidgetType.Label || type == BasicWidgetType.ToolStripLabel ? 0 : -1);
				Assert.AreEqual (nSelections, atkText.NSelections, "NSelections after AddSelect, we should get: " + nSelections);
				Assert.AreEqual (name == null ? null : name.Substring (1, 2), atkText.GetSelection (0, out startOffset, out endOffset), "getSelection after addSelection");
				Assert.AreEqual (supportsSelection ? 1 : 0, startOffset, "startOffset, we got: " + startOffset);
				Assert.AreEqual (supportsSelection ? 3 : 0, endOffset, "endOffset, we got: " + endOffset);
				
				StartEventMonitor ();
				Assert.AreEqual (supportsSelection, atkText.SetSelection (0, 2, 4), "AddSelection#2");
				// Ideally, we would only send one
				// event, but that would be hard with
				// the current design, so allowing 2
				int min = 1, max = 2;
				if (!supportsSelection || type == BasicWidgetType.PasswordCharTextBoxEntry) {
					min = 0;
					max = 0;
				}
				ExpectEvents (min, max, GetRole (type), "object:text-selection-changed");
				
				Assert.AreEqual (name == null ? null : name.Substring (2, 2), atkText.GetSelection (0, out startOffset, out endOffset), "getSelection after addSelection");
				Assert.AreEqual (supportsSelection ? 2 : 0, startOffset, "startOffset");
				Assert.AreEqual (supportsSelection ? 4 : 0, endOffset, "endOffset");
			});
		}

		// Simpler text test with a variable string
		protected void InterfaceText (Atk.Object accessible, string text)
		{
			Atk.Text atkText = null;
			RunInGuiThread (delegate () {
				atkText = CastToAtkInterface <Atk.Text> (accessible);
			});

			int length = text.Length;
			Assert.AreEqual (text, atkText.GetText (0, -1), "GetText");
			Assert.AreEqual (length, atkText.CharacterCount, "Character count");
			for (int i = 0; i < length; i++)
				Assert.AreEqual (text [i], atkText.GetCharacterAtOffset (i), "GetCharacterAtOffset" + i);

			if (accessible.Role == Atk.Role.SpinButton)
				TextSelection (BasicWidgetType.DomainUpDown, atkText, text);
		}

		

		protected string simpleTestText = "This is a test sentence.";

		protected void Parent (BasicWidgetType type, Atk.Object accessible)
		{
			Relations (accessible, type);
			
			Atk.Object parent = accessible.Parent;
			Assert.IsNotNull (parent, "parent not null");
			if (type == BasicWidgetType.Window)
				Assert.AreEqual (parent.Role, Atk.Role.Application, "Parent of a frame should be an application");

			Assert.AreEqual (parent.RefStateSet().ContainsState (Atk.StateType.ManagesDescendants),
			  accessible.RefStateSet().ContainsState (Atk.StateType.Transient),
			  "Transient state should match parent's ManagesDescendants state");

			int count = parent.NAccessibleChildren;
			for (int i = 0; i < count; i++)
				if (parent.RefAccessibleChild (i) == accessible)
					return;
			Assert.Fail ("Object should be child of parent");
		}

		private static Type [] atkTypes = 
		  new Type [] { 
			typeof (Atk.Action) ,
			typeof (Atk.Component) ,
			typeof (Atk.EditableText) ,
			typeof (Atk.Image) ,
			typeof (Atk.Table) ,
			typeof (Atk.Text) ,
			typeof (Atk.Selection) ,
			typeof (Atk.Value) };
		
		protected void Interfaces (Atk.Object accessible, params Type [] expected)
		{
			var expectedTypes = new List <Type> (expected);
			var missingTypes = new List <Type> ();
			var superfluousTypes = new List <Type> ();
			
			foreach (Type t in atkTypes) {
				object o = CastToAtkInterface (t, accessible);
				if (expectedTypes.Contains (t) && o == null)
					missingTypes.Add (t);
				else if ((!expectedTypes.Contains (t)) && o != null)
					superfluousTypes.Add (t);
			}

			string missingTypesMsg = string.Empty;
			string superfluousTypesMsg = string.Empty;

			if (missingTypes.Count != 0) {
				missingTypesMsg = "Missing interfaces: ";
				foreach (Type type in missingTypes)
					missingTypesMsg += type.Name + ",";
			}
			if (superfluousTypes.Count != 0) {
				superfluousTypesMsg = "Superfluous interfaces: ";
				foreach (Type type in superfluousTypes)
					superfluousTypesMsg += type.Name + ",";
			}
			Assert.IsTrue ((missingTypes.Count == 0) && (superfluousTypes.Count == 0),
				missingTypesMsg + " .. " + superfluousTypesMsg);
		}

		protected void PrintStates (Atk.Object accessible)
		{
			string res = String.Empty;
			Atk.StateSet stateSet = accessible.RefStateSet ();
			foreach (Atk.StateType state in Enum.GetValues (typeof (Atk.StateType))) {
				if (stateSet.ContainsState (state))
					res += state.ToString () + ",";
			}
			Console.WriteLine ("Object of type " + accessible.Role.ToString () + 
			                   " contains the following states: " + res);
		}
			
		protected void Relations (Atk.Object accessible, BasicWidgetType type)
		{
			if (type != BasicWidgetType.RadioButton) {
				Assert.AreEqual (0, accessible.RefRelationSet ().NRelations, 
				                 "NRelations != 0, now " + accessible.RefRelationSet ().NRelations);
			} else {
				Assert.AreEqual (1, accessible.RefRelationSet ().NRelations, 
				                 "NRelations != 1, now " + accessible.RefRelationSet ().NRelations);
				Assert.AreEqual (accessible.RefRelationSet ().GetRelation (0).RelationType, Atk.RelationType.MemberOf);
			}
			
		}

		protected int GetExpectedIndexAtRowColumn (int row, int column, int numRows, int numCols)
		{
			if (row < 0 || column < 0)
				return -1;

			if (row >= numRows || column >= numCols)
				return -1;

			return (row * numCols) + column + 1;
		}

		protected void InterfaceTable (Atk.Table implementor, int expectedRows, int expectedCols,
		                               int expectedSelectedRows, int expectedSelectedColumns,
		                               bool supportsRowColExtents)
		{
			int numRows = implementor.NRows;
			int numCols = implementor.NColumns;

			Assert.AreEqual (expectedCols, numCols,
			                 "Incorrect number of columns");
			Assert.AreEqual (expectedRows, numRows,
			                 "Incorrect number of rows");

			Assert.IsNull (implementor.Caption, "Caption is not null");
			Assert.IsNull (implementor.Summary, "Summary is not null");

			Atk.Object test = new Misc.AtkTestObject ();
			implementor.Caption = test;
			Assert.AreEqual (implementor.Caption, test, "caption after set");

			implementor.Summary = test;
			Assert.AreEqual (implementor.Summary, test, "summary after set");

#if HAVE_OLD_ATK_SHARP
			int numSelected;
			Assert.AreEqual (expectedSelectedRows, 
			                 implementor.GetSelectedRows (out numSelected),
			                 "Incorrect number of selected rows");
			Assert.AreEqual (expectedSelectedRows, numSelected,
			                 "Incorrect number of selected rows in out parameter");
#else
			Assert.AreEqual (expectedSelectedRows, implementor.SelectedRows,
			                 "Incorrect number of selected rows");
#endif


#if HAVE_OLD_ATK_SHARP
			Assert.AreEqual (expectedSelectedColumns,
			                 implementor.GetSelectedColumns (out numSelected),
			                 "Incorrect number of selected columns");
			Assert.AreEqual (expectedSelectedColumns, numSelected,
			                 "Incorrect number of selected columns in out parameter");
#else
			Assert.AreEqual (expectedSelectedColumns, implementor.SelectedColumns,
			                 "Incorrect number of selected columns");
#endif



			for (int r = -1; r <= numRows; r++) {
				for (int c = -1; c <= numCols; c++) {
					int expectedIndex = GetExpectedIndexAtRowColumn (
						r, c, numRows, numCols);
					
					Assert.AreEqual (
						expectedIndex,
						implementor.GetIndexAt (r, c),
						String.Format ("Incorrect index at ({0}, {1})",
						               r, c)
					);

					int expectedRow = expectedIndex < 0 ? -1 : r;
					Assert.AreEqual (
						expectedRow,
						implementor.GetRowAtIndex (expectedIndex),
						String.Format ("Incorrect row at index {0}",
						               expectedIndex)
					);

					int expectedCol = expectedIndex < 0 ? 0 : c;
					Assert.AreEqual (
						expectedCol,
						implementor.GetColumnAtIndex (expectedIndex),
						String.Format ("Incorrect column at index {0}",
						               expectedIndex)
					);
					
					// FIXME: Remove this gross simplification
					int expectedExtents = 1;
					if (r < 0 || c < 0 || r >= numRows || c >= numCols)
						expectedExtents = -1;

					if (!supportsRowColExtents)
						expectedExtents = 0;

					Assert.AreEqual (
						expectedExtents,
						implementor.GetColumnExtentAt (r, c),
						String.Format ("Incorrect column extent at ({0}, {1})",
						               r, c)
					);

					Assert.AreEqual (
						expectedExtents,
						implementor.GetRowExtentAt (r, c),
						String.Format ("Incorrect row extent at ({0}, {1})",
						               r, c)
					);
				}
			}
		}
		
		protected void Relation (Atk.RelationType type, Atk.Object source, params Atk.Object [] expectedTarget)
		{
			RunInGuiThread (delegate () {
				Atk.RelationSet set = source.RefRelationSet ();
				Atk.Relation relation = set.GetRelationByType (type);
				Assert.IsNotNull (relation, "Relation (" + type + ")");
				Atk.Object [] target = relation.Target;
				foreach (Atk.Object obj in expectedTarget)
					Assert.IsTrue (Array.IndexOf (target, obj, 0) >= 0, "Missing relation target");
				foreach (Atk.Object obj in target)
					Assert.IsTrue (Array.IndexOf (expectedTarget, obj, 0) >= 0, "Superfluous relation target");
			});
		}

		protected Atk.Object FindObjectByName (Atk.Object parent, string name)
		{
			return FindObjectByName (parent, name, false);
		}

		protected Atk.Object FindObjectByName (Atk.Object parent, string name, bool recurse)
		{
			int count = parent.NAccessibleChildren;
			for (int i = 0; i < count; i++) {
				Atk.Object child = parent.RefAccessibleChild (i);
				if (child.Name == name)
					return child;
				if (recurse) {
					Atk.Object obj = FindObjectByName (child, name, true);
					if (obj != null)
						return obj;
				}
			}
			return null;
		}

		protected Atk.Object FindObjectByRole (Atk.Object parent, Atk.Role role)
		{
			return FindObjectByRole (parent, role, false);
		}

		protected Atk.Object FindObjectByRole (Atk.Object parent, Atk.Role role, bool recurse)
		{
			int count = parent.NAccessibleChildren;
			for (int i = 0; i < count; i++) {
				Atk.Object child = parent.RefAccessibleChild (i);
				if (child.Role == role)
					return child;
				if (recurse) {
					Atk.Object obj = FindObjectByRole (child, role, true);
					if (obj != null)
						return obj;
				}
			}
			return null;
		}

		protected string[] NamesFromTableXml (string text)
		{
			return NamesFromTableXml (text, 0);
		}

		protected string[] NamesFromTableXml (string text, int minDepth)
		{
			XmlDocument xml = new XmlDocument ();
			xml.LoadXml (text);
			List<string> list = new List<string> ();
			for (XmlNode child = xml.FirstChild; child != null; child = child.NextSibling)
				NamesFromTableXmlHelper (list, child, minDepth + 1, 0);
			return list.ToArray ();
		}

		private void NamesFromTableXmlHelper (List<string> list, XmlNode node, int minDepth, int curDepth)
		{
			for (XmlNode child = node.FirstChild; child != null; child = child.NextSibling)
				if (child.Name == "tr")
					NamesFromTableXmlHelper (list, child, minDepth, curDepth + 1);
				else if (child.Name == "td" && curDepth >= minDepth)
				list.Add (child.InnerText);
		}

		protected void Focus (Atk.Object accessible)
		{
			Focus (accessible, true);
		}

		protected void Focus (Atk.Object accessible, bool testState)
		{
			bool transient = (accessible.RefStateSet ().ContainsState (Atk.StateType.Transient));

			Atk.Component atkComponent = CastToAtkInterface<Atk.Component> (accessible);
			EventMonitor.Start ();
			RunInGuiThread (delegate () {
				atkComponent.GrabFocus ();
			});
			EventCollection events = EventMonitor.Pause ();
			int expectedCount = (transient ? 1 : 2);
			string evType = (transient? "object:active-descendant-changed": "object:state-changed:focused");
			Atk.Object focusedAccessible = (transient ? accessible.Parent: accessible);
			EventCollection evs = events.FindByRole (focusedAccessible.Role).FindByType (evType);
			string eventsInXml = String.Format (" events in XML: {0}", Environment.NewLine + events.OriginalGrossXml);
			Assert.AreEqual (expectedCount, evs.Count, "bad number of events expected!" + eventsInXml);
			if (testState)
				Assert.IsTrue (focusedAccessible.RefStateSet ().ContainsState (Atk.StateType.Focused), "List focused");
		}

		protected double GetMinimumValue (Atk.Value value)
		{
			GLib.Value gv = new GLib.Value (0);
			value.GetMinimumValue (ref gv);
			return double.Parse (gv.Val.ToString());
		}

		protected double GetMaximumValue (Atk.Value value)
		{
			GLib.Value gv = new GLib.Value (0);
			value.GetMaximumValue (ref gv);
			return double.Parse (gv.Val.ToString());
		}

		protected double GetCurrentValue (Atk.Value value)
		{
			GLib.Value gv = new GLib.Value (0);
			value.GetCurrentValue (ref gv);
			return double.Parse (gv.Val.ToString());
		}

		protected bool SetCurrentValue (Atk.Value value, double n)
		{
			GLib.Value gv = new GLib.Value (0);
			value.GetCurrentValue (ref gv);
			if (gv.Val is int)
				gv.Val = (int)n;
			else
				gv.Val = n;
			return value.SetCurrentValue (gv);
		}

		protected bool DoActionByName (Atk.Object accessible, string name)
		{
			Atk.Action atkAction = CastToAtkInterface<Atk.Action> (accessible);
			Assert.IsNotNull (atkAction,
			                  String.Format ("Accessible with Name = {0} and Role = {1} does not implement AtkAction.",
			                                 accessible.Name, accessible.Role));

			int nActions = atkAction.NActions;
			for (int i = 0; i < nActions; i++) {
				string actionName = atkAction.GetName (i);
				if (actionName == name)
					return atkAction.DoAction (i);
			}

			Assert.Fail (String.Format ("Couldn't find action {0} on {1}.  Type is {2}",
			                            name, accessible.Name, accessible.GetType ()));

			return false;	// keep the compiler happy
		}

		protected virtual void ExpandTreeView (Atk.Object accessible)
		{
			if (accessible == null)
				throw new ArgumentNullException ("accessible");

			int nChildren = accessible.NAccessibleChildren;
			for (int i = 0; i < nChildren; i++) {
				Atk.Object child = accessible.RefAccessibleChild (i);
				if (child.Role == Atk.Role.TableCell
				    && child.RefStateSet().ContainsState (Atk.StateType.Expandable)
				    && !child.RefStateSet().ContainsState (Atk.StateType.Expanded))
					DoActionByName (child, "expand or contract");
			}
		}

		protected virtual void CollapseTreeView (Atk.Object accessible)
		{
			int nChildren = accessible.NAccessibleChildren;
			for (int i = 0; i < nChildren; i++) {
				Atk.Object child = accessible.RefAccessibleChild (i);
				if (child.Role == Atk.Role.TableCell
				    && child.RefStateSet().ContainsState (Atk.StateType.Expandable)
				    && child.RefStateSet().ContainsState (Atk.StateType.Expanded)) 
					DoActionByName (child, "expand or contract");
			}
		}

		public abstract void RunInGuiThread (System.Action d);
	}
}
