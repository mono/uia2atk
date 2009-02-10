
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
    SEPARATOR = "Separator"
    LABEL = "ToolBar and ToolBarButton example"


    def __init__(self, accessible):
        super(ToolBarButtonFrame, self).__init__(accessible)
        self.toolbar = self.findToolBar(None)
        self.pushbutton_style = self.toolbar.findPushButton(self.PUSHBUTTON)
        self.dropdown_button = self.toolbar.findPushButton(self.DROPDOWNBUTTON)
        self.dropdown_toggle = self.toolbar.findToggleButton(self.DROPDOWNBUTTON)
        self.toggle_style = self.toolbar.findPushButton(self.TOGGLE)
        self.nop_unable = self.toolbar.findPushButton(self.UNEDITED,\
                                                  checkShowing=False)
        self.separator_style = self.toolbar.findSeparator(self.SEPARATOR)
        self.label = self.findLabel(self.LABEL)

    def click(self, accessible):
        accessible.click()

    ##test for PushButton style ToolBarButton
    def PushButtonStyle(self, accessible):
        #test AtkText
        procedurelogger.action('check Text for %s' % accessible)

        procedurelogger.expectedResult(" %s's Text is %s" % (accessible, "PushButton"))
        assert accessible.text == "PushButton", "%s doesn't match \
                                        PushButton" % accessible.text
        #test AtkAction
        accessible.click()

        sleep(config.SHORT_DELAY)
        procedurelogger.expectedResult("label shows you have clicked %s 1 time" % accessible)
        assert self.label.text == "You clicked PushButton 1 times", \
                "lable shows %s" % self.label.text
                                          
        #test AtkComponent by mouse click it to check its position
        accessible.mouseClick()

        sleep(config.SHORT_DELAY)
        procedurelogger.expectedResult("label shows you clicked %s 2 times" % accessible)
        assert self.label.text == "You clicked PushButton 2 times", \
                "lable shows %s" % self.label.text

    ##test for DropDownButton style ToolBarButton
    def DropDownButtonStyle(self, pushbutton, togglebutton):
        #test AtkText
        procedurelogger.action('check Text for %s and %s' % (pushbutton, togglebutton))

        procedurelogger.expectedResult(" %s and %s's Text are %s" % (pushbutton, togglebutton, "DropDownButton"))
        assert pushbutton.text == "DropDownButton" and \
               togglebutton.text == "DropDownButton",  "%s and %s doesn't\
           match DropDownButton" % (pushbutton.text,togglebutton.text)

        #test AtkAction for normal push button doesn't show menu items list
        pushbutton.click()

        sleep(config.SHORT_DELAY)
        procedurelogger.expectedResult("click normal push button doesn't show menu items list")
        try:
            self.toolbar.findMenuItem("Red")
        except searchError:
            pass

        #test AtkAction for toggle button to show menu items list
        togglebutton.click()

        sleep(config.SHORT_DELAY)
        procedurelogger.expectedResult("click toggle button to show menu items list")
        self.menuitem1 = self.dropdown_toggle.findMenuItem("Red")
        self.menuitem2 = self.dropdown_toggle.findMenuItem("Blue")
        assert self.menuitem1 and self.menuitem2
                                          
        #test AtkComponent by mouse click normal push button to check its position and size
        pushbutton.mouseClick()

        sleep(config.SHORT_DELAY)
        procedurelogger.expectedResult("mouse click normal push button doesn't show menu items list")
        try:
            self.toolbar.findMenuItem("Blue")
        except searchError:
            pass

        #mouse click toggle button again
        togglebutton.mouseClick()

        sleep(config.SHORT_DELAY)
        procedurelogger.expectedResult("mouse click toggle button to show menu items list")
        self.menuitem1 = self.dropdown_toggle.findMenuItem("Red")
        self.menuitem2 = self.dropdown_toggle.findMenuItem("Blue")
        assert self.menuitem1 and self.menuitem2

        #click menuitem to change label's text
        self.menuitem1.click()

        sleep(config.SHORT_DELAY)
        procedurelogger.expectedResult("label shows you selected item Red")
        assert self.label.text == "You selected dropdownbutton item Red", \
                "lable shows %s" % self.label.text

    ##test for Toggle style ToolBarButton
    def ToggleStyle(self, accessible):
        #test AtkText
        procedurelogger.action('check Text for %s' % accessible)

        procedurelogger.expectedResult(" %s's Text is %s" % (accessible, "Toggle"))
        assert accessible.text == "Toggle", "%s doesn't match \
                                        Toggle" % accessible.text
        #test AtkAction to unable label
        accessible.click()
        sleep(config.SHORT_DELAY)
        statesCheck(self.label, "Label", invalid_states=["enabled", "sensitive"])
        #click again to enable label
        accessible.click()
        sleep(config.SHORT_DELAY)
        statesCheck(self.label, "Label")
                                          
        #test AtkComponent by mouse click to check its position
        accessible.mouseClick()
        sleep(config.SHORT_DELAY)
        statesCheck(self.label, "Label", invalid_states=["enabled", "sensitive"])
        #mouse click again to enable label
        accessible.mouseClick()
        sleep(config.SHORT_DELAY)
        statesCheck(self.label, "Label")

    ##test for unable ToolBarButton
    def UnableButton(self, accessible):
        #test AtkText
        procedurelogger.action('check Text for %s' % accessible)

        procedurelogger.expectedResult(" %s's Text is %s" % (accessible, "nop"))
        assert accessible.text == "nop", "%s doesn't match \
                                        nop" % accessible.text
        #test AtkAction for nop button doesn't change label's text
        current_label = self.label.text

        accessible.click()
        sleep(config.SHORT_DELAY)
        procedurelogger.expectedResult("click unable nop button doesn't change label")
        assert self.label.text == current_label, "label is changed to %s" % \
                                                     self.label.text

    ##test for Separator style ToolBarButton
    def SeparatorStyle(self, accessible):
        #test AtkText
        procedurelogger.action('check Text for %s' % accessible)

        procedurelogger.expectedResult(" %s's Text is %s" % (accessible, "separator"))
        assert accessible.text == "separator", "%s doesn't match \
                                        separator" % accessible.text

        #AtkAction is unimplementd for separator style toolbarbutton
        procedurelogger.action('check Action for %s' % accessible)
        procedurelogger.expectedResult("AtkAction is unimplemented")
        try:
           accessible.click()
        except NotImplementedError:
           pass
    
    #in this example all buttons with 24*24 image size, except separator
    def assertImageSize(self, button, width=0, height=0):
        procedurelogger.action("assert %s's image size" % button)
        size = button.imageSize

        procedurelogger.expectedResult('"%s" image size is %s x %s' %
                                                  (button, width, height))

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

    #close main window
    def quit(self):
        self.altF4()
