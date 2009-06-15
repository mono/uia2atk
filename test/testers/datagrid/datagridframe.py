
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
        # find all the column headers and store them in a single list
        self.column_headers = self.findAllTableColumnHeaders(None)
        # find all the table cells and store them in a single list
        self.table_cells = self.findAllTableCells(None)
        # find the column headers and table cells individually so we can test
        # the headers and cells more granularly
        # there are 4 columns under TreeTable
        self.bool_column = \
                       self.treetable.findTableColumnHeader(self.COLUMN_A)
        self.edittext_column = \
                       self.treetable.findTableColumnHeader(self.COLUMN_C)
        self.readtext_column = \
                       self.treetable.findTableColumnHeader(self.COLUMN_B)
        self.combobox_column = \
                       self.treetable.findTableColumnHeader(self.COLUMN_D)
        # there are 4 kind of TableCells under TreeTable
        self.null_cell = self.treetable.findTableCell("(null)")
        self.true_cell = self.treetable.findTableCell("True")
        self.false_cell = self.treetable.findTableCell("False")

        # find the table cells of each table and ensure that they have the
        # correct names
        self.edit_cells = \
                      self.treetable.findAllTableCells(re.compile("Edit[0-9]"))
        for i, edit_cell in enumerate(self.edit_cells):
            self.assertName(edit_cell, "%s%s" % ("Edit", i))

        self.read_cells = \
                      self.treetable.findAllTableCells(re.compile("Read[0-9]"))
        for i, read_cell in enumerate(self.read_cells):
            self.assertName(read_cell, "%s%s" % ("Read", i))

        self.combobox_cells = \
                       self.treetable.findAllTableCells(re.compile("Box[0-9]"))
        for i, combobox_cell in enumerate(self.combobox_cells):
            self.assertName(combobox_cell, "%s%s" % ("Box", i))

        self.item_menuitems = \
                       self.treetable.findAllMenuItems(re.compile("Item[0-9]"),
                                                       checkShowing=False)
        for i, item_menuitem in enumerate(self.item_menuitems):
            self.assertName(item_menuitem, "%s%s" % ("Item", i))


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

    def assertLabel(self, expected_text):
        """
        Assert that the text of the label matches the expected text
        """
        procedurelogger.expectedResult('label is changed to "%s"' % \
                                                                 expected_text)

        assert self.label.text == expected_text, \
                                       'Label text was "%s", expected "%s"' % \
                                               (self.label.text, expected_text)

    def assertDefaultText(self):
        """
        Ensure that the DataGrid accessibles' text is what is expected. 
        """
        # All of the children of the tree table accessible have text that
        # are the same as their names.  This behavior mimics GAIL.  Therefore, 
        # we can simply ensure that the text matches the names, because the
        #names have already been verified in __init__
        self.assertText(self.label, self.label.name)
        self.assertText(self.bool_column, self.bool_column.name)
        self.assertText(self.edittext_column, self.edittext_column.name)
        self.assertText(self.readtext_column, self.readtext_column.name)
        self.assertText(self.combobox_column, self.combobox_column.name)
        self.assertText(self.null_cell, self.null_cell.name)
        self.assertText(self.true_cell, self.true_cell.name)
        self.assertText(self.false_cell, self.false_cell.name)
        for i, edit_cell in enumerate(self.edit_cells):
            self.assertText(edit_cell, "%s%s" % ("Edit", i))
        for i, read_cell in enumerate(self.read_cells):
            self.assertText(read_cell, "%s%s" % ("Read", i))
        for i, combobox_cell in enumerate(self.combobox_cells):
            self.assertText(combobox_cell, "%s%s" % ("Box", i))
        for i, item_menuitem in enumerate(self.item_menuitems):
            self.assertText(item_menuitem, "%s%s" % ("Item", i))
        # the tree table accessible should does not 

    def assertText(self, accessible, expected_text):
        """
        Ensure accessible's Text is changed to expected_text
        """
        procedurelogger.expectedResult("%s\'s Text is changed to %s" % \
                                                   (accessible, expected_text))
        assert accessible.text == expected_text, \
                                           'Text was "%s", expected "%s"' % \
                                           (accessible.text, expected_text)

    def assertName(self, accessible, expected_name):
        """
        Ensure accessible's name is changed to expected_name
        """
        procedurelogger.expectedResult("%s\'s name is changed to %s" % \
                                                   (accessible, expected_name))
        assert accessible.name == expected_name, \
                                           'Name was "%s", expected "%s"' % \
                                           (accessible.name, expected_name)

    def selectChild(self, accessible, childIndex):
        """
        Print a log and then call Strongwind's selectChild method
        """
        procedurelogger.action('selecte childIndex %s in "%s"' % \
                                                    (childIndex, accessible))
        accessible.selectChild(childIndex)

    def clearSelection(self, accessible):
        '''
        Print a log and then call Strongwind's clearSelection method
        '''
        procedurelogger.action('clear selection in "%s"' % (accessible))
        accessible.clearSelection()

    def assertRowAndColumnCount(self, accessible, row, col):
        """
        Assert that the accessible has the expected number of rows and columns
        """
        procedurelogger.action('check row and column count of "%s"' % accessible)
        itable = accessible._accessible.queryTable()

        procedurelogger.expectedResult('"%s" has %s row(s) and %s column(s)' %\
                                                        (accessible, row, col))
        assert itable.nRows == row, \
                                  "Had %s row(s), expected %s" % (itable.nRows)
        assert itable.nColumns == col, \
                            "Had %s column(s), expected %s" % (itable.nColumns)

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
        Make sure accessible's text can't be edited
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
            assert False, "EditableText should not be implemented for %s" % \
                                                                     accessible

    def assertTypeText(self, accessible, expected_text):
        """
        Call strongwind's typeText method and assert if accessible's text is
        changed
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
