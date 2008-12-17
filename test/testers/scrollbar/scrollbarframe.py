
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
        self.list = self.findAllLists(None)
        #listbox1 with vertical scrollbar
        self.listbox1 = self.list[0]
        #listbox2 with horizontal scrollbar
        self.listbox2 = self.list[1]
        self.vscrollbar = self.listbox1.findScrollBar(None)
        self.hscrollbar = self.listbox2.findScrollBar(None)
        self.list1item = dict([(x, self.listbox1.findListItem(str(x), checkShowing=False)) for x in range(30)])
        self.list2item = dict([(x, self.listbox2.findListItem(str(x), checkShowing=False)) for x in range(30)])

    #change scrollbar's value
    def valueScrollBar(self, scrollbar, newValue=None):
        procedurelogger.action('set %s value to "%s"' % (scrollbar, newValue))
        scrollbar.value = newValue

    def assertScrollbar(self, scrollbar, newValue=None):
        maximumValue = scrollbar._accessible.queryValue().maximumValue
        minimumValue = scrollbar._accessible.queryValue().minimumValue

        if 0 <= newValue <= maximumValue:
            procedurelogger.expectedResult('the %s\'s current value is "%s"' % (scrollbar, newValue))
            assert scrollbar.__getattr__('value') == newValue, \
                                     "%s's current value is %s:" % \
                                     scrollbar.__getattr__(scrollbar, 'value')
        else:
            if newValue > maximumValue:
                procedurelogger.expectedResult('value "%s" out of run %s' % (newValue, maximumValue))
            elif newValue < minimumValue:
                procedurelogger.expectedResult('value "%s" out of run %s' % (newValue, minimumValue))
            assert not scrollbar.__getattr__('value') == newValue, \
                                     "%s's current value is %s:" % \
                                     scrollbar.__getattr__(scrollbar, 'value')
    
    #close application window
    def quit(self):
        self.altF4()
