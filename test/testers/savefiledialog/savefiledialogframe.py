
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        02/17/2009
# Description: savefiledialog.py wrapper script
#              Used by the savefiledialog-*.py tests
##############################################################################$

import sys
import os
import actions
import states

from strongwind import *
from savefiledialog import *

class SearchFolderError(Exception):
    'Raised when search an extra item method fails'
    pass

# class to represent the main window.
class SaveFileDialogFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    BUTTON1 = "Click me"

    def __init__(self, accessible):
        super(SaveFileDialogFrame, self).__init__(accessible)
        self.opendialog_button = self.findPushButton(self.BUTTON1)

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
        procedurelogger.action("search for all widgets from SaveDialog windows")

        procedurelogger.expectedResult("All widgets in SaveDialog is showing up")
        #click button will invoke dialog page
        self.savedialog = self.app.findDialog("Save As")
        #there are 3 labels in dialog
        self.lookin_label = self.savedialog.findLabel("Save in:")
        self.filename_label = self.savedialog.findLabel("File name:")
        self.savetype_label = self.savedialog.findLabel("Save as type:")
        #there are 2 push button in dialog
        self.Save_button = self.savedialog.findPushButton("Save")
        self.cancel_button = self.savedialog.findPushButton("Cancel")
        #there are 5 popupbutton in popupbuttonpanel on the left side
        self.toolbars = self.savedialog.findAllToolBars(None)
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
        self.comboboxs = self.savedialog.findAllComboBoxs(None)
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
        #a treetable to show files
        self.treetable = self.savedialog.findTreeTable(None)
        self.tablecells = self.treetable.findAllTableCells(None)

        assert len(self.tablecells) != 0, "no table cells under treetable"

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
        #enter folder name to text click OK, then check list item from treetable
        self.newfolder_text.enterText("ANewFolder")
        sleep(config.SHORT_DELAY)
        button.click()
        sleep(config.SHORT_DELAY)
        if button == self.newfolder_cancel:
            procedurelogger.expectedResult("never create new folder")
            try:
                new_menuitem = self.treetable.findTableCell("ANewFolder")
                if new_menuitem:
                    raise SearchFolderError
            except SearchError:
                pass
        elif button == self.newfolder_ok:
            procedurelogger.expectedResult("create new folder")
            self.treetable.findTableCell("ANewFolder")

    #assert the foldername isn't been showing after click dir menuitem
    def AssertMenuItemClick(self, foldername):

        procedurelogger.expectedResult("Directory is changed, you can't find %s folder again" % foldername)
        try:
            foldername_menuitem = self.treetable.findTableCell(foldername)
            if foldername_menuitem:
               raise SearchFolderError
        except SearchError:
            pass

    #assert activate table cell to enter folder or open file
    def assertActivate(self, cellname):
        #show all files in treetable
        savetype = self.savedialog.findMenuItem(re.compile('^All'), checkShowing=False)
        savetype.click()
        sleep(config.LONG_DELAY)
        #do activate action
        tablecell = self.treetable.findTableCell(cellname, checkShowing=False)
        ##tablecell.activate()

        sleep(config.SHORT_DELAY)

        if cellname == "ANewFolder":
            procedurelogger.expectedResult('Enter to "%s" folder' % cellname)
            try:
                table_cell = self.treetable.findTableCell(None)
                if len(table_cell) > 0:
                    raise SearchFolderError
            except SearchError:
                pass
        else:
            procedurelogger.expectedResult('"%s" file is saved' % cellname)
            self.app.findDialog("Save").findPushButton("OK").click()
            sleep(config.SHORT_DELAY)
            self.opendialog.assertClosed()

   
    #close application main window after running test
    def quit(self):
        self.altF4()
