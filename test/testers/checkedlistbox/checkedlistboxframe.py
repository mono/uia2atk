
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/21/2008
# Description: checkedlistbox.py wrapper script
#              Used by the checkedlistbox-*.py tests
##############################################################################

import sys
import os
import actions
import states

from strongwind import *
from checkedlistbox import *


# class to represent the main window.
class CheckedListBoxFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    LABEL1 = "CheckOnClick True"
    LABEL2 = "CheckOnClick False"

    def __init__(self, accessible):
        super(CheckedListBoxFrame, self).__init__(accessible)
        self.label1 = self.findLabel(self.LABEL1)
        self.label2 = self.findLabel(self.LABEL2)
        self.listbox1 = self.findList(self.LABEL1)
        self.listbox2 = self.findList(self.LABEL2)
        self.listitem = dict([(x, self.findCheckBox(str(x))) for x in range(50)])            

    #give 'click' action
    def click(self,item):
        item.click()

    #give 'toggle' action
    def toggle(self, item):
        procedurelogger.action('Toggle the "%s"' % (item))
        item.toggle()

    #click listitem0-19 would rise checked state and showing which items are checked
    #click listitem20-39 would rise selected state and doesn't show which item 
    #is checked in label text, but mouse click two times or click and toggle again
    #would showing which items are checked
    def assertLabel(self, itemnum, itemname):
        'Raise exception if the accessible does not match the given result'   
        if 0 <= itemnum <= 19 and self.listitem[itemnum].checked:
            procedurelogger.expectedResult('Item "%s" is %s' % (itemname, 'checked'))
            assert self.label1.text == "Item %s : Checked" % itemname
        elif itemnum > 19 and self.listitem[itemnum].checked:
            procedurelogger.expectedResult('Item "%s" is %s' % (itemname, 'checked'))
            assert self.label2.text == "Item %s : Checked" % itemname
        elif itemnum > 19 and self.listitem[itemnum].selected:
            procedurelogger.expectedResult('Item "%s" is %s' % (itemname, 'selected'))
            assert self.label2.text == "Item : Checked"

    #assert Selection implementation
    def assertSelectionChild(self, accessible, childIndex):
        procedurelogger.action('selecte childIndex %s in "%s"' % (childIndex, accessible))

        accessible.selectChild(childIndex)

    def assertClearSelection(self, accessible):
        procedurelogger.action('clear selection in "%s"' % (accessible))

        accessible.clearSelection()
    
    #close application main window after running test
    def quit(self):
        self.altF4()
