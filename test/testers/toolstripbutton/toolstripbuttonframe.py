
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
    LABEL = "Font:"


    def __init__(self, accessible):
        super(ToolStripButtonFrame, self).__init__(accessible)
        self.toolbar = self.findToolBar(None)
        #BUG514300 ToolBarButton: the "new" toggle button is identified as check box 
        #self.toggle_style = self.toolbar.findToggleButton(self.NEWBUTTON)

        #this should be findPushButton, not ToggleButton (bug: 478832)
        self.pushbutton_style = self.toolbar.findPushButton(self.OPENBUTTON)
        self.label = self.findLabel(self.LABEL)

    def click(self, accessible):
        accessible.click()

    ##test for PushButton style ToolBarButton
    def PushButtonStyle(self, accessible):
        #test AtkText
        procedurelogger.action('check Text for %s' % accessible)

        procedurelogger.expectedResult(" %s's Text is %s" % (accessible, "&Open"))
        assert accessible.text == "&Open", "%s doesn't match \
                                        PushButton" % accessible.text
        #test AtkAction
        accessible.click()

        sleep(config.SHORT_DELAY)
        procedurelogger.expectedResult("label shows you have clicked %s 1 time" % accessible)
        assert self.label.text == "You clicked PushButton 1 times", \
                "label shows %s" % self.label.text
                                          
        #test AtkComponent by mouse click it to check its position
        accessible.mouseClick()

        sleep(config.SHORT_DELAY)

        accessible.mouseClick()

        sleep(config.SHORT_DELAY)
        procedurelogger.expectedResult("label shows you clicked %s 3 times" % accessible)
        assert self.label.text == "You clicked PushButton 3 times", \
                "label shows %s" % self.label.text

    ##test for Toggle style ToolBarButton
    def ToggleStyle(self, accessible):
        #test AtkText
        procedurelogger.action('check Text for %s' % accessible)

        procedurelogger.expectedResult(" %s's Text is %s" % (accessible, "&New"))
        assert accessible.text == "&New", "%s doesn't match \
                                        Toggle" % accessible.text
        #test AtkAction to unable label
        accessible.click()
        sleep(config.SHORT_DELAY)
        #BUG514300 ToolBarButton: the "new" toggle button is identified as check box
        #statesCheck(self.toggle_style, "Button", add_states=["armed", "checked"])
        statesCheck(self.label, "Label", invalid_states=["enabled", "sensitive"])
        #click again to enable label
        accessible.click()
        sleep(config.SHORT_DELAY)

        #BUG514300 ToolBarButton: the "new" toggle button is identified as check box
        #statesCheck(self.toggle_style, "Button", invalid_states=["focusable"])
        statesCheck(self.label, "Label")
                                          
        #test AtkComponent by mouse click to check its position
        accessible.mouseClick()
        sleep(config.SHORT_DELAY)

        statesCheck(self.toggle_style, "Button", add_states=["armed", "checked"])
        statesCheck(self.label, "Label", invalid_states=["enabled", "sensitive"])
        #mouse click again to enable label
        accessible.mouseClick()

        sleep(config.SHORT_DELAY)
        #BUG514300 ToolBarButton: the "new" toggle button is identified as check box
        #statesCheck(self.toggle_style, "Button")
        statesCheck(self.label, "Label")

    #in this example all buttons with 24*24 image size, except separator
    def assertImageSize(self, button, width=0, height=0):
        procedurelogger.action("assert %s's image size" % button)
#	print dir(button) 
#this is failing, and I don't know why:
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
