
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
        scrollbar = self.findScrollBar(None)
        sleep(config.MEDIUM_DELAY)
        scrollbar.__setattr__('value', newValue)

    def assertScrollbar(self, newValue=None):
        maximumValue = self.findScrollBar(None)._accessible.queryValue().maximumValue

        if 0 <= newValue <= maximumValue:
            procedurelogger.expectedResult('the scrollbar\'s current value is "%s"' % newValue)
            assert self.findScrollBar(None).__getattr__('value') == newValue, \
                       "scrollbar's current value is %s:" % self.findScrollBar(None).__getattr__('value')
        else:
            procedurelogger.expectedResult('value "%s" out of run' % newValue)
            assert not self.findScrollBar(None).__getattr__('value') == newValue, \
                       "scrollbar's current value is %s:" % self.findScrollBar(None).__getattr__('value')
    
    #close application window
    def quit(self):
        self.altF4()
