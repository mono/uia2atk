
##############################################################################
# Written by:  Andres G. Aragoneses <aaragoneses@novell.com>
# Date:        02/23/2009
# Description: toolstripbutton.py wrapper script
#              Used by the toolstripbutton.py test
##############################################################################

import sys
import os
import actions
import states

from strongwind import *
from toolbarbutton import *
from helpers import *


# class to represent the main window.
class ToolStripButtonFrame(accessibles.Frame):
    NEWBUTTON = "&New"
    OPENBUTTON = "&Open"
#    TOGGLE = "Toggle"
#    UNEDITED = "nop"
#    SEPARATOR = "Separator"
#    LABEL = "ToolBar and ToolBarButton example"


    def __init__(self, accessible):
        super(ToolStripButtonFrame, self).__init__(accessible)
        self.toolbar = self.findToolBar(None)
#this should be findPushButton, not ToggleButton (bug: 478832)
        self.pushbutton1_style = self.toolbar.findToggleButton(self.NEWBUTTON)
        self.pushbutton2_style = self.toolbar.findToggleButton(self.OPENBUTTON)
#        self.dropdown_toggle = self.toolbar.findToggleButton(self.DROPDOWNBUTTON)
#        self.toggle_style = self.toolbar.findPushButton(self.TOGGLE)
#        self.nop_unable = self.toolbar.findPushButton(self.UNEDITED,\
#                                                  checkShowing=False)
#        self.separator_style = self.toolbar.findSeparator(self.SEPARATOR)
#        self.label = self.findLabel(self.LABEL)

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

    ##test for Toggle style ToolBarButton
#    def ToggleStyle(self, accessible):
        #test AtkText
#        procedurelogger.action('check Text for %s' % accessible)

#        procedurelogger.expectedResult(" %s's Text is %s" % (accessible, "Toggle"))
#        assert accessible.text == "Toggle", "%s doesn't match \
#                                        Toggle" % accessible.text
        #test AtkAction to unable label
#        accessible.click()
#        sleep(config.SHORT_DELAY)
#        statesCheck(self.toggle_style, "Button", add_states=["armed", "checked"])
#        statesCheck(self.label, "Label", invalid_states=["enabled", "sensitive"])
        #click again to enable label
#        accessible.click()
#        sleep(config.SHORT_DELAY)
#        statesCheck(self.toggle_style, "Button")
#        statesCheck(self.label, "Label")
                                          
        #test AtkComponent by mouse click to check its position
#        accessible.mouseClick()
#        sleep(config.SHORT_DELAY)
#        statesCheck(self.toggle_style, "Button", add_states=["armed", "checked"])
#        statesCheck(self.label, "Label", invalid_states=["enabled", "sensitive"])
        #mouse click again to enable label
#        accessible.mouseClick()
#        sleep(config.SHORT_DELAY)
#        statesCheck(self.toggle_style, "Button")
#        statesCheck(self.label, "Label")

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
