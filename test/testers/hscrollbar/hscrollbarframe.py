
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/05/2008
# Description: hscrollbar.py wrapper script
#              Used by the hscrollbar-*.py tests
##############################################################################$

import sys
import os
import actions
import states

from strongwind import *
from hscrollbar import *


# class to represent the main window.
class HScrollBarFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    LABEL = "Value:"

    def __init__(self, accessible):
        super(HScrollBarFrame, self).__init__(accessible)
        self.label = self.findLabel(self.LABEL)
        self.hscrollbar = self.findScrollBar(None)

    #change hscrollbar's value
    def valueScrollBar(self, newValue=None):

        procedurelogger.action('set scrollbar value to "%s"' % newValue)
        self.hscrollbar.value = newValue

    def assertLabel(self, newValue=None):
        procedurelogger.expectedResult('label\'s value changed to "%s"' % newValue)
        label = self.findLabel("Value: %s" % newValue)
        assert label

    def assertScrollbar(self, newValue=None):
        maximumValue = self.hscrollbar._accessible.queryValue().maximumValue
        minimumValue = self.hscrollbar._accessible.queryValue().minimumValue

        if 0 <= newValue <= maximumValue:
            procedurelogger.expectedResult('the scrollbar\'s current value is "%s"' % newValue)
            assert self.hscrollbar.value == newValue, \
                       "scrollbar's current value is %s:" % self.hscrollbar.value
        else:
            if newValue > maximumValue:
                procedurelogger.expectedResult('value "%s" out of run %s' % (newValue, maximumValue))
            elif newValue < minimumValue:
                procedurelogger.expectedResult('value "%s" out of run %s' % (newValue, minimumValue))
            assert not self.hscrollbar.value == newValue, \
                       "scrollbar's current value is %s:" % self.hscrollbar.value
    
    #close application window
    def quit(self):
        self.altF4()
