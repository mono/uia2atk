
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        11/10/2008
# Description: statusbarpanel.py wrapper script
#              Used by the statusbarpanel-*.py tests
##############################################################################

import sys
import os
import actions
import states

from strongwind import *
from statusbarpanel import *


# class to represent the main window.
class StatusBarPanelFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    BUTTON_ONE = "button1"
    BUTTON_TWO = "button2"
    PANEL1 = "statusbarpanel1"
    PANEL2 = "statusbarpanel2"
    PANEL3 = "Icon"

    def __init__(self, accessible):
        super(StatusBarPanelFrame, self).__init__(accessible)
        self.button1 = self.findPushButton(self.BUTTON_ONE)
        self.button2 = self.findPushButton(self.BUTTON_TWO)
        self.statusbar = self.findStatusBar("texts in statusbar")
        self.panel1 = self.findLabel(self.PANEL1)
        self.panel2 = self.findLabel(self.PANEL2)
        self.panel3 = self.findLabel(self.PANEL3)

    def click(self,button):
        """
        Wrap strongwind click action

        """
        button.click()

    def assertUnimplementedEditableText(self, accessible):
        """
        Make sure EditableText is not implemented for accessible

        """
        procedurelogger.action('check %s\'s EditableText interface' % accessible)

        procedurelogger.expectedResult('EditableText interface is unimplemented')

        try:
            accessible._accessible.queryEditableText()
        except NotImplementedError:
            print "aaaaa", return
            return
        assert False, "%s should not implement the EditableText interface" % accessible

    def assertText(self, accessible, textValue):
        """
        Make sure accessible's text is changed to textValue

        """
        procedurelogger.expectedResult('the text of "%s" change to "%s"' % (accessible, textValue))
        def resultMatches():
            return accessible.text == textValue
        assert retryUntilTrue(resultMatches)

    def assertImageSize(self, accessible, width, height):
        """
        Make sure image size of accessible is expected to test AtkImage is 
	implemented

        """
        procedurelogger.action("assert %s's image size" % accessible)
        size = accessible._accessible.queryImage().getImageSize()

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
        

    
    # close application main window after running test
    def quit(self):
        self.altF4()
