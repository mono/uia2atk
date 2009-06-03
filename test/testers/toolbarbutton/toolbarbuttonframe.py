
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


    def __init__(self, accessible):
        super(ToolBarButtonFrame, self).__init__(accessible)
        self.toolbar = self.findToolBar(None)
        self.pushbutton_style = self.toolbar.findPushButton(self.PUSHBUTTON)
        self.dropdown_button = self.toolbar.findPushButton(self.DROPDOWNBUTTON)
        self.dropdown_toggle = self.toolbar.findToggleButton(self.DROPDOWNBUTTON)
        self.toggle_style = self.toolbar.findToggleButton(self.TOGGLE)
        self.nop_unable = self.toolbar.findPushButton(self.UNEDITED,\
                                                  checkShowing=False)
        self.separator_style = self.toolbar.findSeparator(self.SEPARATOR)
        self.label = self.findLabel(self.LABEL)

    def assertText(self, accessible, expected_text):
        """make sure accessible's text is expected_text"""
        procedurelogger.action('check Text for %s' % accessible)

        procedurelogger.expectedResult(" %s's Text is %s" % (accessible, expected_text))
        assert accessible.text == expected_text, \
                                    "actual text is %s, expected text is %s" % \
                                    (accessible.text, expected_text)

    def assertLabel(self, expected_label):
        """
        this method will be used in below tests to make sure label is changed 
	after click toolbar button
        """
        procedurelogger.expectedResult("label shows %s" % expected_label)
        assert self.label.text == expected_label, \
                                      "actual label: %s, expected label: %s" % \
                                             (self.label.text, expected_label)

    def PushButtonStyle(self, accessible):
        """test wraper for ToolBarButton with PushButton style"""
        # test AtkText
        self.assertText(accessible, "PushButton")

        # test AtkAction
        accessible.click(log=True)
        sleep(config.SHORT_DELAY)
        self.assertLabel("You clicked PushButton 1 times")
                                          
        # test AtkComponent by mouse click it to check its position
        accessible.mouseClick()
        sleep(config.SHORT_DELAY)
        self.assertLabel("You clicked PushButton 2 times")

    def CheckDropDownMenuItems(self):
        self.dropdown_window = self.app.findWindow(None)
        self.dropdown_menuitem_red = self.dropdown_window.findMenuItem(self.DROPDOWN_ITEM1)
        self.dropdown_menuitem_blue = self.dropdown_window.findMenuItem(self.DROPDOWN_ITEM2)

    def DropDownButtonStyle(self):
        """test wraper for ToolBarButton with DropDownButton style"""
        # test AtkText
        self.assertText(self.dropdown_button, "DropDownButton")
        # BUG498724: missing AtkText implemented
        #self.assertText(self.dropdown_toggle, "DropDownButton")
        
        # test AtkAction for normal push button doesn't show menu list
        self.dropdown_button.click(log=True)
        sleep(config.SHORT_DELAY)
        procedurelogger.expectedResult("click %s doesn't show menu list" % \
                                                           self.dropdown_button)
        try:
           self.CheckDropDownMenuItems()
        except SearchError:
            return
        assert False, "dropdown menu list shouldn't be shown"

        # test AtkAction for toggle button to show menu list
        self.dropdown_toggle.click(log=True)
        sleep(config.SHORT_DELAY)

        procedurelogger.expectedResult("click toggle button to show menu list")
        self.CheckDropDownMenuItems()

        #states test for window, menu, menuitems
        statesCheck(self.dropdown_window, "Form", \
                            invalid_states=["resizable"], add_states=["active"])
        statesCheck(self.dropdown_menuitem_red, "MenuItem")
        statesCheck(self.dropdown_menuitem_blue, "MenuItem")

        # click menuitem_red to change label's text
        self.dropdown_menuitem_red.click(log=True)
        sleep(config.SHORT_DELAY)
        self.assertLabel("You selected dropdownbutton item Red")
                                          
        # test AtkComponent by mouse click normal push button to check its position and size
        self.dropdown_menuitem_button.mouseClick()
        sleep(config.SHORT_DELAY)

        procedurelogger.expectedResult("mouse click normal push button doesn't show menu list")
        try:
           self.CheckDropDownMenuItems()
        except SearchError:
            return
        assert False, "dropdown menu list shouldn't be shown"

        # mouse click toggle button again
        # BUG490105: dropdown_toggle has wrong postion
        '''
        self.dropdown_toggle.mouseClick()
        sleep(config.SHORT_DELAY)

        procedurelogger.expectedResult("mouse click toggle button to show menu list")
        self.CheckDropDownMenuItems()

        # click menuitem_blue to change label's text
        self.dropdown_menuitem_blue.click(log=True)
        sleep(config.SHORT_DELAY)
        self.assertLabel("You selected dropdownbutton item Blue")
        '''
    def ToggleStyle(self, accessible):
        """test wraper for ToolBarButton with Toggle style"""
        # test AtkText
        self.assertText(accessible, "Toggle")

        # test AtkAction to unable label
        accessible.click(log=True)
        sleep(config.SHORT_DELAY)
        statesCheck(self.toggle_style, "Button", add_states=["armed", "checked"])
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

    def UnsensitiveButton(self, accessible):
        """test wraper for unable ToolBarButton"""
        # test AtkText
        self.assertText(accessible, "nop")

        # test AtkAction for nop button doesn't change label's text
        current_label = self.label.text
        try:
            accessible.click()
        except NotSensitiveError:
            pass
        procedurelogger.expectedResult("click unable nop button doesn't change label")
        assert self.label.text == current_label, "label is changed to %s" % \
                                                     self.label.text

    def SeparatorStyle(self, accessible):
        """test wraper for ToolBarButton with Separator style"""
        # test AtkText
        procedurelogger.action('check Text for %s' % accessible)

        procedurelogger.expectedResult("AtkText is unimplemented")
        try:
            accessible._accessible.queryText()
        except NotImplementedError:
            return
        assert False, "AtkText shouldn't implemented"

        # AtkAction is unimplementd for separator style toolbarbutton
        procedurelogger.action('check Action for %s' % accessible)
        procedurelogger.expectedResult("AtkAction is unimplemented")
        try:
           accessible._accessible.queryAction()
        except NotImplementedError:
            return
        assert False, "AtkAction shouldn't implemented"
    
    def assertImageSize(self, accessible, width=0, height=0):
        """make sure accessible's image size is expected """
        procedurelogger.action("assert %s's image size" % accessible)
        if accessible == self.separator_style:
            procedurelogger.expectedResult('%s image is unimplemented' % accessible)
            try:
                accessible._accessible.queryImage()
            except NotImplementedError:
                return
            assert False

        else:
            size = accessible.imageSize

            procedurelogger.expectedResult('"%s" image size is %s x %s' %
                                                  (accessible, width, height))

            assert width == size[0], "%s (%s), %s (%s)" %\
                                            ("expected width",
                                              width,
                                             "does not match actual width",
                                              size[0])
            assert height == size[1], "%s (%s), %s (%s)" %\
                                            ("expected height",
                                              height,
                                             "does not match actual height",
                                              size[1])

    # close main window
    def quit(self):
        self.altF4()
