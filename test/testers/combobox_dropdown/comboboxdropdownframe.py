
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
from helpers import *


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

    # check menu action is not implemented
    def menuAction(self, accessible):

        procedurelogger.action('check %s Action' % accessible)
        try:
            accessible._accessible.queryAction()
        except NotImplementedError:
            procedurelogger.expectedResult("Action is unimplemented")

    # give 'click' action
    def click(self,accessible):
        procedurelogger.action('click %s' % accessible)
        accessible.click()

    # give 'press' action
    def press(self,accessible):
        procedurelogger.action('press %s' % accessible)
        accessible.press()
        sleep(config.SHORT_DELAY)

        procedurelogger.expectedResult('menu item list is showing')
        self.menu = self.findMenu(None)
        self.menuitem = dict([(x, self.findMenuItem(str(x), checkShowing=False)) \
                                                        for x in range(10)])

    # assert label change after select menu item
    def assertLabel(self, newlabel):
        procedurelogger.expectedResult('Label change to "%s"' % newlabel)

        def resultMatches():
            return self.findLabel(newlabel)
	assert retryUntilTrue(resultMatches)

    # assert Text implementation for MenuItem
    def assertItemText(self, textValue=None):
        procedurelogger.action('check MenuItem\'s Text')

        for textValue in range(10):
            procedurelogger.expectedResult('"%s"\'s Text is %s' % \
                                     (self.menuitem[textValue],str(textValue)))
            assert self.menuitem[textValue].text == str(textValue)

    # assert TextBox's text after click MenuItem
    def assertTextChanged(self, accessible, textvalue):
        procedurelogger.expectedResult('%s is change to %s' % \
                                                      (accessible, textvalue))
        assert accessible.text == textvalue

    # assert Selection implementation for ComboBox and Menu
    def assertSelectionChild(self, accessible, childIndex):
        procedurelogger.action('selecte childIndex %s in "%s"' % \
                                                      (childIndex, accessible))

        accessible.selectChild(childIndex)

    def assertClearSelection(self, accessible):
        procedurelogger.action('clear selection in "%s"' % (accessible))

        accessible.clearSelection()

    # type MenuItem's name into text box to change label and Text, expand 
    # combobox may raise focused and selected for the MenuItem
    def typeMenuItem(self, textvalue):
        self.combobox.press()
        sleep(config.SHORT_DELAY)
        self.textbox.typeText(textvalue)
        sleep(config.SHORT_DELAY)

        # label's text is changed
        self.assertLabel("You select %s" % textvalue)
        # the text of textbox is changed
        self.assertTextChanged(self.textbox, textvalue)
        # press combobox to expand menu, menuitem raise focused and selected
        self.combobox.press()
        sleep(config.SHORT_DELAY)
        statesCheck(self.menuitem[int(textvalue)], "MenuItem", \
                                   add_states=["focused", "selected"])

    # insert MenuItem's name into text box to change label and Text, expand 
    # combobox may raise focused and selected for the MenuItem
    def insertMenuItem(self, textvalue):
        self.combobox.press()
        sleep(config.SHORT_DELAY)
        self.textbox.deleteText()
        sleep(config.SHORT_DELAY)
        self.textbox.insertText(textvalue)

        # label's text is changed
        self.assertLabel("You select %s" % textvalue)
        # the text of textbox is changed
        self.assertTextChanged(self.textbox, textvalue)
        # press combobox to expand menu, menuitem raise focused and selected
        self.combobox.press()
        sleep(config.SHORT_DELAY)
        statesCheck(self.menuitem[int(textvalue)], "MenuItem", \
                                   add_states=["focused", "selected"])

    # assert Streamable Content implementation
    def assertContent(self, accessible):
        procedurelogger.action("Verify Streamable Content for %s" % accessible)
        expect = ['text/plain',]
        result = accessible._accessible.queryStreamableContent().getContentTypes()

        procedurelogger.expectedResult("%s Contents is %s" % (accessible, expect))
        assert result == expect, "Contents %s not match the expected %s" % \
                                                               (result, expect)
    
    # close application main window after running test
    def quit(self):
        self.altF4()
