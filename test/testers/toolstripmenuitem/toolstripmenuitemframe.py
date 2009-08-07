
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
        self.text_area = self.findText("")
        self.view_menu = self.findMenu(self.VIEW_MENU)
        self.help_menu = self.findMenu(self.HELP_MENU)
        self.file_menu = self.findMenu(self.FILE_MENU)
        self.edit_menu = self.findMenu(self.EDIT_MENU)
        self.create_menu_item = self.findMenuItem(self.CREATE_ITEM, checkShowing=False)
        self.write_menu_item = self.findMenuItem(self.WRITE_ITEM, checkShowing=False)
        self.financial_menu_item = self.findMenuItem(self.FINANCIAL_ITEM, checkShowing=False)
        self.medical_menu_item = self.findMenuItem(self.MEDICAL_ITEM, checkShowing=False)
        self.new_menu_item = self.findMenuItem(self.NEW_ITEM, checkShowing=False)
        self.open_menu_item = self.findMenuItem(self.OPEN_ITEM, checkShowing=False)
        self.copy_this_menu_item = self.findMenuItem(self.COPY_THIS_ITEM, checkShowing=False)
        self.paste_that_menu_item = self.findMenuItem(self.PASTE_THAT_ITEM, checkShowing=False)
        print "Found all accessibles"

    # make sure we find a MessageBox was clicked with the specified title
    def assertMessageBoxAppeared(self, title):
        procedurelogger.action('Verify that we can find the "%s" MessageBox' %\
                                                                         title)
        procedurelogger.expectedResult('Message Box "%s" exists' % title)
        mb = self.app.findDialog(title)

    def assertText(self, expected_text):
        procedurelogger.action('Verify that the text area contains the text we expected')
        procedurelogger.expectedResult('The text area contains "%s"' % \
                                                                 expected_text)
        actual_text = self.text_area.text 
        assert actual_text == expected_text, 'Text was "%s", expected "%s"' % \
                                             (actual_text, expected_text)

    def assertEditMenuOpen(self):
        '''
        Assert that the "Edit" menu is open by asserting that the "Copy This"
        and "Paste That" menu items are showing
        '''
        procedurelogger.action('Verify that the expected menu items are showing')
        procedurelogger.expectedResult('The following menu items are showing: %s' % map(str, [self.copy_this_menu_item, self.paste_that_menu_item]))
        # BUG486335 MenuItem, ToolStripMenuItem: extraneous "showing" state of
        # menu item when it is not showing
        assert self.copy_this_menu_item.showing
        assert self.paste_that_menu_item.showing

    def clearTextArea(self):
        '''
        Clear the text area directly using at-spi
        '''
        self.text_area.text = ""
        text_area_text = self.text_area.text
        assert text_area_text == "", '%s contained "%s". It should be blank' %\
                                     (self.text_area, text_area_text)
    
    #close main window
    def quit(self):
        self.altF4()
