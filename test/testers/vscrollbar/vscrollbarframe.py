
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/04/2008
# Description: vscrollbar.py wrapper script
#              Used by the vscrollbar-*.py tests
##############################################################################$

import sys
import os
import actions
import states

from strongwind import *
from vscrollbar import *


# class to represent the main window.
class VScrollBarFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    LABEL = "Value:"

    def __init__(self, accessible):
        super(VScrollBarFrame, self).__init__(accessible)
        self.label = self.findLabel(self.LABEL)
        self.vscrollbar = self.findScrollBar(None)

    #change vscrollbar's value
    def valueScrollBar(self, newValue=None):
        procedurelogger.action('set scrollbar value to "%s"' % newValue)
        #self.vscrollbar.__setattr__('value', newValue)
        self.vscrollbar.value = newValue

    def assertLabel(self, newValue=None):
        procedurelogger.expectedResult('label\'s value changed to "%s"' % newValue)
        label = self.findLabel("Value: %s" % newValue)
        assert label

    def assertScrollbar(self, newValue=None):
        maximumValue = self.vscrollbar._accessible.queryValue().maximumValue
        minimumValue = self.vscrollbar._accessible.queryValue().minimumValue

        if 0 <= newValue <= maximumValue:
            procedurelogger.expectedResult('the scrollbar\'s current value is "%s"' % newValue)
            assert self.vscrollbar.value == newValue, \
                       "scrollbar's current value is %s:" % self.vscrollbar.value
        else:
            if newValue > maximumValue:
                procedurelogger.expectedResult('value "%s" out of run %s' % (newValue, maximumValue))
            elif newValue < minimumValue:
                procedurelogger.expectedResult('value "%s" out of run %s' % (newValue, minimumValue))
            assert not self.vscrollbar.value == newValue, \
                       "scrollbar's current value is %s:" % self.vscrollbar.value
    
    #close application window
    def quit(self):
        self.altF4()
