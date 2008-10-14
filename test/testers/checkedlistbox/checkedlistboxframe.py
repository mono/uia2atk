
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
        item.toggle()

    #click listitem0-19 would rise checked state and shows which item is checked
    #click listitem20-39 would rise selected state and doesn't show which item 
    #is checked in label text
    def assertLabel(self, listitem, itemname):
        'Raise exception if the accessible does not match the given result'   
            
        if listitem.checked:
            procedurelogger.expectedResult('Item "%s" is %s' % (itemname, 'checked'))
            assert self.label1.text == "Item %s : Checked" % itemname
        elif listitem.selected:
            procedurelogger.expectedResult('Item "%s" is %s' % (itemname, 'selected'))
            assert self.label2.text == "Item : Checked"

    #assert Selection implementation
    def assertSelectionChild(self, accessible, childIndex):
        procedurelogger.action('selecte childIndex %s in "%s"' % (childIndex, accessible))

        accessible.selectChild(childIndex)

    def assertClearSelection(self, accessible):
        procedurelogger.action('clear selection in "%s"' % (accessible))

        accessible.clearSelection()

    #assert Table implementation for List role to check row and column number is matched
    def assertTable(self, accessible, row=20, col=1):
        procedurelogger.action('check "%s" Table implemetation' % accessible)
        itable = accessible._accessible.queryTable()

        procedurelogger.expectedResult('"%s" have %s Rows and %s Columns' % (accessible, row, col))
        assert itable.nRows == row and itable.nColumns == col, "Not match Rows %s and Columns %s" \
                                                                  % (itable.nRows, itable.nColumns)
    
    #close application main window after running test
    def quit(self):
        self.altF4()
