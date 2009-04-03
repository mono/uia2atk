
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

    def assertMenuAction(self, accessible):
        """
        Check Menu Action is unimplemented
        """

        procedurelogger.action('check %s Action' % accessible)
        try:
            accessible._accessible.queryAction()
        except NotImplementedError:
            procedurelogger.expectedResult("Action is unimplemented")

    def click(self,accessible):
        """
        Wrap strongwind click action to add log

        """
        procedurelogger.action('click %s' % accessible)
        accessible.click()

    # give 'press' action
    def press(self,accessible):
        """
        Wrap stronging press action for ComboBox, then search for subchildren 
	as expected result

        """
        procedurelogger.action('press %s' % accessible)
        accessible.press()
        sleep(config.SHORT_DELAY)

        procedurelogger.expectedResult('menu item list is showing')
        self.menu = self.findMenu(None)
        self.menuitem = dict([(x, self.findMenuItem(str(x), checkShowing=False)) \
                                                        for x in range(10)])

    def assertLabel(self, newlabel):
        """
        Make sure Label is changed; in this test, Label is showing which 
        MenuItem is selected

        """
        procedurelogger.expectedResult('Label change to "%s"' % newlabel)

        def resultMatches():
            return self.findLabel(newlabel)
	assert retryUntilTrue(resultMatches)

    def assertItemText(self, textValue=None):
        """
        Check MenuItems have correct Text implementation

        """
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

    def assertSelectChild(self, parent, childIndex):
        """
        Select childIndex to test its parent's AtkSelection

        """
        procedurelogger.action('selecte %s childIndex %s' % (parent, childIndex))

        parent.selectChild(childIndex)

    def assertClearSelection(self, accessible):
        procedurelogger.action('clear selection in "%s"' % (accessible))

        accessible.clearSelection()

    def typeMenuItemTest(self, textvalue):
        """
        Imitate key press motion to input MenuItem's name into ComboBox 
	TextBox to change label and Text, expand ComboBox may raise focused 
	and selected for the MenuItem

        """
        # Action
        self.combobox.press()
        sleep(config.SHORT_DELAY)
        self.textbox.typeText(textvalue)
        sleep(config.SHORT_DELAY)

        # Expected result
        # label's text is changed
        self.assertLabel("You select %s" % textvalue)
        # the text of textbox is changed
        self.assertTextChanged(self.textbox, textvalue)
        # press combobox to expand menu, menuitem raise focused and selected
        self.combobox.press()
        sleep(config.SHORT_DELAY)
        statesCheck(self.menuitem[int(textvalue)], "MenuItem", \
                                   add_states=["focused", "selected"])

    def insertMenuItemTest(self, textvalue):
        """
        Use strongwind insertText method to input MenuItem's name into ComboBox 
	TextBox to change label and Text, expand ComboBox may raise focused 
	and selected for the MenuItem

        """
        # Action
        self.combobox.press()
        sleep(config.SHORT_DELAY)
        self.textbox.deleteText()
        sleep(config.SHORT_DELAY)
        self.textbox.insertText(textvalue)

        # Expected result
        # label's text is changed
        self.assertLabel("You select %s" % textvalue)
        # the text of textbox is changed
        self.assertTextChanged(self.textbox, textvalue)
        # press combobox to expand menu, menuitem raise focused and selected
        self.combobox.press()
        sleep(config.SHORT_DELAY)
        statesCheck(self.menuitem[int(textvalue)], "MenuItem", \
                                   add_states=["focused", "selected"])

    def assertContent(self, accessible):
        """
        Check AtkStreamable Content implementation

        """
        procedurelogger.action("Check Streamable Content for %s" % accessible)
        expect = ['text/plain',]
        result = accessible._accessible.queryStreamableContent().getContentTypes()

        procedurelogger.expectedResult("%s Contents is %s" % (accessible, expect))
        assert result == expect, "Contents %s not match the expected %s" % \
                                                               (result, expect)
    
    # close application main window after running test
    def quit(self):
        self.altF4()
