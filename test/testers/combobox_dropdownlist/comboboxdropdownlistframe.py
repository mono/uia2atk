
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        11/10/2008
# Description: combobox_simple.py wrapper script
#              Used by the combobox_simple-*.py tests
##############################################################################

import sys
import os
import actions
import states

from strongwind import *
from combobox_dropdownlist import *


# class to represent the main window.
class ComboBoxDropDownListFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    LABEL1 = "You select "

    def __init__(self, accessible):
        super(ComboBoxDropDownListFrame, self).__init__(accessible)
        self.label1 = self.findLabel(self.LABEL1)
        self.combobox = self.findComboBox(None)
        #self.menu = self.findMenu(None)
        #self.menuitem = dict([(x, self.findMenuItem(str(x))) for x in range(10)])

    #give 'click' action
    def click(self,accessible):
        accessible.click()

    #give 'press' action
    def press(self,accessible):
        accessible.press()

    #check the label after click listitem
    def assertLabel(self, itemname):
        procedurelogger.expectedResult('item "%s" is %s' % (itemname, 'select'))

        def resultMatches():
            return self.findLabel("You select %s" % itemname)
	assert retryUntilTrue(resultMatches)

    #assert Text implementation for MenuItem
    def assertItemText(self, textValue=None):
        procedurelogger.action('check MenuItem\'s Text Value')

        for textValue in range(10):
            procedurelogger.expectedResult('item "%s"\'s Text is %s' % (self.menuitem[textValue],textValue))
            assert self.menuitem[textValue].text == str(textValue)

    #assert Text value after do click action
    def assertText(self, accessible, value):
        procedurelogger.expectedResult('the text value of %s is %s' % (accessible,values))
        assert accessible.text == str(values)

    #assert Selection implementation for Menu
    def assertSelectionChild(self, accessible, childIndex):
        procedurelogger.action('selecte childIndex %s in "%s"' % (childIndex, accessible))

        accessible.selectChild(childIndex)

    def assertClearSelection(self, accessible):
        procedurelogger.action('clear selection in "%s"' % (accessible))

        accessible.clearSelection()

    
    #close application main window after running test
    def quit(self):
        self.altF4()
