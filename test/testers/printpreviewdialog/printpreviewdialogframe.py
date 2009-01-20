
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

    #do click action
    def click(self, button):
        button.click()

        sleep(config.SHORT_DELAY)

        #click button to invoke dialog page with a toolbar and a panel
        procedurelogger.expectedResult("PrintPreviewDialog page is showing")
        self.dialog = self.app.findDialog("PrintPreviewDialog")
        self.panel = self.dialog.findPanel(None)
        self.toolbar = self.dialog.findToolBar(None)

    #close application main window after running test
    def quit(self):
        self.altF4()
