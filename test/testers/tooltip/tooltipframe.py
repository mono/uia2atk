
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

    #move mouse to x,y point
    def mousePoint(self, accessible, xOffset=0, yOffset=0):
        procedurelogger.action('move mouse to "%s"' % accessible)
        bbox = accessible.extents
        x = bbox.x + (bbox.width / 2) + xOffset
        y = bbox.y + (bbox.height / 2) + yOffset
        pyatspi.Registry.generateMouseEvent(x, y, 'abs')

    #assert tooltip appear
    def assertTooltip(self, tooltiplabel):
        procedurelogger.expectedResult('Found tooltip, the label is "%s"' % tooltiplabel)

        self.tooltip = self.app.findToolTip(tooltiplabel)

        assert self.tooltip

    
    #close application main window after running test
    def quit(self):
        self.altF4()
