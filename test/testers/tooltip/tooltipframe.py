
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/02/2008
# Description: tooltip.py wrapper script
#              Used by the tooltip-*.py tests
##############################################################################

import sys
import os
import actions
import states

from strongwind import *
from tooltip import *


# class to represent the main window.
class ToolTipFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    BUTTON = "ToolTip button"
    CHECKBOX = "Grape"

    def __init__(self, accessible):
        super(ToolTipFrame, self).__init__(accessible)
        self.button = self.findPushButton(self.BUTTON)
        self.checkbox = self.findCheckBox(self.CHECKBOX)
        self.label1 = self.findLabel("Examples for: ToolTip")

    #assert tooltip appear
    def assertTooltip(self, tooltiplabel):
        procedurelogger.action('Check if "%s" is present' % tooltiplabel)
        procedurelogger.expectedResult('Tooltip "%s" is present' % tooltiplabel)
        self.tooltip = self.app.findToolTip(tooltiplabel)

        assert self.tooltip

    def assertNoTooltip(self, tooltiplabel):
        procedurelogger.action('Check if "%s" is present' % tooltiplabel)
        procedurelogger.expectedResult('Tooltip "%s" is not present' % tooltiplabel)
        try:
            self.app.findToolTip(tooltiplabel)
        except errors.SearchError:
            return
        assert False, 'Tooltip "%s" should not be present' 
             
    #close application main window after running test
    def quit(self):
        self.altF4()
