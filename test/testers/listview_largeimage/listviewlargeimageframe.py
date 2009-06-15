
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        11/11/2008
# Description: listview_largeimage.py wrapper script
#              Used by the listview_largeimage-*.py tests
##############################################################################

import sys
import os
import actions
import states

from strongwind import *
from listview_largeimage import *


# class to represent the main window.
class ListViewLargeImageFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    LABEL = "click the CheckBox"

    def __init__(self, accessible):
        super(ListViewLargeImageFrame, self).__init__(accessible)
        self.label = self.findLabel(self.LABEL)
        self.list = self.findTreeTable(None)
        self.listitem = dict([(x, self.findCheckBox("Item" + str(x))) for x in range(6)]) 

    def assertLabel(self, expected_label):
        """
        Raise exception if the accessible does not match the given result.
        mouse click checkbox or do toggle action, label shows which item is 
        checked.
        """            
        procedurelogger.expectedResult('label change to %s' % expected_label)
        actual_label = self.label.text
        assert expected_label == actual_label, \
                                "actual label is: %s, expected label is: %s" % \
                                  (actual_label, expected_label)

    def assertText(self, accessible, expected_text):
        """
        make sure accessible's Text is expected
        """
        procedurelogger.action("check %s's Text Value" % accessible)

        procedurelogger.expectedResult('the text of "%s" is %s' \
                                        % (accessible,expected_text))

        assert accessible.text == expected_text, \
                                "actual text is: %s, expected text is: %s" % \
                                  (accessible.text, expected_text)

    #assert Selection implementation
    def assertSelectionChild(self, accessible, childIndex):
        procedurelogger.action('selecte childIndex %s in "%s"' \
                                        % (childIndex, accessible))

        accessible.selectChild(childIndex)

    def assertClearSelection(self, accessible):
        procedurelogger.action('clear selection in "%s"' % (accessible))

        accessible.clearSelection()

    def assertImageSize(self, accessible, expected_width=64, expected_height=64):
        """
        make sure accessible's Image size is expected
        """
        procedurelogger.action("assert %s's image size" % accessible)
        actual_width, actual_height = \
                              accessible._accessible.queryImage().getImageSize()

        procedurelogger.expectedResult('"%s" image size is %s x %s' %
                                  (accessible, expected_width, expected_height))

        assert expected_width == actual_width, "%s (%s), %s (%s)" %\
                                            ("expected width",
                                              expected_width,
                                             "does not match actual width",
                                              actual_width)
        assert expected_height == actual_height, "%s (%s), %s (%s)" %\
                                            ("expected height",
                                             expected_height,
                                             "does not match actual height",
                                              actual_height)  

    #close application main window after running test
    def quit(self):
        self.altF4()
