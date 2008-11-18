
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        11/03/2008
# Description: listview_list.py wrapper script
#              Used by the listview_list-*.py tests
##############################################################################

import sys
import os
import actions
import states

from strongwind import *
from listview_list import *


# class to represent the main window.
class ListViewFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    CHECKBOX = "MultiSelect"

    def __init__(self, accessible):
        super(ListViewFrame, self).__init__(accessible)
        self.label = self.findLabel(re.compile('^View.List mode'))
        self.checkbox = self.findCheckBox(self.CHECKBOX)
        self.list = self.findList(None)
        self.listitem = dict([(x, self.findListItem("Item " + str(x))) for x in range(5)]) 

    #give 'click' action
    def click(self,listitem):
        procedurelogger.action('do click action for %s' % listitem)
        listitem.click()

    #assert Text implementation for ListItem role
    def assertText(self, accessible, item):
        procedurelogger.action("check ListItem's Text Value")

        procedurelogger.expectedResult('the text of "%s" is %s' % (accessible,item))

        assert accessible.text == item

    #assert Selection implementation
    def assertSelectionChild(self, accessible, childIndex):
        procedurelogger.action('selecte childIndex %s in "%s"' % (childIndex, accessible))

        accessible.selectChild(childIndex)

    def assertClearSelection(self, accessible):
        procedurelogger.action('clear selection in "%s"' % (accessible))

        accessible.clearSelection()

    #assert Table implementation for List role to check row and column number is matched
    def assertTable(self, accessible, row=5, col=1):
        procedurelogger.action('check "%s" Table implemetation' % accessible)
        itable = accessible._accessible.queryTable()

        procedurelogger.expectedResult('"%s" have %s Rows and %s Columns' % (accessible, row, col))
        assert itable.nRows == row and itable.nColumns == col, "Not match Rows %s and Columns %s" \
                                                                  % (itable.nRows, itable.nColumns)
    
    #close application main window after running test
    def quit(self):
        self.altF4()
