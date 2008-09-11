
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
    BUTTON_ONE = "Click me"

    def __init__(self, accessible):
        super(StatusBarFrame, self).__init__(accessible)
        self.button1 = self.findPushButton(self.BUTTON_ONE)

    #check statusbar's all expectant states
    def statesCheck(self):
        procedurelogger.action('check %s\'s all states' % self)

        procedurelogger.expectedResult('%s\'s all states can be found' % self)
        for s in states.StatusBar.states:
            cmd = "state = self." + s
            exec(cmd)

            if state == False:
                print "ERROR: %s can't be checked" % cmd
            else:
                pass

    #give 'click' action
    def click(self,button):
        #procedurelogger.action('Click the %s.' % button)
        #button._doAction("click")
        button.click()

    #assert if can find statusbar
    def assertStatusBar(self):
        procedurelogger.action('search for statusbar role')

        procedurelogger.expectedResult('succeeded in finding StatusBar role')
        def resultMatches():
            return self.findStatusBar(None)
        assert retryUntilTrue(resultMatches)
    
    #close application main window after running test
    def quit(self):
        self.altF4()
