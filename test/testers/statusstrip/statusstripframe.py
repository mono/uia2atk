
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        11/07/2008
# Description: statusstrip.py wrapper script
#              Used by the statusstrip-*.py tests
##############################################################################$

import sys
import os
import actions
import states

from strongwind import *
from statusstrip import *


# class to represent the main window.
class StatusStripFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    BUTTON_ONE = "button1"
    BUTTON_TWO = "ToolStripDropDownButton"
    BUTTON_THREE = "ToolStripSplitButton"
    LABEL_ONE = "Examples for: StatusStrip."
    LABEL_TWO = "ToolStripLabel Text..."

    def __init__(self, accessible):
        super(StatusStripFrame, self).__init__(accessible)
        self.button = self.findPushButton(self.BUTTON_ONE)
        self.statusstrip = self.findStatusBar(None)
        self.MainLabel = self.findLabel(self.LABEL_ONE)
        self.StripLabel = self.findLabel(self.LABEL_TWO)
        self.ProgressBar = self.findProgressBar(None)
        self.DropDownButton_item1 = self.findMenuItem("Red")
        self.DropDownButton_item2 = self.findMenuItem("Blue")
        self.SplitButton_item1 = self.findMenuItem("Blue Color")
        self.SplitButton_item2 = self.findMenuItem("Red Color")

    #give 'click' action
    def click(self,button):
        button.click()

    #assert label's text is changed after click button1 or click menu item
    def assertLabel(self, label, newlabel):
        procedurelogger.expectedResult('"%s" is changed to "%s"' % (label, newlabel))

        def resultMatches():
            return label.text == newlabel
        assert retryUntilTrue(resultMatches)

    #assert progressbar's value after click button1
    def assertProgressBarValue(self, accessible, value):
        procedurelogger.expectedResult('ProgressBar\'s current value is "%s"' % value)

        assert accessible.value == value, \
                       "progressbar's current value is %s:" % accessible.value

    
    #close application main window after running test
    def quit(self):
        self.altF4()
