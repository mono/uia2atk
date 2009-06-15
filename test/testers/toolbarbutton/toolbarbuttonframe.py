##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        02/09/2009
# Description: toolbarbutton.py wrapper script
#              Used by the toolbarbutton-*.py tests
##############################################################################

import sys
import os
import actions
import states

from strongwind import *
from toolbarbutton import *
from helpers import *

# class to represent the main window.
class ToolBarButtonFrame(accessibles.Frame):
    PUSHBUTTON = "PushButton"
    DROPDOWNBUTTON = "DropDownButton"
    TOGGLE = "Toggle"
    UNEDITED = "nop"
    SEPARATOR = "separator"
    LABEL = "ToolBar and ToolBarButton example"
    DROPDOWN_ITEM1 = "Red"
    DROPDOWN_ITEM2 = "Blue"
    NOP_BUTTON_CLICKED_TEXT = "You clicked nop button"

    def __init__(self, accessible):
        super(ToolBarButtonFrame, self).__init__(accessible)
        self.toolbar = self.findToolBar(None)
        self.pushbutton_style = self.toolbar.findPushButton(self.PUSHBUTTON)
        self.dropdown_button = self.toolbar.findPushButton(self.DROPDOWNBUTTON)
        self.dropdown_toggle = \
                             self.toolbar.findToggleButton(self.DROPDOWNBUTTON)
        self.toggle_style = self.toolbar.findToggleButton(self.TOGGLE)
        self.nop_unable = self.toolbar.findPushButton(self.UNEDITED,
                                                      checkShowing=False)
        self.separator_style = self.toolbar.findSeparator(self.SEPARATOR)
        self.label = self.findLabel(self.LABEL)

    def assertText(self, accessible, expected_text):
        """make sure accessible's text is expected_text"""
        procedurelogger.action('check Text for %s' % accessible)
        procedurelogger.expectedResult("%s's Text is %s" % \
                                                   (accessible, expected_text))
        assert accessible.text == expected_text, \
                                   "actual text is %s, expected text is %s" % \
                                               (accessible.text, expected_text)

    def assertLabel(self, expected_label):
        """
        Assert that the label's text is what we expected.  This method be used 
        in below tests to make sure label is changed after click toolbar button.
        """
        procedurelogger.expectedResult("label shows %s" % expected_label)
        assert self.label.text == expected_label, \
                                     "actual label: %s, expected label: %s" % \
                                              (self.label.text, expected_label)

    def TestBasicPushButtonStyle(self, accessible, n_clicks=1):
        """
        Run some basic tests for the Push Button tool bar button.  Ensure that
        the push button accessible has the expected text, perform some actions
        on the accessible, and ensure that the label updates appropriately.
        """
        # test AtkText
        self.assertText(accessible, "PushButton")

        # test AtkAction
        accessible.click(log=True)
        sleep(config.SHORT_DELAY)
        self.assertLabel("You clicked PushButton %s times" % n_clicks)
                                          
        # test AtkComponent by mouse click it to check its position
        accessible.mouseClick()
        sleep(config.SHORT_DELAY)
        self.assertLabel("You clicked PushButton %s times" % (n_clicks + 1))

    def FindDropDownMenuItems(self):
        self.dropdown_window = self.app.findWindow(None)
        self.dropdown_menuitem_red = self.dropdown_window.findMenuItem(self.DROPDOWN_ITEM1)
        self.dropdown_menuitem_blue = self.dropdown_window.findMenuItem(self.DROPDOWN_ITEM2)

    def TestBasicDropDownButtonStyle(self, n_clicks=1):
        """
        Run some basic tests for the DropDownButton tool bar button.  Ensure
        that the push button accessible has the expected text, perform some
        actions on the accessible, and ensure that the label updates
        appropriately.
        """
        # test AtkText
        self.assertText(self.dropdown_button, "DropDownButton")
        # BUG498724: missing AtkText implemented
        #self.assertText(self.dropdown_toggle, "DropDownButton")
        
        # test AtkAction for normal push button doesn't show menu list
        self.dropdown_button.click(log=True)
        sleep(config.SHORT_DELAY)
        self.assertLabel("You clicked the DropDownButton push button %s times" % n_clicks)
        procedurelogger.expectedResult("click %s doesn't show menu list" % \
                                                          self.dropdown_button)
        try:
           self.FindDropDownMenuItems()
        except SearchError:
            pass # expected
        else:
            assert False, "dropdown menu list shouldn't be shown"

        # test AtkAction for toggle button to show menu list
        self.dropdown_toggle.click(log=True)
        sleep(config.SHORT_DELAY)

        procedurelogger.expectedResult("click toggle button to show menu list")
        self.FindDropDownMenuItems()

        #states test for window, menu, menuitems
        statesCheck(self.dropdown_window, "Form", \
                            invalid_states=["resizable"], add_states=["active"])
        statesCheck(self.dropdown_menuitem_red, "MenuItem")
        statesCheck(self.dropdown_menuitem_blue, "MenuItem")

        # click menuitem_red to change label's text
        self.dropdown_menuitem_red.click(log=True)
        sleep(config.SHORT_DELAY)
        self.assertLabel("You selected dropdownbutton item Red")
                                          
        # test AtkComponent by mouse click normal push button to check its
        # position and size
        self.dropdown_button.mouseClick()
        sleep(config.SHORT_DELAY)
        self.assertLabel("You clicked the DropDownButton push button %s times" % (n_clicks + 1))
        procedurelogger.expectedResult("mouse click normal push button doesn't show menu list")
        try:
           self.FindDropDownMenuItems()
        except SearchError:
            pass
        else:
            assert False, "dropdown menu list shouldn't be shown"

        # mouse click toggle button again
        # BUG490105: dropdown_toggle has wrong postion
        '''
        self.dropdown_toggle.mouseClick()
        sleep(config.SHORT_DELAY)

        procedurelogger.expectedResult("mouse click toggle button to show menu list")
        self.FindDropDownMenuItems()

        # click menuitem_blue to change label's text
        self.dropdown_menuitem_blue.click(log=True)
        sleep(config.SHORT_DELAY)
        self.assertLabel("You selected dropdownbutton item Blue")
        '''

    def TestBasicToggleStyle(self, accessible):
        """
        Run some basic tests for the ToggleStyle tool bar button.
        """
        # test AtkText
        self.assertText(accessible, "Toggle")

        # clicking on the toggle button should disable the label
        accessible.click(log=True)
        sleep(config.SHORT_DELAY)
        statesCheck(self.toggle_style,
                    "Button",
                    add_states=["armed", "checked"])
        assert not self.label.sensitive

        # click again to enable label
        accessible.click()
        sleep(config.SHORT_DELAY)
        statesCheck(self.toggle_style, "Button")
        assert self.label.sensitive
                                          
        # test AtkComponent by mouse click to check its position
        accessible.mouseClick()
        sleep(config.SHORT_DELAY)
        statesCheck(self.toggle_style, "Button", add_states=["armed", "checked"])
        assert not self.label.sensitive

    def TestBasicInsensitiveButton(self, accessible):
        """ Ensure that an insensitive button behaves as expected"""
        # test AtkText
        self.assertText(accessible, "nop")

        # test AtkAction for nop button doesn't change label's text
        current_label = self.label.text
        assert current_label != self.NOP_BUTTON_CLICKED_TEXT, 'This method cannot be called when the current label already reads "%s"' % self.NOP_BUTTON_CLICKED_TEXT
        try:
            accessible.click()
        except NotSensitiveError:
            pass
        procedurelogger.expectedResult("click unable nop button doesn't change label")
        assert self.label.text == current_label, \
                                'Label was changed to "%s", expected "%s"' % \
                                (self.label.text, self.NOP_BUTTON_CLICKED_TEXT)

    def TestBasicSeparatorStyle(self, accessible):
        """Ensure the desired basic functionality for the separator"""
        # test AtkText
        procedurelogger.action('check Text for %s' % accessible)
        procedurelogger.expectedResult("AtkText is unimplemented")
        try:
            accessible._accessible.queryText()
        except NotImplementedError:
            pass
        else:
            assert False, "AtkText shouldn't implemented"

        # AtkAction is unimplementd for separator style toolbarbutton
        procedurelogger.action('check Action for %s' % accessible)
        procedurelogger.expectedResult("AtkAction is unimplemented")
        try:
           accessible._accessible.queryAction()
        except NotImplementedError:
            pass
        else:
            assert False, "AtkAction shouldn't implemented"
    
    def assertImageSize(self, accessible, expected_width=0, expected_height=0):
        """make sure accessible's image size is expected """
        procedurelogger.action("assert %s's image size" % accessible)
        if accessible is self.separator_style:
            procedurelogger.expectedResult('%s image is unimplemented' % accessible)
            try:
                accessible._accessible.queryImage()
            except NotImplementedError:
                return
            assert False, "Separator should not implement the Image interface"

        else:
            actual_width, actual_height = accessible.imageSize
            procedurelogger.expectedResult('"%s" image size is %s x %s' %
                                 (accessible, expected_width, expected_height))
            assert actual_width == expected_width, "%s (%s), %s (%s)" %\
                                            ("expected width",
                                              expected_width,
                                             "does not match actual width",
                                              actual_width)
            assert actual_height == expected_height, "%s (%s), %s (%s)" %\
                                            ("expected height",
                                              expected_height,
                                             "does not match actual height",
                                              actual_height)

    # close main window
    def quit(self):
        self.altF4()
