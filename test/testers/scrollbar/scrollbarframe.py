
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/06/2008
# Description: scrollbar.py wrapper script
#              Used by the scrollbar-*.py tests
##############################################################################$

import sys
import os
import actions
import states

from strongwind import *
from scrollbar import *


# class to represent the main window.
class ScrollBarFrame(accessibles.Frame):

    def __init__(self, accessible):
        super(ScrollBarFrame, self).__init__(accessible)
        self.listbox1 = self.findList("listbox with vertical scrollbar")
        self.listbox2 = self.findList("listbox with horizontal scrollbar")
        self.vscrollbar = self.listbox1.findScrollBar(None)
        self.hscrollbar = self.listbox2.findScrollBar(None)

    #change scrollbar's value
    def valueScrollBar(self, scrollbar, newValue=None):
        procedurelogger.action('set %s value to "%s"' % (scrollbar, newValue))
        scrollbar.__setattr__('value', newValue)

    def assertScrollbar(self, scrollbar, newValue=None):
        maximumValue = scrollbar._accessible.queryValue().maximumValue

        if 0 <= newValue <= maximumValue:
            procedurelogger.expectedResult('the %s\'s current value is "%s"' % (scrollbar, newValue))
            assert scrollbar.__getattr__('value') == newValue, \
                       "scrollbar's current value is %s:" % scrollbar.__getattr__('value')
        else:
            procedurelogger.expectedResult('value "%s" out of run' % newValue)
            assert not scrollbar.__getattr__('value') == newValue, \
                       "scrollbar's current value is %s:" % scrollbar.__getattr__('value')
    
    #close application window
    def quit(self):
        self.altF4()
