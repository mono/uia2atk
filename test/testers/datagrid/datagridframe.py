
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        2/26/2008
# Description: datagrid.py wrapper script
#              Used by the datagrid-*.py tests
##############################################################################

import sys
import os
import actions
import states

from strongwind import *
from datagrid import *


# class to represent the main window.
class DataGridFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    LABEL = "CurrentCell:"
    COLUMN_A = "BoolColumn"
    COLUMN_B = "TextBox_Read"
    COLUMN_C = "TextBox_Edit"
    COLUMN_D = "ComboBox"

    def __init__(self, accessible):
        super(DataGridFrame, self).__init__(accessible)
        self.label = self.findLabel(self.LABEL)
        self.treetable = self.findTreeTable(None)
        #there are 4 columns under TreeTable
        self.bool_column = self.treetable.findTableColumnHeader(self.COLUMN_A, checkShowing=False)
        self.edittext_column = self.treetable.findTableColumnHeader(self.COLUMN_C, checkShowing=False)
        self.readtext_column = self.treetable.findTableColumnHeader(self.COLUMN_B, checkShowing=False)
        self.combobox_column = self.treetable.findTableColumnHeader(self.COLUMN_D, checkShowing=False)
        #there are 4 kind of TableCells under TreeTable
        self.nullbool_cell = self.treetable.findTableCell("(null)", checkShowing=False)
        self.true_cell = self.treetable.findTableCell("True", checkShowing=False)
        self.false_cell = self.treetable.findTableCell("False", checkShowing=False)
        self.edit_cells = self.treetable.findAllTableCells(re.compile("Edit*"), checkShowing=False)
        self.read_cells = self.treetable.findAllTableCells(re.compile("Read*"), checkShowing=False)
        self.combobox_cells = self.treetable.findAllTableCells(re.compile("Box*"), checkShowing=False)
        self.item_menuitems = self.treetable.findAllMenuItems(re.compile("Item*"), checkShowing=False)
        #search for initial position for assert order test
        self.read0_position = self.read_cells[0]._getAccessibleCenter()
        self.edit0_position = self.edit_cells[0]._getAccessibleCenter()
        self.read2_position = self.read_cells[2]._getAccessibleCenter()
        self.edit2_position = self.edit_cells[2]._getAccessibleCenter()

    #give 'click' action
    def click(self,accessible):
        procedurelogger.action("click %s" % accessible)
        accessible.click()

    #assert Label is changed due to CurrentCellChanged
    def assertLabel(self, newlabel):

        procedurelogger.expectedResult('label is changed to "%s"' % newlabel)

        assert self.label.text == newlabel, "%s not match %s" % \
                                           (self.label.text, newlabel)

    #assert defaul AtkText for all widgets
    def assertDefaultText(self, testtype, actual=[], expected=[]):
        #test column header
        if testtype == "ColumnHeader":
            actual = [self.bool_column.text, self.edittext_column.text, self.readtext_column.text, self.combobox_column.text]
            expected = ["BoolColumn", "TextBox_Edit", "TextBox_Read", "ComboBox"]

        #test boolean
        elif testtype == "BoolColumn":
            actual = [self.nullbool_cell.text, self.true_cell.text, self.false_cell.text]
            expected = ["(null)", "True", "False"]

        #test edit text box
        elif testtype == "TextBox_Edit":
            for i in range(3):
                actual.append(self.edit_cells[i].text)
                expected.append("Edit%s" % i)

        #test read text box
        elif testtype == "TextBox_Read":
            for i in range(3):
                actual.append(self.read_cells[i].text)
                expected.append("Read%s" % i)

        #test combobox type
        elif testtype == "Combobox":
            for i in range(3):
                actual.append(self.box_cells[i].text)
                expected.append("Box%s" % i)                   

        for a, e in zip(actual, expected):
            procedurelogger.action("check Text default Value for %s" % e)

            procedurelogger.expectedResult("%s Text is %s" % (a, e))
            assert a == e, "%s not matsh %s" % (a, e)

    #assert Text implementation for cells
    def assertText(self, accessible, textvalue):
        procedurelogger.action("check %s Text Value" % accessible)

        assert accessible.text == textvalue, "%s not match %s" % (accessible, textvalue)

    #assert Name implementation for cells
    def assertName(self, accessible, namevalue):
        procedurelogger.expectedResult('%s Name is %s' % (accessible, namevalue))

        assert accessible.name == namevalue, "%s not match %s" % (accessible, namevalue)

    #assert Selection implementation
    def assertSelectionChild(self, accessible, childIndex):
        procedurelogger.action('selecte childIndex %s in "%s"' % (childIndex, accessible))

        accessible.selectChild(childIndex)

    def assertClearSelection(self, accessible):
        procedurelogger.action('clear selection in "%s"' % (accessible))

        accessible.clearSelection()

    #assert Table implementation for List role to check row and column number is matched
    def assertTable(self, accessible, row=4, col=1):
        procedurelogger.action('check "%s" Table implemetation' % accessible)
        itable = accessible._accessible.queryTable()

        procedurelogger.expectedResult('"%s" have %s Rows and %s Columns' % (accessible, row, col))
        assert itable.nRows == row and itable.nColumns == col, "Not match Rows %s and Columns %s" \
                                                                  % (itable.nRows, itable.nColumns)

    #assert item's order after click TableColumnHeader
    def assertOrder(self, itemone=None):        
        if itemone == "Read2":
            procedurelogger.expectedResult('Read2 and Edit2 change position to %s and %s' % (self.read0_position, self.edit0_position))
            read2_new_position = self.read_cells[2]._getAccessibleCenter()
            edit2_new_position = self.edit_cells[2]._getAccessibleCenter()

            assert read2_new_position == self.edit0_position and \
                   edit2_new_position == self.edit0_position
        elif itemone == "Read0":
            procedurelogger.expectedResult('Read0 and Edit0 change position to %s and %s' % (self.Read0_position, self.Edit0_position))
            Read0_new_position = self.read_cells[0]._getAccessibleCenter()
            Edit0_new_position = self.edit_cells[1]._getAccessibleCenter()

            assert Read0_new_position == self.Read0_position and \
                   Edit0_new_position == self.Edit0_position

    #enter Text Value to make sure the ReadOnly TextBox is uneditable
    def enterTextValue(self, accessible, entertext, oldtext=None):
        procedurelogger.action('try input %s in %s which is uneditable' % (entertext, accessible))
        if entertext == "uneditable":
            try:
                accessible.text = entertext
            except NotImplementedError:
                pass

            sleep(config.SHORT_DELAY)

            procedurelogger.expectedResult("%s text still is %s" % (accessible, oldtext))
            assert accessible.text == oldtext
        elif entertext == "editable":
            procedurelogger.expectedResult("%s text is %s" % (accessible,entertext)
            assert accessible.text == entertext

    
    #close application main window after running test
    def quit(self):
        self.altF4()
