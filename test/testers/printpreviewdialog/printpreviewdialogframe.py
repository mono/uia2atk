
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        01/16/2008
# Description: printpreviewdialog.py wrapper script
#              Used by the printpreviewdialog-*.py tests
##############################################################################$

import sys
import os
import actions
import states

from strongwind import *
from printpreviewdialog import *


# class to represent the main window.
class PrintPreviewDialogFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    BUTTON = "PrintPreviewDialog"

    def __init__(self, accessible):
        super(PrintPreviewDialogFrame, self).__init__(accessible)
        self.button = self.findPushButton(self.BUTTON)

    def click(self, button):
        """
        Wrap strongwind click action, then make sure all widgets are showing up

        """
        button.click()

        sleep(config.SHORT_DELAY)

        # click button to invoke dialog page with a toolbar and a panel
        self.dialog = self.app.findDialog("PrintPreviewDialog")
        self.toolbar = self.dialog.findToolBar(None)
        panels = self.dialog.findAllPanels(None)
        self.dialog_panel = panels[-1]

    # search for items on toolbar
    def searchItems(self, itemname, number):
        """
        Make sure all widgtes are accessible

        """
        procedurelogger.action("how many %s items on toolbar" % itemname)

        procedurelogger.expectedResult("%s %s on toolbar" % (number, itemname))
        if itemname == "push button":
            count = self.toolbar.findAllPushButtons(None)
            assert len(count) == number, "find out %s push buttons" % len(count)
        elif itemname == "toggle button":
            count = self.toolbar.findAllToggleButtons(None)
            assert len(count) == number, "find out %s toggle buttons" % len(count)
        elif itemname == "separator":
            count = self.toolbar.findAllSeparators(None)
            assert len(count) == number, "find out %s separators" % len(count)
        elif itemname == "spin button":
            count = self.toolbar.findAllSpinButtons(None)
            assert len(count) == number, "find out %s spin button" % len(count)
   
    # close application main window after running test
    def quit(self):
        self.altF4()
