
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
        self.list = self.findAllLists(None)
        #listbox1 with CheckOnClick = True
        self.listbox1 = self.list[0]
        #listbox2 with CheckOnClick = False
        self.listbox2 = self.list[1]
        self.listitem = dict([(x, self.findCheckBox(str(x), checkShowing=False)) for x in range(50)])

    #give 'click' action to select item
    def click(self,item):
        item.click()

    #give 'toggle' action to check item
    def toggle(self, item):
        procedurelogger.action('Toggle the "%s"' % (item))
        item.toggle()

    #assert label change after doing click and toggle or mouseClick action
    def assertLabel(self, accessible, newlabel):
        procedurelogger.expectedResult('label is changed to %s' % newlabel)
        assert accessible.text == newlabel

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
