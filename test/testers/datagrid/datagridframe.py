
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
        # there are 4 columns under TreeTable
        self.bool_column = self.treetable.findTableColumnHeader(self.COLUMN_A, checkShowing=False)
        self.edittext_column = self.treetable.findTableColumnHeader(self.COLUMN_C, checkShowing=False)
        self.readtext_column = self.treetable.findTableColumnHeader(self.COLUMN_B, checkShowing=False)
        self.combobox_column = self.treetable.findTableColumnHeader(self.COLUMN_D, checkShowing=False)
        # there are 4 kind of TableCells under TreeTable
        self.null_cell = self.treetable.findTableCell("(null)", checkShowing=False)
        self.true_cell = self.treetable.findTableCell("True", checkShowing=False)
        self.false_cell = self.treetable.findTableCell("False", checkShowing=False)
        self.edit_cells = self.treetable.findAllTableCells(re.compile("Edit*"), checkShowing=False)
        self.read_cells = self.treetable.findAllTableCells(re.compile("Read*"), checkShowing=False)
        self.combobox_cells = self.treetable.findAllTableCells(re.compile("Box*"), checkShowing=False)
        self.item_menuitems = self.treetable.findAllMenuItems(re.compile("Item*"), checkShowing=False)
        # search for initial position for assert order test
        self.top_read_position = self.read_cells[0]._getAccessibleCenter()
        self.top_edit_position = self.edit_cells[0]._getAccessibleCenter()
        self.bottom_read_position = self.read_cells[2]._getAccessibleCenter()
        self.bottom_edit_position = self.edit_cells[2]._getAccessibleCenter()

    def click(self,accessible):
        """
        Wrap strongwind click action to add log
        """
        procedurelogger.action("click %s" % accessible)
        accessible.click()

    def assertLabel(self, newlabel):
        """
        Assert label is changed to newlabel
        """
        procedurelogger.expectedResult('label is changed to "%s"' % newlabel)

        assert self.label.text == newlabel, "%s not match %s" % \
                                           (self.label.text, newlabel)

    def assertDefaultText(self, column_type, actual=[], expected=[]):
        """
        Ensure that each widgets has the text value that is expected by compare
	actual list and expected list via different testtype
        """
        # test column header type
        if column_type == "ColumnHeader":
            actual = [self.bool_column.text, self.edittext_column.text, self.readtext_column.text, self.combobox_column.text]
            expected = ["BoolColumn", "TextBox_Edit", "TextBox_Read", "ComboBox"]

        # test boolean type
        elif column_type == "BoolColumn":
            actual = [self.null_cell.text, self.true_cell.text, self.false_cell.text]
            expected = ["(null)", "True", "False"]

        # test edit text box type
        elif column_type == "TextBox_Edit":
            for i in range(3):
                actual.append(self.edit_cells[i].text)
                expected.append("Edit%s" % i)

        # test read text box type
        elif column_type == "TextBox_Read":
            for i in range(3):
                actual.append(self.read_cells[i].text)
                expected.append("Read%s" % i)

        # test combobox type type
        elif column_type == "Combobox":
            for i in range(3):
                actual.append(self.box_cells[i].text)
                expected.append("Box%s" % i)                   

        for a, e in zip(actual, expected):
            procedurelogger.action("check Text default Value for %s" % e)

            procedurelogger.expectedResult("%s Text is %s" % (a, e))
            assert a == e, "%s not match %s" % (a, e)

    def assertText(self, accessible, newtext):
        """
        Ensure accessible's Text is changed to newtext
        """
        procedurelogger.expectedResult("%s\' Text is changed to %s" % \
						(accessible, newtext))

        assert accessible.text == newtext, "%s not match %s" % (accessible, newtext)

    def assertSelectionChild(self, accessible, childIndex):
        """
        Select childIndex to test its parent's AtkSelection
        """
        procedurelogger.action('selecte childIndex %s in "%s"' % (childIndex, accessible))

        accessible.selectChild(childIndex)

    def assertClearSelection(self, accessible):
        procedurelogger.action('clear selection in "%s"' % (accessible))

        accessible.clearSelection()

    def assertTable(self, accessible, row=4, col=1):
        """
        Assert AtkTable implementation to ensure TreeTable has row and column 
	number that is expected
        """
        procedurelogger.action('check "%s" Table implemetation' % accessible)
        itable = accessible._accessible.queryTable()

        procedurelogger.expectedResult('"%s" have %s Rows and %s Columns' % (accessible, row, col))
        assert itable.nRows == row and itable.nColumns == col, "Not match Rows %s and Columns %s" \
                                                                  % (itable.nRows, itable.nColumns)

    def assertSortedOrder(self, is_reversed):  
        """
        Ensure that the order of the rows is correct.  If is_reversed is True,
        the list should be in reverse order.  Otherwise, the list should be
        in descending order.
        """
        procedurelogger.action('Ensure that the rows are in the correct order')

        if is_reversed:
            procedurelogger.expectedResult('The list is reversed.')
            procedurelogger.expectedResult('read2 and edit2 change position to %s and %s' % (self.top_read_position, self.top_edit_position))
            read2_new_position = self.read_cells[2]._getAccessibleCenter()
            edit2_new_position = self.edit_cells[2]._getAccessibleCenter()

            assert read2_new_position == self.top_read_position and \
                   edit2_new_position == self.top_edit_position, \
                   "The list is not reversed and it should be"
        else:
            procedurelogger.expectedResult('The list is not reversed.')
            procedurelogger.expectedResult('read0 and edit0 change position to %s and %s' % (self.top_read_position, self.top_edit_position))
            read0_new_position = self.read_cells[0]._getAccessibleCenter()
            edit0_new_position = self.edit_cells[0]._getAccessibleCenter()

            assert read0_new_position == self.top_read_position and \
                   edit0_new_position == self.top_edit_position, \
                   "The list is not in descending order and it should be"


    def assertEditableText(self, accessible, expected_text):
        """
        Check EditableText for accessibles by delete and insert text 
        """
        procedurelogger.action("check the text of %s is editable" % accessible)

        accessible.deleteText()
        sleep(config.SHORT_DELAY)
        accessible.insertText("editable")
        sleep(config.SHORT_DELAY)
        actual_text = accessible.text

        procedurelogger.expectedResult("the text of %s is editable" % accessible)
        assert actual_text == expected_text, \
                                    "actual text is %s, expected text is %s" % \
						(actual_text, expected_text)

    def assertUnEditableText(self, accessible, is_implemented, expected_text):
        """
        make sure accessible's text can't be edited
        """
        # cells under read textbox column have implemented EditableText, but 
        # they can't be edited, so the expected_text may remain the old one.
        # other cells shouldn't implement EditableText
        procedurelogger.action("check the text of %s is uneditable" % accessible)
        procedurelogger.expectedResult("the text of %s is still remain %s" % \
                                                    (accessible, expected_text))

        if is_implemented:
            accessible.insertText("insert something")
            sleep(config.SHORT_DELAY)
            actual_text = accessible.text

            assert actual_text == expected_text, \
                                    "actual text is %s, expected text is %s" % \
						(actual_text, expected_text)    
        else:   
            try:
                accessible._accessible.queryEditableText()
            except NotImplementedError:
                return
            assert False ,"EditableText should not be implemented for %s" % \
                                                                  accessible

    def assertTypeText(self, accessible, expected_text):
        """
        Wrap strongwind typeText to assert if accessible's text is changed.
        """
        accessible.typeText("type something")
        sleep(config.SHORT_DELAY)

        procedurelogger.expectedResult("%s text is %s" % (accessible,expected_text))
        actual_text = accessible.text
        assert actual_text == expected_text, \
                                   "actual text is %s, expected text is %s" % \
                                                    (actual_text, expected_text)
    
    # close application main window after running test
    def quit(self):
        self.altF4()
