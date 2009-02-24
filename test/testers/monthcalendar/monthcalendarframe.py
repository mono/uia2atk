# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Sandy Armstrong <saarmstrong@novell.com>
# Date:        02/23/2009
# Description: monthcalendar.py wrapper script
#              Used by the monthcalendar-*.py tests
##############################################################################$

import sys
import os
import actions
import states

from Numeric import *
from strongwind import *
from monthcalendar import *


# class to represent the main window.
class MonthCalendarFrame(accessibles.Frame):

    def __init__(self, accessible):
        super(MonthCalendarFrame, self).__init__(accessible)
        #sleep(config.SHORT_DELAY)
        self.monthcalendar = self.findFiller(None)
        self.treetable = self.monthcalendar.findTreeTable(None)
        self.refreshAccessibleRefs()

    def refreshAccessibleRefs (self):
        self.label = self.findLabel(re.compile('^Your selection is.*'))
        self.backbutton = self.treetable.findPushButton('Back by one month')
        self.forwardbutton = self.treetable.findPushButton('Forward by one month')
        self.columnheaders = (self.treetable.findTableColumnHeader('Sun'),
                              self.treetable.findTableColumnHeader('Mon'),
                              self.treetable.findTableColumnHeader('Tue'),
                              self.treetable.findTableColumnHeader('Wed'),
                              self.treetable.findTableColumnHeader('Thu'),
                              self.treetable.findTableColumnHeader('Fri'),
                              self.treetable.findTableColumnHeader('Sat'))
        for i in range(7):
            if self.treetable.getChildAtIndex(i)._accessible != self.columnheaders[i]._accessible:
                raise AssertionError ('Column headers not ordered as expected')

        self.tablecells = [[self.treetable.getChildAtIndex(7*x+y+7) for y in range(7)] for x in range(6)]

    #give 'click' action
    def click(self,monthcalendar):
        monthcalendar.click()

    #check the Label text after click monthcalendar2
    def assertLabel(self, labelText):
        procedurelogger.expectedResult('Label text has been changed to "%s"' % labelText)
        self.findLabel(labelText)
 
    #close application main window after running test
    def quit(self):
        self.altF4()
