
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        01/04/2009
# Description: columnheader.py wrapper script
#              Used by the columnheader-*.py tests
##############################################################################

import sys
import os
import actions
import states

from strongwind import *
from columnheader import *


# class to represent the main window.
class ColumnHeaderFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    LABEL = "Click column header to sort the order for items"
    COLUMN_A = "Column A"
    COLUMN_B = "Num"

    def __init__(self, accessible):
        super(ColumnHeaderFrame, self).__init__(accessible)
        self.label = self.findLabel(self.LABEL)
        self.treetable = self.findTreeTable(None)
        self.column_a = self.findTableColumnHeader(self.COLUMN_A)
        self.column_b = self.findTableColumnHeader(self.COLUMN_B)
        self.item0 = self.findTableCell("Item0")
        self.num0 = self.findTableCell("0")
        self.item5 = self.findTableCell("Item5")
        self.num5 = self.findTableCell("5")
        # search for initial position for assertOrder test
        self.top_column_a_position = self.item0._getAccessibleCenter()
        self.top_num_position = self.num0._getAccessibleCenter()
        self.bottom_column_a_position = self.item5._getAccessibleCenter()
        self.bottom_num_position = self.num5._getAccessibleCenter()

    def assertText(self, accessible, expected_text):
        """
        Check TableColumnHeader's Text value is match up to expected_text
        """
        procedurelogger.action("check ColumnHeader text value")

        procedurelogger.expectedResult('text of %s is %s' % \
                                                    (accessible,expected_text))
        actual_text = accessible.text
        assert actual_text == expected_text, \
                  'Text was "%s", expected "%s"' % (actual_text, expected_text)

    def assertSortedOrder(self, is_reversed):  
        """
        Ensure that the order of the rows is correct.  If is_reversed is True,
        the list should be in reverse order.  Otherwise, the list should be
        in descending order.
        """
        procedurelogger.action('Ensure that the rows are in the correct order')

        if is_reversed:
            procedurelogger.expectedResult('The list is reversed.')
            procedurelogger.expectedResult('Item5 and Num5 change position to %s and %s' % (self.top_column_a_position, self.top_num_position))
            item5_new_position = self.item5._getAccessibleCenter()
            num5_new_position = self.num5._getAccessibleCenter()

            assert item5_new_position == self.top_column_a_position and \
                   num5_new_position == self.top_num_position, \
                   "The list is not reversed and it should be"
        else:
            procedurelogger.expectedResult('The list is not reversed.')
            procedurelogger.expectedResult('Item0 and Num0 change position to %s and %s' % (self.top_column_a_position, self.top_num_position))
            item0_new_position = self.item0._getAccessibleCenter()
            num0_new_position = self.num0._getAccessibleCenter()

            assert item0_new_position == self.top_column_a_position and \
                   num0_new_position == self.top_num_position, \
                   "The list is not in descending order and it should be"

    def assertImageSize(self,
                        accessible,
                        expected_width=16,
                        expected_height=16):
        """
        This method be used to check Image implementation for TableColumnHeader
        """
        procedurelogger.action('Ensure "%s" image size is what we expect' % \
                                                                    accessible)
        actual_width, actual_height = \
                             accessible._accessible.queryImage().getImageSize()

        procedurelogger.expectedResult('"%s" image size is %s x %s' % \
                                 (accessible, expected_width, expected_height))

        assert actual_width == expected_width, "%s (%s), %s (%s)" %\
                                            ("expected width",
                                              expected_width,
                                             "does not match actual width",
                                              actual_width)
        assert actual_height == expected_height, "%s (%s), %s (%s)" %\
                                            ("expected height",
                                              expected_height,
                                             "does not match actual height",
                                              actual_height) 
    
    # close application main window after running test
    def quit(self):
        self.altF4()
