
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/15/2008
# Description: statusbar.py wrapper script
#              Used by the statusbar-*.py tests
##############################################################################$

import sys
import os
import actions
import states

from strongwind import *
from statusbar import *


# class to represent the main window.
class StatusBarFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    BUTTON_ONE = "button1"

    def __init__(self, accessible):
        super(StatusBarFrame, self).__init__(accessible)
        self.button1 = self.findPushButton(self.BUTTON_ONE)

    #give 'click' action
    def click(self,button):
        button.click()

    #assert if can find statusbar
    def assertStatusBar(self):
        procedurelogger.action('search for statusbar role')
        self.statusbar = self.findStatusBar("Ye Olde Status Bar Text")
        procedurelogger.expectedResult('Succeeded in finding StatusBar')

    #assert statusbar's text value
    def assertText(self, accessible, textValue):
        self.statusbar = self.findStatusBar(None)

        procedurelogger.expectedResult('the text of "%s" change to "%s"' % (accessible, textValue))
        def resultMatches():
            return self.statusbar.text == textValue
        assert retryUntilTrue(resultMatches)

    
    #close application main window after running test
    def quit(self):
        self.altF4()
