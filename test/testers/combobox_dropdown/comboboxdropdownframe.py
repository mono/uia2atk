
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        11/14/2008
# Description: combobox_dropdown.py wrapper script
#              Used by the combobox_dropdown-*.py tests
##############################################################################

import sys
import os
import actions
import states

from strongwind import *
from combobox_dropdown import *


# class to represent the main window.
class ComboBoxDropDownFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    LABEL1 = "You select 1"

    def __init__(self, accessible):
        super(ComboBoxDropDownFrame, self).__init__(accessible)
        self.label1 = self.findLabel(self.LABEL1)
        self.combobox = self.findComboBox(None)
        self.textbox = self.findText(None)

    #check menu action is not implemented
    def menuAction(self, accessible):

        procedurelogger.action('check %s Action' % accessible)
        try:
            accessible._accessible.queryAction()
        except NotImplementedError:
            procedurelogger.expectedResult("Action is unimplemented")

    #give 'click' action
    def click(self,accessible):
        procedurelogger.action('click %s' % accessible)
        accessible.click()

    #give 'press' action
    def press(self,accessible):
        procedurelogger.action('press %s' % accessible)
        accessible.press()
        sleep(config.SHORT_DELAY)

        procedurelogger.expectedResult('menu item list is showing')
        self.menu = self.findMenu(None)
        self.menuitem = dict([(x, self.findMenuItem(str(x), checkShowing=False)) for x in range(10)])

    #check the label after click listitem
    def assertLabel(self, newlabel):
        procedurelogger.expectedResult('Label change to "%s"' % newlabel)

        def resultMatches():
            return self.findLabel(newlabel)
	assert retryUntilTrue(resultMatches)

    #assert Text implementation for MenuItem
    def assertItemText(self, textValue=None):
        procedurelogger.action('check MenuItem\'s Text')

        for textValue in range(10):
            procedurelogger.expectedResult('item "%s"\'s Text is %s' % (self.menuitem[textValue],textValue))
            assert self.menuitem[textValue].text == str(textValue)

    #assert Text value of TextBox after do click action
    def assertText(self, accessible, values):
        procedurelogger.expectedResult('the text of %s is %s' % (accessible,values))
        assert accessible.text == str(values)

    #assert Selection implementation for Menu
    def assertSelectionChild(self, accessible, childIndex):
        procedurelogger.action('selecte childIndex %s in "%s"' % (childIndex, accessible))

        accessible.selectChild(childIndex)

    def assertClearSelection(self, accessible):
        procedurelogger.action('clear selection in "%s"' % (accessible))

        accessible.clearSelection()

    #input value into text box
    def inputText(self, textbox, values):
        textbox.typeText(values)

    #enter Text Value for EditableText
    def enterTextValue(self, textbox, values):
        procedurelogger.action('in %s enter %s' % (textbox, values))
        textbox.text = values

    #assert Streamable Content implementation
    def assertContent(self, accessible):
        procedurelogger.action("Verify Streamable Content for %s" % accessible)
        #text in gtk.textview shows the expected contents
        expect = ['text/plain',]
        result = accessible._accessible.queryStreamableContent().getContentTypes()

        procedurelogger.expectedResult("%s Contents is %s" % (accessible, expect))
        assert result == expect, "Contents %s not match the expected" % result
    
    #close application main window after running test
    def quit(self):
        self.altF4()
