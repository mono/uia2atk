# the tests_list list keeps track of enabled tests.  A test will be
# executed only after the tests sample application is added to this list

# this allows tests to be worked on and developed in SVN, but only executed
# as part of the test suite when they are ready

# only add finished tests that should execute successfully to this list

winforms_tests_list = [
	"button-regression.py",
	"checkbox-regression.py",
	"checkedlistbox-regression.py",
	"colordialog-regression.py",
	"columnheader-regression.py",
	"combobox_dropdownlist-regression.py",
	"combobox_dropdown-regression.py",
	"combobox_simple-regression.py",
	"combobox_stylechanges-regression.py",
	"containercontrol-regression.py",
	"contextmenu-regression.py",
	"contextmenustrip-regression.py",
	"datagridboolcolumn-regression.py",
	"datagridcomboboxcolumn-regression.py",
	"datagrid-regression.py",
	"datagridtextboxcolumn-regression.py",
	"datagridview_detail-regression.py",
	"datetimepicker_dropdown-regression.py",
	"datetimepicker_showupdown-regression.py",
	"domainupdown-regression.py",
	"errorprovider-regression.py",
	"flowlayoutpanel-regression.py",
	"fontdialog-regression.py",
	"form-regression.py",
	"groupbox-regression.py",
	"helpprovider-regression.py",
	"hscrollbar-regression.py",
	"label-regression.py",
	"linklabel-regression.py",
	"listbox-regression.py",
	"listview_largeimage-regression.py",
	"listview_list-regression.py",
	"listview-regression.py",
	"listview_smallimage-regression.py",
	"mainmenu-regression.py",
	"maskedtextbox-regression.py",
	"menustrip-regression.py",
	"monthcalendar-regression.py",
	"notifyicon-regression.py",
	"numericupdown-regression.py",
	"openfiledialog-regression.py",
	"pagesetupdialog-regression.py",
	"panel-regression.py",
	"picturebox-regression.py",
	"printpreviewcontrol-regression.py",
	"printpreviewdialog-regression.py",
	"progressbar-regression.py",
	"propertygrid-regression.py",
	"radiobutton-regression.py",
	"richtextbox-regression.py",
	"savefiledialog-regression.py",
	"scrollbar-regression.py",
	"splitter_horizontal-regression.py",
	"splitter_vertical-regression.py",
	"statusbarpanel-regression.py",
	"statusbar-regression.py",
	"statusstrip-regression.py",
	"tabcontrol-regression.py",
	"tablelayoutpanel-regression.py",
	"tabpage-regression.py",
	"textbox-regression.py",
	"threadexceptiondialog-regression.py",
	"toolbarbutton-regression.py",
	"toolbar-regression.py",
	"toolstripbutton-regression.py",
	"toolstripcombobox-regression.py",
	"toolstripdropdownbutton-regression.py",
	"toolstriplabel-regression.py",
	"toolstripmenuitem-regression.py",
	"toolstripprogressbar-regression.py",
	"toolstrip-regression.py",
	"toolstripseparator-regression.py",
	"toolstripsplitbutton-regression.py",
	"toolstriptextbox-regression.py",
	"tooltip-regression.py",
	"trackbar-regression.py",
	"treeview-regression.py",
	"vscrollbar-regression.py"
]

moonlight_tests_list = [
	"button-regression.py",
	"checkbox-regression.py",
	"combobox-regression.py",
	"gridsplitter-regression.py",
	"hyperlinkbutton-regression.py",
	"image-regression.py",
	"listbox-regression.py",
	"passwordbox-regression.py",
	"progressbar-regression.py",
	"radiobutton-regression.py",
	"repeatbutton-regression.py",
	"scrollbar-regression.py",
	"scrollviewer-regression.py",
	"slider-regression.py",
	"textblock-regression.py",
	"textbox-regression.py",
	"thumb-regression.py",
	"togglebutton-regression.py"
]

uiaclient_winforms_tests_list = [
	"Winforms-KeePassTests-RunTestCase101-regression.py",
	"Winforms-KeePassTests-RunTestCase102-regression.py",
	"Winforms-KeePassTests-RunTestCase103-regression.py",
	"Winforms-KeePassTests-RunTestCase104-regression.py",
	"Winforms-DockPatternTests-RunTestCase105-regression.py",
	"Winforms-WindowPatternTests-RunTestCase106-regression.py"
]

uiaclient_moonlight_tests_list = [
	"Moonlight-PuzzleGameTest-RunTestCase301-regression.py",
	"Moonlight-PuzzleGameTest-RunTestCase302-regression.py",
	"Moonlight-PuzzleGameTest-RunTestCase303-regression.py",
	"Moonlight-SL2WithPrismTest-RunTestCase304-regression.py",
	"Moonlight-SL2WithPrismTest-RunTestCase305-regression.py",
	"Moonlight-DiggSearchTest-RunTestCase306-regression.py",
	"Moonlight-DiggSearchTest-RunTestCase307-regression.py",
	"Moonlight-DiggSearchTest-RunTestCase308-regression.py",
	"Moonlight-DiggSearchTest-RunTestCase309-regression.py"
]

uiaclient_gtk_tests_list = [
	"Gtk-FSpot-RunTestCase201-regression.py",
	"Gtk-FSpot-RunTestCase202-regression.py"
	"Gtk-FSpot-RunTestCase203-regression.py"
	"Gtk-FSpot-RunTestCase204-regression.py"
	"Gtk-FSpot-RunTestCase205-regression.py"
	"Gtk-FSpot-RunTestCase206-regression.py"
	"Gtk-FSpot-RunTestCase207-regression.py"
	"Gtk-FSpot-RunTestCase208-regression.py"
]
