
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
    N_MENU_ITEMS = 10

    def __init__(self, accessible):
        super(ComboBoxDropDownFrame, self).__init__(accessible)
        self.label1 = self.findLabel(self.LABEL1)
        self.combobox = self.findComboBox(None)
        self.textbox = self.findText(None)
        self.menu = self.findMenu(None, checkShowing=False)
        self.menu_items = self.menu.findAllMenuItems(None, checkShowing=False)

    def assertUnimplementedActionInterface(self, accessible):
        """
        Assert that the action interface is unimplemented for the accessible
        """

        procedurelogger.action('check %s\'s action interface' % accessible)
        procedurelogger.expectedResult("action interface is unimplemented")
        try:
            accessible._accessible.queryAction()
        except NotImplementedError:
            return
        assert False, "%s should not implement the action interface" % \
                       (accessible)

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

    def assertLabel(self, newlabel):
        """
        Make sure Label is changed; in this test, Label is showing which 
        MenuItem is selected

        """
        procedurelogger.expectedResult('Label change to "%s"' % newlabel)

        def resultMatches():
            return self.findLabel(newlabel)
	assert retryUntilTrue(resultMatches)

    def assertAllItemTexts(self, textValue=None):
        """
        Ensure that each menu item has the text value that is expected.
        Strongwind uses
        _accessible.queryText().getText(0, _accessible.characterCount) to get
        the text attribute.

        """
        procedurelogger.action('check each menu item\'s text')

        for i in range(10):
            procedurelogger.expectedResult('"%s"\'s Text is %s' % \
                                     (self.menu_items[i],str(i)))
            assert self.menu_items[i].text == str(i)

    # assert TextBox's text after click MenuItem
    def assertText(self, accessible, textvalue):
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
        self.assertText(self.textbox, textvalue)
        # press combobox to expand menu, menu_items raise focused and selected
        self.combobox.press()
        sleep(config.SHORT_DELAY)
        statesCheck(self.menu_items[int(textvalue)], "MenuItem", \
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
        self.assertText(self.textbox, textvalue)
        # press combobox to expand menu, menu_items raise focused and selected
        self.combobox.press()
        sleep(config.SHORT_DELAY)
        statesCheck(self.menu_items[int(textvalue)], "MenuItem", \
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

    def checkAllStates(self, focused_item):
        """
        Check the states of all the menu items of the menu accessible.  The
        focused_item should have +selected +focused states.  All other menu
        items should have -showing -visible states.
        """
        for item in self.menu_items:
            if item is focused_item:
                statesCheck(item,
                            "MenuItem",
                            add_states=["focused", "selected"])
            else:
                statesCheck(item,
                            "MenuItem",
                            invalid_states=["showing"])
    
    # close application main window after running test
    def quit(self):
        self.altF4()
