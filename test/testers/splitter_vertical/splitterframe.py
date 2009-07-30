##############################################################################
# Written by:  Brian G. Merrell <bgmerrell@novell.com>
# Date:        01/26/2009
# Description: splitter.py wrapper script
#              Used by the splitter-*.py tests
##############################################################################

import sys
import os
import actions
import states

from strongwind import *
from splitter_vertical import *


# class to represent the main window.
class SplitterFrame(accessibles.Frame):

    # Contants
    NODE0 = "TreeView Node"
    NODE1 = "Another Node"
    RIGHT_BUTTON = "Right Side"

    def __init__(self, accessible):
        super(SplitterFrame, self).__init__(accessible)
        self.split_pane = self.findSplitPane("")
        self.tree_table = self.findTreeTable("")
        self.table_cell0 = self.findTableCell(self.NODE0)
        self.tabel_cell1 = self.findTableCell(self.NODE1)
        self.push_button = self.findPushButton(self.RIGHT_BUTTON)

    def assignValue(self, expected_value):
        """Assign expected value for split pane"""
        procedurelogger.action('set split pane to value %s' %  expected_value)
        self.split_pane.value = expected_value

    def assertValue(self, expected_value):
        """Make sure split pane's value is expected"""
        actual_value = self.split_pane.value

        procedurelogger.expectedResult("split pane's value is %s" % \
                                                               expected_value)
        assert actual_value == expected_value, \
                                "actual value:%s, expected:%s" % \
                                 (actual_value, expected_value)

    def assertSplitterMoved(self, accessible, expected_width):
        """
        Make sure assign value will move the split pane by checking size of  
        accessible that is the first node's width is expected width
        """
        new_size = accessible._accessible.queryComponent().getSize()
        actual_width = new_size[0]

        procedurelogger.expectedResult("width of %s is changed to %s" % \
                                                  (accessible, expected_width))
        assert actual_width == expected_width, \
                       "acutal width of tree_table:%s, expected:%s" % \
                           (actual_width, expected_width)

    def quit(self):
        """
        Close the application window
        """
        self.altF4()
