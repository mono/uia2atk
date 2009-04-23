
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
from helpers import *


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
        self.list = self.findAllTreeTables(None)
        #listbox1 with CheckOnClick = True
        self.listbox1 = self.list[0]
        #listbox2 with CheckOnClick = False
        self.listbox2 = self.list[1]
        
        # This would probably be more clear if findAllCheckBoxes was used.
        # This approach works, but it a not as straightforward.  A simple
        # approach would be to create two variables and then search for
        # all check boxes from each tree table accessible.  For example:
        # self.listbox1.findAllCheckBoxes and self.listbox2.findAllCheckBoxes.
        # Making this change would require a significant rewrite of the test.
        self.listitem = dict([(x, self.findCheckBox(str(x), checkShowing=False)) for x in range(50)])

    # give 'click' action to select item
    def click(self,item):
        item.click()

    # give 'toggle' action to check item
    def toggle(self, item):
        procedurelogger.action('Toggle the "%s"' % (item))
        item.toggle()

    # assert label change after doing click and toggle or mouseClick action
    def assertLabel(self, accessible, newlabel):
        procedurelogger.expectedResult('label is changed to %s' % newlabel)
        assert accessible.text == newlabel

    def selectChildAndCheckStates(self, accessible, childIndex, add_states=[], invalid_states=[]):
        '''
        Select the child at childIndex and then assert that the appropriate
        accessible was indeed selected
        '''
        procedurelogger.action('Select child at index %s in "%s"' % (childIndex, accessible))
        accessible.selectChild(childIndex)
        sleep(config.SHORT_DELAY)
        procedurelogger.expectedResult('Child at index %s is selected and has the appropriate states' % childIndex)
        statesCheck(accessible.getChildAtIndex(childIndex), "ListItem", invalid_states, add_states)

    def clearAccessibleSelection(self, accessible):
        '''
        Clear the selection for the accessible
        '''
        procedurelogger.action('Clear selection in "%s"' % (accessible))
        accessible.clearSelection()
        procedurelogger.expectedResult('%s should have its selected cleared' % accessible)
    
    #close application main window after running test
    def quit(self):
        self.altF4()
