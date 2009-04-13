
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        01/13/2008
# Description: printpreviewcontrol.py wrapper script
#              Used by the printpreviewcontrol-*.py tests
##############################################################################$

import sys
import os
import actions
import states

from strongwind import *
from printpreviewcontrol import *


# class to represent the main window.
class PrintPreviewControlFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    BUTTON = "Button"
    PANEL = "PrintPreviewPage"

    def __init__(self, accessible):
        super(PrintPreviewControlFrame, self).__init__(accessible)
        self.button = self.findPushButton(self.BUTTON)

    def click(self, button):
        """
        Wrap strongwind click action, then make sure all widgets are showing up

        """
        button.click()

        procedurelogger.expectedResult("PrintPreviewControl page is showing")
        self.panel = self.findPanel(self.PANEL)
        self.scrollbars = self.findAllScrollBars(None)
        self.vscrollbar = self.scrollbars[0]
        self.hscrollbar = self.scrollbars[1]

    # Zoom property can show both vscrollbar and hscrollbar. make sure 
    # scrollbar's AtkValue is implemented, 
    def valueScrollBar(self, scrollbar, newValue=None):
        procedurelogger.action('set %s value to "%s"' % (scrollbar, newValue))
        scrollbar.value = newValue
        sleep(config.SHORT_DELAY)

        procedurelogger.expectedResult("%s value is %s" % (scrollbar, scrollbar.value))
        assert scrollbar._accessible.queryValue().currentValue == newValue 

    #close application main window after running test
    def quit(self):
        self.altF4()
