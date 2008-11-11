
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
        self.list = self.findList(None)
        self.listitem = dict([(x, self.findCheckBox("Item" + str(x))) for x in range(6)]) 

    #give 'click' action
    def click(self,item):
        item.click()

    #give 'toggle' action
    def toggle(self, item):
        procedurelogger.action('do toggle action for %s' % item)
        item.toggle()

    #mouse click checkbox or do toggle action would rise checked state and shows 
    #which item is checked
    def assertLabel(self, item, itemname):
        'Raise exception if the accessible does not match the given result'   
            
        if item.checked:
            procedurelogger.expectedResult('Item "%s" is %s' % (item, 'checked'))
            assert self.label.text == "%s Checked" % itemname
        elif not item.checked:
            procedurelogger.expectedResult('Item "%s" is %s' % (item, 'Unchecked'))
            assert self.label.text == "%s Unchecked" % itemname

    #assert Text implementation for ListItem role
    def assertText(self, accessible, item):
        procedurelogger.action("check ListItem's Text Value")

        procedurelogger.expectedResult('the text of "%s" is %s' \
                                        % (accessible,item))

        assert accessible.text == item

    #assert Selection implementation
    def assertSelectionChild(self, accessible, childIndex):
        procedurelogger.action('selecte childIndex %s in "%s"' \
                                        % (childIndex, accessible))

        accessible.selectChild(childIndex)

    def assertClearSelection(self, accessible):
        procedurelogger.action('clear selection in "%s"' % (accessible))

        accessible.clearSelection()

    # assert the size of an image to test image implementation
    def assertImageSize(self, item, width=64, height=64):
        procedurelogger.action("assert %s's image size" % item)
        size = item._accessible.queryImage().getImageSize()

        procedurelogger.expectedResult('"%s" image size is %s x %s' %
                                                  (item, width, height))

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

    #close application main window after running test
    def quit(self):
        self.altF4()
