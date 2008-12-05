
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        10/31/2008
# Description: toolstripmenuitem.py wrapper script
#              Used by the toolstripmenuitem-*.py tests
##############################################################################

import sys
import os
import actions
import states

from strongwind import *
from toolstripmenuitem import *


# class to represent the main window.
class ToolStripMenuItemFrame(accessibles.Frame):

    VIEW_MENU = "View"
    HELP_MENU = "Help"
    FILE_MENU = "File"
    EDIT_MENU = "Edit"
    CREATE_ITEM = "Create"
    WRITE_ITEM = "Write"
    FINANCIAL_ITEM = "Financial"
    MEDICAL_ITEM = "Medical"
    NEW_ITEM = "New"
    OPEN_ITEM = "Open"
    COPY_THIS_ITEM = "Copy This"
    PASTE_THAT_ITEM = "Paste That"

    def __init__(self, accessible):
        super(ToolStripMenuItemFrame, self).__init__(accessible)
        print "Finding accessibles..."
        sys.stdout.flush()
        self.view_menu = self.findMenu(self.VIEW_MENU)
        self.help_menu = self.findMenu(self.HELP_MENU)
        self.file_menu = self.findMenu(self.FILE_MENU)
        self.edit_menu = self.findMenu(self.EDIT_MENU)
        self.create_menu_item = self.findMenuItem(self.CREATE_ITEM)
        self.write_menu_item = self.findMenuItem(self.WRITE_ITEM)
        self.financial_menu_item = self.findMenuItem(self.FINANCIAL_ITEM)
        self.medical_menu_item = self.findMenuItem(self.MEDICAL_ITEM)
        self.new_menu_item = self.findMenuItem(self.NEW_ITEM)
        self.open_menu_item = self.findMenuItem(self.OPEN_ITEM)
        self.copy_this_menu_item = self.findMenuItem(self.COPY_THIS_ITEM)
        self.paste_that_menu_item = self.findMenuItem(self.PASTE_THAT_ITEM)
        print "Found all accessibles"

    # make sure we find a MessageBox was clicked with the specified title
    def assert_message_box_appeared(self, title):
        procedurelogger.action('Verify that we can find the %s MessageBox' %\
                                                                         title)
        procedurelogger.expectedResult('Message Box %s exists' % title)
        mb = self.findFrame("Create Clicked")

    #close main window
    def quit(self):
        self.altF4()
