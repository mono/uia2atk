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
from splitter_horizontal import *


# class to represent the main window.
class SplitterFrame(accessibles.Frame):

    # CONSTANTS
    LABEL0 = "label0 on one side against splitter"
    LABEL1 = "label1 on one side against splitter"
    LABEL2 = "label2 on one side against splitter"
    LABEL3 = "label3 on one side against splitter"
    LABEL4 = "label4 on the other side against splitter"

    def __init__(self, accessible):
        super(SplitterFrame, self).__init__(accessible)
        self.split_pane = self.findSplitPane(None)
        self.label0 = self.findLabel(self.LABEL0)
        self.label1 = self.findLabel(self.LABEL1)
        self.label2 = self.findLabel(self.LABEL2)
        self.label3 = self.findLabel(self.LABEL3)
        self.label4 = self.findLabel(self.LABEL4)

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

    def assertSplitterMoved(self, accessible, expected_height):
        """
        Make sure assign value will move the split pane by checking size of  
        accessible that is the first node's width is expected height
        """
        new_size = accessible._accessible.queryComponent().getSize()
        actual_height = new_size[1]

        procedurelogger.expectedResult("width of %s is changed to %s" % \
                                                  (accessible, expected_height))
        assert actual_height == expected_height, \
                       "acutal width of tree_table:%s, expected:%s" % \
                           (actual_height, expected_height)

    def quit(self):
        """
        Close the application window
        """
        self.altF4()
