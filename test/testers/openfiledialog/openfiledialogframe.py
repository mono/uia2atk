
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        02/11/2009
# Description: openfiledialog.py wrapper script
#              Used by the openfiledialog-*.py tests
##############################################################################$

import sys
import os
import actions
import states

from strongwind import *
from openfiledialog import *

class SearchNewFolderError(Exception):
    'Raised when search an extra item method fails'
    print "ListView shows aa new folder"
    pass

# class to represent the main window.
class OpenFileDialogFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    BUTTON1 = "OpenDialog"
    BUTTON2 = "EnableVisible"

    def __init__(self, accessible):
        super(OpenFileDialogFrame, self).__init__(accessible)
        self.opendialog_button = self.findPushButton(self.BUTTON1)
        self.enable_button = self.findPushButton(self.BUTTON2)

    #do click action
    def click(self, button):
        button.click()

    #do click action and give action log
    def ItemClick(self, itemname):
        procedurelogger.action("click %s" % itemname)
        itemname.click()

    #do press action
    def press(self, itemname):
        procedurelogger.action("press %s" % itemname)
        itemname.press()

    #assert if all widgets on Open dialog are showing
    def AssertWidgets(self, button=None):
        procedurelogger.action("search for all widgets from OpenDialog windows")

        procedurelogger.expectedResult("All widgets in OpenDialog is showing up")
        #click OpenDialog to invoke dialog page
        self.opendialog = self.app.findDialog("Open")
        #there are 3 labels in dialog
        self.lookin_label = self.opendialog.findLabel("Look in:")
        self.filename_label = self.opendialog.findLabel("File name:")
        self.filesoftype_label = self.opendialog.findLabel("Files of type:")
        #there are 2 push button in dialog
        self.open_button = self.opendialog.findPushButton("Open")
        self.cancel_button = self.opendialog.findPushButton("Cancel")
        #there are 5 popupbutton in popupbuttonpanel on the left side
        self.toolbars = self.opendialog.findAllToolBars(None)
        self.popuptoolbar = self.toolbars[1]
        self.recentlyused_popup = self.popuptoolbar.findMenuItem(re.compile('^Recently'))
        self.desktop_popup = self.popuptoolbar.findMenuItem("Desktop")
        self.personal_popup = self.popuptoolbar.findMenuItem("Personal")
        self.mycomputer_popup = self.popuptoolbar.findMenuItem("My Computer")
        self.mynetwork_popup = self.popuptoolbar.findMenuItem("My Network")
        #toolbar with 4 small toolbarbuttons on the top right side
        self.smalltoolbar = self.toolbars[0]
        self.toolbarbuttons = self.smalltoolbar.findAllPushButtons(None)
        self.backtoolbarbutton = self.toolbarbuttons[0]
        self.uptoolbarbutton = self.toolbarbuttons[1]
        self.newdirtoolbarbutton = self.toolbarbuttons[2]
        self.menutoolbarbutton = self.toolbarbuttons[3]
        self.menutogglebutton = self.smalltoolbar.findToggleButton(None)
        #there are 5 menuitems under menutogglebutton
        self.smallicon_menuitem = self.menutogglebutton.findMenuItem("Small Icon", checkShowing=False)
        self.tiles_menuitem = self.menutogglebutton.findMenuItem("Tiles", checkShowing=False)
        self.largeicon_menuitem = self.menutogglebutton.findMenuItem("Large Icon", checkShowing=False)
        self.list_menuitem = self.menutogglebutton.findMenuItem("List", checkShowing=False)
        self.details_menuitem = self.menutogglebutton.findMenuItem("Details", checkShowing=False)
        #there are 2 normal combobox on bottom and 1 dirComboBox on top
        self.comboboxs = self.opendialog.findAllComboBoxs(None)
        self.filename_combobox = self.comboboxs[0]
        self.filetype_combobox = self.comboboxs[1]
        self.dir_combobox = self.comboboxs[2]
        #there are 5 menuitems under dir_combobox
        self.dir_menu = self.dir_combobox.findMenu(None, checkShowing=False)
        self.recentlyused_menuitem = self.dir_menu.findMenuItem("Recently used", checkShowing=False)
        self.desktop_menuitem = self.dir_menu.findMenuItem("Desktop", checkShowing=False)
        self.personal_menuitem = self.dir_menu.findMenuItem("Personal", checkShowing=False)
        self.mycomputer_menuitem = self.dir_menu.findMenuItem("My Computer", checkShowing=False)
        self.mynetwork_menuitem = self.dir_menu.findMenuItem("My Network", checkShowing=False)
        #a listview to show files
        self.listview = self.opendialog.findList(None)
        self.listitems = self.listview.findAllListItems(None)

        assert len(self.listitems) != 0, "no list items under listview"

    #assert if help button and readonly checkbox are showing
    def AssertVisibleWidget(self):
        procedurelogger.expectedResult('"Help" button and "Open Readonly" checkbox  widgets is showing up')
        self.help_button = self.opendialog.findPushButton("Help")
        self.readonly_checkbox = self.opendialog.findCheckBox("Open Readonly")

    #assert if all widgets on creat new folder dialog are showing
    def NewFolderCheck(self):

        procedurelogger.expectedResult("All widgets in NewFolderOrFile dialog is showing up")
        self.newfolderdialog = self.app.findDialog("New Folder or File")
        self.newfolder_panel = self.newfolderdialog.findPanel("New Name")
        self.newfolder_label =self.newfolder_panel.findLabel("Enter Name:")
        self.newfolder_icon = self.newfolder_panel.findIcon(None)
        self.newfolder_text = self.newfolder_panel.findText(None)
        self.newfolder_ok = self.newfolderdialog.findPushButton("OK")
        self.newfolder_cancel = self.newfolderdialog.findPushButton("Cancel")

    #creat a new folder
    def CreatFolder(self, button):       
        #enter folder name to text click OK, then check list item from listview
        self.newfolder_text.enterText("ANewFolder")
        sleep(config.SHORT_DELAY)
        button.click()
        sleep(config.SHORT_DELAY)
        if button == self.newfolder_cancel:
            procedurelogger.expectedResult("never create new folder")
            try:
                aa_menuitem = self.listview.findListItem("ANewFolder")
                if aa_menuitem:
                    raise SearchNewFolderError
            except SearchError:
                pass
        elif button == self.newfolder_ok:
            procedurelogger.expectedResult("create new folder")
            self.listview.findListItem("ANewFolder")

    #assert the foldername isn't been showing after click dir menuitem
    def AssertMenuItemClick(self, foldername):

        procedurelogger.expectedResult("Directory is changed, you can't find aa folder again")
        try:
            foldername_menuitem = self.listview.findListItem(foldername)
            if foldername_menuitem:
               raise SearchNewFolderError
        except SearchError:
            pass

   
    #close application main window after running test
    def quit(self):
        self.altF4()
