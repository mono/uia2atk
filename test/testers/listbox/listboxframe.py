
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/19/2008
# Description: listbox.py wrapper script
#              Used by the listbox-*.py tests
##############################################################################

import sys
import os
import actions
import states

from strongwind import *
from listbox import *


# class to represent the main window.
class ListBoxFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    LABEL1 = "You select "

    def __init__(self, accessible):
        super(ListBoxFrame, self).__init__(accessible)
        self.label1 = self.findLabel(self.LABEL1)
        self.listbox = self.findList(None)
        self.listitem = dict([(x, self.findListItem(str(x))) for x in range(20)])            

    #give 'click' action
    def click(self,listitem):
        listitem.click()

    #check the label after click listitem
    def assertLabel(self, itemname):
        'Raise exception if the accessible does not match the given result'   
        procedurelogger.expectedResult('item "%s" is %s' % (itemname, 'select'))

        def resultMatches():
            return self.findLabel("You select %s" % itemname)
	
        assert retryUntilTrue(resultMatches)

    #assert Text implementation for ListItem role
    def assertText(self, textValue=None):
        procedurelogger.action('check ListItem\'s Text Value')

        for textValue in range(20):
            procedurelogger.expectedResult('item "%s"\'s Text is %s' % (self.listitem[textValue],textValue))
            assert self.listitem[textValue].text == str(textValue)

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
