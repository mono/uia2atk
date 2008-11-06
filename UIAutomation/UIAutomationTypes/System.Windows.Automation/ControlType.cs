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
//	  Sandy Armstrong <sanfordarmstrong@gmail.com>
// 

using System;

namespace System.Windows.Automation
{
	public class ControlType : AutomationIdentifier
	{
#region Private Fields
		
		private string localizedControlType;
		private AutomationPattern [] neverSupportedPatterns;
		private AutomationProperty [] requiredProperties;
		private AutomationPattern [][] requiredPatternSets;
		
#endregion
		
#region Internal Constructor
		
		internal ControlType (int id, string programmaticName) :
			base (id, programmaticName)
		{
		}
		
#endregion
		
#region Public Methods
		
		public AutomationPattern [] GetNeverSupportedPatterns ()
		{
			return neverSupportedPatterns;
		}
		
		public AutomationProperty [] GetRequiredProperties ()
		{
			return requiredProperties;
		}
		
		public AutomationPattern [][] GetRequiredPatternSets ()
		{
			return requiredPatternSets;
		}
		
#endregion
		
#region Public Properties
		
		public string LocalizedControlType
		{
			get {
				return localizedControlType;
			}
		}
		
#endregion
		
#region Internal Constants
		
		private const int ButtonId = 50000;
		private const int CalendarId = 50001;
		private const int CheckBoxId = 50002;
		private const int ComboBoxId = 50003;
		private const int CustomId = 50025;
		private const int DataGridId = 50028;
		private const int DataItemId = 50029;
		private const int DocumentId = 50030;
		private const int EditId = 50004;
		private const int GroupId = 50026;
		private const int HeaderItemId = 50035;
		private const int HeaderId = 50034;
		private const int HyperlinkId = 50005;
		private const int ImageId = 50006;
		private const int ListId = 50008;
		private const int ListItemId = 50007;
		private const int MenuId = 50009;
		private const int MenuBarId = 50010;
		private const int MenuItemId = 50011;
		private const int PaneId = 50033;
		private const int ProgressBarId = 50012;
		private const int RadioButtonId = 50013;
		private const int ScrollBarId = 50014;
		private const int SeparatorId = 50038;
		private const int SliderId = 50015;
		private const int SpinnerId = 50016;
		private const int SplitButtonId = 50031;
		private const int StatusBarId = 50017;
		private const int TabId = 50018;
		private const int TabItemId = 50019;
		private const int TableId = 50036;
		private const int TextId = 50020;
		private const int ThumbId = 50027;
		private const int TitleBarId = 50037;
		private const int ToolBarId = 50021;
		private const int ToolTipId = 50022;
		private const int TreeId = 50023;
		private const int TreeItemId = 50024;
		private const int WindowId = 50032;
		
#endregion
		
#region Static Members
		
		static ControlType ()
		{			
			Button = new ControlType (ButtonId, "ControlType.Button");
			Button.localizedControlType = "button";
			Button.neverSupportedPatterns = new AutomationPattern [] {};
			Button.requiredProperties = new AutomationProperty [] {};
			Button.requiredPatternSets = new AutomationPattern [] [] {
				new AutomationPattern [] {
					InvokePatternIdentifiers.Pattern}};
			
			Calendar = new ControlType (CalendarId, "ControlType.Calendar");
			Calendar.localizedControlType = "calendar";
			Calendar.neverSupportedPatterns = new AutomationPattern [] {};
			Calendar.requiredProperties = new AutomationProperty [] {};
			Calendar.requiredPatternSets = new AutomationPattern [] [] {
				new AutomationPattern [] {
					GridPatternIdentifiers.Pattern,
					ValuePatternIdentifiers.Pattern,
					SelectionPatternIdentifiers.Pattern}};
			
			CheckBox = new ControlType (CheckBoxId, "ControlType.CheckBox");
			CheckBox.localizedControlType = "check box";
			CheckBox.neverSupportedPatterns = new AutomationPattern [] {};
			CheckBox.requiredProperties = new AutomationProperty [] {};
			CheckBox.requiredPatternSets = new AutomationPattern [] [] {
				new AutomationPattern [] {
					TogglePatternIdentifiers.Pattern}};
			
			ComboBox = new ControlType (ComboBoxId, "ControlType.ComboBox");
			ComboBox.localizedControlType = "combo box";
			ComboBox.neverSupportedPatterns = new AutomationPattern [] {};
			ComboBox.requiredProperties = new AutomationProperty [] {};
			ComboBox.requiredPatternSets = new AutomationPattern [] [] {
				new AutomationPattern [] {
					SelectionPatternIdentifiers.Pattern,
					ExpandCollapsePatternIdentifiers.Pattern}};
			
			Custom = new ControlType (CustomId, "ControlType.Custom");
			Custom.localizedControlType = "custom";
			Custom.neverSupportedPatterns = new AutomationPattern [] {};
			Custom.requiredProperties = new AutomationProperty [] {};
			Custom.requiredPatternSets = new AutomationPattern [] [] {};
			
			DataGrid = new ControlType (DataGridId, "ControlType.DataGrid");
			DataGrid.localizedControlType = "datagrid";
			DataGrid.neverSupportedPatterns = new AutomationPattern [] {};
			DataGrid.requiredProperties = new AutomationProperty [] {};
			DataGrid.requiredPatternSets = new AutomationPattern [] [] {
				new AutomationPattern [] {
					SelectionPatternIdentifiers.Pattern},
				new AutomationPattern [] {
					GridPatternIdentifiers.Pattern},
				new AutomationPattern [] {
					TablePatternIdentifiers.Pattern}};
			
			DataItem = new ControlType (DataItemId, "ControlType.DataItem");
			DataItem.localizedControlType = "dataitem";
			DataItem.neverSupportedPatterns = new AutomationPattern [] {};
			DataItem.requiredProperties = new AutomationProperty [] {};
			DataItem.requiredPatternSets = new AutomationPattern [] [] {
				new AutomationPattern [] {
					SelectionItemPatternIdentifiers.Pattern}
			};
			
			Document = new ControlType (DocumentId, "ControlType.Document");
			Document.localizedControlType = "document";
			Document.neverSupportedPatterns = new AutomationPattern [] {
				ValuePatternIdentifiers.Pattern
			};
			Document.requiredProperties = new AutomationProperty [] {};
			Document.requiredPatternSets = new AutomationPattern [] [] {
				new AutomationPattern [] {
					ScrollPatternIdentifiers.Pattern},
				new AutomationPattern [] {
					TextPatternIdentifiers.Pattern}
			};
			
			Edit = new ControlType (EditId, "ControlType.Edit");
			Edit.localizedControlType = "edit";
			Edit.neverSupportedPatterns = new AutomationPattern [] {};
			Edit.requiredProperties = new AutomationProperty [] {};
			Edit.requiredPatternSets = new AutomationPattern [] [] {
				new AutomationPattern [] {
					ValuePatternIdentifiers.Pattern}
			};
			
			Group = new ControlType (GroupId, "ControlType.Group");
			Group.localizedControlType = "group";
			Group.neverSupportedPatterns = new AutomationPattern [] {};
			Group.requiredProperties = new AutomationProperty [] {};
			Group.requiredPatternSets = new AutomationPattern [] [] {};
			
			HeaderItem = new ControlType (HeaderItemId, "ControlType.HeaderItem");
			HeaderItem.localizedControlType = "header item";
			HeaderItem.neverSupportedPatterns = new AutomationPattern [] {};
			HeaderItem.requiredProperties = new AutomationProperty [] {};
			HeaderItem.requiredPatternSets = new AutomationPattern [] [] {};
			
			Header = new ControlType (HeaderId, "ControlType.Header");
			Header.localizedControlType = "header";
			Header.neverSupportedPatterns = new AutomationPattern [] {};
			Header.requiredProperties = new AutomationProperty [] {};
			Header.requiredPatternSets = new AutomationPattern [] [] {};
			
			Hyperlink = new ControlType (HyperlinkId, "ControlType.Hyperlink");
			Hyperlink.localizedControlType = "hyperlink";
			Hyperlink.neverSupportedPatterns = new AutomationPattern [] {};
			Hyperlink.requiredProperties = new AutomationProperty [] {};
			Hyperlink.requiredPatternSets = new AutomationPattern [] [] {
				new AutomationPattern [] {
					InvokePatternIdentifiers.Pattern}
			};
			
			Image = new ControlType (ImageId, "ControlType.Image");
			Image.localizedControlType = "image";
			Image.neverSupportedPatterns = new AutomationPattern [] {};
			Image.requiredProperties = new AutomationProperty [] {};
			Image.requiredPatternSets = new AutomationPattern [] [] {};
			
			ListItem = new ControlType (ListItemId, "ControlType.ListItem");
			ListItem.localizedControlType = "list item";
			ListItem.neverSupportedPatterns = new AutomationPattern [] {};
			ListItem.requiredProperties = new AutomationProperty [] {};
			ListItem.requiredPatternSets = new AutomationPattern [] [] {
				new AutomationPattern [] {
					SelectionItemPatternIdentifiers.Pattern}
			};
			
			List = new ControlType (ListId, "ControlType.List");
			List.localizedControlType = "list view";
			List.neverSupportedPatterns = new AutomationPattern [] {};
			List.requiredProperties = new AutomationProperty [] {};
			List.requiredPatternSets = new AutomationPattern [] [] {
				new AutomationPattern [] {
					SelectionPatternIdentifiers.Pattern,
					TablePatternIdentifiers.Pattern,
					GridPatternIdentifiers.Pattern,
					MultipleViewPatternIdentifiers.Pattern}
			};
			
			MenuBar = new ControlType (MenuBarId, "ControlType.MenuBar");
			MenuBar.localizedControlType = "menu bar";
			MenuBar.neverSupportedPatterns = new AutomationPattern [] {};
			MenuBar.requiredProperties = new AutomationProperty [] {};
			MenuBar.requiredPatternSets = new AutomationPattern [] [] {};
			
			MenuItem = new ControlType (MenuItemId, "ControlType.MenuItem");
			MenuItem.localizedControlType = "menu item";
			MenuItem.neverSupportedPatterns = new AutomationPattern [] {};
			MenuItem.requiredProperties = new AutomationProperty [] {};
			MenuItem.requiredPatternSets = new AutomationPattern [] [] {
				new AutomationPattern [] {
					InvokePatternIdentifiers.Pattern},
				new AutomationPattern [] {
					ExpandCollapsePatternIdentifiers.Pattern},
				new AutomationPattern [] {
					TogglePatternIdentifiers.Pattern}
			};
			
			Menu = new ControlType (MenuId, "ControlType.Menu");
			Menu.localizedControlType = "menu";
			Menu.neverSupportedPatterns = new AutomationPattern [] {};
			Menu.requiredProperties = new AutomationProperty [] {};
			Menu.requiredPatternSets = new AutomationPattern [] [] {};
			
			Pane = new ControlType (PaneId, "ControlType.Pane");
			Pane.localizedControlType = "pane";
			Pane.neverSupportedPatterns = new AutomationPattern [] {};
			Pane.requiredProperties = new AutomationProperty [] {};
			Pane.requiredPatternSets = new AutomationPattern [] [] {
				new AutomationPattern [] {
					TransformPatternIdentifiers.Pattern}
			};
			
			ProgressBar = new ControlType (ProgressBarId, "ControlType.ProgressBar");
			ProgressBar.localizedControlType = "progress bar";
			ProgressBar.neverSupportedPatterns = new AutomationPattern [] {};
			ProgressBar.requiredProperties = new AutomationProperty [] {};
			ProgressBar.requiredPatternSets = new AutomationPattern [] [] {
				new AutomationPattern [] {
					ValuePatternIdentifiers.Pattern}
			};
			
			RadioButton = new ControlType (RadioButtonId, "ControlType.RadioButton");
			RadioButton.localizedControlType = "radio button";
			RadioButton.neverSupportedPatterns = new AutomationPattern [] {};
			RadioButton.requiredProperties = new AutomationProperty [] {};
			RadioButton.requiredPatternSets = new AutomationPattern [] [] {};
			
			ScrollBar = new ControlType (ScrollBarId, "ControlType.ScrollBar");
			ScrollBar.localizedControlType = "scroll bar";
			ScrollBar.neverSupportedPatterns = new AutomationPattern [] {};
			ScrollBar.requiredProperties = new AutomationProperty [] {};
			ScrollBar.requiredPatternSets = new AutomationPattern [] [] {};
			
			Separator = new ControlType (SeparatorId, "ControlType.Separator");
			Separator.localizedControlType = "separator";
			Separator.neverSupportedPatterns = new AutomationPattern [] {};
			Separator.requiredProperties = new AutomationProperty [] {};
			Separator.requiredPatternSets = new AutomationPattern [] [] {};
			
			Slider = new ControlType (SliderId, "ControlType.Slider");
			Slider.localizedControlType = "slider";
			Slider.neverSupportedPatterns = new AutomationPattern [] {};
			Slider.requiredProperties = new AutomationProperty [] {};
			Slider.requiredPatternSets = new AutomationPattern [] [] {
				new AutomationPattern [] {
					RangeValuePatternIdentifiers.Pattern},
				new AutomationPattern [] {
					SelectionPatternIdentifiers.Pattern}
			};
			
			Spinner = new ControlType (SpinnerId, "ControlType.Spinner");
			Spinner.localizedControlType = "spinner";
			Spinner.neverSupportedPatterns = new AutomationPattern [] {};
			Spinner.requiredProperties = new AutomationProperty [] {};
			Spinner.requiredPatternSets = new AutomationPattern [] [] {
				new AutomationPattern [] {
					RangeValuePatternIdentifiers.Pattern},
				new AutomationPattern [] {
					SelectionPatternIdentifiers.Pattern}
			};
			
			SplitButton = new ControlType (SplitButtonId, "ControlType.SplitButton");
			SplitButton.localizedControlType = "split button";
			SplitButton.neverSupportedPatterns = new AutomationPattern [] {};
			SplitButton.requiredProperties = new AutomationProperty [] {};
			SplitButton.requiredPatternSets = new AutomationPattern [] [] {
				new AutomationPattern [] {
					InvokePatternIdentifiers.Pattern},
				new AutomationPattern [] {
					ExpandCollapsePatternIdentifiers.Pattern}
			};
			
			StatusBar = new ControlType (StatusBarId, "ControlType.StatusBar");
			StatusBar.localizedControlType = "status bar";
			StatusBar.neverSupportedPatterns = new AutomationPattern [] {};
			StatusBar.requiredProperties = new AutomationProperty [] {};
			StatusBar.requiredPatternSets = new AutomationPattern [] [] {};
			
			TabItem = new ControlType (TabItemId, "ControlType.TabItem");
			TabItem.localizedControlType = "tab item";
			TabItem.neverSupportedPatterns = new AutomationPattern [] {};
			TabItem.requiredProperties = new AutomationProperty [] {};
			TabItem.requiredPatternSets = new AutomationPattern [] [] {};
			
			Table = new ControlType (TableId, "ControlType.Table");
			Table.localizedControlType = "table";
			Table.neverSupportedPatterns = new AutomationPattern [] {};
			Table.requiredProperties = new AutomationProperty [] {};
			Table.requiredPatternSets = new AutomationPattern [] [] {
				new AutomationPattern [] {
					GridPatternIdentifiers.Pattern},
				new AutomationPattern [] {
					SelectionPatternIdentifiers.Pattern},
				new AutomationPattern [] {
					TablePatternIdentifiers.Pattern}
			};
			
			Tab = new ControlType (TabId, "ControlType.Tab");
			Tab.localizedControlType = "tab";
			Tab.neverSupportedPatterns = new AutomationPattern [] {};
			Tab.requiredProperties = new AutomationProperty [] {};
			Tab.requiredPatternSets = new AutomationPattern [] [] {};
			
			Text = new ControlType (TextId, "ControlType.Text");
			Text.localizedControlType = "text";
			Text.neverSupportedPatterns = new AutomationPattern [] {};
			Text.requiredProperties = new AutomationProperty [] {};
			Text.requiredPatternSets = new AutomationPattern [] [] {};
			
			Thumb = new ControlType (ThumbId, "ControlType.Thumb");
			Thumb.localizedControlType = "thumb";
			Thumb.neverSupportedPatterns = new AutomationPattern [] {};
			Thumb.requiredProperties = new AutomationProperty [] {};
			Thumb.requiredPatternSets = new AutomationPattern [] [] {};
			
			TitleBar = new ControlType (TitleBarId, "ControlType.TitleBar");
			TitleBar.localizedControlType = "title bar";
			TitleBar.neverSupportedPatterns = new AutomationPattern [] {};
			TitleBar.requiredProperties = new AutomationProperty [] {};
			TitleBar.requiredPatternSets = new AutomationPattern [] [] {};
			
			ToolBar = new ControlType (ToolBarId, "ControlType.ToolBar");
			ToolBar.localizedControlType = "tool bar";
			ToolBar.neverSupportedPatterns = new AutomationPattern [] {};
			ToolBar.requiredProperties = new AutomationProperty [] {};
			ToolBar.requiredPatternSets = new AutomationPattern [] [] {};
			
			ToolTip = new ControlType (ToolTipId, "ControlType.ToolTip");
			ToolTip.localizedControlType = "tool tip";
			ToolTip.neverSupportedPatterns = new AutomationPattern [] {};
			ToolTip.requiredProperties = new AutomationProperty [] {};
			ToolTip.requiredPatternSets = new AutomationPattern [] [] {};
			
			TreeItem = new ControlType (TreeItemId, "ControlType.TreeItem");
			TreeItem.localizedControlType = "tree view item";
			TreeItem.neverSupportedPatterns = new AutomationPattern [] {};
			TreeItem.requiredProperties = new AutomationProperty [] {};
			TreeItem.requiredPatternSets = new AutomationPattern [] [] {};
			
			Tree = new ControlType (TreeId, "ControlType.Tree");
			Tree.localizedControlType = "tree view";
			Tree.neverSupportedPatterns = new AutomationPattern [] {};
			Tree.requiredProperties = new AutomationProperty [] {};
			Tree.requiredPatternSets = new AutomationPattern [] [] {};
			
			Window = new ControlType (WindowId, "ControlType.Window");
			Window.localizedControlType = "window";
			Window.neverSupportedPatterns = new AutomationPattern [] {};
			Window.requiredProperties = new AutomationProperty [] {};
			Window.requiredPatternSets = new AutomationPattern [] [] {
				new AutomationPattern [] {
					TransformPatternIdentifiers.Pattern},
				new AutomationPattern [] {
					WindowPatternIdentifiers.Pattern}
			};
		}
		
		public static ControlType LookupById (int id)
		{
			if (id == ButtonId)
				return Button;
			else if (id == CalendarId)
				return Calendar;
			else if (id == CheckBoxId)
				return CheckBox;
			else if (id == ComboBoxId)
				return ComboBox;
			else if (id == CustomId)
				return Custom;
			else if (id == DataGridId)
				return DataGrid;
			else if (id == DataItemId)
				return DataItem;
			else if (id == DocumentId)
				return Document;
			else if (id == EditId)
			         return Edit;
			else if (id == GroupId)
				return Group;
			else if (id == HeaderItemId)
				return HeaderItem;
			else if (id == HeaderId)
				return Header;
			else if (id == HyperlinkId)
				return Hyperlink;
			else if (id == ImageId)
				return Image;
			else if (id == ListItemId)
				return ListItem;
			else if (id == ListId)
				return List;
			else if (id == MenuBarId)
				return MenuBar;
			else if (id == MenuItemId)
				return MenuItem;
			else if (id == MenuId)
				return Menu;
			else if (id == PaneId)
				return Pane;
			else if (id == ProgressBarId)
				return ProgressBar;
			else if (id == RadioButtonId)
				return RadioButton;
			else if (id == ScrollBarId)
				return ScrollBar;
			else if (id == SeparatorId)
				return Separator;
			else if (id == SliderId)
				return Slider;
			else if (id == SpinnerId)
				return Spinner;
			else if (id == SplitButtonId)
				return SplitButton;
			else if (id == StatusBarId)
				return StatusBar;
			else if (id == TabItemId)
				return TabItem;
			else if (id == TableId)
				return Table;
			else if (id == TabId)
				return Tab;
			else if (id == TextId)
				return Text;
			else if (id == ThumbId)
				return Thumb;
			else if (id == TitleBarId)
				return TitleBar;
			else if (id == ToolBarId)
				return ToolBar;
			else if (id == ToolTipId)
				return ToolTip;
			else if (id == TreeItemId)
				return TreeItem;
			else if (id == TreeId)
				return Tree;
			else if (id == WindowId)
				return Window;
			else
				return null;
		}
		
		public static readonly ControlType Button;
		public static readonly ControlType Calendar;
		public static readonly ControlType CheckBox;
		public static readonly ControlType ComboBox;
		public static readonly ControlType Custom;
		public static readonly ControlType DataGrid;
		public static readonly ControlType DataItem;
		public static readonly ControlType Document;
		public static readonly ControlType Edit;
		public static readonly ControlType Group;
		public static readonly ControlType Header;
		public static readonly ControlType HeaderItem;
		public static readonly ControlType Hyperlink;
		public static readonly ControlType Image;
		public static readonly ControlType List;
		public static readonly ControlType ListItem;
		public static readonly ControlType Menu;
		public static readonly ControlType MenuBar;
		public static readonly ControlType MenuItem;
		public static readonly ControlType Pane;
		public static readonly ControlType ProgressBar;
		public static readonly ControlType RadioButton;
		public static readonly ControlType ScrollBar;
		public static readonly ControlType Separator;
		public static readonly ControlType Slider;
		public static readonly ControlType Spinner;
		public static readonly ControlType SplitButton;
		public static readonly ControlType StatusBar;
		public static readonly ControlType Tab;
		public static readonly ControlType TabItem;
		public static readonly ControlType Table;
		public static readonly ControlType Text;
		public static readonly ControlType Thumb;
		public static readonly ControlType TitleBar;
		public static readonly ControlType ToolBar;
		public static readonly ControlType ToolTip;
		public static readonly ControlType Tree;
		public static readonly ControlType TreeItem;
		public static readonly ControlType Window;
		
#endregion
	}
}

