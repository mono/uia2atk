
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        12/15/2008
# Description: listview_detail.py wrapper script
#              Used by the listview_detail-*.py tests
##############################################################################

import sys
import os
import actions
import states

from strongwind import *
from listview_detail import *


# class to represent the main window.
class ListViewFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    LABEL = "click the CheckBox to sum the num"
    COLUMN_A = "Column A"
    COLUMN_B = "Num"

    def __init__(self, accessible):
        super(ListViewFrame, self).__init__(accessible)
        self.label = self.findLabel(self.LABEL)
        self.treetable = self.findTreeTable(None)
        self.column_a = self.findTableColumnHeader(self.COLUMN_A, checkShowing=False)
        self.column_b = self.findTableColumnHeader(self.COLUMN_B, checkShowing=False)
        self.checkbox = dict([(x, self.findCheckBox("Item" + str(x))) for x in range(6)])
        self.tablecells = self.findAllTableCells(None)
        #search for initial position for assert order test
        self.item0_position = self.tablecells[0]._getAccessibleCenter()
        self.num0_position = self.tablecells[1]._getAccessibleCenter()
        self.item5_position = self.tablecells[10]._getAccessibleCenter()
        self.num5_position = self.tablecells[11]._getAccessibleCenter()

        print "item0:%s, item5:%s, num0:%s, num5:%s" % (self.item0_position,self.item5_position, self.num0_position,self.num5_position)

    #give 'click' action
    def click(self,accessible):
        accessible.click()

    #assert Text implementation for ListView Items
    def assertText(self, accessible):
        procedurelogger.action("check ListView Items Text Value")

        items = ["Item0", "0", "Item1", "1", "Item2", "2", "Item3", "3", "Item4", "4", "Item5", "5"]
        if accessible == self.tablecells:
            for index in range(12):
                procedurelogger.expectedResult('the text[%s] is %s' \
                                    % (index,items[index]))
                assert accessible[index].text == items[index]

        elif accessible == self.checkbox:
            for index in range(6):
                procedurelogger.expectedResult('the CheckBox[%s] is %s' \
                                    % (index,"Item" + str(index)))
                assert accessible[index].text == "Item" + str(index)

    #assert Selection implementation
    def assertSelectionChild(self, accessible, childIndex):
        procedurelogger.action('selecte childIndex %s in "%s"' % (childIndex, accessible))

        accessible.selectChild(childIndex)

    def assertClearSelection(self, accessible):
        procedurelogger.action('clear selection in "%s"' % (accessible))

        accessible.clearSelection()

    #assert Table implementation for List role to check row and column number is matched
    def assertTable(self, accessible, row=0, col=0):
        procedurelogger.action('check "%s" Table implemetation' % accessible)
        itable = accessible._accessible.queryTable()

        procedurelogger.expectedResult('"%s" have %s Rows and %s Columns' % (accessible, row, col))
        assert itable.nRows == row and itable.nColumns == col, "Not match Rows %s and Columns %s" \
                                                                  % (itable.nRows, itable.nColumns)

    #assert item's order after click TableColumnHeader
    def assertOrder(self, itemone=None):        
        if itemone == "Item5":
            procedurelogger.expectedResult('Item5 and Num5 change position to %s and %s' % (self.item0_position, self.num0_position))
            item5_new_position = self.tablecells[10]._getAccessibleCenter()
            num5_new_position = self.tablecells[11]._getAccessibleCenter()

            assert item5_new_position == self.item0_position and \
                   num5_new_position == self.num0_position
        elif itemone == "Item0":
            procedurelogger.expectedResult('Item0 and Num0 change position to %s and %s' % (self.item0_position, self.num0_position))
            item0_new_position = self.tablecells[0]._getAccessibleCenter()
            num0_new_position = self.tablecells[1]._getAccessibleCenter()

            assert item0_new_position == self.item0_position and \
                   num0_new_position == self.num0_position

    #enter Text Value to make sure the text is uneditable
    def enterTextValue(self, accessible, entertext, oldtext=None):
        procedurelogger.action('try input %s in %s which is uneditable' % (entertext, accessible))

        try:
            accessible.text = entertext
        except NotImplementedError:
            pass

        sleep(config.SHORT_DELAY)

        procedurelogger.expectedResult("%s text still is %s" % (accessible, oldtext))
        assert accessible.text == oldtext

    
    #close application main window after running test
    def quit(self):
        self.altF4()
