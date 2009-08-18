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
        self.column_a = \
                  self.findTableColumnHeader(self.COLUMN_A, checkShowing=False)
        self.column_b = \
                  self.findTableColumnHeader(self.COLUMN_B, checkShowing=False)
        self.checkboxes = {}
        for i in range(6):
            self.checkboxes[i] = self.findCheckBox('%s%s' % ('Item', i))
        # "Default Group" may need to be added to this list if BUG459054 is
        # invalid
        table_cell_names = ["Item0", "0", "Item1", "1", "Item2", "2", "Item3",
                            "3", "Item4", "4", "Item5", "5"]
        self.table_cells = {}
        for table_cell_name in table_cell_names:
            self.table_cells[table_cell_name] = \
                                            self.findTableCell(table_cell_name)
        self.all_table_cells = self.findAllTableCells(None)
        # "tree table" has extraneous "table cell" BUG459054
        #assert len(self.table_cells) == len(self.all_table_cells), \
        #                    "Found %s table cell(s), expected %s" % \
        #                    (len(self.table_cells), len(self.all_table_cells))
        #search for initial position for assert order test
        self.item0_position = self.table_cells['Item0']._getAccessibleCenter()
        self.num0_position = self.table_cells['0']._getAccessibleCenter()
        self.item5_position = self.table_cells['Item5']._getAccessibleCenter()
        self.num5_position = self.table_cells['5']._getAccessibleCenter()

    def assertDefaultText(self):
        """
        Check Text implementation for TableCells and CheckBoxs
        """
        procedurelogger.action("check ListView Items Text Value")

        for table_cell in self.table_cells.values():
            procedurelogger.expectedResult('%s text is %s' % \
                                               (table_cell, table_cell.name))
            assert table_cell.text == table_cell.name

        for checkbox in self.checkboxes.values():
            procedurelogger.expectedResult('%s text is %s' % \
                                                (checkbox, checkbox.name))
            assert checkbox.text == checkbox.name

    def selectChild(self, accessible, child_index):
        """
        Select accessibles child at the given index.  This method just logs and
        then calls Strongwind's selectChild method.
        """
        procedurelogger.action('select index %s of "%s"' % \
                                                     (child_index, accessible))

        accessible.selectChild(child_index)

    def clearSelection(self, accessible):
        '''
        Log and then call Strongwind's clearSelection method from the
        accessible.
        '''
        procedurelogger.action('clear selection in "%s"' % (accessible))
        accessible.clearSelection()

    def assertTable(self, accessible, row=0, col=0):
        """
        Ensure that the number of rows and number of columns are what is
        expected for the accessible
        """
        procedurelogger.action('Check number of rows and columsn for %s' % accessible)
        itable = accessible._accessible.queryTable()
        procedurelogger.expectedResult('"%s" has %s Row(s) and %s Column(s)' %\
                                                        (accessible, row, col))
        assert itable.nRows == row,\
                        "%s actual rows, expected %s" % (itable.nRows, row)
        assert itable.nColumns == col,\
                        "%s actual cols, expected %s" % (itable.nColumns, col)

    def assertOrder(self, is_ascending=True):
        """
        Check the order of the table cells after clicking the
        TableColumnHeader.
        """       
        if is_ascending:
            procedurelogger.expectedResult('Item0 and Num0 are at position %s and %s respectively' % (self.item0_position, self.num0_position))
            item0_new_position = self.table_cells['Item0']._getAccessibleCenter()
            num0_new_position = self.table_cells['0']._getAccessibleCenter()

            assert item0_new_position == self.item0_position, \
                               "Item0 position was %s, expected %s" % \
                               (item0_new_position, self.item0_position)
            assert num0_new_position == self.num0_position, \
                               "num0 position was %s, expected %s" % \
                               (num0_new_position, self.num0_position)
        else:
            procedurelogger.expectedResult('Item5 and Num5 are at position %s and %s respectively' % (self.item0_position, self.num0_position))
            item5_new_position = self.table_cells['Item5']._getAccessibleCenter()
            num5_new_position = self.table_cells['5']._getAccessibleCenter()

            assert item5_new_position == self.item0_position, \
                               "Item5 position was %s, expected %s" % \
                               (item5_new_position, self.item0_position)
            assert num5_new_position == self.num0_position, \
                               "num5 position was %s, expected %s" % \
                               (num5_new_position, self.num0_position)

    def assertUneditableText(self, accessible, text):
        """
        Attempt to set the accessible's text and then assert that the
        accessibles text did not change.  The accessible should not be
        editable.
        """
        procedurelogger.action('Try to input "%s" in "%s", which is uneditable' % (text, accessible))
        before_text = accessible.text
        try:
            accessible.text = text
        except NotImplementedError:
            pass # a NotImplementedError is acceptable here
        sleep(config.SHORT_DELAY)
        after_text = accessible.text

        procedurelogger.expectedResult('"%s" text still is "%s"' % (accessible, before_text))
        assert before_text == after_text

    
    #close application main window after running test
    def quit(self):
        self.altF4()
