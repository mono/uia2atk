
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
        self.list = self.findAllTreeTables(None)
        #listbox1 with vertical scrollbar
        self.listbox1 = self.list[0]
        #listbox2 with horizontal scrollbar
        self.listbox2 = self.list[1]
        self.vscrollbar = self.listbox1.findScrollBar(None)
        self.hscrollbar = self.listbox2.findScrollBar(None)
        self.list1item = dict([(x, self.listbox1.findTableCell(str(x), checkShowing=False)) for x in range(30)])
        self.list2item = dict([(x, self.listbox2.findTableCell(str(x), checkShowing=False)) for x in range(30)])

    #change scrollbar's value
    def valueScrollBar(self, scrollbar, newValue=None):
        procedurelogger.action('set %s value to "%s"' % (scrollbar, newValue))
        scrollbar.value = newValue

    def assertScrollbar(self, scrollbar, newValue=None):
        self.maximumValue = scrollbar._accessible.queryValue().maximumValue
        self.minimumValue = scrollbar._accessible.queryValue().minimumValue

        if 0 <= newValue <= self.maximumValue:
            procedurelogger.expectedResult('the %s\'s current value is "%s"' % (scrollbar, newValue))
            assert scrollbar.value == newValue, \
                                     "current value is %s:" % \
                                     scrollbar.value
        else:
            if newValue > self.maximumValue:
                procedurelogger.expectedResult('value "%s" out of run %s' % (newValue, self.maximumValue))
            elif newValue < self.minimumValue:
                procedurelogger.expectedResult('value "%s" out of run %s' % (newValue,self. minimumValue))
            assert not scrollbar.value == newValue, \
                                     "current value is %s:" % \
                                     scrollbar.value
    
    #close application window
    def quit(self):
        self.altF4()
