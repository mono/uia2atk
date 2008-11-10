
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        11/10/2008
# Description: statusbarpanel.py wrapper script
#              Used by the statusbarpanel-*.py tests
##############################################################################$

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

    def __init__(self, accessible):
        super(StatusBarPanelFrame, self).__init__(accessible)
        self.button1 = self.findPushButton(self.BUTTON_ONE)
        self.button2 = self.findPushButton(self.BUTTON_TWO)

    #give 'click' action
    def click(self,button):
        button.click()

    #assert if can find statusbar
    def assertStatusBar(self):
        procedurelogger.action('search for statusbar role')

        procedurelogger.expectedResult('succeeded in finding StatusBar role')
        self.statusbar = self.findStatusBar("texts in statusbar")
        assert self.statusbar

    #assert if can find statusbarpanel
    def assertPanel(self):
        procedurelogger.action('search for StatusBarPanle as "text" role name')
        self.panel = self.findAllTexts(None)
        self.panel1 = self.panel[0]
        self.panel2 = self.panel[1]

        procedurelogger.expectedResult('succeeded in finding "text" role')
        assert self.panel1 and self.panel2

    #assert statusbarpanel's text value
    def assertText(self, accessible, textValue):
        procedurelogger.expectedResult('the text of "%s" change to "%s"' % (accessible, textValue))
        def resultMatches():
            return accessible.text == textValue
        assert retryUntilTrue(resultMatches)

    
    #close application main window after running test
    def quit(self):
        self.altF4()
