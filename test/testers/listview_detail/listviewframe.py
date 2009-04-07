
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
        tablecell_list = ["Item0", "0", "Item1", "1", "Item2", "2", "Item3", "3", "Item4", "4", "Item5", "5"]
        self.tablecells = dict([(x, self.findTableCell(x)) for x in tablecell_list])
        self.alltablecell = self.findAllTableCells(None)
        #search for initial position for assert order test
        self.item0_position = self.tablecells['Item0']._getAccessibleCenter()
        self.num0_position = self.tablecells['0']._getAccessibleCenter()
        self.item5_position = self.tablecells['Item5']._getAccessibleCenter()
        self.num5_position = self.tablecells['5']._getAccessibleCenter()

        print "item0:%s, item5:%s, num0:%s, num5:%s" % (self.item0_position, \
			self.item5_position, self.num0_position,self.num5_position)

    def click(self,accessible):
        """
        Wrap strongwind click action to add log if the accessible is 
	ColumnHeader

        """
        if accessible == self.column_a or accessible == self.column_b:
            procedurelogger.action("click %s" % accessible)
            accessible.click()
        else:
            accessible.click()

    def assertText(self, accessible):
        """
        Check Text implementation for TableCells and CheckBoxs

        """
        procedurelogger.action("check ListView Items Text Value")

        items = [
                 "Item0", "0", "Item1", "1", 
                 "Item2", "2", "Item3", "3", 
                 "Item4", "4", "Item5", "5"
                ]
        if accessible == self.tablecells:
            for index in items:
                procedurelogger.expectedResult('%s text is %s' \
                                    % (accessible[index],index))
                assert accessible[index].text == index

        elif accessible == self.checkbox:
            for index in range(6):
                procedurelogger.expectedResult('the CheckBox[%s] is %s' \
                                    % (index,"Item" + str(index)))
                assert accessible[index].text == "Item" + str(index)

    def assertSelectionChild(self, accessible, childIndex):
        """
        Check Selection implementation

        """
        procedurelogger.action('selecte childIndex %s in "%s"' % (childIndex, accessible))

        accessible.selectChild(childIndex)

    def assertClearSelection(self, accessible):
        procedurelogger.action('clear selection in "%s"' % (accessible))

        accessible.clearSelection()

    def assertTable(self, accessible, row=0, col=0):
        """
        Check Table implementation for TreeTable to check row and column
        
        """
        procedurelogger.action('check "%s" Table implemetation' % accessible)
        itable = accessible._accessible.queryTable()

        procedurelogger.expectedResult('"%s" have %s Rows and %s Columns' \
				% (accessible, row, col))
        assert itable.nRows == row and itable.nColumns == col, \
				"Not match Rows %s and Columns %s" \ 
				% (itable.nRows, itable.nColumns)

    def assertOrder(self, itemone=None): 
        """
        Check TableCells' order after click TableColumnHeader, itemone means the first TableCell

        """       
        if itemone == "Item5":
            procedurelogger.expectedResult('Item5 and Num5 change position to %s and %s' % (self.item0_position, self.num0_position))
            item5_new_position = self.tablecells['Item5']._getAccessibleCenter()
            num5_new_position = self.tablecells['5']._getAccessibleCenter()

            assert item5_new_position == self.item0_position and \
                   num5_new_position == self.num0_position
        elif itemone == "Item0":
            procedurelogger.expectedResult('Item0 and Num0 change position to %s and %s' % (self.item0_position, self.num0_position))
            item0_new_position = self.tablecells['Item0']._getAccessibleCenter()
            num0_new_position = self.tablecells['0']._getAccessibleCenter()

            assert item0_new_position == self.item0_position and \
                   num0_new_position == self.num0_position

    def enterTextValue(self, accessible, entertext, oldtext=None):
        """
        Change text with some character to make sure EditableText isn't implemented, 
	text doesn't being changed

        """
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
