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
import pyatspi

from strongwind import *
from printpreviewcontrol import *

# class to represent the main window.
class PrintPreviewControlFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    BUTTON = "Button"
    PANEL = "PrintPreviewPage"
    FRAME = "Printing"

    def __init__(self, accessible):
        super(PrintPreviewControlFrame, self).__init__(accessible)
        self.button = self.findPushButton(self.BUTTON)

    def findAllPrintPreviewControlAccessibles(self):
        
        self.panel = self.findPanel(self.PANEL)
        self.scrollbars = self.findAllScrollBars(None)
        assert len(self.scrollbars) == 2, "Found %s scroll bars, expected 2" %\
                                                           len(self.scrollbars)
        self.vscrollbar = self.scrollbars[0]
        self.hscrollbar = self.scrollbars[1]
        assert self.vscrollbar.name == "Vertical Scroll Bar"
        assert self.hscrollbar.name == "Horizontal Scroll Bar"

    # Zoom property can show both vscrollbar and hscrollbar. make sure 
    # scrollbar's AtkValue is implemented, 
    def assignScrollBarValue(self, scrollbar, new_value):
        '''set the scrollbar value to new_value'''
        procedurelogger.action('set "%s" value to %s' % (scrollbar, new_value))
        scrollbar.value = new_value
        sleep(config.SHORT_DELAY)

    def assertScrollBarValue(self, scrollbar, expected_value):
        '''ensure the value of scrollbar matches the expected_value'''
        procedurelogger.expectedResult("%s value is %s" % \
                                                 (scrollbar, expected_value))
        actual_value = scrollbar.value
        assert actual_value == expected_value, \
                                    'The value of "%s" was %s, expected %s' % \
                                                 (actual_value, expected_value)

    #close application main window after running test
    def quit(self):
        self.altF4()
