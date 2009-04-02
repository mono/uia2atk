
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
        self.column_a = self.findTableColumnHeader(self.COLUMN_A, \
                                                            checkShowing=False)
        self.column_b = self.findTableColumnHeader(self.COLUMN_B, \
                                                            checkShowing=False)
        self.item0 = self.findTableCell("Item0")
        self.num0 = self.findTableCell("0")
        self.item5 = self.findTableCell("Item5")
        self.num5 = self.findTableCell("5")
        # search for initial position for assertOrder test
        self.item0_position = self.item0._getAccessibleCenter()
        self.num0_position = self.num0._getAccessibleCenter()
        self.item5_position = self.item5._getAccessibleCenter()
        self.num5_position = self.num5._getAccessibleCenter()

    # give 'click' action
    def click(self,accessible):
        procedurelogger.action("click %s" % accessible)
        accessible.click()

    # assert Text implementation for ColumnHeader
    def assertText(self, accessible, textvelue):
        procedurelogger.action("check ColumnHeader Text Value")

        procedurelogger.expectedResult('text of %s is %s' \
                                    % (accessible,textvelue))
        assert accessible.text == textvelue

    # assert item's order is changed after click TableColumnHeader
    def assertOrder(self, firstitem=None):        
        if firstitem == "Item5":
            procedurelogger.expectedResult('Item5 and Num5 change position to \
                        %s and %s' % (self.item0_position, self.num0_position))
            item5_new_position = self.item5._getAccessibleCenter()
            num5_new_position = self.num5._getAccessibleCenter()

            assert item5_new_position == self.item0_position and \
                   num5_new_position == self.num0_position
        elif firstitem == "Item0":
            procedurelogger.expectedResult('Item0 and Num0 change position to \
                        %s and %s' % (self.item0_position, self.num0_position))
            item0_new_position = self.item0._getAccessibleCenter()
            num0_new_position = self.num0._getAccessibleCenter()

            assert item0_new_position == self.item0_position and \
                   num0_new_position == self.num0_position

    # assert TableColumnHeaders image
    def assertImageSize(self, accessible, width=16, height=16):
        procedurelogger.action("assert %s's image size" % accessible)
        size = accessible._accessible.queryImage().getImageSize()

        procedurelogger.expectedResult('"%s" image size is %s x %s' %
                                                  (accessible, width, height))

        assert width == size[0], "%s (%s), %s (%s)" %\
                                            ("expected width",
                                              width,
                                             "does not match actual width",
                                              size[0])
        assert height == size[1], "%s (%s), %s (%s)" %\
                                            ("expected height",
                                              height,
                                             "does not match actual height",
                                              size[1]) 
    
    # close application main window after running test
    def quit(self):
        self.altF4()
